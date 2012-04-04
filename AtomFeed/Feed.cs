﻿/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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

        public DateTime LastUpdate { get; private set; }
        public bool RunWatchLoop { get; set; }
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

        private XmlDocument Fetch(Uri uri)
        {
            var xmlDocument = new XmlDocument();
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.UserAgent = string.Format(CultureInfo.CurrentCulture, "Mozilla/5.0 (compatible; Atom-RSSFeedHandler/{0}; http://feedling.net", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
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

                if (xPathNavigator.NameTable != null)
                {
                    var xmlNamespaceManager = new XmlNamespaceManager(xPathNavigator.NameTable);
                    xmlNamespaceManager.AddNamespace("atom", "http://www.w3.org/2005/Atom");


                    SetFeedTitle(xPathNavigator, xmlNamespaceManager);
                    SetFeedLink(xPathNavigator, xmlNamespaceManager);
                    SetFeedDescription(xPathNavigator, xmlNamespaceManager);

                    //Set up the node iterater to jump through all the entries
                    var xPathExpression = xPathNavigator.Compile("/atom:feed/atom:entry");
                    xPathExpression.SetContext(xmlNamespaceManager);
                    var xPathNodeIterator = xPathNavigator.Select(xPathExpression);

                    feeditems.Clear();
                    while (xPathNodeIterator.MoveNext())
                    {
                        if (xPathNodeIterator.Current != null)
                        {
                            if (!xPathNodeIterator.Current.HasChildren) continue;

                            var item = GetItemFromFeedEntry(xPathNodeIterator, xmlNamespaceManager);

                            if (!oldfeeditems.Contains(item))
                            {
                                item.Updated = true;
                            }
                            feeditems.Add(item);
                        }
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
            catch (IOException ex)
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

        private static FeedItem GetItemFromFeedEntry(XPathNodeIterator xPathNodeIterator, IXmlNamespaceResolver xmlNamespaceManager)
        {
            var item = new FeedItem();

            if (xPathNodeIterator.Current != null)
            {
                var subnav = xPathNodeIterator.Current.CreateNavigator();
                var textnode = subnav.SelectSingleNode("atom:title/text()", xmlNamespaceManager);
                item.Title = textnode == null ? "(untitled)" : WebUtility.HtmlDecode(textnode.ToString()).Trim();
                var linkiterator = subnav.Select("atom:link", xmlNamespaceManager);
                while (linkiterator.MoveNext() && item.Link == null)
                {
                    var linknav = linkiterator.Current;
                    if (linknav != null)
                    {
                        if (!linknav.HasAttributes) continue;
                        var rel = linknav.GetAttribute("rel", "");
                        if (rel.Equals("alternate"))
                        {
                            item.Link = new Uri(linknav.GetAttribute("href", ""));
                        }
                    }
                }
                // If no link with rel = 'alternate' in the entry, just pick the first link
                if (item.Link == null)
                {
                    var linknav = subnav.SelectSingleNode("atom:link", xmlNamespaceManager);
                    if (linknav != null)
                    {
                        Uri result;
                        if (Uri.TryCreate(linknav.GetAttribute("href", ""), UriKind.Absolute, out result))
                        {
                            item.Link = result;
                        }
                    }
                }
                var updatednav = subnav.SelectSingleNode("atom:updated/text()", xmlNamespaceManager);
                if (updatednav != null)
                {
                    DateTime gdt;
                    var res = DateTime.TryParse(updatednav.ToString(), out gdt);
                    if (res)
                    {
                        item.Date = gdt;
                    }
                }
            }
            return item;
        }

        private void SetFeedDescription(XPathNavigator xPathNavigator, IXmlNamespaceResolver xmlNamespaceManager)
        {
            var subtitlenode = xPathNavigator.SelectSingleNode("/atom:feed/atom:subtitle", xmlNamespaceManager);
            if (subtitlenode != null)
            {
                Description = subtitlenode.ToString().Trim();
            }
        }

        private void SetFeedTitle(XPathNavigator xPathNavigator, IXmlNamespaceResolver xmlNamespaceManager)
        {
            var titlenode = xPathNavigator.SelectSingleNode("/atom:feed/atom:title/text()", xmlNamespaceManager);
            Title = titlenode == null ? "(untitled)" : WebUtility.HtmlDecode(titlenode.ToString().Trim());
        }

        private void SetFeedLink(XPathNavigator xPathNavigator, IXmlNamespaceResolver xmlNamespaceManager)
        {
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
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
