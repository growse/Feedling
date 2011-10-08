﻿/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FeedHanderPluginInterface;
using NLog;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for NewFeed.xaml
    /// </summary>
    public partial class NewFeed : Window
    {
        private Logger Log = LogManager.GetCurrentClassLogger();
        public NewFeed()
        {
            Log.Debug("Loading NewFeed Window");
            fci = new FeedConfigItem();
            InitializeComponent();
        }
        public NewFeed(FeedConfigItem givenfci)
        {
            fci = givenfci;
            Log.Debug("Loading NewFeed Window with given feed {0}", fci.Url);
            InitializeComponent();
            this.Title = string.Format("Edit {0}", givenfci.Url);
        }

        private FeedConfigItem fci;
        public FeedConfigItem FeedConfig
        {
            get { return fci; }
        }

        private void LoadFeedConfigItem()
        {
            Log.Debug("Loading settings from the feedconfigitem");
            try
            {
                urlbox.Text = fci.Url;
                defaultcolourbox.Fill = new SolidColorBrush(Color.FromRgb((byte)fci.DefaultColorR, (byte)fci.DefaultColorG, (byte)fci.DefaultColorB));
                hovercolourbox.Fill = new SolidColorBrush(Color.FromRgb((byte)fci.HoverColorR, (byte)fci.HoverColorG, (byte)fci.HoverColorB));
                titlefontlabel.FontFamily = fci.TitleFontFamily;
                titlefontlabel.FontSize = fci.TitleFontSize;
                titlefontlabel.FontWeight = fci.TitleFontWeight;
                titlefontlabel.FontStyle = fci.TitleFontStyle;
                titlefontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}", fci.TitleFontFamily, fci.TitleFontSize, fci.TitleFontStyle, fci.TitleFontWeight);
                fontlabel.FontFamily = fci.FontFamily;
                fontlabel.FontSize = fci.FontSize;
                fontlabel.FontWeight = fci.FontWeight;
                fontlabel.FontStyle = fci.FontStyle;
                fontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}", fci.FontFamily, fci.FontSize, fci.FontStyle, fci.FontWeight);

                updateintervalbox.Text = fci.UpdateInterval.ToString();
                displayeditemsbox.Text = fci.DisplayedItems.ToString();

                usernamebox.Text = fci.UserName;
                passwordbox.Password = fci.Password;
                switch (fci.AuthType)
                {
                    case FeedAuthTypes.Other:
                        otherauthradio.IsChecked = true;
                        break;
                    case FeedAuthTypes.Basic:
                        httpauthradio.IsChecked = true;
                        break;
                    default:
                        noauthradio.IsChecked = true;
                        break;
                }
                usernamebox.IsEnabled = (noauthradio.IsChecked == false);
                passwordbox.IsEnabled = (noauthradio.IsChecked == false);
                //Proxy
                switch (fci.ProxyType)
                {
                    case ProxyType.Global:
                        globalproxybtn.IsChecked = true;
                        break;
                    case ProxyType.System:
                        systemproxybtn.IsChecked = true;
                        break;
                    case ProxyType.None:
                        noproxybtn.IsChecked = true;
                        break;
                    case ProxyType.Custom:
                        customproxybtn.IsChecked = true;
                        proxyhostbox.IsEnabled = proxyportbox.IsEnabled = proxyauthcheck.IsEnabled = proxyuserbox.IsEnabled = proxypassbox.IsEnabled = true;
                        break;
                }
                proxyhostbox.Text = fci.ProxyHost;
                proxyportbox.Text = fci.ProxyPort.ToString();
                proxyauthcheck.IsChecked = fci.ProxyAuth;
                proxyuserbox.Text = fci.ProxyUser;
                proxypassbox.Password = fci.ProxyPass;
                proxyuserbox.IsEnabled = proxypassbox.IsEnabled = (proxyauthcheck.IsChecked == true);
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown when loading Newfeed Window with settings", ex);
                throw ex;
            }
        }

        #region Events

        private void proxyauthcheck_Checked(object sender, RoutedEventArgs e)
        {
            proxyuserbox.IsEnabled = proxypassbox.IsEnabled = (proxyauthcheck.IsChecked == true);
        }

        private void proxyradio_Checked(object sender, RoutedEventArgs e)
        {
            proxyhostbox.IsEnabled = proxyportbox.IsEnabled = proxyauthcheck.IsEnabled = (customproxybtn.IsChecked == true);
            proxyuserbox.IsEnabled = proxypassbox.IsEnabled = (proxyauthcheck.IsEnabled == true && proxyauthcheck.IsChecked == true);
        }

        private void authradio_Checked(object sender, RoutedEventArgs e)
        {
            fci.AuthType = FeedAuthTypes.None;
            if (httpauthradio != null && httpauthradio.IsChecked == true)
            {
                fci.AuthType = FeedAuthTypes.Basic;
            }
            if (otherauthradio != null && otherauthradio.IsChecked == true)
            {
                fci.AuthType = FeedAuthTypes.Other;
            }
            if (usernamebox != null && passwordbox != null)
            {
                usernamebox.IsEnabled = passwordbox.IsEnabled = (fci.AuthType != FeedAuthTypes.None);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            urlbox.Focus();
            LoadFeedConfigItem();
        }

        private void okbtn_Click(object sender, RoutedEventArgs e)
        {
            fci.Url = urlbox.Text;
            fci.AuthType = FeedAuthTypes.None;
            if (httpauthradio.IsChecked == true)
            {
                fci.AuthType = FeedAuthTypes.Basic;
            }
            if (otherauthradio.IsChecked == true)
            {
                fci.AuthType = FeedAuthTypes.Other;
            }

            fci.FontFamily = fontlabel.FontFamily;
            fci.FontSize = fontlabel.FontSize;
            fci.FontStyle = fontlabel.FontStyle;
            fci.FontWeight = fontlabel.FontWeight;
            fci.Password = passwordbox.Password;

            fci.ProxyType = ProxyType.Global;
            if (noproxybtn.IsChecked == true)
            {
                fci.ProxyType = ProxyType.None;
            }
            if (systemproxybtn.IsChecked == true)
            {
                fci.ProxyType = ProxyType.System;
            }
            if (customproxybtn.IsChecked == true)
            {
                fci.ProxyType = ProxyType.Custom;
            }
            fci.ProxyAuth = (proxyauthcheck.IsChecked == true);
            fci.ProxyHost = proxyhostbox.Text;
            fci.ProxyPass = proxypassbox.Password;
            int proxyport = fci.ProxyPort;
            if (!string.IsNullOrEmpty(proxyportbox.Text) && int.TryParse(proxyportbox.Text, out proxyport) && proxyport > 0 && proxyport < 63536)
            {
                fci.ProxyPort = proxyport;
            }

            fci.ProxyUser = proxyuserbox.Text;
            fci.TitleFontFamily = titlefontlabel.FontFamily;
            fci.TitleFontSize = titlefontlabel.FontSize;
            fci.TitleFontStyle = titlefontlabel.FontStyle;
            fci.TitleFontWeight = titlefontlabel.FontWeight;
            fci.DefaultColor = ((SolidColorBrush)defaultcolourbox.Fill).Color;
            fci.HoverColor = ((SolidColorBrush)hovercolourbox.Fill).Color;
            int updateinterval = fci.UpdateInterval;
            if (!string.IsNullOrEmpty(updateintervalbox.Text) && int.TryParse(updateintervalbox.Text, out updateinterval) && updateinterval > 0)
            {
                fci.UpdateInterval = updateinterval;
            }

            int displayeditems = fci.DisplayedItems;
            if (!string.IsNullOrEmpty(displayeditemsbox.Text) && int.TryParse(displayeditemsbox.Text, out displayeditems) && displayeditems > 0)
            {
                fci.DisplayedItems = displayeditems;
            }

            fci.Url = urlbox.Text;
            fci.UserName = usernamebox.Text;

            if (fci.Url.Trim().Length > 0)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Feed URL cannot be blank", "Blank URL", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void fontchooserbtn_Click(object sender, RoutedEventArgs e)
        {
            FontChooser fc = new FontChooser();
            fc.SelectedFontFamily = fontlabel.FontFamily;
            fc.SelectedFontSize = fontlabel.FontSize;
            fc.SelectedFontStyle = fontlabel.FontStyle;
            fc.SelectedFontWeight = fontlabel.FontWeight;

            Nullable<bool> dr = fc.ShowDialog();
            if (dr == true)
            {
                fontlabel.FontFamily = fc.SelectedFontFamily;
                fontlabel.FontSize = fc.SelectedFontSize;
                fontlabel.FontStyle = fc.SelectedFontStyle;
                fontlabel.FontWeight = fc.SelectedFontWeight;
                fontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}", fontlabel.FontFamily, fontlabel.FontSize, fontlabel.FontStyle, fontlabel.FontWeight);
            }
        }

        private void defaultcolorchooserbtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            System.Drawing.Color initialcol = System.Drawing.Color.FromArgb(((SolidColorBrush)defaultcolourbox.Fill).Color.R, ((SolidColorBrush)defaultcolourbox.Fill).Color.G, ((SolidColorBrush)defaultcolourbox.Fill).Color.B);
            cd.Color = initialcol;
            System.Windows.Forms.DialogResult dr = cd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                defaultcolourbox.Fill = new SolidColorBrush(Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
            }
        }

        private void titlefontchooserbtn_Click(object sender, RoutedEventArgs e)
        {
            FontChooser fc = new FontChooser();
            fc.SelectedFontFamily = titlefontlabel.FontFamily;
            fc.SelectedFontSize = titlefontlabel.FontSize;
            fc.SelectedFontStyle = titlefontlabel.FontStyle;
            fc.SelectedFontWeight = titlefontlabel.FontWeight;
            Nullable<bool> dr = fc.ShowDialog();
            if (dr == true)
            {
                titlefontlabel.FontFamily = fc.SelectedFontFamily;
                titlefontlabel.FontSize = fc.SelectedFontSize;
                titlefontlabel.FontStyle = fc.SelectedFontStyle;
                titlefontlabel.FontWeight = fc.SelectedFontWeight;
                titlefontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}", titlefontlabel.FontFamily, titlefontlabel.FontSize, titlefontlabel.FontStyle, titlefontlabel.FontWeight);
            }
        }

        private void hovercolorchooserbtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            System.Drawing.Color initialcol = System.Drawing.Color.FromArgb(((SolidColorBrush)hovercolourbox.Fill).Color.R, ((SolidColorBrush)hovercolourbox.Fill).Color.G, ((SolidColorBrush)hovercolourbox.Fill).Color.B);
            cd.Color = initialcol;
            System.Windows.Forms.DialogResult dr = cd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                hovercolourbox.Fill = new SolidColorBrush(Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
            }
        }
        #endregion
    }
}
