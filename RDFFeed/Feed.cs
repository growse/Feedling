/*
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
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using FeedHanderPluginInterface;

namespace RdfFeed
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

        protected XmlDocument Feedxml
        {
            get { return feedxml; }
            set { feedxml = value; }
        }
        protected int updateinterval = 10;
        public int UpdateInterval
        {
            get { return updateinterval; }
            set { updateinterval = value; }
        }
        protected Uri feeduri;
        public Uri FeedUri
        {
            get { return feeduri; }
            set { feeduri = value; }
        }
        protected Collection<FeedItem> feeditems = new Collection<FeedItem>();
        public Collection<FeedItem> FeedItems
        {
            get { return feeditems; }
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
        private IWebProxy feedproxy;
        private FeedAuthTypes feedauthtype;
        private string feedusername;
        private string feedpassword;
        private DateTime lastupdate;
        public DateTime LastUpdate
        {
            get
            {
                return lastupdate;
            }
        }
        #endregion

        #region Methods
        public Feed()
        {
        }

        public Feed(Uri url, FeedAuthTypes authtype, string username, string password, IWebProxy proxy)
        {
            feedproxy = proxy;
            feeduri = url;
            feedauthtype = authtype;
            feedusername = username;
            feedpassword = password;
        }


        private XmlDocument Fetch(Uri feeduri)
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(feeduri);
            req.UserAgent = "Mozilla/5.0";
            req.Proxy = feedproxy;
            if (feedauthtype == FeedAuthTypes.Basic)
            {
                req.Credentials = new NetworkCredential(feedusername, feedpassword);
            }
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

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
                Feedxml = Fetch(feeduri);
                XPathNavigator nav;
                XPathNavigator subnav;
                XPathNodeIterator nodeiterator;
                nav = Feedxml.CreateNavigator();

                XmlNamespaceManager xnm = new XmlNamespaceManager(nav.NameTable);
                xnm.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                xnm.AddNamespace("rss", "http://purl.org/rss/1.0/");
                xnm.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
                XPathExpression xpe = nav.Compile("/rdf:RDF//rss:item");
                xpe.SetContext(xnm);
                nodeiterator = nav.Select(xpe);
                if (nav.SelectSingleNode("/rdf:RDF/rss:channel/rss:title/text()", xnm) != null)
                {
                    this.Title = System.Web.HttpUtility.HtmlDecode(nav.SelectSingleNode("/rdf:RDF/rss:channel/rss:title/text()", xnm).ToString()).Trim();
                }
                else
                {
                    this.Title = "(untitled)";
                }
                if (nav.SelectSingleNode("/rdf:RDF/rss:channel/rss:link/text()", xnm) != null)
                {
                    this.Url = new Uri(nav.SelectSingleNode("/rdf:RDF/rss:channel/rss:link/text()", xnm).ToString());
                }
                if (nav.SelectSingleNode("/rdf:RDF/rss:channel/rss:description/text()", xnm) != null)
                {
                    this.Description = nav.SelectSingleNode("/rdf:RDF/rss:channel/rss:description/text()", xnm).ToString().Trim();
                }
                feeditems.Clear();
                while (nodeiterator.MoveNext())
                {
                    if (nodeiterator.Current.HasChildren)
                    {
                        FeedItem item = new FeedItem();
                        subnav = nodeiterator.Current.CreateNavigator();
                        if (subnav.SelectSingleNode("rss:title", xnm) != null)
                        {
                            item.Title = subnav.SelectSingleNode("rss:title", xnm).ToString().Trim();
                        }
                        else
                        {
                            item.Title = "(untitled)";
                        }
                        if (subnav.SelectSingleNode("rss:link", xnm) != null)
                        {
                            item.Link = new Uri(subnav.SelectSingleNode("rss:link", xnm).ToString());
                        }
                        if (subnav.SelectSingleNode("rss:description", xnm) != null)
                        {
                            item.Description = subnav.SelectSingleNode("rss:description", xnm).ToString();
                        }
                        try
                        {
                            DateTime gdt;
                            XPathNavigator tempnav = subnav.SelectSingleNode("dc:date", xnm);
                            if (tempnav != null)
                            {
                                bool res = DateTime.TryParse(tempnav.ToString(), out gdt);
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
