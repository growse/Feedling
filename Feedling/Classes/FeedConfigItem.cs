/*
Copyright © 2008-2011, Andrew Rowson
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
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using FeedHanderPluginInterface;

namespace Feedling
{
    public class FeedConfigItem : Object
    {
        public FeedConfigItem()
        {
            this.FontFamilyString = Properties.Settings.Default.DefaultFontFamily;
            this.FontSize = Properties.Settings.Default.DefaultFontSize;
            this.FontStyleString = Properties.Settings.Default.DefaultFontStyle;
            this.FontWeightString = Properties.Settings.Default.DefaultFontWeight;
            this.TitleFontFamilyString = Properties.Settings.Default.DefaultTitleFontFamily;
            this.TitleFontSize = Properties.Settings.Default.DefaultTitleFontSize;
            this.TitleFontStyleString = Properties.Settings.Default.DefaultTitleFontStyle;
            this.TitleFontWeightString = Properties.Settings.Default.DefaultTitleFontWeight;

            this.DefaultColorR = Properties.Settings.Default.DefaultFeedColorR;
            this.DefaultColorG = Properties.Settings.Default.DefaultFeedColorG;
            this.DefaultColorB = Properties.Settings.Default.DefaultFeedColorB;

            this.HoverColorR = Properties.Settings.Default.DefaultFeedHoverColorR;
            this.HoverColorG = Properties.Settings.Default.DefaultFeedHoverColorG;
            this.HoverColorB = Properties.Settings.Default.DefaultFeedHoverColorB;
        }
        public FeedConfigItem Copy()
        {
            FeedConfigItem fci = new FeedConfigItem();
            fci.Url = this.Url;
            fci.DefaultColor = this.DefaultColor;
            fci.FontFamily = this.FontFamily;
            fci.FontSize = this.FontSize;
            fci.FontStyle = this.FontStyle;
            fci.FontWeight = this.FontWeight;
            fci.HoverColor = this.HoverColor;
            fci.Position = this.Position;
            fci.TitleFontFamily = this.TitleFontFamily;
            fci.TitleFontSize = this.TitleFontSize;
            fci.TitleFontStyle = this.TitleFontStyle;
            fci.TitleFontWeight = this.TitleFontWeight;
            fci.AuthType = this.AuthType;
            fci.UserName = this.UserName;
            fci.Password = this.Password;
            fci.ProxyAuth = this.ProxyAuth;
            fci.ProxyHost = this.ProxyHost;
            fci.ProxyType = this.ProxyType;
            fci.ProxyUser = this.ProxyUser;
            fci.ProxyPass = this.ProxyPass;
            fci.ProxyPort = this.ProxyPort;
            fci.UpdateInterval = this.UpdateInterval;
            fci.DisplayedItems = this.DisplayedItems;
            fci.guid = this.guid;
            return fci;
        }
        internal Point Position
        {
            get { return new Point(xpos, ypos); }
            set
            {
                xpos = value.X;
                ypos = value.Y;
            }
        }
        private double xpos = 5;
        [XmlAttribute("XPos")]
        public double XPos
        {
            get { return xpos; }
            set { xpos = value; }
        }
        private double ypos = 5;
        [XmlAttribute("YPos")]
        public double YPos
        {
            get { return ypos; }
            set { ypos = value; }
        }


        private string url = "";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings"), XmlAttribute("Url")]
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        internal Color DefaultColor
        {
            get { return Color.FromRgb((byte)defaultcolorr, (byte)defaultcolorg, (byte)defaultcolorb); }
            set
            {
                defaultcolorr = value.R;
                defaultcolorg = value.G;
                defaultcolorb = value.B;
            }
        }

        private byte defaultcolorr = 255;
        [XmlAttribute("DefaultColorR")]
        public byte DefaultColorR
        {
            get { return defaultcolorr; }
            set { defaultcolorr = value; }
        }
        private byte defaultcolorg = 255;
        [XmlAttribute("DefaultColorG")]
        public byte DefaultColorG
        {
            get { return defaultcolorg; }
            set { defaultcolorg = value; }
        }
        private byte defaultcolorb = 255;
        [XmlAttribute("DefaultColorB")]
        public byte DefaultColorB
        {
            get { return defaultcolorb; }
            set { defaultcolorb = value; }
        }

        internal Color HoverColor
        {
            get { return Color.FromRgb((byte)hovercolorr, (byte)hovercolorg, (byte)hovercolorb); }
            set
            {
                hovercolorr = value.R;
                hovercolorg = value.G;
                hovercolorb = value.B;
            }
        }

        private byte hovercolorr = 255;
        [XmlAttribute("HoverColorR")]
        public byte HoverColorR
        {
            get { return hovercolorr; }
            set { hovercolorr = value; }
        }
        private byte hovercolorg = 255;
        [XmlAttribute("HoverColorG")]
        public byte HoverColorG
        {
            get { return hovercolorg; }
            set { hovercolorg = value; }
        }
        private byte hovercolorb = 255;
        [XmlAttribute("HoverColorB")]
        public byte HoverColorB
        {
            get { return hovercolorb; }
            set { hovercolorb = value; }
        }


        private string fontfamily = FeedwinManager.thisinst.FontFamily.ToString();
        [XmlAttribute("FontFamily")]
        public string FontFamilyString
        {
            get { return fontfamily; }
            set { fontfamily = value; }
        }
        [XmlIgnore()]
        public FontFamily FontFamily
        {
            get { return new FontFamily(fontfamily); }
            set { if (value != null) { fontfamily = value.ToString(); } }
        }
        private double fontsize = FeedwinManager.thisinst.FontSize;
        [XmlAttribute("FontSize")]
        public double FontSize
        {
            get { return fontsize; }
            set { fontsize = value; }
        }
        private string fontstyle = FontConversions.FontStyleToString(FeedwinManager.thisinst.FontStyle);
        [XmlAttribute("FontStyle")]
        public string FontStyleString
        {
            get { return fontstyle; }
            set { fontstyle = value; }
        }
        [XmlIgnore()]
        public FontStyle FontStyle
        {
            get { return FontConversions.FontStyleFromString(fontstyle); }
            set { fontstyle = FontConversions.FontStyleToString(value); }
        }

        private string fontweight = FontConversions.FontWeightToString(FeedwinManager.thisinst.FontWeight);
        [XmlAttribute("FontWeight")]
        public string FontWeightString
        {
            get { return fontweight; }
            set { fontweight = value; }
        }
        [XmlIgnore()]
        public FontWeight FontWeight
        {
            get { return FontConversions.FontWeightFromString(fontweight); }
            set { fontweight = FontConversions.FontWeightToString(value); }
        }

        private string titlefontfamily = FeedwinManager.thisinst.FontFamily.ToString();
        [XmlAttribute("TitleFontFamily")]
        public string TitleFontFamilyString
        {
            get { return titlefontfamily; }
            set { titlefontfamily = value; }
        }
        [XmlIgnore()]
        public FontFamily TitleFontFamily
        {
            get { return new FontFamily(titlefontfamily); }
            set { if (value != null) { titlefontfamily = value.ToString(); } }
        }
        private double titlefontsize = FeedwinManager.thisinst.FontSize;
        [XmlAttribute("TitleFontSize")]
        public double TitleFontSize
        {
            get { return titlefontsize; }
            set { titlefontsize = value; }
        }
        private string titlefontstyle = FontConversions.FontStyleToString(FeedwinManager.thisinst.FontStyle);
        [XmlAttribute("TitleFontStyle")]
        public string TitleFontStyleString
        {
            get { return titlefontstyle; }
            set { titlefontstyle = value; }
        }
        [XmlIgnore()]
        public FontStyle TitleFontStyle
        {
            get { return FontConversions.FontStyleFromString(titlefontstyle); }
            set { titlefontstyle = FontConversions.FontStyleToString(value); }
        }
        private string titlefontweight = FontConversions.FontWeightToString(FeedwinManager.thisinst.FontWeight);
        [XmlAttribute("TitleFontWeight")]
        public string TitleFontWeightString
        {
            get { return titlefontweight; }
            set { titlefontweight = value; }
        }
        [XmlIgnore()]
        public FontWeight TitleFontWeight
        {
            get { return FontConversions.FontWeightFromString(titlefontweight); }
            set { titlefontweight = FontConversions.FontWeightToString(value); }
        }
        private int updateinterval = 10;
        [XmlAttribute("UpdateInterval")]
        public int UpdateInterval
        {
            get { if (updateinterval > 0) { return updateinterval; } else { return 10; } }
            set { updateinterval = value; }
        }

        private int displayeditems = 10;
        [XmlAttribute("DisplayedItems")]
        public int DisplayedItems
        {
            get { if (displayeditems > 0) { return displayeditems; } else { return 10; } }
            set { displayeditems = value; }
        }

        private FeedAuthTypes authtype = FeedAuthTypes.None;
        [XmlAttribute("AuthType")]
        public FeedAuthTypes AuthType
        {
            get { return authtype; }
            set { authtype = value; }
        }
        private string username;
        [XmlAttribute("Username")]
        public string UserName
        {
            get { return username; }
            set { username = value; }
        }

        private string password;
        [XmlAttribute("Password")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        private double width = 300;
        [XmlAttribute("Width")]
        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        private ProxyType proxytype;
        [XmlAttribute("Proxytype")]
        public ProxyType ProxyType
        {
            get { return proxytype; }
            set { proxytype = value; }
        }
        private string proxyhost;
        [XmlAttribute("Proxyhost")]
        public string ProxyHost
        {
            get { return proxyhost; }
            set { proxyhost = value; }
        }
        private int proxyport;
        [XmlAttribute("Proxyport")]
        public int ProxyPort
        {
            get { if (proxyport > 0 && proxyport < 63536) { return proxyport; } else { return 80; } }
            set { proxyport = value; }
        }
        private bool proxyauth;
        [XmlAttribute("Proxyauth")]
        public bool ProxyAuth
        {
            get { return proxyauth; }
            set { proxyauth = value; }
        }
        private string proxyuser;
        [XmlAttribute("Proxyuser")]
        public string ProxyUser
        {
            get { return proxyuser; }
            set { proxyuser = value; }
        }
        private string proxypass;
        [XmlAttribute("Proxypass")]
        public string ProxyPass
        {
            get { return proxypass; }
            set { proxypass = value; }
        }

        private Guid guid = Guid.NewGuid();
        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        public IWebProxy Proxy
        {
            get
            {
                IWebProxy proxy = null;
                switch (ProxyType)
                {
                    case ProxyType.Custom:
                        proxy = new WebProxy(ProxyHost, ProxyPort);
                        if (ProxyAuth)
                        {
                            string user, domain = null;
                            if (proxyuser.Contains("\\"))
                            {
                                string[] bits = proxyuser.Split("\\".ToCharArray(), 2);
                                user = bits[1];
                                domain = bits[0];
                            }
                            else
                            {
                                user = proxyuser;
                            }
                            proxy.Credentials = new NetworkCredential(user, ProxyPass, domain);
                        }
                        break;
                    case ProxyType.System:
                        proxy = WebRequest.GetSystemWebProxy();
                        break;
                    case ProxyType.None:
                    case ProxyType.Global:
                        proxy = null;
                        break;
                }
                return proxy;
            }
        }

        public override string ToString()
        {
            return url;
        }
    }
}
