using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using FeedHanderPluginInterface;

namespace RSSFeed
{
    public class Feed : IFeed
    {
        #region Properties
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
        private int updateinterval=10;
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
        protected List<FeedItem> feeditems = new List<FeedItem>();
        public List<FeedItem> FeedItems
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
                return "RSSFeed";
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

        public Feed(Uri url,FeedAuthTypes authtype,string username,string password, IWebProxy proxy)
        {
            feedproxy = proxy;
            feeduri = url;
            feedauthtype = authtype;
            feedusername = username;
            feedpassword = password;
        }
        public bool CanHandle(IXPathNavigable document)
        {
            if (document != null)
            {
                XPathNavigator nav = document.CreateNavigator();
                if (nav.SelectSingleNode("/rss") != null)
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
        public IFeed Factory(Uri givenuri, FeedAuthTypes authtype, string username, string password, IWebProxy proxy)
        {
            return new Feed(givenuri, authtype, username, password, proxy);
        }
        private XmlDocument Fetch(Uri feeduri)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(feeduri);
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
            catch
            {
                return null;
            }
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
                nodeiterator = nav.Select("/rss/channel/item");
                if (nav.SelectSingleNode("/rss/channel/title") == null)
                {
                    this.Title = "(untitled)";
                }
                else
                {
                    this.Title = nav.SelectSingleNode("/rss/channel/title").ToString().Trim();
                }
                if (nav.SelectSingleNode("/rss/channel/link") != null)
                {
                    this.Url = new Uri(nav.SelectSingleNode("/rss/channel/link").ToString());
                }
                if (nav.SelectSingleNode("/rss/channel/description") != null)
                {
                    this.Description = nav.SelectSingleNode("/rss/channel/description").ToString().Trim();
                }
                if (nav.SelectSingleNode("/rss/channel/image/url") != null)
                {
                    this.imageurl = new Uri(nav.SelectSingleNode("/rss/channel/image/url").ToString());
                }
                feeditems.Clear();
                while (nodeiterator.MoveNext())
                {
                    if (nodeiterator.Current.HasChildren)
                    {
                        FeedItem item = new FeedItem();
                        subnav = nodeiterator.Current.CreateNavigator();
                        if (subnav.SelectSingleNode("title") == null)
                        {
                            item.Title = "(untitled)";
                        }
                        else
                        {
                            item.Title = System.Web.HttpUtility.HtmlDecode(subnav.SelectSingleNode("title").ToString()).Trim();
                        }
                        if (subnav.SelectSingleNode("link") != null)
                        {
                            item.Link = new Uri(subnav.SelectSingleNode("link").ToString());
                        }

                        try
                        {
                            DateTime gdt;
                            XPathNavigator tempnav = subnav.SelectSingleNode("pubDate");
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

                        if (!oldfeeditems.Contains(item))
                        {
                            item.Updated = true;
                        }
                        feeditems.Add(item);
                    }

                }
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
            Updated(null, new EventArgs());
        }
        #endregion

        #region Events
        public virtual event EventHandler Updated;
        #endregion

        public string PluginVersion
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public string PluginCopyright
        {
            get { return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LegalCopyright; }
        }
    }
}
