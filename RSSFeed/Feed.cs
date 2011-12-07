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
using System.Windows.Forms;
using System.Xml;
using FeedHanderPluginInterface;

namespace RssFeed
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
        private Collection<FeedItem> feeditems = new Collection<FeedItem>();
        public Collection<FeedItem> FeedItems
        {
            get { return feeditems; }
        }
        public string Title { get; set; }
        public Uri Url { get; set; }
        public string Description { get; set; }
        public Form ConfigForm
        {
            get { return null; }
        }
        public string PluginName
        {
            get
            {
                return "RSSFeed";
            }
        }
        public string PluginVersion
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }
        public string PluginCopyright
        {
            get { return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LegalCopyright; }
        }
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
            req.UserAgent = string.Format("Mozilla/5.0 (compatible; Feedling-RSSFeedHandler/{0}; http://feedling.net", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            req.Proxy = feedproxy;
            if (feedauthtype == FeedAuthTypes.Basic)
            {
                req.Credentials = new NetworkCredential(feedusername, feedpassword);
            }
            var webResponse = req.GetResponse();
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(webResponse.GetResponseStream());
            webResponse.Close();
            return xmlDocument;
        }

        public void Update()
        {
            Exception retexception = null;
            try
            {
                var oldfeeditems = feeditems.Select(item => (FeedItem)item.Clone()).ToList();
                Feedxml = Fetch(FeedUri);
                var xPathNavigator = Feedxml.CreateNavigator();
                var xPathNodeIterator = xPathNavigator.Select("/rss/channel/item");

                var titlenode = xPathNavigator.SelectSingleNode("/rss/channel/title");
                Title = titlenode == null ? "(untitled)" : WebUtility.HtmlDecode(titlenode.ToString().Trim());

                var linknode = xPathNavigator.SelectSingleNode("/rss/channel/link");
                if (linknode != null)
                {
                    Uri result;
                    if (Uri.TryCreate(linknode.ToString(), UriKind.Absolute, out result))
                    {
                        Url = result;
                    }
                }

                var descriptionnode = xPathNavigator.SelectSingleNode("/rss/channel/description");
                if (descriptionnode != null)
                {
                    Description = descriptionnode.ToString().Trim();
                }
                feeditems.Clear();
                while (xPathNodeIterator.MoveNext())
                {
                    if (xPathNodeIterator.Current == null || !xPathNodeIterator.Current.HasChildren) continue;
                    var item = new FeedItem();
                    var pathNavigator = xPathNodeIterator.Current.CreateNavigator();

                    titlenode = pathNavigator.SelectSingleNode("title");
                    item.Title = titlenode == null ? "(untitled)" : WebUtility.HtmlDecode(titlenode.ToString()).Trim();

                    linknode = pathNavigator.SelectSingleNode("link");
                    if (linknode != null)
                    {
                        Uri result;
                        if (Uri.TryCreate(linknode.ToString(), UriKind.Absolute, out result))
                        {
                            item.Link = result;
                        }
                    }

                    try
                    {
                        var tempnav = pathNavigator.SelectSingleNode("pubDate");
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
