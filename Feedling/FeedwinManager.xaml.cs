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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        private OpenFileDialog importfeeddlg = new OpenFileDialog();
        private SaveFileDialog exportfeeddlg = new SaveFileDialog();
        public event EventHandler RedrawAll;
        public static FeedwinManager thisinst;
        private Hashtable windowlist = new Hashtable();
        private FeedConfigItemList FeedConfigItems = new FeedConfigItemList();
        public event Action<bool> ToggleMoveMode;
        private XmlSerializer serializer;
        private static Logger Log = LogManager.GetCurrentClassLogger();
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

                this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.updateAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.checkforUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.moveModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.quititem = new System.Windows.Forms.ToolStripMenuItem();
                this.contextmenustrip = new System.Windows.Forms.ContextMenuStrip();
                // 
                // aboutToolStripMenuItem
                // 
                this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
                this.aboutToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
                this.aboutToolStripMenuItem.Text = "About...";
                this.aboutToolStripMenuItem.Click += new EventHandler(aboutToolStripMenuItem_Click);
                // 
                // checkforUpdatesToolStripMenuItem
                // 
                this.checkforUpdatesToolStripMenuItem.Name = "checkforUpdatesToolStripMenuItem";
                this.checkforUpdatesToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
                this.checkforUpdatesToolStripMenuItem.Text = "Check for updates...";
                this.checkforUpdatesToolStripMenuItem.Click += new EventHandler(checkforUpdatesToolStripMenuItem_Click);
                // 
                // updateAllToolStripMenuItem
                // 
                this.updateAllToolStripMenuItem.Name = "updateAllToolStripMenuItem";
                this.updateAllToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
                this.updateAllToolStripMenuItem.Text = "Update All";
                this.updateAllToolStripMenuItem.Click += new System.EventHandler(this.updateAllToolStripMenuItem_Click);
                // 
                // moveModeToolStripMenuItem
                // 
                //this.moveModeToolStripMenuItem.CanSelect
                this.moveModeToolStripMenuItem.Name = "moveModeToolStripMenuItem";
                this.moveModeToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
                this.moveModeToolStripMenuItem.Text = "Move Mode";
                this.moveModeToolStripMenuItem.CheckOnClick = true;
                this.moveModeToolStripMenuItem.Click += new System.EventHandler(this.moveModeToolStripMenuItem_Click);
                // 
                // configurationToolStripMenuItem
                // 
                this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
                this.configurationToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
                this.configurationToolStripMenuItem.Text = "Configuration...";
                this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
                // 
                // quititem
                // 
                this.quititem.Name = "quititem";
                this.quititem.Size = new System.Drawing.Size(157, 22);
                this.quititem.Text = "Quit";
                this.quititem.Click += new System.EventHandler(this.quititem_Click);

                this.contextmenustrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.checkforUpdatesToolStripMenuItem,
            this.updateAllToolStripMenuItem,
            this.moveModeToolStripMenuItem,
            this.configurationToolStripMenuItem,
            this.quititem});
                this.contextmenustrip.Name = "menustrip";
                this.contextmenustrip.Size = new System.Drawing.Size(158, 114);

                this.notifyicon = new System.Windows.Forms.NotifyIcon();
                this.notifyicon.BalloonTipText = Properties.Resources.FirstTimeStartBalloonText;
                this.notifyicon.Text = "Feedling";
                this.notifyicon.Icon = Properties.Resources.FeedlingIcon;
                this.notifyicon.Visible = true;
                this.notifyicon.ContextMenuStrip = contextmenustrip;


                importfeeddlg.Filter = exportfeeddlg.Filter = "Feeling Config Files (*.xml)|*.xml";

                #endregion

                ServicePointManager.Expect100Continue = false;
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
                thisinst = this;
                Log.Debug("Removing SSL cert validation");
                //We currently don't care if your RSS feed is being MITM'd.
                ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    bool validationResult = true;
                    return validationResult;
                };

                //Load our Plugins. Find everything that's a Dll, figure out if it's a plugin and load it.
                Log.Debug("Loading Plugins");
                string[] pluginfiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
                plugins = new List<IPlugin>();
                Log.Debug("{0} plugin candidates found", pluginfiles.Length);
                for (int ii = 0; ii < pluginfiles.Length; ii++)
                {
                    string args = pluginfiles[ii].Substring(pluginfiles[ii].LastIndexOf("\\") + 1, pluginfiles[ii].IndexOf(".dll") - pluginfiles[ii].LastIndexOf("\\") - 1);
                    Assembly ass = Assembly.Load(args);

                    Type[] types = ass.GetTypes();
                    foreach (Type t in types)
                    {
                        if (typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract)
                        {
                            Log.Debug("Found valid plugin: {0}", t);
                            plugins.Add((IPlugin)Activator.CreateInstance(t));
                        }
                    }
                    //type = ass.GetTypes(args + ".Feed");
                }
                foreach (IPlugin feedplugin in plugins)
                {
                    pluginlistbox.Items.Add(feedplugin.PluginName);
                }

                //We serialize the configuration to the app.config. To let us do this, we need a serlializer.
                Log.Debug("Creating XML serializer for the saved FeedConfigItems");
                serializer = new XmlSerializer(FeedConfigItems.GetType());
                ReloadFeedConfigItems();

                //If we don't have any feeds loaded, prompt the user to add some.
                if (FeedConfigItems.Items.Count == 0)
                {
                    this.notifyicon.ShowBalloonTip(1000);
                }

                gridwidthbox.Text = Properties.Settings.Default.GridWidth.ToString();
                feedbackgroundimagescheck.IsChecked = Properties.Settings.Default.DisplayBackgroundImages;
                opacitytrack.Value = Properties.Settings.Default.BackgroundImageOpacity;
                opacitytrack.IsEnabled = (feedbackgroundimagescheck.IsChecked == true);

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
                proxyauthcheck.IsChecked = Properties.Settings.Default.ProxyAuth;
                proxyhostbox.Text = Properties.Settings.Default.ProxyHost;
                proxyportbox.Text = Properties.Settings.Default.ProxyPort.ToString();
                proxypassbox.Password = Properties.Settings.Default.ProxyPass;
                proxyuserbox.Text = Properties.Settings.Default.ProxyUser;
                proxyhostbox.IsEnabled = proxyportbox.IsEnabled = proxyauthcheck.IsEnabled = proxyuserbox.IsEnabled = proxypassbox.IsEnabled = (customproxybtn.IsChecked == true);
                this.Visibility = System.Windows.Visibility.Collapsed;
                this.Hide();

                Log.Debug("Checking for udpates on startup");
                AutoUpdate updater = new AutoUpdate();
                updater.CheckForUpdates(true);
            }
            catch (Exception ex)
            {
                Log.Error("Exception caught during FeedwinManager constructor", ex);
                throw ex;
            }
        }



        #region Methods

        public static string FetchUserAgentString() {
            return string.Format(Properties.Resources.UserAgentString, Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
        }

        public static IXPathNavigable Fetch(FeedConfigItem fci)
        {
            Log.Debug("Fetching feed from intarweb");
            HttpWebResponse resp = null;
            try
            {
                Uri requri = new Uri(fci.Url);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requri);
                req.UserAgent = FetchUserAgentString();
                if (fci.ProxyType != ProxyType.Global) { req.Proxy = fci.Proxy; }
                else { req.Proxy = GetGlobalProxy(); }
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
                XmlDocument tempdoc = new XmlDocument();
                if (resp != null)
                {
                    Stream respstream = resp.GetResponseStream();
                    if (respstream != null)
                    {
                        string streamtext = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                        tempdoc.LoadXml(streamtext);
                    }
                }
                return tempdoc;
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown when fetching feed", ex);
                throw ex;
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

            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                Log.Debug("Received request to Reload FeedConfigItems - thread miss, reinvoking");
                ReloadFeedConfigItemsCallBack d = new ReloadFeedConfigItemsCallBack(ReloadFeedConfigItems);
                this.Dispatcher.Invoke(d, new object[] { clearall });
            }
            else
            {
                Log.Debug("Received request to Reload FeedConfigItems");
                try
                {
                    feedlistbox.Items.Clear();
                    LoadFeedSettings();
                    foreach (FeedConfigItem fci in FeedConfigItems.Items)
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
                    foreach (FeedConfigItem fci in FeedConfigItems.Items)
                    {
                        if (!windowlist.ContainsKey(fci.Guid))
                        {
                            FeedWin fw = new FeedWin(fci);
                            fw.LocationChanged += new EventHandler(fw_LocationChanged);
                            fw.Show();
                            windowlist.Add(fw.FeedConfig.Guid, fw);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exception thrown when trying to reload the FeedConfigItems", ex);
                    throw ex;
                }
            }
        }

        void fw_LocationChanged(object sender, EventArgs e)
        {
            FeedWin obj = (FeedWin)sender;
            foreach (FeedConfigItem fci in FeedConfigItems.Items)
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
                XmlReader xmlr = XmlReader.Create(new StringReader(Properties.Settings.Default.FeedConfigItems));
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
                throw ex;
            }
        }

        private void SaveFeedSettings()
        {
            Log.Debug("Saving Feed Settings");
            try
            {
                StringBuilder sb = new StringBuilder();
                serializer.Serialize(new StringWriter(sb), FeedConfigItems);
                Properties.Settings.Default.FeedConfigItems = sb.ToString();
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown when Saving the feed settings", ex);
                throw ex;
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
                    proxy = null;
                    break;
                case ProxyType.Global:
                    proxy = null;
                    break;
            }
            return proxy;
        }

        #endregion

        #region Events
        void checkforUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoUpdate updater = new AutoUpdate();
            updater.CheckForUpdates();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            OKBtn_Click(this, new RoutedEventArgs());
        }

        void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutfrm = new About();
            aboutfrm.ShowDialog();

        }

        private void pluginaboutbtn_Click(object sender, EventArgs e)
        {
            if (pluginlistbox.SelectedItems.Count == 1)
            {
                foreach (IPlugin feedplugin in plugins)
                {
                    if (feedplugin.PluginName == pluginlistbox.SelectedItem.ToString())
                    {
                        MessageBox.Show(string.Format("{0}\nVersion:\t{1}\n{2}", feedplugin.PluginName, feedplugin.PluginVersion, feedplugin.PluginCopyright), feedplugin.PluginName, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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
            if (!this.IsVisible)
            {
                this.Show();
            }
            if (this.WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }

            this.ShowInTaskbar = true;
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
                Properties.Settings.Default.GridWidth = Convert.ToInt32(gridwidthbox.Text);
                Properties.Settings.Default.DisplayBackgroundImages = Convert.ToBoolean(feedbackgroundimagescheck.IsChecked);
                Properties.Settings.Default.BackgroundImageOpacity = Convert.ToInt16(opacitytrack.Value);
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
                throw ex;
            }
            finally
            {
                this.Hide();
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
                ThreadPool.QueueUserWorkItem(new WaitCallback(fw.UpdateNow));
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

        private void opacitytrack_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Properties.Settings.Default.BackgroundImageOpacity = opacitytrack.Value;
            if (RedrawAll != null) { RedrawAll(this, new EventArgs()); }
        }
        private void proxyportbox_LostFocus(object sender, RoutedEventArgs e)
        {
            int port = 0;
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

            FeedConfigItem fci = (FeedConfigItem)feedlistbox.SelectedItem;
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
            if (feedlistbox.SelectedItems.Count == 1)
            {
                FeedConfigItem fci = ((FeedConfigItem)feedlistbox.SelectedItem).Copy();
                NewFeed nf = new NewFeed(fci);
                bool? dr = nf.ShowDialog();
                if (dr == true && nf.FeedConfig.Url.Trim().Length > 0)
                {
                    FeedConfigItems.Remove(((FeedConfigItem)feedlistbox.SelectedItem));
                    FeedConfigItems.Add(nf.FeedConfig);
                    SaveFeedSettings();

                    ((FeedWin)windowlist[nf.FeedConfig.Guid]).Close();
                    windowlist.Remove(nf.FeedConfig.Guid);

                    FeedWin nfw = new FeedWin(nf.FeedConfig);
                    windowlist.Add(nf.FeedConfig.Guid, nfw);
                    nfw.Show();

                    feedlistbox.Items.Clear();
                    foreach (FeedConfigItem fcil in FeedConfigItems.Items)
                    {
                        feedlistbox.Items.Add(fcil);
                    }
                }
            }
        }

        private void feedtemplatebtn_Click(object sender, RoutedEventArgs e)
        {
            if (feedlistbox.SelectedItems.Count == 1)
            {
                FeedConfigItem fci = ((FeedConfigItem)feedlistbox.SelectedItem).Copy();
                fci.Url = "";
                NewFeed nf = new NewFeed(fci);
                Nullable<bool> dr = nf.ShowDialog();
                if (dr == true && nf.FeedConfig.Url.Trim().Length > 0)
                {
                    FeedConfigItems.Add(nf.FeedConfig);
                    SaveFeedSettings();
                    ReloadFeedConfigItems();
                }
            }
        }
        private void feeddeletebtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show(Properties.Resources.FeedDeleteQuestion, Properties.Resources.FeedDeleteQuestionCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dr == MessageBoxResult.Yes)
            {
                if (feedlistbox.SelectedItems.Count > 0)
                {
                    List<FeedConfigItem> toberemoved = new List<FeedConfigItem>();
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
                    foreach (FeedConfigItem fci in toberemoved)
                    {
                        feedlistbox.Items.Remove(fci);
                    }
                    SaveFeedSettings();
                }
            }
        }
        private void feedaddbtn_Click(object sender, RoutedEventArgs e)
        {
            NewFeed nf = new NewFeed();
            Nullable<bool> dr = nf.ShowDialog();
            if (dr == true && nf.FeedConfig.Url.Trim().Length > 0)
            {
                FeedConfigItems.Add(nf.FeedConfig);
                SaveFeedSettings();
                ReloadFeedConfigItems();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Hide();
        }
        private void feedimportbtn_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> dr = importfeeddlg.ShowDialog();
            if (dr == true)
            {
                Log.Debug("Importing feed list from {0}", importfeeddlg.FileName);
                StreamReader sr = new StreamReader(importfeeddlg.FileName);
                try
                {
                    XmlReader xmlr = XmlReader.Create(sr);
                    if (serializer.CanDeserialize(xmlr))
                    {
                        LoadFeedConfig(xmlr);
                    }
                    xmlr.Close();
                    sr.Close();
                }
                catch (XmlException)
                {
                }
            }
        }

        private void feedexportbtn_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> dr = exportfeeddlg.ShowDialog();
            if (dr == true)
            {
                Log.Debug("Exporting feed list to {0}", exportfeeddlg.FileName);
                SaveFeedSettings();
                try
                {
                    TextWriter tw = new StreamWriter(exportfeeddlg.FileName);
                    tw.Write(Properties.Settings.Default.FeedConfigItems);
                    tw.Close();
                }
                catch (IOException ex)
                {
                    Log.Error("IOException thrown when trying to export the feed list", ex);
                    MessageBox.Show("There was an error writing to the file");
                }
            }
        }
        private void feedbackgroundimagescheck_Click(object sender, RoutedEventArgs e)
        {
            opacitytrack.IsEnabled = (feedbackgroundimagescheck.IsChecked == true);
            Properties.Settings.Default.DisplayBackgroundImages = (feedbackgroundimagescheck.IsChecked == true);
            if (RedrawAll != null) { RedrawAll(this, new EventArgs()); }
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
