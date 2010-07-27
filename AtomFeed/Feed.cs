using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Xml;
using System.Windows.Forms;
using System.Xml.XPath;
using FeedHanderPluginInterface;

namespace AtomFeed
{
    public class Feed : IFeed
    {
        #region Properties

        private bool loaded = false;

        public bool Loaded {
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
                return "AtomFeed";
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
        public Feed(Uri url,FeedAuthTypes authtype,string username,string password,IWebProxy proxy)
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
                XmlNamespaceManager xnm = new XmlNamespaceManager(nav.NameTable);
                xnm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                if (nav.SelectSingleNode("/atom:feed",xnm) != null)
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

        public IFeed Factory(Uri givenuri, IWebProxy proxy)
        {
            return new Feed(givenuri, FeedAuthTypes.None, "", "", proxy);
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
