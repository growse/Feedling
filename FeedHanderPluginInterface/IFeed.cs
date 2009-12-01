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
