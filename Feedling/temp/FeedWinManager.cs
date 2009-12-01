using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using FeedHanderPluginInterface;

namespace Feedling
{
    public partial class FeedWinManager : Form
    {
        #region Variables
        private NotifyIcon trayicon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem quititem;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem configurationToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel1;
        private Button OKbtn;
        private ListBox feedlistbox;
        private Button feedaddbtn;
        public static FeedWinManager thisinst;
        private ToolStripMenuItem moveModeToolStripMenuItem;
        private Button feeddeletebtn;
        private Button feedimportbtn;
        private Button feedexportbtn;
        private OpenFileDialog importfeeddlg;
        private SaveFileDialog exportfeeddlg;
        public event EventHandler RedrawAll;
        public event Action<bool> ToggleMoveMode;

        private ToolStripMenuItem updateAllToolStripMenuItem;
        private Button feededitbtn;
        private Hashtable windowlist = new Hashtable();
        private TableLayoutPanel tableLayoutPanel2;
        private TabControl tabControl1;
        private TabPage Feedspage;
        private TabPage Pluginspage;
        private TableLayoutPanel tableLayoutPanel3;
        private ListBox pluginlistbox;
        private Button pluginaboutbtn;
        private List<IFeed> plugins;
        private FeedConfigItemList FeedConfigItems = new FeedConfigItemList();
        private Button feedtemplatebtn;
        private XmlSerializer serializer;
        public ICollection<IFeed> Plugins
        {
            get { return plugins; }
        }
        private string previousselectedurl;
        #endregion

        #region Methods
        public FeedWinManager()
        {
            
            InitializeComponent();
            System.Net.ServicePointManager.Expect100Continue = false;
            if (Properties.Settings.Default.CallUpgrade)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.CallUpgrade = false;
            }
            Properties.Settings.Default.Save();

            Properties.Settings.Default.SettingChanging += new System.Configuration.SettingChangingEventHandler(Default_SettingChanging);
            //Make sure we like all the certificates for the moment
            thisinst = this;
            ServicePointManager.ServerCertificateValidationCallback += delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                bool validationResult = true;
                return validationResult;
            };
            //Load our Plugins
            string[] pluginfiles = Directory.GetFiles(Application.StartupPath, "*.dll");
            plugins = new List<IFeed>();
            for (int ii = 0; ii < pluginfiles.Length; ii++)
            {
                Type type = null;
                string args = pluginfiles[ii].Substring(pluginfiles[ii].LastIndexOf("\\") + 1, pluginfiles[ii].IndexOf(".dll") - pluginfiles[ii].LastIndexOf("\\") - 1);
                Assembly ass = Assembly.Load(args);
                try
                {
                    type = ass.GetType(args + ".Feed");
                }
                catch (TypeLoadException)
                {
                    type = null;
                }
                if (type != null)
                {
                    plugins.Add((IFeed)Activator.CreateInstance(type));
                }
            }
            foreach (IFeed feedplugin in plugins)
            {
                pluginlistbox.Items.Add(feedplugin.PluginName);
            }
            serializer = new XmlSerializer(FeedConfigItems.GetType());
            ReloadFeedConfigItems();
            gridwidthbox.Text = Properties.Settings.Default.GridWidth.ToString();
            feedbackgroundimagescheck.Checked = Properties.Settings.Default.DisplayBackgroundImages;
            opacitytrack.Value = Properties.Settings.Default.BackgroundImageOpacity;
            opacitytrack.Enabled = feedbackgroundimagescheck.Checked;

            ProxyType proxytype;
            if (Enum.IsDefined(typeof(ProxyType), Properties.Settings.Default.ProxyType))
            {
                proxytype = (ProxyType)Enum.Parse(typeof(ProxyType), Properties.Settings.Default.ProxyType);
            }
            else
            {
                proxytype = ProxyType.None;
            }
            if (proxytype == ProxyType.Global) { proxytype = ProxyType.None; }
            switch (proxytype)
            {
                case ProxyType.None:
                    noproxybtn.Checked = true;
                    break;
                case ProxyType.System:
                    systemproxybtn.Checked = true;
                    break;
                case ProxyType.Custom:
                    customproxybtn.Checked = true;
                    break;
            }
            ProxyAuthCheck.Checked = Properties.Settings.Default.ProxyAuth;
            ProxyHostBox.Text = Properties.Settings.Default.ProxyHost;
            ProxyPortBox.Text = Properties.Settings.Default.ProxyPort.ToString();
            ProxyPassBox.Text = Properties.Settings.Default.ProxyPass;
            ProxyUserBox.Text = Properties.Settings.Default.ProxyUser;
            CustomProxyTable.Enabled = customproxybtn.Checked;
        }

        void Default_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
        public static IXPathNavigable Fetch(FeedConfigItem fci)
        {
            HttpWebResponse resp = null;
            try
            {
                Uri requri = new Uri(fci.Url);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requri);
                req.UserAgent = "Mozilla/5.0";
                if (fci.Proxytype != ProxyType.Global) { req.Proxy = fci.Proxy; }
                else { req.Proxy = GetGlobalProxy(); }
                switch (fci.AuthType)
                {
                    case FeedAuthTypes.Basic:
                        req.Credentials = new NetworkCredential(fci.Username, fci.Password);
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
            if (this.InvokeRequired)
            {
                ReloadFeedConfigItemsCallBack d = new ReloadFeedConfigItemsCallBack(ReloadFeedConfigItems);
                this.Invoke(d, new object[]{clearall});
            }
            else
            {
                feedlistbox.Items.Clear();
                LoadFeedSettings();
                feedlistbox.Items.AddRange(FeedConfigItems.Items);
                feedexportbtn.Enabled = (feedlistbox.Items.Count > 0);
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
                    if (!windowlist.ContainsKey(fci.Url))
                    {
                        FeedWin fw = new FeedWin(fci);
                        if (!fw.IsDisposed)
                        {
                            fw.FormMoved += new Action<FeedWin>(fw_FormMoved);
                            fw.Show();
                            windowlist.Add(fw.FeedConfig.Url, fw);
                        }
                    }
                }
            }
        }

       
        private void LoadFeedSettings()
        {
            try
            {
                XmlReader xmlr = XmlReader.Create(new StringReader(Properties.Settings.Default.FeedConfigItems));
                if (serializer.CanDeserialize(xmlr))
                {
                    FeedConfigItems = (FeedConfigItemList)serializer.Deserialize(xmlr);
                }
                xmlr.Close();
            }
            catch (XmlException)
            {
                FeedConfigItems = new FeedConfigItemList();
            }
            catch (InvalidOperationException)
            {
                FeedConfigItems = new FeedConfigItemList();
            }
        }

        private void SaveFeedSettings()
        {
            StringBuilder sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), FeedConfigItems);
            Properties.Settings.Default.FeedConfigItems = sb.ToString();
            Properties.Settings.Default.Save();
        }

        internal static IWebProxy GetGlobalProxy()
        {
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
                        proxy.Credentials = new NetworkCredential(user, Properties.Settings.Default.ProxyPass,domain);
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
        private void pluginaboutbtn_Click(object sender, EventArgs e)
        {
            if (pluginlistbox.SelectedItems.Count == 1)
            {
                foreach (IFeed feedplugin in plugins)
                {
                    if (feedplugin.PluginName == pluginlistbox.SelectedItem.ToString())
                    {
                        MessageBox.Show(string.Format("{0}\nVersion:\t{1}\n{2}", feedplugin.PluginName, feedplugin.PluginVersion, feedplugin.PluginCopyright), feedplugin.PluginName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void pluginlistbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            pluginaboutbtn.Enabled = (pluginlistbox.SelectedItems.Count == 1);
        }

        void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Properties.Settings.Default.Save();
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
            if (!this.Visible)
            {
                this.Show();
            }
            if (this.WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            this.ShowInTaskbar = true;
        }

        private void quititem_Click(object sender, EventArgs e)
        {
            trayicon.Visible = false;
            Application.Exit();
        }


        private void OKbtn_Click(object sender, EventArgs e)
        {
            if (previousselectedurl != null && windowlist.Contains(previousselectedurl))
            {
                ((FeedWin)windowlist[previousselectedurl]).Deselect();
            }
            Properties.Settings.Default.GridWidth = Convert.ToInt32(gridwidthbox.Text);
            Properties.Settings.Default.DisplayBackgroundImages = feedbackgroundimagescheck.Checked;
            Properties.Settings.Default.BackgroundImageOpacity = opacitytrack.Value;
            if (noproxybtn.Checked)
            {
                Properties.Settings.Default.ProxyType = ProxyType.Global.ToString();
            }
            else if (systemproxybtn.Checked)
            {
                Properties.Settings.Default.ProxyType = ProxyType.System.ToString();
            }
            else if (customproxybtn.Checked)
            {
                Properties.Settings.Default.ProxyType = ProxyType.Custom.ToString();
            }

            Properties.Settings.Default.ProxyAuth = ProxyAuthCheck.Checked;
            Properties.Settings.Default.ProxyHost = ProxyHostBox.Text;
            Properties.Settings.Default.ProxyUser = ProxyUserBox.Text;
            Properties.Settings.Default.ProxyPass = ProxyPassBox.Text;
            try
            {
                Properties.Settings.Default.ProxyPort = int.Parse(ProxyPortBox.Text);
            }
            catch (FormatException)
            {
                Properties.Settings.Default.ProxyPort = 0;
            }
            Properties.Settings.Default.Save();
            this.Hide();
        }
        void fw_FormMoved(FeedWin obj)
        {
            foreach (FeedConfigItem fci in FeedConfigItems.Items)
            {
                if (fci.Url == obj.FeedConfig.Url)
                {
                    fci.Position = obj.FeedConfig.Position;
                }
            }
            SaveFeedSettings();
        }

        private void feedexportbtn_Click(object sender, EventArgs e)
        {
            DialogResult dr = exportfeeddlg.ShowDialog();
            if (dr == DialogResult.OK)
            {
                SaveFeedSettings();
                try
                {
                    TextWriter tw = new StreamWriter(exportfeeddlg.FileName);
                    tw.Write(Properties.Settings.Default.FeedConfigItems);
                    tw.Close();
                }
                catch (IOException)
                {
                    MessageBox.Show("There was an error writing to the file");
                }
            }
        }

        private void feedimportbtn_Click(object sender, EventArgs e)
        {
            DialogResult dr = importfeeddlg.ShowDialog();
            if (dr == DialogResult.OK)
            {
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
        internal void LoadFeedConfig(XmlReader xmlr)
        {
            FeedConfigItems = (FeedConfigItemList)serializer.Deserialize(xmlr);
            SaveFeedSettings();
            ReloadFeedConfigItems(true);
        }
        private void feedaddbtn_Click(object sender, EventArgs e)
        {
            NewFeed nf = new NewFeed();
            DialogResult dr = nf.ShowDialog();
            if (dr == DialogResult.OK && nf.FeedConfig.Url.Trim().Length > 0)
            {
                FeedConfigItems.Add(nf.FeedConfig);
                SaveFeedSettings();
                ReloadFeedConfigItems();
            }
        }
        private void feeddeletebtn_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(Properties.Resources.FeedDeleteQuestion,Properties.Resources.FeedDeleteQuestionCaption,MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                if (feedlistbox.SelectedItems.Count > 0)
                {
                    List<FeedConfigItem> toberemoved = new List<FeedConfigItem>();
                    foreach (FeedConfigItem fci in feedlistbox.SelectedItems)
                    {
                        FeedConfigItems.Remove(fci);
                        if (windowlist.ContainsKey(fci.Url))
                        {
                            ((FeedWin)windowlist[fci.Url]).Close();
                            windowlist.Remove(fci.Url);
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
        private void FeedWinManager_Load(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void updateAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (FeedWin fw in windowlist.Values)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(fw.UpdateNow));
            }
        }

        private void feedtemplatebtn_Click(object sender, EventArgs e)
        {
            if (feedlistbox.SelectedItems.Count == 1)
            {
                FeedConfigItem fci = ((FeedConfigItem)feedlistbox.SelectedItem).Copy();
                fci.Url = "";
                NewFeed nf = new NewFeed(fci);
                DialogResult dr = nf.ShowDialog();
                if (dr == DialogResult.OK && nf.FeedConfig.Url.Trim().Length > 0)
                {
                    FeedConfigItems.Add(nf.FeedConfig);
                    SaveFeedSettings();
                    ReloadFeedConfigItems();
                }
            }
        }

        private void feededitbtn_Click(object sender, EventArgs e)
        {
            if (feedlistbox.SelectedItems.Count == 1)
            {
                FeedConfigItem fci = ((FeedConfigItem)feedlistbox.SelectedItem).Copy();
                NewFeed nf = new NewFeed(fci);
                DialogResult dr = nf.ShowDialog();
                if (dr == DialogResult.OK && nf.FeedConfig.Url.Trim().Length > 0)
                {
                    FeedConfigItems.Remove(((FeedConfigItem)feedlistbox.SelectedItem));
                    FeedConfigItems.Add(nf.FeedConfig);
                    SaveFeedSettings();

                    ((FeedWin)windowlist[nf.FeedConfig.Url]).Close();
                    ((FeedWin)windowlist[nf.FeedConfig.Url]).Dispose();
                    windowlist.Remove(nf.FeedConfig.Url);

                    FeedWin nfw = new FeedWin(nf.FeedConfig);
                    windowlist.Add(nf.FeedConfig.Url, nfw);
                    nfw.Show();

                    feedlistbox.Items.Clear();
                    feedlistbox.Items.AddRange(FeedConfigItems.Items);
                }
            }
        }

        private void feedlistbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            feededitbtn.Enabled = (feedlistbox.SelectedItems.Count == 1);
            feedtemplatebtn.Enabled = (feedlistbox.SelectedItems.Count == 1);
            feeddeletebtn.Enabled = (feedlistbox.SelectedItems.Count > 0);
            
            FeedConfigItem fci = (FeedConfigItem)feedlistbox.SelectedItem;
            if (fci != null)
            {
                if (previousselectedurl != null && windowlist.Contains(previousselectedurl))
                {
                    ((FeedWin)windowlist[previousselectedurl]).Deselect();
                }
                previousselectedurl = fci.Url;

                ((FeedWin)windowlist[previousselectedurl]).Select();
            }
            
        }

        private void feedbackgroundimagescheck_CheckedChanged(object sender, EventArgs e)
        {
            opacitytrack.Enabled = feedbackgroundimagescheck.Checked;
            Properties.Settings.Default.DisplayBackgroundImages = feedbackgroundimagescheck.Checked;
            if (RedrawAll != null) { RedrawAll(this, new EventArgs()); }
        }

        private void opacitytrack_Scroll(object sender, EventArgs e)
        {
            Properties.Settings.Default.BackgroundImageOpacity = opacitytrack.Value;
            if (RedrawAll != null) { RedrawAll(this, new EventArgs()); }
        }
        private void customproxybtn_CheckedChanged(object sender, EventArgs e)
        {
            CustomProxyTable.Enabled = customproxybtn.Checked;
        }

        private void ProxyPortBox_Leave(object sender, EventArgs e)
        {
            int port = 0;
            if (int.TryParse(ProxyPortBox.Text, out port) && port > 0)
            {
            }
            else
            {
                MessageBox.Show("Please enter a valid port number", "Invalid Port Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ProxyPortBox.Select();
            }
        }

        #endregion

        #region AboutForm
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form aboutfrm = new Form();
            aboutfrm.Text = "About Feedling";
            aboutfrm.StartPosition = FormStartPosition.CenterScreen;
            aboutfrm.Size = new Size(315, 250);
            aboutfrm.Icon = Properties.Resources.FeedlingIcon;
            aboutfrm.MaximizeBox = false;
            aboutfrm.MinimizeBox = false;
            aboutfrm.FormBorderStyle = FormBorderStyle.FixedSingle;
            aboutfrm.Paint += new PaintEventHandler(this.aboutfrm_Paint);

            Label versionlabel = new Label();
            aboutfrm.Controls.Add(versionlabel);
            versionlabel.Text = string.Format("Version {0}\n{1} 2008 Andrew Rowson",Assembly.GetExecutingAssembly().GetName().Version.Major.ToString(System.Globalization.CultureInfo.InvariantCulture) + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString(System.Globalization.CultureInfo.InvariantCulture) + " Build " + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString(System.Globalization.CultureInfo.InvariantCulture),((char)169).ToString());
            versionlabel.Size = new Size(290, 30);
            versionlabel.Location = new Point(10, 110);
            versionlabel.BackColor = Color.Transparent;

            

            Label weblink = new Label();
            aboutfrm.Controls.Add(weblink);
            weblink.Text = "For documentation, help and bug reports, please visit http://www.sourceforge.net/projects/feedling/";
            weblink.Size = new Size(290, 30);
            weblink.Location = new Point(10, 150);
            weblink.BackColor = Color.Transparent;
            weblink.Tag = "http://www.sourceforge.net/projects/feedling/";
            weblink.TabIndex = 2;
            weblink.MouseClick += new MouseEventHandler(weblink_MouseClick);
            weblink.Cursor = Cursors.Hand;

            Button closebtn = new Button();
            closebtn.Size = new Size(0x4b, 0x19);
            closebtn.Text = "Close";
            closebtn.Location = new Point(225, 185);
            closebtn.FlatStyle = FlatStyle.System;
            closebtn.TabIndex = 0;
            aboutfrm.CancelButton = closebtn;
            aboutfrm.Controls.Add(closebtn);
            aboutfrm.ShowDialog();
            aboutfrm.Dispose();
        }

        void weblink_MouseClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start(((Label)sender).Tag.ToString());
        }

        private void aboutfrm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            e.Graphics.DrawImage(Properties.Resources.AboutFrmImage,0,0);
        }

        #endregion AboutForm

        #region Properties
        public bool MoveMode
        {
            get
            {
                return moveModeToolStripMenuItem.Checked;
            }
        }
        #endregion

        private void ProxyAuthCheck_CheckedChanged(object sender, EventArgs e)
        {
            ProxyUserBox.Enabled = ProxyPassBox.Enabled = ProxyAuthCheck.Checked;
        }

       
    }
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

    public class FeedConfigItem : Object
    {
        public FeedConfigItem()
        {
        }
        public FeedConfigItem Copy()
        {
            FeedConfigItem fci = new FeedConfigItem();
            fci.Url = this.Url;
            fci.DefaultColor = this.DefaultColor;
            fci.Font = this.Font;
            fci.HoverColor = this.HoverColor;
            fci.Position = this.Position;
            fci.TitleFont = this.TitleFont;
            fci.AuthType = this.AuthType;
            fci.Username = this.Username;
            fci.Password = this.Password;
            fci.Proxyauth = this.Proxyauth;
            fci.Proxyhost = this.Proxyhost;
            fci.Proxytype = this.Proxytype;
            fci.Proxyuser = this.Proxyuser;
            fci.Proxypass = this.Proxypass;
            fci.Proxyport = this.Proxyport;
            return fci;
        }
        internal Point Position
        {
            get { return new Point(xpos,ypos); }
            set
            {
                xpos = value.X;
                ypos = value.Y;
            }
        }
        private int xpos = 5;
        [XmlAttribute("XPos")]
        public int XPos
        {
            get { return xpos; }
            set { xpos = value; }
        }
        private int ypos = 5;
        [XmlAttribute("YPos")]
        public int YPos
        {
            get { return ypos; }
            set { ypos = value; }
        }


        private string url = "";
        [XmlAttribute("Url")]
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        internal Color DefaultColor
        {
            get { return Color.FromArgb(defaultcolorr, defaultcolorg, defaultcolorb); }
            set
            {
                defaultcolorr = value.R;
                defaultcolorg = value.G;
                defaultcolorb = value.B;
            }
        }

        private int defaultcolorr = 255;
        [XmlAttribute("DefaultColorR")]
        public int DefaultColorR
        {
            get { return defaultcolorr; }
            set { defaultcolorr = value; }
        }
        private int defaultcolorg = 255;
        [XmlAttribute("DefaultColorG")]
        public int DefaultColorG
        {
            get { return defaultcolorg; }
            set { defaultcolorg = value; }
        }
        private int defaultcolorb = 255;
        [XmlAttribute("DefaultColorB")]
        public int DefaultColorB
        {
            get { return defaultcolorb; }
            set { defaultcolorb = value; }
        }

        internal Color HoverColor
        {
            get { return Color.FromArgb(hovercolorr,hovercolorg,hovercolorb); }
            set
            {
                hovercolorr = value.R;
                hovercolorg = value.G;
                hovercolorb = value.B;
            }
        }

        private int hovercolorr = 255;
        [XmlAttribute("HoverColorR")]
        public int HoverColorR
        {
            get { return hovercolorr; }
            set { hovercolorr = value; }
        }
        private int hovercolorg = 255;
        [XmlAttribute("HoverColorG")]
        public int HoverColorG
        {
            get { return hovercolorg; }
            set { hovercolorg = value; }
        }
        private int hovercolorb = 255;
        [XmlAttribute("HoverColorB")]
        public int HoverColorB
        {
            get { return hovercolorb; }
            set { hovercolorb = value; }
        }

        internal Font Font
        {
            get { return new Font(fontfamily, fontsize, fontstyle); }
            set
            {
                fontfamily = value.FontFamily.Name;
                fontstyle = value.Style;
                fontsize = value.Size;
            }
        }
        private string fontfamily = FeedWinManager.thisinst.Font.FontFamily.Name;
        [XmlAttribute("FontFamily")]
        public string FontFamily
        {
            get { return fontfamily; }
            set { fontfamily = value; }
        }
        private float fontsize = FeedWinManager.thisinst.Font.Size;
        [XmlAttribute("FontSize")]
        public float FontSize
        {
            get { return fontsize; }
            set { fontsize = value; }
        }
        private FontStyle fontstyle = FeedWinManager.thisinst.Font.Style;
        [XmlAttribute("FontStyle")]
        public FontStyle FontStyle
        {
            get { return fontstyle; }
            set { fontstyle = value; }
        }
        
        internal Font TitleFont
        {
            get { return new Font(titlefontfamily,titlefontsize,titlefontstyle); }
            set
            {
                titlefontfamily = value.FontFamily.Name;
                titlefontstyle = value.Style;
                titlefontsize = value.Size;
            }
        }

        private string titlefontfamily=FeedWinManager.thisinst.Font.FontFamily.Name;
        [XmlAttribute("TitleFontFamily")]
        public string TitleFontFamily
        {
            get { return titlefontfamily; }
            set { titlefontfamily = value; }
        }
        private float titlefontsize = FeedWinManager.thisinst.Font.Size;
        [XmlAttribute("TitleFontSize")]
        public float TitleFontSize
        {
            get { return titlefontsize; }
            set { titlefontsize = value; }
        }
        private FontStyle titlefontstyle = FeedWinManager.thisinst.Font.Style;
        [XmlAttribute("TitleFontStyle")]
        public FontStyle TitleFontStyle
        {
            get { return titlefontstyle; }
            set { titlefontstyle = value; }
        }
        private int updateinterval = 10;
        [XmlAttribute("UpdateInterval")]
        public int UpdateInterval
        {
            get { return updateinterval; }
            set { updateinterval = value; }
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
        public string Username
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
        private int width = 300;
        [XmlAttribute("Width")]
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        private ProxyType proxytype;
        [XmlAttribute("Proxytype")]
        public ProxyType Proxytype
        {
            get { return proxytype; }
            set { proxytype = value; }
        }
        private string proxyhost;
        [XmlAttribute("Proxyhost")]
        public string Proxyhost
        {
            get { return proxyhost; }
            set { proxyhost = value; }
        }
        private int proxyport;
        [XmlAttribute("Proxyport")]
        public int Proxyport
        {
            get { return proxyport; }
            set { proxyport = value; }
        }
        private bool proxyauth;
        [XmlAttribute("Proxyauth")]
        public bool Proxyauth
        {
            get { return proxyauth; }
            set { proxyauth = value; }
        }
        private string proxyuser;
        [XmlAttribute("Proxyuser")]
        public string Proxyuser
        {
            get { return proxyuser; }
            set { proxyuser = value; }
        }
        private string proxypass;
        [XmlAttribute("Proxypass")]
        public string Proxypass
        {
            get { return proxypass; }
            set { proxypass = value; }
        }

        public IWebProxy Proxy
        {
            get
            {
                IWebProxy proxy = null;
                switch (Proxytype)
                {
                    case ProxyType.Custom:
                        proxy = new WebProxy(Proxyhost, Proxyport);
                        if (Proxyauth)
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
                            proxy.Credentials = new NetworkCredential(user, Proxypass,domain);
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
    public enum ProxyType
    {
        Global,
        None,
        System,
        Custom
    }
}
