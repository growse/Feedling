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

[assembly: CLSCompliant(false)]
namespace AtomFeed
{
    public class Feed : IFeed
    {
        #region Properties

        public bool Loaded { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        private XmlDocument feedxml;

        protected IXPathNavigable FeedXml
        {
            get { return feedxml; }
        }

        public int UpdateInterval { get; set; }

        public Uri FeedUri { get; set; }

        private readonly Collection<FeedItem> feeditems = new Collection<FeedItem>();
        public Collection<FeedItem> FeedItems
        {
            get { return feeditems; }
        }

        public string Title { get; set; }

        public Uri Url { get; set; }

        public string Description { get; set; }

        public DateTime LastUpdate { get; private set; }

        private readonly IWebProxy feedproxy;
        private readonly FeedAuthTypes feedauthtype;
        private readonly string feedusername;
        private readonly string feedpassword;
        #endregion

        #region Methods
        public Feed()
        {
            UpdateInterval = 10;
            HasError = false;
            Loaded = false;
        }

        public Feed(Uri url, FeedAuthTypes authType, string userName, string password, IWebProxy proxy)
        {
            UpdateInterval = 10;
            HasError = false;
            Loaded = false;
            feedproxy = proxy;
            FeedUri = url;
            feedauthtype = authType;
            feedusername = userName;
            feedpassword = password;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private XmlDocument Fetch(Uri uri)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.UserAgent = string.Format("Mozilla/5.0 (compatible; Atom-RSSFeedHandler/{0}; http://feedling.net", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            req.Proxy = feedproxy;
            if (feedauthtype == FeedAuthTypes.Basic)
            {
                req.Credentials = new NetworkCredential(feedusername, feedpassword);
            }
            var resp = req.GetResponse();
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
                feedxml = Fetch(FeedUri);

                var xPathNavigator = FeedXml.CreateNavigator();

                var xmlNamespaceManager = new XmlNamespaceManager(xPathNavigator.NameTable);
                xmlNamespaceManager.AddNamespace("atom", "http://www.w3.org/2005/Atom");

                var xPathExpression = xPathNavigator.Compile("/atom:feed/atom:entry");
                xPathExpression.SetContext(xmlNamespaceManager);

                var xPathNodeIterator = xPathNavigator.Select(xPathExpression);

                var titlenode = xPathNavigator.SelectSingleNode("/atom:feed/atom:title/text()", xmlNamespaceManager);
                Title = titlenode == null ? "(untitled)" : WebUtility.HtmlDecode(titlenode.ToString().Trim());

                // Create link from the first link element in feed with rel = 'alternative'
                var linkiterator = xPathNavigator.Select("atom:feed/atom:link", xmlNamespaceManager);
                while (linkiterator.MoveNext() && Url == null)
                {
                    var linknav = linkiterator.Current;
                    if (linknav == null || !linknav.HasAttributes) continue;
                    var rel = linknav.GetAttribute("rel", "");
                    if (rel.Equals("alternate"))
                    {
                        Url = new Uri(linknav.GetAttribute("href", ""));
                    }
                }
                // If no link with rel = 'alternate' in the feed, just pick the first link
                if (Url == null)
                {
                    var linknode = xPathNavigator.SelectSingleNode("/atom:feed/atom:link", xmlNamespaceManager);
                    if (linknode != null)
                    {
                        Uri result;
                        if (Uri.TryCreate(linknode.GetAttribute("href", ""), UriKind.Absolute, out result))
                        {
                            Url = result;
                        }
                    }
                }

                var subtitlenode = xPathNavigator.SelectSingleNode("/atom:feed/atom:subtitle", xmlNamespaceManager);
                if (subtitlenode != null)
                {
                    Description = subtitlenode.ToString().Trim();
                }

                feeditems.Clear();
                while (xPathNodeIterator.MoveNext())
                {
                    if (!xPathNodeIterator.Current.HasChildren) continue;

                    var item = new FeedItem();
                    var subnav = xPathNodeIterator.Current.CreateNavigator();
                    var textnode = subnav.SelectSingleNode("atom:title/text()", xmlNamespaceManager);
                    item.Title = textnode == null ? "(untitled)" : WebUtility.HtmlDecode(textnode.ToString()).Trim();
                    linkiterator = subnav.Select("atom:link", xmlNamespaceManager);
                    while (linkiterator.MoveNext() && item.Link == null)
                    {
                        var linknav = linkiterator.Current;
                        if (!linknav.HasAttributes || linknav.GetAttribute("rel", "") == null ||
                            linknav.GetAttribute("href", "") == null) continue;
                        var rel = linknav.GetAttribute("rel", "");
                        if (rel.Equals("alternate"))
                        {
                            item.Link = new Uri(linknav.GetAttribute("href", ""));
                        }
                    }
                    // If no link with rel = 'alternate' in the entry, just pick the first link
                    if (item.Link == null)
                    {
                        if (subnav.SelectSingleNode("atom:link", xmlNamespaceManager) != null && subnav.SelectSingleNode("atom:link", xmlNamespaceManager).GetAttribute("href", "") != null)
                        {
                            Uri result;
                            if (Uri.TryCreate(subnav.SelectSingleNode("atom:link", xmlNamespaceManager).GetAttribute("href", ""), UriKind.Absolute, out result))
                            {
                                item.Link = result;
                            }
                        }
                    }
                    if (subnav.SelectSingleNode("atom:updated/text()", xmlNamespaceManager) != null)
                    {
                        try
                        {
                            var tempnav = subnav.SelectSingleNode("atom:updated/text()", xmlNamespaceManager);
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
                        catch (FormatException)
                        {

                        }
                    }
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
                int fuzz = new Random().Next(1000, 30000);
                Thread.Sleep((UpdateInterval * 60000) + fuzz);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
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
