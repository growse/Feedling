/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Net;
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
