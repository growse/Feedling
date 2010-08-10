﻿/*
Copyright © 2008-2010, Andrew Rowson
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of Feedling nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using FeedHanderPluginInterface;

[assembly: CLSCompliant(false)]
namespace AtomFeed
{
    public class Feed : IFeed
    {
        #region Properties

        private bool loaded = false;

        public bool Loaded
        {
            get { return loaded; }
            set { loaded = value; }
        }

        private bool haserror = false;

        public bool HasError
        {
            get { return haserror; }
            set { haserror = value; }
        }
        private string errormessage;

        public string ErrorMessage
        {
            get { return errormessage; }
            set { errormessage = value; }
        }
        private XmlDocument feedxml;

        protected IXPathNavigable FeedXml
        {
            get { return feedxml; }
        }
        private int updateinterval = 10;
        public int UpdateInterval
        {
            get { return updateinterval; }
            set { updateinterval = value; }
        }
        private Uri feeduri;
        public Uri FeedUri
        {
            get { return feeduri; }
            set { feeduri = value; }
        }

        private Collection<FeedItem> feeditems = new Collection<FeedItem>();
        public Collection<FeedItem> FeedItems
        {
            get { return feeditems; }
        }
        private Uri imageurl;

        public Uri ImageUrl
        {
            get { return imageurl; }
            set { imageurl = value; }
        }
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private Uri url;

        public Uri Url
        {
            get { return url; }
            set { url = value; }
        }
        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public Form ConfigForm
        {
            get { return null; }
        }
        public string PluginName
        {
            get
            {
                return "AtomFeed";
            }
        }

        public string PluginVersion
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public string PluginCopyright
        {
            [SecurityCriticalAttribute]
            get { return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LegalCopyright; }
        }

        private DateTime lastupdate;
        public DateTime LastUpdate
        {
            get
            {
                return lastupdate;
            }
        }

        private IWebProxy feedproxy;
        private FeedAuthTypes feedauthtype;
        private string feedusername;
        private string feedpassword;
        #endregion

        #region Methods
        public Feed()
        {
        }
        public Feed(Uri url, FeedAuthTypes authType, string userName, string password, IWebProxy proxy)
        {
            feedproxy = proxy;
            feeduri = url;
            feedauthtype = authType;
            feedusername = userName;
            feedpassword = password;
        }
        public bool CanHandle(IXPathNavigable xml)
        {
            if (xml != null)
            {
                XPathNavigator nav = xml.CreateNavigator();
                XmlNamespaceManager xnm = new XmlNamespaceManager(nav.NameTable);
                xnm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                if (nav.SelectSingleNode("/atom:feed", xnm) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
            } return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IFeed Factory(Uri uri, IWebProxy proxy)
        {
            return new Feed(uri, FeedAuthTypes.None, "", "", proxy);
        }
        public IFeed Factory(Uri uri, FeedAuthTypes authtype, string username, string password, IWebProxy proxy)
        {
            return new Feed(uri, authtype, username, password, proxy);
        }
        private XmlDocument Fetch(Uri uri)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.UserAgent = "Mozilla/5.0";
            req.Proxy = feedproxy;
            if (feedauthtype == FeedAuthTypes.Basic)
            {
                req.Credentials = new NetworkCredential(feedusername, feedpassword);
            }
            WebResponse resp = req.GetResponse();
            XmlDocument tempdoc = new XmlDocument();
            tempdoc.Load(resp.GetResponseStream());
            resp.Close();
            return tempdoc;
        }

        public void Update()
        {
            Exception retexception = null;
            try
            {
                List<FeedItem> oldfeeditems = new List<FeedItem>();
                foreach (FeedItem item in feeditems)
                {
                    oldfeeditems.Add((FeedItem)item.Clone());
                }
                this.feedxml = Fetch(feeduri);

                XPathNavigator nav;
                XPathNavigator subnav;
                XPathNodeIterator nodeiterator;
                nav = FeedXml.CreateNavigator();

                XmlNamespaceManager xnm = new XmlNamespaceManager(nav.NameTable);
                xnm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                XPathExpression xpe = nav.Compile("/atom:feed/atom:entry");
                xpe.SetContext(xnm);
                nodeiterator = nav.Select(xpe);
                if (nav.SelectSingleNode("/atom:feed/atom:title/text()", xnm) == null)
                {
                    this.Title = "(untitled)";
                }
                else
                {
                    this.Title = nav.SelectSingleNode("/atom:feed/atom:title/text()", xnm).ToString().Trim();
                }
                if (nav.SelectSingleNode("/atom:feed/atom:link", xnm) != null && nav.SelectSingleNode("/atom:feed/atom:link", xnm).GetAttribute("href", "") != null)
                {
                    this.Url = new Uri(nav.SelectSingleNode("/atom:feed/atom:link", xnm).GetAttribute("href", "").ToString());
                }
                if (nav.SelectSingleNode("/atom:feed/atom:subtitle", xnm) != null)
                {
                    this.Description = nav.SelectSingleNode("/atom:feed/atom:subtitle", xnm).ToString().Trim();
                }
                if (nav.SelectSingleNode("/atom:feed/atom:logo", xnm) != null)
                {
                    this.imageurl = new Uri(nav.SelectSingleNode("/atom:feed/atom:logo", xnm).ToString());
                }
                feeditems.Clear();
                while (nodeiterator.MoveNext())
                {
                    if (nodeiterator.Current.HasChildren)
                    {
                        FeedItem item = new FeedItem();
                        subnav = nodeiterator.Current.CreateNavigator();
                        if (subnav.SelectSingleNode("atom:title/text()", xnm) == null)
                        {
                            item.Title = "(untitled)";
                        }
                        else
                        {
                            item.Title = System.Web.HttpUtility.HtmlDecode(subnav.SelectSingleNode("atom:title/text()", xnm).ToString()).Trim();
                        }
                        if (subnav.SelectSingleNode("atom:link", xnm) != null && subnav.SelectSingleNode("atom:link", xnm).GetAttribute("href", "") != null)
                        {
                            item.Link = new Uri(subnav.SelectSingleNode("atom:link", xnm).GetAttribute("href", "").ToString());
                        }
                        if (subnav.SelectSingleNode("atom:updated/text()", xnm) != null)
                        {
                            try
                            {
                                DateTime gdt;
                                XPathNavigator tempnav = subnav.SelectSingleNode("atom:updated/text()", xnm);
                                if (tempnav != null)
                                {
                                    bool res = DateTime.TryParse(tempnav.ToString(), out gdt);
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

            }
            catch (WebException ex)
            {
                retexception = ex;
            }
            catch (XmlException ex)
            {
                retexception = ex;
                throw;
            }
            catch (NullReferenceException ex)
            {
                retexception = ex;
            }

            if (retexception != null)
            {
                haserror = true;
                errormessage = retexception.Message;
            }
            else
            {
                haserror = false;
                errormessage = null;
            }
            loaded = true;
            FireUpdated();
        }

        public void Watch(object state)
        {
            while (true)
            {
                Update();
                //Add Fuzz factor of up to 30s to prevent everything from fetching at the same time.
                int fuzz = new Random().Next(1000, 30000);
                Thread.Sleep((updateinterval * 60000) + fuzz);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected void FireUpdated()
        {
            lastupdate = DateTime.Now;
            Updated(null, new EventArgs());
        }
        #endregion

        #region Events
        public virtual event EventHandler Updated;
        #endregion

    }
}
