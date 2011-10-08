/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace Feedling
{
    [XmlRoot("FeedConfigItemList")]
    public class FeedConfigItemList
    {
        private Collection<FeedConfigItem> feedConfigList;
        public FeedConfigItemList()
        {
            feedConfigList = new Collection<FeedConfigItem>();
        }
        public void Add(FeedConfigItem feedConfigItem)
        {
            feedConfigList.Add(feedConfigItem);
        }
        public void Remove(FeedConfigItem feedConfigItem)
        {
            feedConfigList.Remove(feedConfigItem);
        }
        [XmlElement("item")]
        public Collection<FeedConfigItem> Items
        {
            get
            {
                return feedConfigList;
            }
            
        }
    }
}
