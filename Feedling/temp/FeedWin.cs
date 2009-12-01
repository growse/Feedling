using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using FeedHanderPluginInterface;

namespace Feedling
{
    
    public partial class FeedWin : Form
    {
        public event Action<FeedWin> FormMoved;
        private FeedConfigItem fci;
        private Image feedimage;
        private Uri oldimageurl;
        private bool selected = false;
        
        public FeedConfigItem FeedConfig
        {
            get { return fci; }
            set { fci = value; RedrawWin(); }
        }

        public FeedWin(FeedConfigItem feeditem)
        {
            InitializeComponent();
            fci = feeditem;
            updatedcolor = fci.DefaultColor;
            ThreadPool.QueueUserWorkItem(new WaitCallback(GetFeedType),fci);
        }

        public new void Select()
        {
            selected = true;
            RedrawWin();
        }

        public void Deselect()
        {
            selected = false;
            RedrawWin();
        }

        private void GetFeedType(object state)
        {
            try
            {
                FeedConfigItem fci = (FeedConfigItem)state;
                XmlDocument document = (XmlDocument)FeedWinManager.Fetch(fci);
                foreach (IFeed feedplugin in FeedWinManager.thisinst.Plugins)
                {
                    if (feedplugin.CanHandle(document))
                    {
                        IWebProxy reqproxy;
                        if (fci.Proxytype != ProxyType.Global) { reqproxy = fci.Proxy; }
                        else { reqproxy = FeedWinManager.GetGlobalProxy(); }
                        rssfeed = feedplugin.Factory(new Uri(fci.Url), fci.AuthType, fci.Username, fci.Password, reqproxy);
                        rssfeed.UpdateInterval = fci.UpdateInterval;
                        break;
                    }
                }
            }
            catch (XmlException) { errormsg = "Invalid XML Document"; }
            catch (UriFormatException) { errormsg = "Invalid URI"; }
            catch (WebException ex) { errormsg = ex.Message; }
            if (rssfeed == null)
            {
                if (errormsg == string.Empty)
                {
                    errormsg = "No Plugin to handle feed";
                }
            }
            else
            {
                rssfeed.Updated += new EventHandler(rssfeed_Updated);
                ThreadPool.QueueUserWorkItem(new WaitCallback(rssfeed.Watch));
            }
            RedrawWin();
        }

        private void FeedWin_Load(object sender, EventArgs e)
        {
            //this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            origparent = NativeMethods.GetParent(this.Handle);
            this.Width = fci.Width;
            this.Height = 20;
            blitimage = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(blitimage);
            g.Clear(Color.White);
            g.Dispose();
            this.SetBitmap(blitimage,255);
            if (FeedWinManager.thisinst.MoveMode)
            {
                UnpinFromDesktop();
            }
            else
            {
                PinToDesktop();
            }
            this.Location = fci.Position;
            FeedWinManager.thisinst.RedrawAll += new EventHandler(thisinst_RedrawAll);
            this.MouseDown += new MouseEventHandler(FeedWin_MouseDown);
            this.MouseEnter += new EventHandler(FeedWin_MouseEnter);
            this.MouseLeave += new EventHandler(FeedWin_MouseLeave);
            this.MouseMove += new MouseEventHandler(FeedWin_MouseMove);
            this.MouseUp += new MouseEventHandler(FeedWin_MouseUp);
            this.LocationChanged += new EventHandler(FeedWin_LocationChanged);
            this.SizeChanged += new EventHandler(FeedWin_SizeChanged);
            FeedWinManager.thisinst.ToggleMoveMode += new Action<bool>(thisinst_ToggleMoveMode);
        }

        void FeedWin_SizeChanged(object sender, EventArgs e)
        {
            if (pinned)
            {
                this.Width = fci.Width;
            }
            else
            {
                fci.Width = this.Width;
            }
        }

        void FeedWin_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && !pinned)
            {
                this.Cursor = Cursors.SizeAll;
            }
        }

        void FeedWin_LocationChanged(object sender, EventArgs e)
        {
            if (pinned)
            {
                this.Location = fci.Position;
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.Location = new Point(NearestX(this.Location.X), NearestX(this.Location.Y));
                fci.Position = this.Location;
            }
        }

        void FeedWin_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && !pinned)
            {
                this.Width += e.X - rightstartpoint.X;
                if (this.Width < 30) { this.Width = 30; }
                rightstartpoint = e.Location;
            }
            RedrawWin();
        }
        
        void FeedWin_MouseLeave(object sender, EventArgs e)
        {
            RedrawWin();
        }


        void FeedWin_MouseEnter(object sender, EventArgs e)
        {
            RedrawWin();
        }

        private string errormsg = "Fetching...";
        void thisinst_RedrawAll(object sender, EventArgs e)
        {
            RedrawWin();
        }

        private void FetchImage(object state)
        {
            WebResponse wresp;
            try
            {
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(oldimageurl);
                wr.UserAgent = "Mozilla/5.0";
                wr.Timeout = 10000;
                wresp = wr.GetResponse();
                feedimage = Image.FromStream(wresp.GetResponseStream());
                RedrawWin();
            }
            catch (WebException) { }
        }

        void rssfeed_Updated(object sender, EventArgs e)
        {
            if (rssfeed.ImageUrl != null && rssfeed.ImageUrl != oldimageurl)
            {
                oldimageurl = rssfeed.ImageUrl;
                ThreadPool.QueueUserWorkItem(new WaitCallback(FetchImage));
            }
            updatedcolor = fci.HoverColor;
            ThreadPool.QueueUserWorkItem(new WaitCallback(FaderClock));
            RedrawWin();
        }
        public void UpdateNow(object state)
        {
            if (rssfeed == null)
            {
                GetFeedType(fci);
            }
            else
            {
                rssfeed.Update();
            }
        }
        private delegate void RedrawWinCallback();
        private void RedrawWin() {
            if (!this.IsDisposed)
            {
                if (this.InvokeRequired)
                {
                    RedrawWinCallback d = new RedrawWinCallback(RedrawWin);
                    this.Invoke(d);
                }
                else
                {
                    SolidBrush defaultbrush = new SolidBrush(fci.DefaultColor);
                    SolidBrush hoverbrush = new SolidBrush(fci.HoverColor);
                    if (!pinned)
                    {
                        defaultbrush = new SolidBrush(Color.FromArgb(100, fci.DefaultColor));
                        hoverbrush = new SolidBrush(Color.FromArgb(100, fci.HoverColor));
                    }
                    hotrects.Clear();
                    Color hoverareacolor = Color.FromArgb(10, fci.DefaultColor);
                    SolidBrush hoverareabrush = new SolidBrush(hoverareacolor);
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Far;
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    blitimage = new Bitmap(this.Width, 1000, PixelFormat.Format32bppArgb);
                    Graphics g = Graphics.FromImage(blitimage);
                    if (pinned)
                    {
                        if (selected) {
                            g.Clear(Color.FromArgb(fci.DefaultColor.ToArgb() ^ 0x00ffffff));
                        }
                        else
                        {
                            g.Clear(Color.Transparent);
                        }
                    }
                    else
                    {
                        g.Clear(Color.FromArgb(fci.DefaultColor.ToArgb() ^ 0x00ffffff));
                    }
                    if (feedimage != null && Properties.Settings.Default.DisplayBackgroundImages)
                    {
                        ColorMatrix cm = new ColorMatrix();
                        cm.Matrix00 = cm.Matrix11 = cm.Matrix22 = cm.Matrix44 = 1;
                        cm.Matrix33 = (float)Properties.Settings.Default.BackgroundImageOpacity / 100;
                        ImageAttributes ia = new ImageAttributes();
                        ia.SetColorMatrix(cm);
                        int imagewidth, imageheight;
                        if (feedimage.Width > this.Width)
                        {
                            imagewidth = this.Width;
                            imageheight = (int)Math.Round(feedimage.Height * ((decimal)this.Width / (decimal)feedimage.Width));
                            g.DrawImage(feedimage, new Rectangle(0, 0, imagewidth, imageheight), 0, 0, feedimage.Width,feedimage.Height, GraphicsUnit.Pixel, ia);
                        }
                        else
                        {
                            g.DrawImage(feedimage, new Rectangle(0, 0, feedimage.Width, feedimage.Height), 0, 0, feedimage.Width,feedimage.Height, GraphicsUnit.Pixel, ia);
                        }
                        
                    }
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    float currheightpos = 0f;

                    SizeF stringsize;
                    if (rssfeed == null || rssfeed.FeedItems.Count == 0)
                    {
                        if (rssfeed!=null && rssfeed.HasError)
                        {
                            stringsize = g.MeasureString("Error", fci.TitleFont);
                            g.DrawString("Error", fci.TitleFont, defaultbrush, new PointF(this.Width - 5f, currheightpos), sf);
                            currheightpos += stringsize.Height;

                            string givenerr = rssfeed.ErrorMessage;
                            if (string.IsNullOrEmpty(rssfeed.ErrorMessage))
                            {
                                givenerr = "No error given";
                            }

                            stringsize = g.MeasureString(givenerr, fci.Font);
                            g.DrawString(givenerr, fci.Font, new SolidBrush(updatedcolor), new RectangleF(5f, currheightpos, this.Width - 10f, stringsize.Height), sf);
                            currheightpos += stringsize.Height;

                            this.Height = (int)currheightpos + 5;

                        }
                        else
                        {
                            g.DrawString(errormsg, fci.TitleFont, defaultbrush, new PointF(this.Width - 5f, currheightpos), sf);
                            this.Height = (int)g.MeasureString(errormsg, fci.TitleFont).Height + 5;
                        }
                    }
                    else
                    {
                        stringsize = g.MeasureString(rssfeed.Title, fci.TitleFont);
                        g.DrawString(rssfeed.Title, fci.TitleFont, defaultbrush, new RectangleF(5f, currheightpos, this.Width - 10f, stringsize.Height), sf);
                        currheightpos += stringsize.Height;
                        int maxnumber = 10;
                        if (rssfeed.FeedItems.Count < 10) { maxnumber = rssfeed.FeedItems.Count; }
                        if (maxnumber >= 1)
                        {
                            for (int ii = 0; ii < maxnumber; ii++)
                            {
                                stringsize = g.MeasureString(rssfeed.FeedItems[ii].Title, fci.Font);
                                Rectangle hovrect = new Rectangle(this.Width - 5 - (int)stringsize.Width, (int)currheightpos, (int)stringsize.Width, (int)stringsize.Height);
                                Rectangle absolutehovrect = hovrect;
                                absolutehovrect.Offset(this.Location);
                                hotrects.Add(absolutehovrect, rssfeed.FeedItems[ii].Link);
                                if (absolutehovrect.Contains(Cursor.Position))
                                {
                                    g.DrawString(rssfeed.FeedItems[ii].Title, fci.Font, hoverbrush, new RectangleF(5f, currheightpos, this.Width - 10f, stringsize.Height), sf);
                                }
                                else if (rssfeed.FeedItems[ii].Updated)
                                {
                                    g.DrawString(rssfeed.FeedItems[ii].Title, fci.Font, new SolidBrush(updatedcolor), new RectangleF(5f, currheightpos, this.Width - 10f, stringsize.Height), sf);
                                }
                                else
                                {
                                    g.DrawString(rssfeed.FeedItems[ii].Title, fci.Font, defaultbrush, new RectangleF(5f, currheightpos, this.Width - 10f, stringsize.Height), sf);
                                }
                                g.FillRectangle(new SolidBrush(hoverareacolor), hovrect);
                                currheightpos += stringsize.Height;
                            }

                        }
                        this.Height = (int)currheightpos + 5;
                    }
                    g.DrawLine(new Pen(defaultbrush), this.Width - 1, 0, this.Width - 1, this.Height);
                    if (!pinned)
                    {
                        StringFormat centerformat = new StringFormat();
                        centerformat.Alignment = StringAlignment.Center;
                        centerformat.Trimming = StringTrimming.EllipsisCharacter;
                        string movestr = "Left click moves\nRight click resizes";
                        SizeF size = g.MeasureString(movestr, fci.TitleFont);
                        Font dirfont = fci.TitleFont;
                        while (size.Width >= this.Width)
                        {
                            dirfont = new Font(dirfont.FontFamily, dirfont.Size - 1);
                            size = g.MeasureString(movestr, dirfont);
                        }
                        g.DrawString(movestr, dirfont, new SolidBrush(fci.DefaultColor), this.Width / 2, 5,centerformat);
                    }

                    Bitmap tempblit = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
                    Graphics tempg = Graphics.FromImage(tempblit);
                    tempg.DrawImageUnscaledAndClipped(blitimage, new Rectangle(0, 0, this.Width, this.Height));
                    this.SetBitmap(tempblit, 255);
                    tempg.Dispose();
                    g.Dispose();
                    tempblit.Dispose();
                    blitimage.Dispose();
                }
            }            
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (pinned)
            {
                this.SendToBack();
            }
        }
        private Color updatedcolor;
        private int fader = 0;
        private Hashtable hotrects = new Hashtable();
        private IFeed rssfeed;
        private bool pinned;
        private void FaderClock(object state)
        {
            fader = 100;
            while (fader > 0)
            {
                updatedcolor = Color.FromArgb(
                    (int)((fci.HoverColorR - fci.DefaultColorR) * ((decimal)fader / (decimal)100) + fci.DefaultColorR),
                    (int)((fci.HoverColorG - fci.DefaultColorG) * ((decimal)fader / (decimal)100) + fci.DefaultColorG),
                    (int)((fci.HoverColorB - fci.DefaultColorB) * ((decimal)fader / (decimal)100) + fci.DefaultColorB));
                RedrawWin();
                fader-= 5;
                Thread.Sleep(50);
            }
            updatedcolor = fci.DefaultColor;
        }
        private void PinToDesktop()
        {
            PinToDesktop(false);
        }
        private static int NearestX(int input)
        {
            return Convert.ToInt32(Math.Round((double)input / Properties.Settings.Default.GridWidth) * Properties.Settings.Default.GridWidth);
        }
        private void PinToDesktop(bool setpos)
        {
            pinned = true;
            RedrawWin();
            if (setpos)
            {
                FormMoved(this);
            }
            this.TopMost = false;
            NativeMethods.SetWindowPos(this.Handle, NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, NativeMethods.WS_EX_NOACTIVATE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
            this.Cursor = Cursors.Default;
        }

        private void UnpinFromDesktop()
        {
            pinned = false;
            RedrawWin();
            this.TopMost = true;
            this.Cursor = Cursors.SizeAll;
        }
        private IntPtr origparent;
        private void SetBitmap(Bitmap bm, byte opacity)
        {
            if (bm.PixelFormat == PixelFormat.Format32bppArgb)
            {
                IntPtr hBitmap = IntPtr.Zero;
                IntPtr oldBitmap = IntPtr.Zero;
                IntPtr screenDc = NativeMethods.GetDC(IntPtr.Zero);
                IntPtr memDc = NativeMethods.CreateCompatibleDC(screenDc);
                try
                {
                    hBitmap = bm.GetHbitmap(Color.FromArgb(0));
                    oldBitmap = NativeMethods.SelectObject(memDc, hBitmap);
                    NativeMethods.Size size = new NativeMethods.Size(bm.Width, bm.Height);
                    NativeMethods.Point pointSource = new NativeMethods.Point(0, 0);
                    NativeMethods.Point topPos = new NativeMethods.Point(Left, Top);
                    NativeMethods.BLENDFUNCTION blend = new NativeMethods.BLENDFUNCTION();
                    blend.BlendOp = NativeMethods.AC_SRC_OVER;
                    blend.BlendFlags = 0;
                    blend.SourceConstantAlpha = opacity;
                    blend.AlphaFormat = NativeMethods.AC_SRC_ALPHA;
                    NativeMethods.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, NativeMethods.ULW_ALPHA);
                }
                catch (ObjectDisposedException)
                {
                }
                finally
                {
                    NativeMethods.ReleaseDC(IntPtr.Zero, screenDc);
                    if (hBitmap != IntPtr.Zero)
                    {
                        NativeMethods.SelectObject(memDc, oldBitmap);
                        NativeMethods.DeleteObject(hBitmap);
                    }
                    NativeMethods.DeleteDC(memDc);
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(Color.White);
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= NativeMethods.WS_EX_LAYERED;
                cp.ExStyle |= (int)NativeMethods.WS_EX_NOACTIVATE;
                return cp;
            }
        }
        private void StartProcess(object url)
        {
            if (url.ToString().StartsWith("http://") || url.ToString().StartsWith("https://"))
            {
                try
                {
                    System.Diagnostics.Process.Start(url.ToString());
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    MessageBox.Show("An error occurred in trying to open this URL in the browser. Windows is stupid, so can't really say why. Try again","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
        private Point rightstartpoint = new Point();
        void FeedWin_MouseDown(object sender, MouseEventArgs e)
        {
            if (!pinned && e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
            }
            else if (!pinned && e.Button == MouseButtons.Right)
            {
                rightstartpoint = e.Location;
                this.Cursor = Cursors.SizeWE;
            }
            else if (pinned && e.Button == MouseButtons.Left)
            {
                Uri[] uriarr = new Uri[hotrects.Count];
                Rectangle[] rectarr = new Rectangle[hotrects.Count];
                hotrects.Keys.CopyTo(rectarr, 0);
                hotrects.Values.CopyTo(uriarr, 0);
                for (int ii = 0; ii < hotrects.Count; ii++)
                {

                    if (rectarr[ii].Contains(Cursor.Position))
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(StartProcess), uriarr[ii].ToString());

                    }
                }

                System.Threading.Thread.Sleep(500);
                RedrawWin();
            }
        }
        

        void thisinst_ToggleMoveMode(bool obj)
        {
            if (this.IsDisposed)
            {
                this.Close();
            }
            else
            {
                if (!obj)
                {
                    PinToDesktop(true);
                }
                else
                {
                    UnpinFromDesktop();
                }
            }
        }

        private Bitmap blitimage;
    }    
}
