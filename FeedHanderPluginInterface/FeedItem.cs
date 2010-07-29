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
