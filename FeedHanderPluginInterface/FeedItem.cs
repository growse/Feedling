/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;

namespace FeedHanderPluginInterface
{
    public sealed class FeedItem : ICloneable
    {
        public DateTime Date { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Uri Link { get; set; }

        public bool Updated { get; set; }

        public override bool Equals(object obj)
        {
            var testitem = (FeedItem)obj;
            return testitem.Date == this.Date && testitem.Description == this.Description && testitem.Link == this.Link && testitem.Title == this.Title;
        }

        public override int GetHashCode()
        {
            return this.Link.GetHashCode() ^ this.Description.GetHashCode() ^ this.Title.GetHashCode() ^ this.Date.GetHashCode();
        }

        #region ICloneable Members

        public object Clone()
        {
            var copy = new FeedItem
                           {
                               Date = this.Date,
                               Description = this.Description,
                               Link = this.Link,
                               Title = this.Title,
                               Updated = false
                           };
            return copy;
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public enum FeedAuthTypes
    {
        None=0,
        Basic,
        Other,
    }
}
