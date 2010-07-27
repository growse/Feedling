using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace Feedling
{
    [XmlRoot("FeedConfigItemList")]
    public class FeedConfigItemList
    {
        private ArrayList feedconfiglist;
        public FeedConfigItemList()
        {
            feedconfiglist = new ArrayList();
        }
        public int Add(FeedConfigItem fci)
        {
            return feedconfiglist.Add(fci);
        }
        public void Remove(FeedConfigItem fci)
        {
            feedconfiglist.Remove(fci);
        }
        [XmlElement("item")]
        public FeedConfigItem[] Items
        {
            get
            {
                FeedConfigItem[] items = new FeedConfigItem[feedconfiglist.Count];
                feedconfiglist.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null) return;
                FeedConfigItem[] items = (FeedConfigItem[])value;
                feedconfiglist.Clear();
                foreach (FeedConfigItem item in items)
                {
                    feedconfiglist.Add(item);
                }
            }
        }
    }
}
