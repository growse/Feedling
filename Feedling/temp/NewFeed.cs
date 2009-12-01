using System;
using System.Windows.Forms;
using FeedHanderPluginInterface;

namespace Feedling
{
    public partial class NewFeed : Form
    {
        public NewFeed()
        {
            InitializeComponent();
            fci = new FeedConfigItem();
            LoadFeedConfigItem();
        }
        public NewFeed(FeedConfigItem givenfci)
        {
            InitializeComponent();
            this.Text = string.Format("Edit {0}", givenfci.Url);
            fci = givenfci;
            if (fci.Url.Length > 0)
            {
                urlbox.Enabled = false;
            }
            LoadFeedConfigItem();
        }
        private FeedConfigItem fci;
        public FeedConfigItem FeedConfig
        {
            get { return fci; }
        }
        private void LoadFeedConfigItem()
        {
            urlbox.Text = fci.Url;
            defaultcolourbox.BackColor = fci.DefaultColor;
            hovercolourbox.BackColor = fci.HoverColor;
            titlefontlabel.Font = fci.TitleFont;
            titlefontlabel.Text = string.Format("{0}, {1}pt, {2}", fci.TitleFont.FontFamily.Name, fci.TitleFont.SizeInPoints, fci.TitleFont.Style.ToString());
            fontlabel.Font = fci.Font;
            fontlabel.Text = string.Format("{0}, {1}pt, {2}", fci.Font.FontFamily.Name, fci.Font.SizeInPoints, fci.Font.Style.ToString());
            updateintervalbox.Value = fci.UpdateInterval;
            usernamebox.Text = fci.Username;
            passwordbox.Text = fci.Password;
            switch (fci.AuthType)
            {
                case FeedAuthTypes.Other:
                    otherauthradio.Checked = true;
                    break;
                case FeedAuthTypes.Basic:
                    httpauthradio.Checked = true;
                    break;
                default:
                    noauthradio.Checked = true;
                    break;
            }
            usernamebox.Enabled = !noauthradio.Checked;
            passwordbox.Enabled = !noauthradio.Checked;
            //Proxy
            switch (fci.Proxytype)
            {
                case ProxyType.Global:
                    globalproxybtn.Checked = true;
                    break;
                case ProxyType.System:
                    systemproxybtn.Checked = true;
                    break;
                case ProxyType.None:
                    noproxybtn.Checked = true;
                    break;
                case ProxyType.Custom:
                    customproxybtn.Checked = true;
                    CustomProxyTable.Enabled = true;
                    break;
            }
            ProxyHostBox.Text = fci.Proxyhost;
            ProxyPortBox.Text = fci.Proxyport.ToString();
            ProxyAuthCheck.Checked = fci.Proxyauth;
            ProxyUserBox.Text = fci.Proxyuser;
            ProxyPassBox.Text = fci.Proxypass;
            ProxyUserBox.Enabled = ProxyPassBox.Enabled = ProxyAuthCheck.Checked;
        }
        private void okbtn_Click(object sender, EventArgs e)
        {
            if (fci.Url.Trim().Length > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Feed URL cannot be blank", "Blank URL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void NewFeed_Load(object sender, EventArgs e)
        {
            urlbox.Select();
        }

        private void normalfontchange_Click(object sender, EventArgs e)
        {
            fontdialog.Font = fci.Font;
            DialogResult dr = fontdialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                fci.Font = fontdialog.Font;
                LoadFeedConfigItem();
            }
        }

        private void titlefontchange_Click(object sender, EventArgs e)
        {
            fontdialog.Font = fci.TitleFont;
            DialogResult dr = fontdialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                fci.TitleFont = fontdialog.Font;
                LoadFeedConfigItem();
            }
        }

        private void defaultcolorchange_Click(object sender, EventArgs e)
        {
            colordialog.Color = fci.DefaultColor;
            DialogResult dr = colordialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                fci.DefaultColor = colordialog.Color;
                LoadFeedConfigItem();
            }
        }

        private void hovercolorchange_Click(object sender, EventArgs e)
        {
            colordialog.Color = fci.HoverColor;
            DialogResult dr = colordialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                fci.HoverColor = colordialog.Color;
                LoadFeedConfigItem();
            }
        }

        private void urlbox_Leave(object sender, EventArgs e)
        {
            fci.Url = urlbox.Text;
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            fci.AuthType = FeedAuthTypes.None;
            if (httpauthradio.Checked)
            {
                fci.AuthType = FeedAuthTypes.Basic;
            }
            if (otherauthradio.Checked)
            {
                fci.AuthType = FeedAuthTypes.Other;
            }
            LoadFeedConfigItem();
        }

        private void usernamebox_Leave(object sender, EventArgs e)
        {
            fci.Username = usernamebox.Text;
            LoadFeedConfigItem();
        }

        private void passwordbox_Leave(object sender, EventArgs e)
        {
            fci.Password = passwordbox.Text;
            LoadFeedConfigItem();
        }

        private void NewFeed_SizeChanged(object sender, EventArgs e)
        {
            noauthradio.Padding = new Padding(Convert.ToInt32((double)authpanel.Width*0.4),0,0,0);
            httpauthradio.Padding = noauthradio.Padding;
        }

        private void updateintervalbox_ValueChanged(object sender, EventArgs e)
        {
            fci.UpdateInterval = Convert.ToInt32(updateintervalbox.Value);
            LoadFeedConfigItem();
        }

        private void globalproxybtn_CheckedChanged(object sender, EventArgs e)
        {
            CustomProxyTable.Enabled = customproxybtn.Checked;
            fci.Proxytype = ProxyType.Global;
            if (noproxybtn.Checked)
            {
                fci.Proxytype = ProxyType.None;
            }
            if (systemproxybtn.Checked)
            {
                fci.Proxytype = ProxyType.System;
            }
            if (customproxybtn.Checked)
            {
                fci.Proxytype = ProxyType.Custom;
            }
            LoadFeedConfigItem();
        }

        private void ProxyAuthCheck_CheckedChanged(object sender, EventArgs e)
        {
            fci.Proxyauth = ProxyAuthCheck.Checked;
            ProxyUserBox.Enabled = ProxyPassBox.Enabled = ProxyAuthCheck.Checked;
            LoadFeedConfigItem();
        }

        private void ProxyHostBox_Leave(object sender, EventArgs e)
        {
            fci.Proxyhost = ProxyHostBox.Text;
            LoadFeedConfigItem();
        }

        private void ProxyPortBox_Leave(object sender, EventArgs e)
        {
            int port=0;
            if (int.TryParse(ProxyPortBox.Text, out port) && port > 0)
            {
                fci.Proxyport = port;
                LoadFeedConfigItem();
            }
            else
            {
                MessageBox.Show("Please enter a valid port number","Invalid Port Number",MessageBoxButtons.OK,MessageBoxIcon.Error);
                ProxyPortBox.Select();
            }
        }

        private void ProxyUserBox_Leave(object sender, EventArgs e)
        {
            fci.Proxyuser = ProxyUserBox.Text;
            LoadFeedConfigItem();
        }

        private void ProxyPassBox_Leave(object sender, EventArgs e)
        {
            fci.Proxypass = ProxyPassBox.Text;
            LoadFeedConfigItem();
        }
    }
}
