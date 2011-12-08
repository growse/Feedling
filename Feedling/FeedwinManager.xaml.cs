/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using FeedHanderPluginInterface;
using Microsoft.Win32;
using NLog;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FeedwinManager : Window
    {

        #region Vars and Consts

        private System.Windows.Forms.NotifyIcon notifyicon;
        private System.Windows.Forms.ContextMenuStrip contextmenustrip;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkforUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quititem;
        private List<IPlugin> plugins;
        private readonly OpenFileDialog importfeeddlg = new OpenFileDialog();
        private readonly SaveFileDialog exportfeeddlg = new SaveFileDialog();
        public static FeedwinManager thisinst;
        private readonly Hashtable windowlist = new Hashtable();
        private FeedConfigItemList FeedConfigItems = new FeedConfigItemList();
        public event Action<bool> ToggleMoveMode;
        private readonly XmlSerializer serializer;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public ICollection<IPlugin> Plugins
        {
            get { return plugins; }
        }
        public bool MoveMode
        {
            get
            {
                return moveModeToolStripMenuItem.Selected;
            }
        }
        private Guid previousselectedguid;
        #endregion

        /// <summary>
        /// Constructor. Where the magic lovin' happens.
        /// </summary>
        public FeedwinManager()
        {
            Log.Info("Starting up");
            try
            {
                Log.Debug("Building Manager window");
                InitializeComponent();

                //We're going to use the notifyicon and context menu from Winforms, because the WPF versions are a bit shit at the moment.
                #region ContextMenu

                aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                updateAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                checkforUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                moveModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                quititem = new System.Windows.Forms.ToolStripMenuItem();
                contextmenustrip = new System.Windows.Forms.ContextMenuStrip();
                // 
                // aboutToolStripMenuItem
                // 
                aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
                aboutToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
                aboutToolStripMenuItem.Text = Properties.Resources.FeedwinManager_FeedwinManager_About___;
                aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
                // 
                // checkforUpdatesToolStripMenuItem
                // 
                checkforUpdatesToolStripMenuItem.Name = "checkforUpdatesToolStripMenuItem";
                checkforUpdatesToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
                checkforUpdatesToolStripMenuItem.Text = Properties.Resources.FeedwinManager_FeedwinManager_Check_for_updates___;
                checkforUpdatesToolStripMenuItem.Click += checkforUpdatesToolStripMenuItem_Click;
                // 
                // updateAllToolStripMenuItem
                // 
                updateAllToolStripMenuItem.Name = "updateAllToolStripMenuItem";
                updateAllToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
                updateAllToolStripMenuItem.Text = Properties.Resources.FeedwinManager_FeedwinManager_Update_All;
                updateAllToolStripMenuItem.Click += updateAllToolStripMenuItem_Click;
                // 
                // moveModeToolStripMenuItem
                // 
                //moveModeToolStripMenuItem.CanSelect
                moveModeToolStripMenuItem.Name = "moveModeToolStripMenuItem";
                moveModeToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
                moveModeToolStripMenuItem.Text = Properties.Resources.FeedwinManager_FeedwinManager_Move_Mode;
                moveModeToolStripMenuItem.CheckOnClick = true;
                moveModeToolStripMenuItem.Click += moveModeToolStripMenuItem_Click;
                // 
                // configurationToolStripMenuItem
                // 
                configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
                configurationToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
                configurationToolStripMenuItem.Text = Properties.Resources.FeedwinManager_FeedwinManager_Configuration___;
                configurationToolStripMenuItem.Click += configurationToolStripMenuItem_Click;
                // 
                // quititem
                // 
                quititem.Name = "quititem";
                quititem.Size = new System.Drawing.Size(157, 22);
                quititem.Text = "Quit";
                quititem.Click += quititem_Click;

                contextmenustrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                    aboutToolStripMenuItem,
                    checkforUpdatesToolStripMenuItem,
                    updateAllToolStripMenuItem,
                    moveModeToolStripMenuItem,
                    configurationToolStripMenuItem,
                    quititem});
                contextmenustrip.Name = "menustrip";
                contextmenustrip.Size = new System.Drawing.Size(158, 114);

                notifyicon = new System.Windows.Forms.NotifyIcon();
                notifyicon.BalloonTipText = Properties.Resources.FirstTimeStartBalloonText;
                notifyicon.Text = "Feedling";
                notifyicon.Icon = Properties.Resources.FeedlingIcon;
                notifyicon.Visible = true;
                notifyicon.ContextMenuStrip = contextmenustrip;


                importfeeddlg.Filter = exportfeeddlg.Filter = "Feeling Config Files (*.xml)|*.xml";

                #endregion

                ServicePointManager.Expect100Continue = false;

                LoadSettings();

                thisinst = this;
                Log.Debug("Removing SSL cert validation");
                //We currently don't care if your RSS feed is being MITM'd.
                ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) => true;

                LoadPlugins();

                //We serialize the configuration to the app.config. To let us do this, we need a serlializer.
                Log.Debug("Creating XML serializer for the saved FeedConfigItems");
                serializer = new XmlSerializer(FeedConfigItems.GetType());
                ReloadFeedConfigItems();

                //If we don't have any feeds loaded, prompt the user to add some.
                if (FeedConfigItems.Items.Count == 0)
                {
                    notifyicon.ShowBalloonTip(1000);
                }

                LoadProxy();

                SetGuiConfigValues();

                Visibility = Visibility.Collapsed;
                Hide();

                Log.Debug("Checking for udpates on startup");
                var updater = new AutoUpdate();
                updater.CheckForUpdates(true);
            }
            catch (Exception ex)
            {
                Log.Error("Exception caught during FeedwinManager constructor", ex);
                throw;
            }
        }

        #region Methods

        private void SetGuiConfigValues()
        {
            defaultcolourbox.Fill = new SolidColorBrush(Color.FromRgb(Properties.Settings.Default.DefaultFeedColorR, Properties.Settings.Default.DefaultFeedColorG, Properties.Settings.Default.DefaultFeedColorB));
            hovercolourbox.Fill = new SolidColorBrush(Color.FromRgb(Properties.Settings.Default.DefaultFeedHoverColorR, Properties.Settings.Default.DefaultFeedHoverColorG, Properties.Settings.Default.DefaultFeedHoverColorB));
            titlefontlabel.FontFamily = new FontFamily(Properties.Settings.Default.DefaultTitleFontFamily);
            titlefontlabel.FontSize = Properties.Settings.Default.DefaultTitleFontSize;
            titlefontlabel.FontWeight = FontConversions.FontWeightFromString(Properties.Settings.Default.DefaultTitleFontWeight);
            titlefontlabel.FontStyle = FontConversions.FontStyleFromString(Properties.Settings.Default.DefaultTitleFontStyle);
            titlefontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}",
                Properties.Settings.Default.DefaultTitleFontFamily,
                Properties.Settings.Default.DefaultTitleFontSize,
                Properties.Settings.Default.DefaultTitleFontStyle,
                Properties.Settings.Default.DefaultTitleFontWeight
                );
            fontlabel.FontFamily = new FontFamily(Properties.Settings.Default.DefaultFontFamily);
            fontlabel.FontSize = Properties.Settings.Default.DefaultFontSize;
            fontlabel.FontWeight = FontConversions.FontWeightFromString(Properties.Settings.Default.DefaultFontWeight);
            fontlabel.FontStyle = FontConversions.FontStyleFromString(Properties.Settings.Default.DefaultFontStyle);
            fontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}",
                Properties.Settings.Default.DefaultFontFamily,
                Properties.Settings.Default.DefaultFontSize,
                Properties.Settings.Default.DefaultFontStyle,
                Properties.Settings.Default.DefaultFontWeight
                );

            proxyauthcheck.IsChecked = Properties.Settings.Default.ProxyAuth;
            proxyhostbox.Text = Properties.Settings.Default.ProxyHost;
            proxyportbox.Text = Properties.Settings.Default.ProxyPort.ToString();
            proxypassbox.Password = Properties.Settings.Default.ProxyPass;
            proxyuserbox.Text = Properties.Settings.Default.ProxyUser;
            proxyhostbox.IsEnabled = proxyportbox.IsEnabled = proxyauthcheck.IsEnabled = proxyuserbox.IsEnabled = proxypassbox.IsEnabled = (customproxybtn.IsChecked == true);
        }

        private void LoadProxy()
        {
            Log.Debug("Loading proxy");
            ProxyType proxytype;
            if (Enum.IsDefined(typeof(ProxyType), Properties.Settings.Default.ProxyType))
            {
                proxytype = (ProxyType)Enum.Parse(typeof(ProxyType), Properties.Settings.Default.ProxyType);
            }
            else
            {
                proxytype = ProxyType.System;
            }
            if (proxytype == ProxyType.Global) { proxytype = ProxyType.System; }
            switch (proxytype)
            {
                case ProxyType.None:
                    noproxybtn.IsChecked = true;
                    break;
                case ProxyType.System:
                    systemproxybtn.IsChecked = true;
                    break;
                case ProxyType.Custom:
                    customproxybtn.IsChecked = true;
                    break;
            }
        }

        private void LoadPlugins()
        {
            //Load our Plugins. Find everything that's a Dll, figure out if it's a plugin and load it.
            Log.Debug("Loading Plugins");
            var pluginfiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            plugins = new List<IPlugin>();
            Log.Debug("{0} plugin candidates found", pluginfiles.Length);
            foreach (var t in pluginfiles)
            {
                var args = t.Substring(t.LastIndexOf("\\") + 1, t.IndexOf(".dll") - t.LastIndexOf("\\") - 1);
                var ass = Assembly.Load(args);

                var types = ass.GetTypes();
                foreach (var plugintype in types)
                {
                    if (typeof(IPlugin).IsAssignableFrom(plugintype) && !plugintype.IsAbstract)
                    {
                        Log.Debug("Found valid plugin: {0}", plugintype);
                        plugins.Add((IPlugin)Activator.CreateInstance(plugintype));
                    }
                }
                //type = ass.GetTypes(args + ".Feed");
            }
            foreach (var feedplugin in plugins)
            {
                pluginlistbox.Items.Add(feedplugin.PluginName);
            }
        }

        private void LoadSettings()
        {
            //Update those settings.
            Log.Debug("Loading Settings");
            try
            {
                if (Properties.Settings.Default.CallUpgrade)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.CallUpgrade = false;
                }
                Properties.Settings.Default.Save();
            }
            catch (ConfigurationErrorsException ex)
            {
                Log.Error("Exception thrown trying to initialize the configuration.");
                string filename = "";

                if (!String.IsNullOrEmpty(ex.Filename))
                {
                    filename = ex.Filename;
                }
                else
                {
                    if (ex.InnerException is ConfigurationErrorsException)
                    {
                        filename = ((ConfigurationErrorsException)ex.InnerException).Filename;
                    }
                }
                Log.Error("Filename is {0}. Deleting it and reinitializing", filename);
                if (!String.IsNullOrEmpty(filename))
                {
                    File.Delete(filename);
                }
                System.Windows.Forms.Application.Restart();
                System.Windows.Forms.Application.Exit();
            }
        }

        public static string FetchUserAgentString()
        {
            return string.Format(Properties.Resources.UserAgentString, Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
        }

        public static IXPathNavigable Fetch(FeedConfigItem fci)
        {
            Log.Debug("Fetching feed from intarweb");
            HttpWebResponse resp = null;
            try
            {
                var requri = new Uri(fci.Url);
                var req = (HttpWebRequest)WebRequest.Create(requri);
                req.UserAgent = FetchUserAgentString();
                req.Proxy = fci.ProxyType != ProxyType.Global ? fci.Proxy : GetGlobalProxy();
                switch (fci.AuthType)
                {
                    case FeedAuthTypes.Basic:
                        req.Credentials = new NetworkCredential(fci.UserName, fci.Password);
                        resp = (HttpWebResponse)req.GetResponse();
                        break;
                    case FeedAuthTypes.Other:
                    case FeedAuthTypes.None:
                        resp = (HttpWebResponse)req.GetResponse();
                        break;
                }
                var tempdoc = new XmlDocument();
                if (resp != null)
                {
                    var respstream = resp.GetResponseStream();
                    if (respstream != null)
                    {
                        var streamtext = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                        tempdoc.LoadXml(streamtext);
                    }
                }
                return tempdoc;
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown when fetching feed", ex);
                throw;
            }
            finally
            {
                if (resp != null)
                {
                    resp.Close();
                }
            }
        }

        public void ReloadFeedConfigItems()
        {
            ReloadFeedConfigItems(false);
        }
        public delegate void ReloadFeedConfigItemsCallBack(bool clearall);
        public void ReloadFeedConfigItems(bool clearall)
        {

            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Log.Debug("Received request to Reload FeedConfigItems - thread miss, reinvoking");
                var d = new ReloadFeedConfigItemsCallBack(ReloadFeedConfigItems);
                Dispatcher.Invoke(d, new object[] { clearall });
            }
            else
            {
                Log.Debug("Received request to Reload FeedConfigItems");
                try
                {
                    feedlistbox.Items.Clear();
                    LoadFeedSettings();
                    foreach (var fci in FeedConfigItems.Items)
                    {
                        feedlistbox.Items.Add(fci);
                    }

                    feedexportbtn.IsEnabled = (feedlistbox.Items.Count > 0);
                    if (clearall)
                    {
                        foreach (FeedWin fw in windowlist.Values)
                        {
                            fw.Close();
                        }
                        windowlist.Clear();
                    }
                    foreach (var fci in FeedConfigItems.Items)
                    {
                        if (!windowlist.ContainsKey(fci.Guid))
                        {
                            var fw = new FeedWin(fci);
                            fw.LocationChanged += fw_LocationChanged;
                            fw.SizeChanged += fw_SizeChanged;
                            fw.Show();
                            windowlist.Add(fw.FeedConfig.Guid, fw);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exception thrown when trying to reload the FeedConfigItems", ex);
                    throw;
                }
            }
        }

        void fw_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var obj = (FeedWin)sender;
            foreach (var fci in FeedConfigItems.Items)
            {
                if (fci.Url == obj.FeedConfig.Url)
                {
                    fci.Width = obj.FeedConfig.Width;

                }
            }
            SaveFeedSettings();
        }

        void fw_LocationChanged(object sender, EventArgs e)
        {
            var obj = (FeedWin)sender;
            foreach (var fci in FeedConfigItems.Items)
            {
                if (fci.Url == obj.FeedConfig.Url)
                {
                    fci.Position = obj.FeedConfig.Position;

                }
            }
            SaveFeedSettings();
        }


        private void LoadFeedSettings()
        {
            Log.Debug("Loading Feed Settings");
            try
            {
                var xmlr = XmlReader.Create(new StringReader(Properties.Settings.Default.FeedConfigItems));
                if (serializer.CanDeserialize(xmlr))
                {
                    FeedConfigItems = (FeedConfigItemList)serializer.Deserialize(xmlr);
                }
                xmlr.Close();
            }
            catch (XmlException ex)
            {
                Log.Error("XmlException thrown when Loading the feed settings", ex);
                FeedConfigItems = new FeedConfigItemList();
            }
            catch (InvalidOperationException ex)
            {
                Log.Error("InvalidOperationException thrown when Loading the feed settings", ex);
                FeedConfigItems = new FeedConfigItemList();
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown when Loading the feed settings", ex);
                throw;
            }
        }

        private void SaveFeedSettings()
        {
            Log.Debug("Saving Feed Settings");
            try
            {
                var sb = new StringBuilder();
                serializer.Serialize(new StringWriter(sb), FeedConfigItems);
                Properties.Settings.Default.FeedConfigItems = sb.ToString();
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown when Saving the feed settings", ex);
                throw;
            }
        }

        internal static IWebProxy GetGlobalProxy()
        {
            Log.Debug("Fetching the global proxy");
            IWebProxy proxy = null;
            ProxyType proxytype;
            if (Enum.IsDefined(typeof(ProxyType), Properties.Settings.Default.ProxyType))
            {
                proxytype = (ProxyType)Enum.Parse(typeof(ProxyType), Properties.Settings.Default.ProxyType);
            }
            else
            {
                return null;
            }
            switch (proxytype)
            {
                case ProxyType.Custom:
                    proxy = new WebProxy(Properties.Settings.Default.ProxyHost, Properties.Settings.Default.ProxyPort);
                    if (Properties.Settings.Default.ProxyAuth)
                    {
                        string user, domain = null;
                        if (Properties.Settings.Default.ProxyUser.Contains("\\"))
                        {
                            string[] bits = Properties.Settings.Default.ProxyUser.Split("\\".ToCharArray(), 2);
                            user = bits[1];
                            domain = bits[0];
                        }
                        else
                        {
                            user = Properties.Settings.Default.ProxyUser;
                        }
                        proxy.Credentials = new NetworkCredential(user, Properties.Settings.Default.ProxyPass, domain);
                    }
                    break;
                case ProxyType.System:
                    proxy = WebRequest.GetSystemWebProxy();
                    break;
                case ProxyType.None:
                    break;
                case ProxyType.Global:
                    break;
            }
            return proxy;
        }

        #endregion

        #region Events
        void checkforUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var updater = new AutoUpdate();
            updater.CheckForUpdates();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            OKBtn_Click(this, new RoutedEventArgs());
        }

        void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutfrm = new About();
            aboutfrm.ShowDialog();

        }

        private void pluginaboutbtn_Click(object sender, EventArgs e)
        {
            if (pluginlistbox.SelectedItems.Count != 1) return;
            foreach (var feedplugin in plugins)
            {
                if (feedplugin.PluginName == pluginlistbox.SelectedItem.ToString())
                {
                    MessageBox.Show(
                        string.Format("{0}\nVersion:\t{1}\n{2}", feedplugin.PluginName, feedplugin.PluginVersion,
                                      feedplugin.PluginCopyright), feedplugin.PluginName, MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }

        private void pluginlistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pluginaboutbtn.IsEnabled = (pluginlistbox.SelectedItems.Count == 1);
        }

        private void moveModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ToggleMoveMode != null)
            {
                ToggleMoveMode(moveModeToolStripMenuItem.Checked);
            }
        }
        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsVisible)
            {
                Show();
            }
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
            ShowInTaskbar = true;
        }

        private void quititem_Click(object sender, EventArgs e)
        {
            notifyicon.Visible = false;
            Log.Info("Shutting Down");
            Application.Current.Shutdown();
        }
        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            Log.Debug("FeedwinManager closing - saving settings");
            try
            {
                if (previousselectedguid != null && windowlist.Contains(previousselectedguid))
                {
                    ((FeedWin)windowlist[previousselectedguid]).Deselect();
                }
                if (noproxybtn.IsChecked == true)
                {
                    Properties.Settings.Default.ProxyType = ProxyType.Global.ToString();
                }
                else if (systemproxybtn.IsChecked == true)
                {
                    Properties.Settings.Default.ProxyType = ProxyType.System.ToString();
                }
                else if (customproxybtn.IsChecked == true)
                {
                    Properties.Settings.Default.ProxyType = ProxyType.Custom.ToString();
                }

                Properties.Settings.Default.ProxyAuth = Convert.ToBoolean(proxyauthcheck.IsChecked);
                Properties.Settings.Default.ProxyHost = proxyhostbox.Text;
                Properties.Settings.Default.ProxyUser = proxyuserbox.Text;
                Properties.Settings.Default.ProxyPass = proxypassbox.Password;
                try
                {
                    Properties.Settings.Default.ProxyPort = int.Parse(proxyportbox.Text);
                }
                catch (FormatException)
                {
                    Properties.Settings.Default.ProxyPort = 0;
                }
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown when trying to save the settings on closing the FeedwinManager window", ex);
                throw;
            }
            finally
            {
                Hide();
            }
        }

        internal void LoadFeedConfig(XmlReader xmlr)
        {
            Log.Debug("Loading the FeedConfig");
            try
            {
                FeedConfigItems = (FeedConfigItemList)serializer.Deserialize(xmlr);
                SaveFeedSettings();
                ReloadFeedConfigItems(true);
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown when trying to load the FeedConfig", ex);
                throw ex;
            }
        }

        private void updateAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (FeedWin fw in windowlist.Values)
            {
                ThreadPool.QueueUserWorkItem(fw.UpdateNow);
            }
        }

        private void proxybtn_Checked(object sender, RoutedEventArgs e)
        {
            proxyhostbox.IsEnabled = proxyportbox.IsEnabled = proxyauthcheck.IsEnabled = (customproxybtn.IsChecked == true);
            proxyuserbox.IsEnabled = proxypassbox.IsEnabled = (proxyauthcheck.IsChecked == true && proxyauthcheck.IsEnabled == true);
        }

        private void proxyauthcheck_Click(object sender, RoutedEventArgs e)
        {
            proxyuserbox.IsEnabled = proxypassbox.IsEnabled = (proxyauthcheck.IsChecked == true);
        }

        private void proxyportbox_LostFocus(object sender, RoutedEventArgs e)
        {
            int port;
            if (int.TryParse(proxyportbox.Text, out port) && port > 0)
            {
            }
            else
            {
                proxyportbox.Text = Regex.Replace(proxyportbox.Text, "[^\\d]*", "");
                proxyportbox.Focus();
            }
        }

        private void feedlistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            feededitbtn.IsEnabled = (feedlistbox.SelectedItems.Count == 1);
            feedtemplatebtn.IsEnabled = (feedlistbox.SelectedItems.Count == 1);
            feeddeletebtn.IsEnabled = (feedlistbox.SelectedItems.Count > 0);

            var fci = (FeedConfigItem)feedlistbox.SelectedItem;
            if (fci != null)
            {
                if (previousselectedguid != null && windowlist.Contains(previousselectedguid))
                {
                    ((FeedWin)windowlist[previousselectedguid]).Deselect();
                }
                previousselectedguid = fci.Guid;
                ((FeedWin)windowlist[previousselectedguid]).Select();
            }
        }

        private void feededitbtn_Click(object sender, RoutedEventArgs e)
        {
            if (feedlistbox.SelectedItems.Count != 1) return;
            var fci = ((FeedConfigItem)feedlistbox.SelectedItem).Copy();
            var nf = new NewFeed(fci);
            var dr = nf.ShowDialog();
            if (dr != true || nf.FeedConfig.Url.Trim().Length <= 0) return;
            FeedConfigItems.Remove(((FeedConfigItem)feedlistbox.SelectedItem));
            FeedConfigItems.Add(nf.FeedConfig);
            SaveFeedSettings();

            if (windowlist.ContainsKey(nf.FeedConfig.Guid) && windowlist[nf.FeedConfig.Guid] != null) ((FeedWin)windowlist[nf.FeedConfig.Guid]).Close();
            windowlist.Remove(nf.FeedConfig.Guid);

            var nfw = new FeedWin(nf.FeedConfig);
            windowlist.Add(nf.FeedConfig.Guid, nfw);
            nfw.Show();

            feedlistbox.Items.Clear();
            foreach (var fcil in FeedConfigItems.Items)
            {
                feedlistbox.Items.Add(fcil);
            }
        }

        private void feedtemplatebtn_Click(object sender, RoutedEventArgs e)
        {
            if (feedlistbox.SelectedItems.Count != 1) return;
            var fci = ((FeedConfigItem)feedlistbox.SelectedItem).Copy();
            fci.Url = "";
            var nf = new NewFeed(fci);
            var dr = nf.ShowDialog();

            if (dr != true || nf.FeedConfig.Url.Trim().Length <= 0) return;
            FeedConfigItems.Add(nf.FeedConfig);
            SaveFeedSettings();
            ReloadFeedConfigItems();
        }
        private void feeddeletebtn_Click(object sender, RoutedEventArgs e)
        {
            var dr = MessageBox.Show(Properties.Resources.FeedDeleteQuestion, Properties.Resources.FeedDeleteQuestionCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dr != MessageBoxResult.Yes || feedlistbox.SelectedItems.Count <= 0) return;
            var toberemoved = new List<FeedConfigItem>();
            foreach (FeedConfigItem fci in feedlistbox.SelectedItems)
            {
                FeedConfigItems.Remove(fci);
                if (windowlist.ContainsKey(fci.Guid))
                {
                    ((FeedWin)windowlist[fci.Guid]).Close();
                    windowlist.Remove(fci.Guid);
                }
                toberemoved.Add(fci);
            }
            foreach (var fci in toberemoved)
            {
                feedlistbox.Items.Remove(fci);
            }
            SaveFeedSettings();
        }
        private void feedaddbtn_Click(object sender, RoutedEventArgs e)
        {
            var nf = new NewFeed();
            var dr = nf.ShowDialog();
            if (dr != true || nf.FeedConfig.Url.Trim().Length <= 0) return;
            FeedConfigItems.Add(nf.FeedConfig);
            SaveFeedSettings();
            ReloadFeedConfigItems();
        }

        private void feedimportbtn_Click(object sender, RoutedEventArgs e)
        {
            var dr = importfeeddlg.ShowDialog();
            if (dr != true) return;
            Log.Debug("Importing feed list from {0}", importfeeddlg.FileName);
            var sr = new StreamReader(importfeeddlg.FileName);
            try
            {
                var xmlr = XmlReader.Create(sr);
                if (serializer.CanDeserialize(xmlr))
                {
                    LoadFeedConfig(xmlr);
                }
                xmlr.Close();
                sr.Close();
            }
            catch (XmlException)
            {
                MessageBox.Show(Properties.Resources.FeedwinManager_feedimportbtn_Click_Invalid_Config_file);
            }
        }


        private void feedexportbtn_Click(object sender, RoutedEventArgs e)
        {
            var dr = exportfeeddlg.ShowDialog();
            if (dr != true) return;
            Log.Debug("Exporting feed list to {0}", exportfeeddlg.FileName);
            SaveFeedSettings();
            try
            {
                var tw = new StreamWriter(exportfeeddlg.FileName);
                tw.Write(Properties.Settings.Default.FeedConfigItems);
                tw.Close();
            }
            catch (IOException ex)
            {
                Log.Error("IOException thrown when trying to export the feed list: {0}", ex);
                MessageBox.Show("There was an error writing to the file.");
            }
        }


        private void applytoallbtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var fci in FeedConfigItems.Items)
            {
                fci.DefaultColorR = Properties.Settings.Default.DefaultFeedColorR;
                fci.DefaultColorG = Properties.Settings.Default.DefaultFeedColorG;
                fci.DefaultColorB = Properties.Settings.Default.DefaultFeedColorB;

                fci.HoverColorR = Properties.Settings.Default.DefaultFeedHoverColorR;
                fci.HoverColorG = Properties.Settings.Default.DefaultFeedHoverColorG;
                fci.HoverColorB = Properties.Settings.Default.DefaultFeedHoverColorB;

                fci.FontFamilyString = Properties.Settings.Default.DefaultFontFamily;
                fci.FontSize = Properties.Settings.Default.DefaultFontSize;
                fci.FontStyleString = Properties.Settings.Default.DefaultFontStyle;
                fci.FontWeightString = Properties.Settings.Default.DefaultFontWeight;

                fci.TitleFontFamilyString = Properties.Settings.Default.DefaultTitleFontFamily;
                fci.TitleFontSize = Properties.Settings.Default.DefaultTitleFontSize;
                fci.TitleFontStyleString = Properties.Settings.Default.DefaultTitleFontStyle;
                fci.TitleFontWeightString = Properties.Settings.Default.DefaultTitleFontWeight;
                ((FeedWin)windowlist[fci.Guid]).FeedConfig = fci;
                ((FeedWin)windowlist[fci.Guid]).RedrawWin();

            }
            SaveFeedSettings();
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
            Properties.Settings.Default.DefaultFontFamily = fc.SelectedFontFamily.ToString();
            Properties.Settings.Default.DefaultFontSize = fc.SelectedFontSize;
            Properties.Settings.Default.DefaultFontStyle = FontConversions.FontStyleToString(fc.SelectedFontStyle);
            Properties.Settings.Default.DefaultFontWeight = FontConversions.FontWeightToString(fc.SelectedFontWeight);
            fontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}", fontlabel.FontFamily, fontlabel.FontSize, fontlabel.FontStyle, fontlabel.FontWeight);
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
            Properties.Settings.Default.DefaultTitleFontFamily = fc.SelectedFontFamily.ToString();
            Properties.Settings.Default.DefaultTitleFontSize = fc.SelectedFontSize;
            Properties.Settings.Default.DefaultTitleFontStyle = FontConversions.FontStyleToString(fc.SelectedFontStyle);
            Properties.Settings.Default.DefaultTitleFontWeight = FontConversions.FontWeightToString(fc.SelectedFontWeight);
            titlefontlabel.Content = string.Format("{0}, {1}pt, {2}, {3}", titlefontlabel.FontFamily, titlefontlabel.FontSize, titlefontlabel.FontStyle, titlefontlabel.FontWeight);
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
            Properties.Settings.Default.DefaultFeedColorR = ((SolidColorBrush)defaultcolourbox.Fill).Color.R;
            Properties.Settings.Default.DefaultFeedColorG = ((SolidColorBrush)defaultcolourbox.Fill).Color.G;
            Properties.Settings.Default.DefaultFeedColorB = ((SolidColorBrush)defaultcolourbox.Fill).Color.B;
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
            Properties.Settings.Default.DefaultFeedHoverColorR = ((SolidColorBrush)hovercolourbox.Fill).Color.R;
            Properties.Settings.Default.DefaultFeedHoverColorG = ((SolidColorBrush)hovercolourbox.Fill).Color.G;
            Properties.Settings.Default.DefaultFeedHoverColorB = ((SolidColorBrush)hovercolourbox.Fill).Color.B;
        }
        #endregion


    }

    public enum ProxyType
    {
        Global,
        None,
        System,
        Custom
    }

}
