using System.Windows.Forms;

namespace Feedling
{
    public partial class FeedWinManager
    {
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeedWinManager));
            this.trayicon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quititem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.feedlistbox = new System.Windows.Forms.ListBox();
            this.feedaddbtn = new System.Windows.Forms.Button();
            this.feedimportbtn = new System.Windows.Forms.Button();
            this.feeddeletebtn = new System.Windows.Forms.Button();
            this.feededitbtn = new System.Windows.Forms.Button();
            this.feedtemplatebtn = new System.Windows.Forms.Button();
            this.feedexportbtn = new System.Windows.Forms.Button();
            this.OKbtn = new System.Windows.Forms.Button();
            this.importfeeddlg = new System.Windows.Forms.OpenFileDialog();
            this.exportfeeddlg = new System.Windows.Forms.SaveFileDialog();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Feedspage = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.noproxybtn = new System.Windows.Forms.RadioButton();
            this.systemproxybtn = new System.Windows.Forms.RadioButton();
            this.customproxybtn = new System.Windows.Forms.RadioButton();
            this.CustomProxyTable = new System.Windows.Forms.TableLayoutPanel();
            this.ProxyPortBox = new System.Windows.Forms.TextBox();
            this.ProxyPassBox = new System.Windows.Forms.TextBox();
            this.ProxyUserBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ProxyHostBox = new System.Windows.Forms.TextBox();
            this.ProxyAuthCheck = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Pluginspage = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.pluginlistbox = new System.Windows.Forms.ListBox();
            this.pluginaboutbtn = new System.Windows.Forms.Button();
            this.Miscpage = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.feedbackgroundimagescheck = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gridwidthbox = new System.Windows.Forms.NumericUpDown();
            this.opacitytrack = new System.Windows.Forms.TrackBar();
            this.contextMenuStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.Feedspage.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.CustomProxyTable.SuspendLayout();
            this.Pluginspage.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.Miscpage.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridwidthbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.opacitytrack)).BeginInit();
            this.SuspendLayout();
            // 
            // trayicon
            // 
            this.trayicon.ContextMenuStrip = this.contextMenuStrip;
            this.trayicon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayicon.Icon")));
            this.trayicon.Text = "Feedling";
            this.trayicon.Visible = true;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.updateAllToolStripMenuItem,
            this.moveModeToolStripMenuItem,
            this.configurationToolStripMenuItem,
            this.quititem});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(158, 114);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
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
            this.moveModeToolStripMenuItem.CheckOnClick = true;
            this.moveModeToolStripMenuItem.Name = "moveModeToolStripMenuItem";
            this.moveModeToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.moveModeToolStripMenuItem.Text = "Move Mode";
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
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.28915F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.71084F));
            this.tableLayoutPanel1.Controls.Add(this.feedlistbox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.feedaddbtn, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.feedimportbtn, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.feeddeletebtn, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.feededitbtn, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.feedtemplatebtn, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.feedexportbtn, 1, 5);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49938F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49938F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49938F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49938F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49938F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.50187F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(440, 251);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // feedlistbox
            // 
            this.feedlistbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.feedlistbox.FormattingEnabled = true;
            this.feedlistbox.IntegralHeight = false;
            this.feedlistbox.Location = new System.Drawing.Point(3, 3);
            this.feedlistbox.Name = "feedlistbox";
            this.tableLayoutPanel1.SetRowSpan(this.feedlistbox, 6);
            this.feedlistbox.Size = new System.Drawing.Size(312, 240);
            this.feedlistbox.TabIndex = 1;
            this.feedlistbox.SelectedIndexChanged += new System.EventHandler(this.feedlistbox_SelectedIndexChanged);
            // 
            // feedaddbtn
            // 
            this.feedaddbtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.feedaddbtn.Location = new System.Drawing.Point(321, 9);
            this.feedaddbtn.Name = "feedaddbtn";
            this.feedaddbtn.Size = new System.Drawing.Size(116, 22);
            this.feedaddbtn.TabIndex = 2;
            this.feedaddbtn.Text = "Add...";
            this.feedaddbtn.UseVisualStyleBackColor = true;
            this.feedaddbtn.Click += new System.EventHandler(this.feedaddbtn_Click);
            // 
            // feedimportbtn
            // 
            this.feedimportbtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.feedimportbtn.Location = new System.Drawing.Point(321, 173);
            this.feedimportbtn.Name = "feedimportbtn";
            this.feedimportbtn.Size = new System.Drawing.Size(116, 22);
            this.feedimportbtn.TabIndex = 8;
            this.feedimportbtn.Text = "Import...";
            this.feedimportbtn.UseVisualStyleBackColor = true;
            this.feedimportbtn.Click += new System.EventHandler(this.feedimportbtn_Click);
            // 
            // feeddeletebtn
            // 
            this.feeddeletebtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.feeddeletebtn.Enabled = false;
            this.feeddeletebtn.Location = new System.Drawing.Point(321, 132);
            this.feeddeletebtn.Name = "feeddeletebtn";
            this.feeddeletebtn.Size = new System.Drawing.Size(116, 22);
            this.feeddeletebtn.TabIndex = 7;
            this.feeddeletebtn.Text = "Delete";
            this.feeddeletebtn.UseVisualStyleBackColor = true;
            this.feeddeletebtn.Click += new System.EventHandler(this.feeddeletebtn_Click);
            // 
            // feededitbtn
            // 
            this.feededitbtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.feededitbtn.Enabled = false;
            this.feededitbtn.Location = new System.Drawing.Point(321, 91);
            this.feededitbtn.Name = "feededitbtn";
            this.feededitbtn.Size = new System.Drawing.Size(116, 22);
            this.feededitbtn.TabIndex = 10;
            this.feededitbtn.Text = "Edit...";
            this.feededitbtn.UseVisualStyleBackColor = true;
            this.feededitbtn.Click += new System.EventHandler(this.feededitbtn_Click);
            // 
            // feedtemplatebtn
            // 
            this.feedtemplatebtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.feedtemplatebtn.Enabled = false;
            this.feedtemplatebtn.Location = new System.Drawing.Point(321, 50);
            this.feedtemplatebtn.Name = "feedtemplatebtn";
            this.feedtemplatebtn.Size = new System.Drawing.Size(116, 22);
            this.feedtemplatebtn.TabIndex = 11;
            this.feedtemplatebtn.Text = "Duplicate...";
            this.feedtemplatebtn.UseVisualStyleBackColor = true;
            this.feedtemplatebtn.Click += new System.EventHandler(this.feedtemplatebtn_Click);
            // 
            // feedexportbtn
            // 
            this.feedexportbtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.feedexportbtn.Enabled = false;
            this.feedexportbtn.Location = new System.Drawing.Point(321, 214);
            this.feedexportbtn.Name = "feedexportbtn";
            this.feedexportbtn.Size = new System.Drawing.Size(116, 22);
            this.feedexportbtn.TabIndex = 9;
            this.feedexportbtn.Text = "Export...";
            this.feedexportbtn.UseVisualStyleBackColor = true;
            this.feedexportbtn.Click += new System.EventHandler(this.feedexportbtn_Click);
            // 
            // OKbtn
            // 
            this.OKbtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OKbtn.Location = new System.Drawing.Point(195, 298);
            this.OKbtn.Name = "OKbtn";
            this.OKbtn.Size = new System.Drawing.Size(75, 23);
            this.OKbtn.TabIndex = 0;
            this.OKbtn.Text = "OK";
            this.OKbtn.UseVisualStyleBackColor = true;
            this.OKbtn.Click += new System.EventHandler(this.OKbtn_Click);
            // 
            // importfeeddlg
            // 
            this.importfeeddlg.Filter = "Feedling Configuration Files (*.xml)|*.xml";
            // 
            // exportfeeddlg
            // 
            this.exportfeeddlg.Filter = "Feedling Configuration Files (*.xml)|*.xml";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.OKbtn, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(466, 324);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.Feedspage);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.Pluginspage);
            this.tabControl1.Controls.Add(this.Miscpage);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(460, 289);
            this.tabControl1.TabIndex = 1;
            // 
            // Feedspage
            // 
            this.Feedspage.Controls.Add(this.tableLayoutPanel1);
            this.Feedspage.Location = new System.Drawing.Point(4, 22);
            this.Feedspage.Name = "Feedspage";
            this.Feedspage.Padding = new System.Windows.Forms.Padding(3);
            this.Feedspage.Size = new System.Drawing.Size(452, 263);
            this.Feedspage.TabIndex = 0;
            this.Feedspage.Text = "Feeds";
            this.Feedspage.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel5);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(452, 263);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Proxy";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.noproxybtn, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.systemproxybtn, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.customproxybtn, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.CustomProxyTable, 0, 3);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 4;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(443, 254);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // noproxybtn
            // 
            this.noproxybtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.noproxybtn.AutoSize = true;
            this.noproxybtn.Checked = true;
            this.noproxybtn.Location = new System.Drawing.Point(3, 3);
            this.noproxybtn.Name = "noproxybtn";
            this.noproxybtn.Size = new System.Drawing.Size(67, 17);
            this.noproxybtn.TabIndex = 0;
            this.noproxybtn.TabStop = true;
            this.noproxybtn.Text = "No proxy";
            this.noproxybtn.UseVisualStyleBackColor = true;
            this.noproxybtn.CheckedChanged += new System.EventHandler(this.customproxybtn_CheckedChanged);
            // 
            // systemproxybtn
            // 
            this.systemproxybtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.systemproxybtn.AutoSize = true;
            this.systemproxybtn.Location = new System.Drawing.Point(3, 26);
            this.systemproxybtn.Name = "systemproxybtn";
            this.systemproxybtn.Size = new System.Drawing.Size(88, 17);
            this.systemproxybtn.TabIndex = 1;
            this.systemproxybtn.Text = "System Proxy";
            this.systemproxybtn.UseVisualStyleBackColor = true;
            this.systemproxybtn.CheckedChanged += new System.EventHandler(this.customproxybtn_CheckedChanged);
            // 
            // customproxybtn
            // 
            this.customproxybtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.customproxybtn.AutoSize = true;
            this.customproxybtn.Location = new System.Drawing.Point(3, 49);
            this.customproxybtn.Name = "customproxybtn";
            this.customproxybtn.Size = new System.Drawing.Size(124, 17);
            this.customproxybtn.TabIndex = 2;
            this.customproxybtn.Text = "Specity Proxy Details";
            this.customproxybtn.UseVisualStyleBackColor = true;
            this.customproxybtn.CheckedChanged += new System.EventHandler(this.customproxybtn_CheckedChanged);
            // 
            // CustomProxyTable
            // 
            this.CustomProxyTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CustomProxyTable.ColumnCount = 2;
            this.CustomProxyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.CustomProxyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.CustomProxyTable.Controls.Add(this.ProxyPortBox, 1, 1);
            this.CustomProxyTable.Controls.Add(this.ProxyPassBox, 1, 4);
            this.CustomProxyTable.Controls.Add(this.ProxyUserBox, 1, 3);
            this.CustomProxyTable.Controls.Add(this.label3, 0, 0);
            this.CustomProxyTable.Controls.Add(this.ProxyHostBox, 1, 0);
            this.CustomProxyTable.Controls.Add(this.ProxyAuthCheck, 0, 2);
            this.CustomProxyTable.Controls.Add(this.label4, 0, 3);
            this.CustomProxyTable.Controls.Add(this.label5, 0, 4);
            this.CustomProxyTable.Controls.Add(this.label6, 0, 1);
            this.CustomProxyTable.Location = new System.Drawing.Point(3, 72);
            this.CustomProxyTable.Name = "CustomProxyTable";
            this.CustomProxyTable.RowCount = 5;
            this.CustomProxyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.CustomProxyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.CustomProxyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.CustomProxyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.CustomProxyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.CustomProxyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.CustomProxyTable.Size = new System.Drawing.Size(437, 179);
            this.CustomProxyTable.TabIndex = 4;
            // 
            // ProxyPortBox
            // 
            this.ProxyPortBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ProxyPortBox.Location = new System.Drawing.Point(70, 42);
            this.ProxyPortBox.Name = "ProxyPortBox";
            this.ProxyPortBox.Size = new System.Drawing.Size(39, 20);
            this.ProxyPortBox.TabIndex = 4;
            this.ProxyPortBox.Leave += new System.EventHandler(this.ProxyPortBox_Leave);
            // 
            // ProxyPassBox
            // 
            this.ProxyPassBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProxyPassBox.Enabled = false;
            this.ProxyPassBox.Location = new System.Drawing.Point(70, 149);
            this.ProxyPassBox.Name = "ProxyPassBox";
            this.ProxyPassBox.Size = new System.Drawing.Size(364, 20);
            this.ProxyPassBox.TabIndex = 7;
            this.ProxyPassBox.UseSystemPasswordChar = true;
            // 
            // ProxyUserBox
            // 
            this.ProxyUserBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProxyUserBox.Enabled = false;
            this.ProxyUserBox.Location = new System.Drawing.Point(70, 112);
            this.ProxyUserBox.Name = "ProxyUserBox";
            this.ProxyUserBox.Size = new System.Drawing.Size(364, 20);
            this.ProxyUserBox.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Proxy Host:";
            // 
            // ProxyHostBox
            // 
            this.ProxyHostBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProxyHostBox.Location = new System.Drawing.Point(70, 7);
            this.ProxyHostBox.Name = "ProxyHostBox";
            this.ProxyHostBox.Size = new System.Drawing.Size(364, 20);
            this.ProxyHostBox.TabIndex = 3;
            // 
            // ProxyAuthCheck
            // 
            this.ProxyAuthCheck.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ProxyAuthCheck.AutoSize = true;
            this.CustomProxyTable.SetColumnSpan(this.ProxyAuthCheck, 2);
            this.ProxyAuthCheck.Location = new System.Drawing.Point(3, 79);
            this.ProxyAuthCheck.Name = "ProxyAuthCheck";
            this.ProxyAuthCheck.Size = new System.Drawing.Size(123, 17);
            this.ProxyAuthCheck.TabIndex = 5;
            this.ProxyAuthCheck.Text = "Proxy Authentication";
            this.ProxyAuthCheck.UseVisualStyleBackColor = true;
            this.ProxyAuthCheck.CheckedChanged += new System.EventHandler(this.ProxyAuthCheck_CheckedChanged);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Username:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 153);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Password:";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Proxy Port:";
            // 
            // Pluginspage
            // 
            this.Pluginspage.Controls.Add(this.tableLayoutPanel3);
            this.Pluginspage.Location = new System.Drawing.Point(4, 22);
            this.Pluginspage.Name = "Pluginspage";
            this.Pluginspage.Padding = new System.Windows.Forms.Padding(3);
            this.Pluginspage.Size = new System.Drawing.Size(452, 263);
            this.Pluginspage.TabIndex = 1;
            this.Pluginspage.Text = "Plugins";
            this.Pluginspage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.Controls.Add(this.pluginlistbox, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.pluginaboutbtn, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(440, 254);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // pluginlistbox
            // 
            this.pluginlistbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pluginlistbox.FormattingEnabled = true;
            this.pluginlistbox.Location = new System.Drawing.Point(3, 3);
            this.pluginlistbox.Name = "pluginlistbox";
            this.pluginlistbox.Size = new System.Drawing.Size(324, 238);
            this.pluginlistbox.TabIndex = 0;
            this.pluginlistbox.SelectedIndexChanged += new System.EventHandler(this.pluginlistbox_SelectedIndexChanged);
            // 
            // pluginaboutbtn
            // 
            this.pluginaboutbtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pluginaboutbtn.Enabled = false;
            this.pluginaboutbtn.Location = new System.Drawing.Point(353, 116);
            this.pluginaboutbtn.Name = "pluginaboutbtn";
            this.pluginaboutbtn.Size = new System.Drawing.Size(63, 22);
            this.pluginaboutbtn.TabIndex = 1;
            this.pluginaboutbtn.Text = "About...";
            this.pluginaboutbtn.UseVisualStyleBackColor = true;
            this.pluginaboutbtn.Click += new System.EventHandler(this.pluginaboutbtn_Click);
            // 
            // Miscpage
            // 
            this.Miscpage.Controls.Add(this.tableLayoutPanel4);
            this.Miscpage.Location = new System.Drawing.Point(4, 22);
            this.Miscpage.Name = "Miscpage";
            this.Miscpage.Padding = new System.Windows.Forms.Padding(3);
            this.Miscpage.Size = new System.Drawing.Size(452, 263);
            this.Miscpage.TabIndex = 2;
            this.Miscpage.Text = "Misc";
            this.Miscpage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.feedbackgroundimagescheck, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.gridwidthbox, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.opacitytrack, 1, 2);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(440, 251);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(75, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Grid size (px):";
            // 
            // feedbackgroundimagescheck
            // 
            this.feedbackgroundimagescheck.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.feedbackgroundimagescheck.AutoSize = true;
            this.tableLayoutPanel4.SetColumnSpan(this.feedbackgroundimagescheck, 2);
            this.feedbackgroundimagescheck.Location = new System.Drawing.Point(99, 116);
            this.feedbackgroundimagescheck.Name = "feedbackgroundimagescheck";
            this.feedbackgroundimagescheck.Size = new System.Drawing.Size(241, 17);
            this.feedbackgroundimagescheck.TabIndex = 2;
            this.feedbackgroundimagescheck.Text = "Display feed background images (if they exist)";
            this.feedbackgroundimagescheck.UseVisualStyleBackColor = true;
            this.feedbackgroundimagescheck.CheckedChanged += new System.EventHandler(this.feedbackgroundimagescheck_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 202);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Feed background opacity";
            // 
            // gridwidthbox
            // 
            this.gridwidthbox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gridwidthbox.Location = new System.Drawing.Point(270, 31);
            this.gridwidthbox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.gridwidthbox.Name = "gridwidthbox";
            this.gridwidthbox.Size = new System.Drawing.Size(120, 20);
            this.gridwidthbox.TabIndex = 5;
            this.gridwidthbox.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // opacitytrack
            // 
            this.opacitytrack.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.opacitytrack.BackColor = System.Drawing.SystemColors.Control;
            this.opacitytrack.LargeChange = 10;
            this.opacitytrack.Location = new System.Drawing.Point(258, 186);
            this.opacitytrack.Maximum = 100;
            this.opacitytrack.Name = "opacitytrack";
            this.opacitytrack.Size = new System.Drawing.Size(143, 45);
            this.opacitytrack.TabIndex = 6;
            this.opacitytrack.TickFrequency = 5;
            this.opacitytrack.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.opacitytrack.Value = 50;
            this.opacitytrack.Scroll += new System.EventHandler(this.opacitytrack_Scroll);
            // 
            // FeedWinManager
            // 
            this.AcceptButton = this.OKbtn;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(490, 348);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FeedWinManager";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Configuration";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.FeedWinManager_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.Feedspage.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.CustomProxyTable.ResumeLayout(false);
            this.CustomProxyTable.PerformLayout();
            this.Pluginspage.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.Miscpage.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridwidthbox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.opacitytrack)).EndInit();
            this.ResumeLayout(false);

        }

        private TabPage Miscpage;
        private TableLayoutPanel tableLayoutPanel4;
        private Label label1;
        private CheckBox feedbackgroundimagescheck;
        private Label label2;
        private NumericUpDown gridwidthbox;
        private TrackBar opacitytrack;
        private TabPage tabPage1;
        private TableLayoutPanel tableLayoutPanel5;
        private RadioButton noproxybtn;
        private RadioButton systemproxybtn;
        private RadioButton customproxybtn;
        private TableLayoutPanel CustomProxyTable;
        private TextBox ProxyPortBox;
        private TextBox ProxyPassBox;
        private TextBox ProxyUserBox;
        private Label label3;
        private TextBox ProxyHostBox;
        private CheckBox ProxyAuthCheck;
        private Label label4;
        private Label label5;
        private Label label6;
    }
}
