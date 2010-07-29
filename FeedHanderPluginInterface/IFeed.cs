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
using System.Net;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Windows.Forms;

namespace FeedHanderPluginInterface
{
    public interface IFeed
    {
        /// <summary>
        /// Called by the host when it has initialized the feed object and it's ready to begin listening.
        /// The host calls this method as a new thread.
        /// </summary>
        void Watch(object state);
        /// <summary>
        /// A list of the FeedItems that currently exist in the feed
        /// </summary>
        List<FeedItem> FeedItems { get; }
        /// <summary>
        /// The Uri that is regularly fetched by the feed
        /// </summary>
        Uri FeedUri { get; set; }
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
        /// A text description of the feed
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// A text title of the feed
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// The URL of a background image that the feed may specify
        /// </summary>
        Uri ImageUrl { get; set; }
        /// <summary>
        /// Flag indicating whether the feed has been fetched, parsed and loaded.
        /// </summary>
        bool Loaded { get; set; }
        /// <summary>
        /// Flag indicating whether there was an error fetching the feed
        /// </summary>
        bool HasError { get; set; }
        /// <summary>
        /// The error that was generated
        /// </summary>
        string ErrorMessage { get; set; }
        /// <summary>
        /// Called by the host when an instant update is required
        /// </summary>
        void Update();
        /// <summary>
        /// This event is bound to by the host to be alerted when the plugin has finished updating the feed
        /// </summary>
        event EventHandler Updated;
        /// <summary>
        /// In minutes, how often the plugin should refresh
        /// </summary>
        int UpdateInterval { get; set; }
        /// <summary>
        /// The URL specified by the feed. This URL will be started when the user clicks a feed item
        /// </summary>
        Uri Url { get; set; }
        /// <summary>
        /// Checks if this feed is capable of handling the supplied xml document.
        /// </summary>
        /// <param name="xml">The XML returned by the FeedURI</param>
        /// <returns>True if this plugin should handle this type of feed</returns>
        bool CanHandle(IXPathNavigable xml);
        /// <summary>
        /// Supplies a new instance of the plugin when requested by the host
        /// </summary>
        /// <param name="uri">The URI of the feed</param>
        /// <param name="authtype">The authentication type used by the feed as specified in FeedAuthTypes</param>
        /// <param name="username">The authentication username</param>
        /// <param name="password">The authentication password</param>
        /// <returns>An IFeed which will regularly update and fetch feed items</returns>
        IFeed Factory(Uri uri, FeedAuthTypes authtype, string username, string password, IWebProxy proxy);
        /// <summary>
        /// The Configuration Form.
        /// </summary>
        Form ConfigForm { get; }
    }
}
