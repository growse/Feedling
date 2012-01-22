/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using FeedHanderPluginInterface;
using Feedling.Classes;
using NLog;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for NewFeed.xaml
    /// </summary>
    public partial class NewFeed
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();
        public NewFeed()
        {
            Log.Debug("Loading NewFeed Window");
            FeedConfig = new FeedConfigItem();
            InitializeComponent();
        }
        public NewFeed(FeedConfigItem givenfci)
        {
            FeedConfig = givenfci;
            Log.Debug("Loading NewFeed Window with given feed {0}", FeedConfig.Url);
            InitializeComponent();
            Title = string.Format("Edit {0}", givenfci.Url);
        }

        public FeedConfigItem FeedConfig { get; private set; }

        private void LoadFeedConfigItem()
        {
            Log.Debug("Loading settings from the feedconfigitem");
            try
            {
                urlbox.Text = FeedConfig.Url;
                defaultcolourbox.Fill = new SolidColorBrush(Color.FromRgb(FeedConfig.DefaultColorR, FeedConfig.DefaultColorG, FeedConfig.DefaultColorB));
                hovercolourbox.Fill = new SolidColorBrush(Color.FromRgb(FeedConfig.HoverColorR, FeedConfig.HoverColorG, FeedConfig.HoverColorB));
                titlefontlabel.FontFamily = FeedConfig.TitleFontFamily;
                titlefontlabel.FontSize = FeedConfig.TitleFontSize;
                titlefontlabel.FontWeight = FeedConfig.TitleFontWeight;
                titlefontlabel.FontStyle = FeedConfig.TitleFontStyle;
                titlefontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}", FeedConfig.TitleFontFamily, FeedConfig.TitleFontSize, FeedConfig.TitleFontStyle, FeedConfig.TitleFontWeight);
                fontlabel.FontFamily = FeedConfig.FontFamily;
                fontlabel.FontSize = FeedConfig.FontSize;
                fontlabel.FontWeight = FeedConfig.FontWeight;
                fontlabel.FontStyle = FeedConfig.FontStyle;
                fontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}", FeedConfig.FontFamily, FeedConfig.FontSize, FeedConfig.FontStyle, FeedConfig.FontWeight);

                updateintervalbox.Text = FeedConfig.UpdateInterval.ToString(CultureInfo.InvariantCulture);
                displayeditemsbox.Text = FeedConfig.DisplayedItems.ToString(CultureInfo.InvariantCulture);

                usernamebox.Text = FeedConfig.UserName;
                passwordbox.Password = FeedConfig.Password;
                switch (FeedConfig.AuthType)
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
                switch (FeedConfig.ProxyType)
                {
                    case HttpProxyHelper.ProxyType.Global:
                        globalproxybtn.IsChecked = true;
                        break;
                    case HttpProxyHelper.ProxyType.System:
                        systemproxybtn.IsChecked = true;
                        break;
                    case HttpProxyHelper.ProxyType.None:
                        noproxybtn.IsChecked = true;
                        break;
                    case HttpProxyHelper.ProxyType.Custom:
                        customproxybtn.IsChecked = true;
                        proxyhostbox.IsEnabled = proxyportbox.IsEnabled = proxyauthcheck.IsEnabled = proxyuserbox.IsEnabled = proxypassbox.IsEnabled = true;
                        break;
                }
                proxyhostbox.Text = FeedConfig.ProxyHost;
                proxyportbox.Text = FeedConfig.ProxyPort.ToString(CultureInfo.InvariantCulture);
                proxyauthcheck.IsChecked = FeedConfig.ProxyAuth;
                proxyuserbox.Text = FeedConfig.ProxyUser;
                proxypassbox.Password = FeedConfig.ProxyPass;
                proxyuserbox.IsEnabled = proxypassbox.IsEnabled = (proxyauthcheck.IsChecked == true);
                notificationcheckbox.IsChecked = FeedConfig.NotifyOnNewItem;
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown when loading Newfeed Window with settings", ex);
                throw;
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
            proxyuserbox.IsEnabled = proxypassbox.IsEnabled = (proxyauthcheck.IsEnabled && proxyauthcheck.IsChecked == true);
        }

        private void authradio_Checked(object sender, RoutedEventArgs e)
        {
            FeedConfig.AuthType = FeedAuthTypes.None;
            if (httpauthradio != null && httpauthradio.IsChecked == true)
            {
                FeedConfig.AuthType = FeedAuthTypes.Basic;
            }
            if (otherauthradio != null && otherauthradio.IsChecked == true)
            {
                FeedConfig.AuthType = FeedAuthTypes.Other;
            }
            if (usernamebox != null && passwordbox != null)
            {
                usernamebox.IsEnabled = passwordbox.IsEnabled = (FeedConfig.AuthType != FeedAuthTypes.None);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            urlbox.Focus();
            LoadFeedConfigItem();
        }

        private void okbtn_Click(object sender, RoutedEventArgs e)
        {
            FeedConfig.Url = urlbox.Text;
            FeedConfig.AuthType = FeedAuthTypes.None;
            if (httpauthradio.IsChecked == true)
            {
                FeedConfig.AuthType = FeedAuthTypes.Basic;
            }
            if (otherauthradio.IsChecked == true)
            {
                FeedConfig.AuthType = FeedAuthTypes.Other;
            }

            FeedConfig.FontFamily = fontlabel.FontFamily;
            FeedConfig.FontSize = fontlabel.FontSize;
            FeedConfig.FontStyle = fontlabel.FontStyle;
            FeedConfig.FontWeight = fontlabel.FontWeight;
            FeedConfig.Password = passwordbox.Password;

            FeedConfig.ProxyType = HttpProxyHelper.ProxyType.Global;
            if (noproxybtn.IsChecked == true)
            {
                FeedConfig.ProxyType = HttpProxyHelper.ProxyType.None;
            }
            if (systemproxybtn.IsChecked == true)
            {
                FeedConfig.ProxyType = HttpProxyHelper.ProxyType.System;
            }
            if (customproxybtn.IsChecked == true)
            {
                FeedConfig.ProxyType = HttpProxyHelper.ProxyType.Custom;
            }
            FeedConfig.ProxyAuth = (proxyauthcheck.IsChecked == true);
            FeedConfig.ProxyHost = proxyhostbox.Text;
            FeedConfig.ProxyPass = proxypassbox.Password;
            int proxyport;
            if (!string.IsNullOrEmpty(proxyportbox.Text) && int.TryParse(proxyportbox.Text, out proxyport) && proxyport > 0 && proxyport < 63536)
            {
                FeedConfig.ProxyPort = proxyport;
            }

            FeedConfig.ProxyUser = proxyuserbox.Text;
            FeedConfig.TitleFontFamily = titlefontlabel.FontFamily;
            FeedConfig.TitleFontSize = titlefontlabel.FontSize;
            FeedConfig.TitleFontStyle = titlefontlabel.FontStyle;
            FeedConfig.TitleFontWeight = titlefontlabel.FontWeight;
            FeedConfig.DefaultColor = ((SolidColorBrush)defaultcolourbox.Fill).Color;
            FeedConfig.HoverColor = ((SolidColorBrush)hovercolourbox.Fill).Color;
            int updateinterval;
            if (!string.IsNullOrEmpty(updateintervalbox.Text) && int.TryParse(updateintervalbox.Text, out updateinterval) && updateinterval > 0)
            {
                FeedConfig.UpdateInterval = updateinterval;
            }

            int displayeditems;
            if (!string.IsNullOrEmpty(displayeditemsbox.Text) && int.TryParse(displayeditemsbox.Text, out displayeditems) && displayeditems > 0)
            {
                FeedConfig.DisplayedItems = displayeditems;
            }

            FeedConfig.Url = urlbox.Text;
            FeedConfig.UserName = usernamebox.Text;

            FeedConfig.NotifyOnNewItem = notificationcheckbox.IsChecked.Value;

            if (FeedConfig.Url.Trim().Length > 0)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Feed URL cannot be blank", "Blank URL", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void fontchooserbtn_Click(object sender, RoutedEventArgs e)
        {
            var fc = new FontChooser
                         {
                             SelectedFontFamily = fontlabel.FontFamily,
                             SelectedFontSize = fontlabel.FontSize,
                             SelectedFontStyle = fontlabel.FontStyle,
                             SelectedFontWeight = fontlabel.FontWeight
                         };

            var dr = fc.ShowDialog();
            if (dr != true) return;
            fontlabel.FontFamily = fc.SelectedFontFamily;
            fontlabel.FontSize = fc.SelectedFontSize;
            fontlabel.FontStyle = fc.SelectedFontStyle;
            fontlabel.FontWeight = fc.SelectedFontWeight;
            fontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}", fontlabel.FontFamily, fontlabel.FontSize, fontlabel.FontStyle, fontlabel.FontWeight);
        }

        private void defaultcolorchooserbtn_Click(object sender, RoutedEventArgs e)
        {
            var cd = new System.Windows.Forms.ColorDialog();
            var initialcol = System.Drawing.Color.FromArgb(((SolidColorBrush)defaultcolourbox.Fill).Color.R, ((SolidColorBrush)defaultcolourbox.Fill).Color.G, ((SolidColorBrush)defaultcolourbox.Fill).Color.B);
            cd.Color = initialcol;
            var dr = cd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                defaultcolourbox.Fill = new SolidColorBrush(Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
            }
        }

        private void titlefontchooserbtn_Click(object sender, RoutedEventArgs e)
        {
            var fc = new FontChooser
                                 {
                                     SelectedFontFamily = titlefontlabel.FontFamily,
                                     SelectedFontSize = titlefontlabel.FontSize,
                                     SelectedFontStyle = titlefontlabel.FontStyle,
                                     SelectedFontWeight = titlefontlabel.FontWeight
                                 };
            var dr = fc.ShowDialog();
            if (dr != true) return;
            titlefontlabel.FontFamily = fc.SelectedFontFamily;
            titlefontlabel.FontSize = fc.SelectedFontSize;
            titlefontlabel.FontStyle = fc.SelectedFontStyle;
            titlefontlabel.FontWeight = fc.SelectedFontWeight;
            titlefontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}", titlefontlabel.FontFamily, titlefontlabel.FontSize, titlefontlabel.FontStyle, titlefontlabel.FontWeight);
        }

        private void hovercolorchooserbtn_Click(object sender, RoutedEventArgs e)
        {
            var cd = new System.Windows.Forms.ColorDialog();
            var initialcol = System.Drawing.Color.FromArgb(((SolidColorBrush)hovercolourbox.Fill).Color.R, ((SolidColorBrush)hovercolourbox.Fill).Color.G, ((SolidColorBrush)hovercolourbox.Fill).Color.B);
            cd.Color = initialcol;
            var dr = cd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                hovercolourbox.Fill = new SolidColorBrush(Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
            }
        }
        #endregion
    }
}
