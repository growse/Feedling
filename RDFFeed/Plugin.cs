/*
Copyright © 2008-2011, Andrew Rowson
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

namespace RdfFeed
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
                if (nav.NameTable != null)
                {
                    var xnm = new XmlNamespaceManager(nav.NameTable);
                    xnm.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                    xnm.AddNamespace("rss", "http://purl.org/rss/1.0/");
                    xnm.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
                    return nav.SelectSingleNode("/rdf:RDF", xnm) != null;
                }
            }
            return false;
        }

        public IFeed AddFeed(Uri uri, FeedAuthTypes feedAuthTypes, string username, string password, IWebProxy reqproxy)
        {
            return new Feed(uri, feedAuthTypes, username, password, reqproxy);
        }
    }
}
