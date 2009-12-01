using System;

namespace FeedHanderPluginInterface
{
    public sealed class FeedItem : ICloneable
    {
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        private Uri link;

        public Uri Link
        {
            get { return link; }
            set { link = value; }
        }

        private bool updated;

        public bool Updated
        {
            get { return updated; }
            set { updated = value; }
        }

        public override bool Equals(object obj)
        {
            FeedItem testitem = (FeedItem)obj;
            if (testitem.Date == this.Date && testitem.Description == this.Description && testitem.Link == this.Link && testitem.Title == this.Title)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Link.GetHashCode() ^ this.Description.GetHashCode() ^ this.Title.GetHashCode() ^ this.Date.GetHashCode();
        }

        #region ICloneable Members

        public object Clone()
        {
            FeedItem Copy = new FeedItem();
            Copy.Date = this.Date;
            Copy.Description = this.Description;
            Copy.Link = this.Link;
            Copy.Title = this.Title;
            Copy.Updated = false;
            return Copy;
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
