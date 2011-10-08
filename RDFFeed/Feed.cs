/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using FeedHanderPluginInterface;

namespace RdfFeed
{
    public class Feed : IFeed
    {
        #region Properties
        public bool Loaded { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        protected XmlDocument Feedxml { get; set; }
        public int UpdateInterval { get; set; }
        public Uri FeedUri { get; set; }
        protected Collection<FeedItem> feeditems = new Collection<FeedItem>();
        public Collection<FeedItem> FeedItems
        {
            get { return feeditems; }
        }
        public string Title { get; set; }
        public Uri Url { get; set; }
        public string Description { get; set; }
        private readonly IWebProxy feedproxy;
        private readonly FeedAuthTypes feedauthtype;
        private readonly string feedusername;
        private readonly string feedpassword;
        public DateTime LastUpdate { get; private set; }
        #endregion

        #region Methods
        public Feed()
        {
            UpdateInterval = 10;
            HasError = false;
            Loaded = false;
        }

        public Feed(Uri url, FeedAuthTypes authtype, string username, string password, IWebProxy proxy)
        {
            UpdateInterval = 10;
            HasError = false;
            Loaded = false;
            feedproxy = proxy;
            FeedUri = url;
            feedauthtype = authtype;
            feedusername = username;
            feedpassword = password;
        }


        private XmlDocument Fetch(Uri feeduri)
        {

            var req = (HttpWebRequest)WebRequest.Create(feeduri);
            req.UserAgent = string.Format("Mozilla/5.0 (compatible; Feedling-RDFFeedHandler/{0}; http://feedling.net", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            req.Proxy = feedproxy;
            if (feedauthtype == FeedAuthTypes.Basic)
            {
                req.Credentials = new NetworkCredential(feedusername, feedpassword);
            }
            var resp = (HttpWebResponse)req.GetResponse();

            var tempdoc = new XmlDocument();
            tempdoc.Load(resp.GetResponseStream());
            resp.Close();
            return tempdoc;

        }

        public void Update()
        {
            Exception retexception = null;
            try
            {
                var oldfeeditems = feeditems.Select(item => (FeedItem)item.Clone()).ToList();
                Feedxml = Fetch(FeedUri);
                var xPathNavigator = Feedxml.CreateNavigator();

                var xmlNamespaceManager = new XmlNamespaceManager(xPathNavigator.NameTable);
                xmlNamespaceManager.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                xmlNamespaceManager.AddNamespace("rss", "http://purl.org/rss/1.0/");
                xmlNamespaceManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
                var xPathExpression = xPathNavigator.Compile("/rdf:RDF//rss:item");
                xPathExpression.SetContext(xmlNamespaceManager);
                var xPathNodeIterator = xPathNavigator.Select(xPathExpression);

                var titlenode = xPathNavigator.SelectSingleNode("/rdf:RDF/rss:channel/rss:title/text()", xmlNamespaceManager);
                Title = titlenode == null ? "(untitled)" : WebUtility.HtmlDecode(titlenode.ToString().Trim());

                var urlnode = xPathNavigator.SelectSingleNode("/rdf:RDF/rss:channel/rss:link/text()", xmlNamespaceManager);

                if (urlnode != null)
                {
                    Url = new Uri(urlnode.ToString());
                }

                var descriptionnode = xPathNavigator.SelectSingleNode("/rdf:RDF/rss:channel/rss:description/text()", xmlNamespaceManager);

                if (descriptionnode != null)
                {
                    Description = descriptionnode.ToString().Trim();
                }
                feeditems.Clear();
                while (xPathNodeIterator.MoveNext())
                {
                    if (xPathNodeIterator.Current == null || !xPathNodeIterator.Current.HasChildren) continue;
                    var item = new FeedItem();
                    var subnav = xPathNodeIterator.Current.CreateNavigator();

                    titlenode = subnav.SelectSingleNode("rss:title", xmlNamespaceManager);
                    item.Title = titlenode == null ? "(untitled)" : WebUtility.HtmlDecode(titlenode.ToString()).Trim();

                    var linknode = subnav.SelectSingleNode("rss:link", xmlNamespaceManager);
                    if (linknode != null)
                    {
                        item.Link = new Uri(linknode.ToString());
                    }
                    descriptionnode = subnav.SelectSingleNode("rss:description", xmlNamespaceManager);
                    if (descriptionnode != null)
                    {
                        item.Description = descriptionnode.ToString();
                    }
                    try
                    {
                        var tempnav = subnav.SelectSingleNode("dc:date", xmlNamespaceManager);
                        if (tempnav != null)
                        {
                            DateTime gdt;
                            var res = DateTime.TryParse(tempnav.ToString(), out gdt);
                            if (res)
                            {
                                item.Date = gdt;
                            }
                        }
                    }
                    catch (FormatException) { }
                    if (!oldfeeditems.Contains(item))
                    {
                        item.Updated = true;
                    }
                    feeditems.Add(item);
                }

            }
            catch (WebException ex)
            {
                retexception = ex;
            }
            catch (XmlException ex)
            {
                retexception = ex;
            }
            catch (NullReferenceException ex)
            {
                retexception = ex;
            }

            if (retexception != null)
            {
                HasError = true;
                ErrorMessage = retexception.Message;
            }
            else
            {
                HasError = false;
                ErrorMessage = null;
            }
            Loaded = true;
            FireUpdated();
        }

        public void Watch(object state)
        {
            while (true)
            {
                Update();
                //Add Fuzz factor of up to 30s to prevent everything from fetching at the same time.
                var fuzz = new Random().Next(1000, 30000);
                Thread.Sleep((UpdateInterval * 60000) + fuzz);
            }
        }

        protected void FireUpdated()
        {
            LastUpdate = DateTime.Now;
            Updated(null, new EventArgs());
        }
        #endregion

        #region Events
        public virtual event EventHandler Updated;
        #endregion
    }
}
