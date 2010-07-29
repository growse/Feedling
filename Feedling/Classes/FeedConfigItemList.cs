/*
 * Copyright © 2008-2010, Andrew Rowson
 * http://www.growse.com
 * 
 *  This file is part of Feedling.

    Feedling is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Feedling is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Feedling.  If not, see <http://www.gnu.org/licenses/>.

*/
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
