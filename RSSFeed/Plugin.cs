/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Net;
using System.Reflection;
using System.Security;
using System.Xml.XPath;
using FeedHanderPluginInterface;

namespace RssFeed
{
    class Plugin : IPlugin
    {
        public string PluginName
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                // If there is a Product attribute, return its value
                return attributes.Length == 0 ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
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

        public bool CanHandle(IXPathNavigable document)
        {
            if (document != null)
            {
                var nav = document.CreateNavigator();
                return nav.SelectSingleNode("/rss") != null;
            }
            return false;
        }

        public IFeed AddFeed(Uri uri, FeedAuthTypes feedAuthType, string username, string password, IWebProxy reqproxy)
        {
            return new Feed(uri, feedAuthType, username, password, reqproxy);
        }
    }
}
