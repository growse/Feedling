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
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;
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

        public bool CanHandle(IXPathNavigable document)
        {
            if (document != null)
            {
                XPathNavigator nav = document.CreateNavigator();
                XmlNamespaceManager xnm = new XmlNamespaceManager(nav.NameTable);
                xnm.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                xnm.AddNamespace("rss", "http://purl.org/rss/1.0/");
                xnm.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
                if (nav.SelectSingleNode("/rdf:RDF", xnm) != null)
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

        public IFeed AddFeed(Uri uri, FeedAuthTypes feedAuthTypes, string username, string password, IWebProxy reqproxy)
        {
            return new Feed(uri, feedAuthTypes, username, password, reqproxy);
        }
    }
}