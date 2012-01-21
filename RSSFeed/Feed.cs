/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using FeedHanderPluginInterface;

namespace RssFeed
{
    public class Feed : IFeed
    {
        #region Properties

        private XmlDocument feedxml;
        public bool Loaded { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
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
        public Form ConfigForm
        {
            get { return null; }
        }
        public DateTime LastUpdate { get; private set; }
        private readonly IWebProxy feedproxy;
        private readonly FeedAuthTypes feedauthtype;
        private readonly string feedusername;
        private readonly string feedpassword;
        public bool RunWatchLoop { get; set; }
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
            var xmlDocument = new XmlDocument();
            var req = (HttpWebRequest)WebRequest.Create(feeduri);
            req.UserAgent = string.Format(CultureInfo.CurrentCulture, "Mozilla/5.0 (compatible; Feedling-RSSFeedHandler/{0}; http://feedling.net", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            req.Proxy = feedproxy;
            if (feedauthtype == FeedAuthTypes.Basic)
            {
                req.Credentials = new NetworkCredential(feedusername, feedpassword);
            }
            using (var webResponse = req.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream != null) xmlDocument.Load(responseStream);
                }
            }
            return xmlDocument;
        }

        public void Update()
        {
            Exception retexception = null;
            try
            {
                var oldfeeditems = feeditems.Select(item => (FeedItem)item.Clone()).ToList();
                feedxml = Fetch(FeedUri);

                var xPathNavigator = feedxml.CreateNavigator();

                SetFeedTitle(xPathNavigator);
                SetFeedLink(xPathNavigator);
                SetFeedDescription(xPathNavigator);

                var xPathNodeIterator = xPathNavigator.Select("/rss/channel/item");

                feeditems.Clear();
                while (xPathNodeIterator.MoveNext())
                {
                    if (xPathNodeIterator.Current != null)
                    {
                        if (!xPathNodeIterator.Current.HasChildren) continue;
                        var item = GetItemFromFeedEntry(xPathNodeIterator);

                        if (!oldfeeditems.Contains(item))
                        {
                            item.Updated = true;
                        }
                        feeditems.Add(item);
                    }
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

        private static FeedItem GetItemFromFeedEntry(XPathNodeIterator xPathNodeIterator)
        {
            var item = new FeedItem();
            if (xPathNodeIterator.Current != null)
            {
                var pathNavigator = xPathNodeIterator.Current.CreateNavigator();

                var titlenode = pathNavigator.SelectSingleNode("title");
                item.Title = titlenode == null ? "(untitled)" : WebUtility.HtmlDecode(titlenode.ToString()).Trim();

                var linknode = pathNavigator.SelectSingleNode("link");
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
            }
            return item;
        }

        private void SetFeedDescription(XPathNavigator xPathNavigator)
        {
            var descriptionnode = xPathNavigator.SelectSingleNode("/rss/channel/description");
            if (descriptionnode != null)
            {
                Description = descriptionnode.ToString().Trim();
            }
        }

        private void SetFeedLink(XPathNavigator xPathNavigator)
        {
            var linknode = xPathNavigator.SelectSingleNode("/rss/channel/link");
            if (linknode != null)
            {
                Uri result;
                if (Uri.TryCreate(linknode.ToString(), UriKind.Absolute, out result))
                {
                    Url = result;
                }
            }
        }

        private void SetFeedTitle(XPathNavigator xPathNavigator)
        {
            var titlenode = xPathNavigator.SelectSingleNode("/rss/channel/title");
            Title = titlenode == null ? "(untitled)" : WebUtility.HtmlDecode(titlenode.ToString().Trim());
        }

        public void Watch()
        {
            RunWatchLoop = true;
            while (RunWatchLoop)
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
        public event EventHandler Updated;
        #endregion
    }
}
