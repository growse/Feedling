/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Net;
using System.Reflection;
using System.Security;
using System.Xml;
using System.Xml.XPath;
using FeedHanderPluginInterface;

namespace AtomFeed
{
    public class Plugin : IPlugin
    {

        public string PluginName
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                return attributes.Length == 0 ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
                // If there is a Product attribute, return its value
            }
        }

        public string PluginVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public string PluginCopyright
        {
            [SecurityCriticalAttribute]
            get { return System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).LegalCopyright; }
        }

        public bool CanHandle(IXPathNavigable xml)
        {
            if (xml != null)
            {
                var nav = xml.CreateNavigator();
                if (nav.NameTable != null)
                {
                    var xnm = new XmlNamespaceManager(nav.NameTable);
                    xnm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                    return nav.SelectSingleNode("/atom:feed", xnm) != null;
                }
            }
            return false;
        }

        public IFeed AddFeed(Uri uri, FeedAuthTypes feedAuthType, string username, string password, IWebProxy reqproxy)
        {
            return new Feed(uri, feedAuthType, username, password, reqproxy);
        }
    }
}
