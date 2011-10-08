/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Collections.ObjectModel;

[assembly: CLSCompliant(false)]
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
        Collection<FeedItem> FeedItems { get; }
        /// <summary>
        /// The Uri that is regularly fetched by the feed
        /// </summary>
        Uri FeedUri { get; set; }
        
        /// <summary>
        /// A text description of the feed
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// A text title of the feed
        /// </summary>
        string Title { get; set; }
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
        /// When was the Feed last successfully updated?
        /// </summary>
        DateTime LastUpdate { get; }
        /// <summary>
        /// The URL specified by the feed. This URL will be started when the user clicks a feed item
        /// </summary>
        Uri Url { get; set; }
    }
}
