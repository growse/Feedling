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
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.XPath;

namespace FeedHanderPluginInterface
{
    /// <summary>
    /// Each plugin must implement the IPlugin interface. Feedling will
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// The plugin name
        /// </summary>
        string PluginName { get; }
        /// <summary>
        /// The version of the plugin assembly
        /// </summary>
        string PluginVersion { get; }
        /// <summary>
        /// Copyright information about the plugin.
        /// </summary>
        string PluginCopyright { get; }
        /// <summary>
        /// Checks if this feed is capable of handling the supplied xml document.
        /// </summary>
        /// <param name="xml">The XML returned by the FeedURI</param>
        /// <returns>True if this plugin should handle this type of feed</returns>
        bool CanHandle(IXPathNavigable xml);
        /// <summary>
        /// Called to create a new instance of a feed with the given properties. Creates the plugin-specific version of the feed and returns to Feedling
        /// </summary>
        /// <param name="uri">Feed URI</param>
        /// <param name="feedAuthType">The authentication type used by the feed</param>
        /// <param name="username">The authentication username</param>
        /// <param name="password">The authentication password</param>
        /// <param name="reqproxy">The proxy to be used for the request</param>
        /// <returns></returns>
        IFeed AddFeed(Uri uri, FeedAuthTypes feedAuthType, string username, string password, IWebProxy reqproxy);
    }
}
