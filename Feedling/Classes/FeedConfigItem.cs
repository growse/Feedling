/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using FeedHanderPluginInterface;

namespace Feedling
{
    public class FeedConfigItem : Object
    {
        public FeedConfigItem()
        {
            Url = "";
            YPos = 5;
            XPos = 5;
            Guid = Guid.NewGuid();
            AuthType = FeedAuthTypes.None;
            Width = 300;
            FontFamilyString = Properties.Settings.Default.DefaultFontFamily;
            FontSize = Properties.Settings.Default.DefaultFontSize;
            FontStyleString = Properties.Settings.Default.DefaultFontStyle;
            FontWeightString = Properties.Settings.Default.DefaultFontWeight;
            TitleFontFamilyString = Properties.Settings.Default.DefaultTitleFontFamily;
            TitleFontSize = Properties.Settings.Default.DefaultTitleFontSize;
            TitleFontStyleString = Properties.Settings.Default.DefaultTitleFontStyle;
            TitleFontWeightString = Properties.Settings.Default.DefaultTitleFontWeight;

            DefaultColorR = Properties.Settings.Default.DefaultFeedColorR;
            DefaultColorG = Properties.Settings.Default.DefaultFeedColorG;
            DefaultColorB = Properties.Settings.Default.DefaultFeedColorB;

            HoverColorR = Properties.Settings.Default.DefaultFeedHoverColorR;
            HoverColorG = Properties.Settings.Default.DefaultFeedHoverColorG;
            HoverColorB = Properties.Settings.Default.DefaultFeedHoverColorB;
        }

        /// <summary>
        /// Copy maintains equality between all variables but generates a new GUID.
        /// </summary>
        /// <returns>A new FeedConfigItem object with the same values but a different GUID.</returns>
        public FeedConfigItem Copy()
        {
            var fci = new FeedConfigItem
                          {
                              Url = Url,
                              DefaultColor = DefaultColor,
                              FontFamily = FontFamily,
                              FontSize = FontSize,
                              FontStyleString = FontStyleString,
                              FontWeightString = FontWeightString,
                              HoverColor = HoverColor,
                              Position = Position,
                              TitleFontFamily = TitleFontFamily,
                              TitleFontSize = TitleFontSize,
                              TitleFontStyleString = TitleFontStyleString,
                              TitleFontWeightString = TitleFontWeightString,
                              AuthType = AuthType,
                              UserName = UserName,
                              Password = Password,
                              ProxyAuth = ProxyAuth,
                              ProxyHost = ProxyHost,
                              ProxyType = ProxyType,
                              ProxyUser = ProxyUser,
                              ProxyPass = ProxyPass,
                              ProxyPort = ProxyPort,
                              UpdateInterval = UpdateInterval,
                              DisplayedItems = DisplayedItems,
                              Width = Width,
                              XPos = XPos,
                              YPos = YPos,
                              NotifyOnNewItem = NotifyOnNewItem,
                              FeedLabel = FeedLabel,
                              Guid = Guid.NewGuid()
                          };
            return fci;
        }
        internal Point Position
        {
            get { return new Point(XPos, YPos); }
            set
            {
                XPos = value.X;
                YPos = value.Y;
            }
        }

        [XmlAttribute("XPos")]
        public double XPos { get; set; }

        [XmlAttribute("YPos")]
        public double YPos { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings"), XmlAttribute("Url")]
        public string Url { get; set; }

        internal Color DefaultColor
        {
            get { return Color.FromRgb(DefaultColorR, DefaultColorG, DefaultColorB); }
            set
            {
                DefaultColorR = value.R;
                DefaultColorG = value.G;
                DefaultColorB = value.B;
            }
        }

        [XmlAttribute("DefaultColorR")]
        public byte DefaultColorR { get; set; }

        [XmlAttribute("DefaultColorG")]
        public byte DefaultColorG { get; set; }

        [XmlAttribute("DefaultColorB")]
        public byte DefaultColorB { get; set; }

        internal Color HoverColor
        {
            get { return Color.FromRgb(HoverColorR, hovercolorg, HoverColorB); }
            set
            {
                HoverColorR = value.R;
                hovercolorg = value.G;
                HoverColorB = value.B;
            }
        }

        [XmlAttribute("HoverColorR")]
        public byte HoverColorR { get; set; }

        private byte hovercolorg = 255;
        [XmlAttribute("HoverColorG")]
        public byte HoverColorG
        {
            get { return hovercolorg; }
            set { hovercolorg = value; }
        }

        [XmlAttribute("HoverColorB")]
        public byte HoverColorB { get; set; }


        [XmlAttribute("FontFamily")]
        public string FontFamilyString { get; set; }

        [XmlIgnore]
        public FontFamily FontFamily
        {
            get { return new FontFamily(FontFamilyString); }
            set { if (value != null) { FontFamilyString = value.ToString(); } }
        }

        [XmlAttribute("FontSize")]
        public double FontSize { get; set; }

        [XmlAttribute("FontStyle")]
        public string FontStyleString { get; set; }

        [XmlIgnore]
        public FontStyle FontStyle
        {
            get { return FontConversions.FontStyleFromString(FontStyleString); }
            set { FontStyleString = FontConversions.FontStyleToString(value); }
        }

        [XmlAttribute("FontWeight")]
        public string FontWeightString { get; set; }

        [XmlIgnore]
        public FontWeight FontWeight
        {
            get { return FontConversions.FontWeightFromString(FontWeightString); }
            set { FontWeightString = FontConversions.FontWeightToString(value); }
        }

        [XmlAttribute("TitleFontFamily")]
        public string TitleFontFamilyString { get; set; }

        [XmlIgnore]
        public FontFamily TitleFontFamily
        {
            get { return new FontFamily(TitleFontFamilyString); }
            set { if (value != null) { TitleFontFamilyString = value.ToString(); } }
        }

        [XmlAttribute("TitleFontSize")]
        public double TitleFontSize { get; set; }

        [XmlAttribute("TitleFontStyle")]
        public string TitleFontStyleString { get; set; }

        [XmlIgnore]
        public FontStyle TitleFontStyle
        {
            get { return FontConversions.FontStyleFromString(TitleFontStyleString); }
            set { TitleFontStyleString = FontConversions.FontStyleToString(value); }
        }

        [XmlAttribute("TitleFontWeight")]
        public string TitleFontWeightString { get; set; }

        [XmlIgnore]
        public FontWeight TitleFontWeight
        {
            get { return FontConversions.FontWeightFromString(TitleFontWeightString); }
            set { TitleFontWeightString = FontConversions.FontWeightToString(value); }
        }
        private int updateinterval = 10;
        [XmlAttribute("UpdateInterval")]
        public int UpdateInterval
        {
            get { return updateinterval > 0 ? updateinterval : 10; }
            set { updateinterval = value; }
        }

        private int displayeditems = 10;
        [XmlAttribute("DisplayedItems")]
        public int DisplayedItems
        {
            get { return displayeditems > 0 ? displayeditems : 10; }
            set { displayeditems = value; }
        }

        [XmlAttribute("AuthType")]
        public FeedAuthTypes AuthType { get; set; }

        [XmlAttribute("Username")]
        public string UserName { get; set; }

        [XmlAttribute("Password")]
        public string Password { get; set; }

        [XmlAttribute("Width")]
        public double Width { get; set; }

        [XmlAttribute("NotifyOnNewItem")]
        public bool NotifyOnNewItem { get; set; }

        [XmlAttribute("FeedLabel")]
        public string FeedLabel { get; set; }

        [XmlAttribute("Proxytype")]
        public ProxyType ProxyType { get; set; }

        [XmlAttribute("Proxyhost")]
        public string ProxyHost { get; set; }

        private int proxyport;
        [XmlAttribute("Proxyport")]
        public int ProxyPort
        {
            get { if (proxyport > 0 && proxyport < 63536) { return proxyport; } else { return 80; } }
            set { proxyport = value; }
        }

        [XmlAttribute("Proxyauth")]
        public bool ProxyAuth { get; set; }

        [XmlAttribute("Proxyuser")]
        public string ProxyUser { get; set; }

        [XmlAttribute("Proxypass")]
        public string ProxyPass { get; set; }

        public Guid Guid { get; set; }

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
                            if (ProxyUser.Contains("\\"))
                            {
                                string[] bits = ProxyUser.Split("\\".ToCharArray(), 2);
                                user = bits[1];
                                domain = bits[0];
                            }
                            else
                            {
                                user = ProxyUser;
                            }
                            proxy.Credentials = new NetworkCredential(user, ProxyPass, domain);
                        }
                        break;
                    case ProxyType.System:
                        proxy = WebRequest.GetSystemWebProxy();
                        break;
                    case ProxyType.None:
                    case ProxyType.Global:
                        break;
                }
                return proxy;
            }
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(FeedLabel) ? Url : FeedLabel;
        }

        public override bool Equals(object obj)
        {
            var fci = obj as FeedConfigItem;
            return fci != null && GetHashCode() == fci.GetHashCode();
        }
        public override int GetHashCode()
        {
            return string.Concat(Url, Guid.ToString()).GetHashCode();
        }
    }
}
