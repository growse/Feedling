using System;
using System.Collections;
using System.Windows.Input;
using System.Windows.Controls;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Xml;
using FeedHanderPluginInterface;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for FeedWin.xaml
    /// </summary>
    public partial class FeedWin : Window
    {
        public event Action<FeedWin> FormMoved;
        private FeedConfigItem fci;
        private System.Drawing.Image feedimage;
        private Uri oldimageurl;
        private bool selected = false;
        private Color updatedcolor;
        private int fader = 0;
        private Hashtable hotrects = new Hashtable();
        private IFeed rssfeed;
        private bool pinned;
        private string errormsg = "Fetching...";

        private ColorAnimation fadein, fadeout;

        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public FeedConfigItem FeedConfig
        {
            get { return fci; }
            set { fci = value; RedrawWin(); }
        }
        public FeedWin(FeedConfigItem feeditem)
        {
            Log.DebugFormat("Constructing feedwin for [{0}]", feeditem.Url);


            InitializeComponent();

            fci = feeditem;
            updatedcolor = fci.DefaultColor;
            ThreadPool.QueueUserWorkItem(new WaitCallback(GetFeedType), fci);

            fadein = new ColorAnimation() { AutoReverse = false, From = fci.DefaultColor, To = fci.HoverColor, Duration = new Duration(TimeSpan.FromMilliseconds(200)), RepeatBehavior = new RepeatBehavior(1) };
            fadeout = new ColorAnimation() { AutoReverse = false, To = fci.DefaultColor, From = fci.HoverColor, Duration = new Duration(TimeSpan.FromMilliseconds(200)), RepeatBehavior = new RepeatBehavior(1) };

        }
        public void Select()
        {
            selected = true;
            RedrawWin();
        }

        public void Deselect()
        {
            selected = false;
            RedrawWin();
        }
        private delegate void RedrawWinCallback();
        private void RedrawWin()
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                RedrawWinCallback d = new RedrawWinCallback(RedrawWin);
                this.Dispatcher.Invoke(d, null);
            }
            else
            {
                SolidColorBrush textbrush = new SolidColorBrush();
                if (selected)
                {
                    this.Background = new SolidColorBrush(Colors.White);
                    textbrush = new SolidColorBrush(Colors.Black);
                    fadein.To = fadein.From = Colors.Black;
                    fadeout.To = fadeout.From = Colors.Black;
                }
                else if (pinned)
                {
                    this.Background = new SolidColorBrush(Colors.Transparent);
                    textbrush = new SolidColorBrush(fci.DefaultColor);
                    fadein.To = fadeout.From = fci.HoverColor;
                    fadeout.To = fadein.From = fci.DefaultColor;
                }
                else
                {
                    fadein.To = fadein.From = Colors.White;
                    fadeout.To = fadeout.From = Colors.White;
                    textbrush = new SolidColorBrush(Colors.White);
                    this.Background = new SolidColorBrush(Colors.Black);
                }
                double heightcounter = 0;
                if (rssfeed != null && rssfeed.FeedItems.Count > 0)
                {
                    titleTextBlock.Text = System.Web.HttpUtility.HtmlDecode(rssfeed.Title);
                    titleTextBlock.Margin = new Thickness(0, 0, 0, 0);
                    titleTextBlock.Foreground = textbrush.Clone();
                    titleTextBlock.Tag = rssfeed.FeedUri;
                    titleTextBlock.FontFamily = fci.TitleFontFamily;
                    titleTextBlock.FontSize = fci.TitleFontSize;
                    titleTextBlock.FontStyle = fci.TitleFontStyle;
                    titleTextBlock.FontWeight = fci.TitleFontWeight;
                    heightcounter += titleTextBlock.ActualHeight;

                    for (int n = 1; n <= 10; n++)
                    {
                        TextBlock textblock = ((TextBlock)FindName(string.Format("TextBlock{0}", n)));
                        if (textblock != null)
                        {
                            if (rssfeed.FeedItems.Count >= n)
                            {
                                textblock.TextTrimming = TextTrimming.CharacterEllipsis;
                                textblock.Text = System.Web.HttpUtility.HtmlDecode(rssfeed.FeedItems[n - 1].Title);
                                textblock.Tag = rssfeed.FeedItems[n - 1].Link;
                                textblock.Margin = new Thickness(0, heightcounter, 0, 0);
                                textblock.Foreground = textbrush.Clone();
                                textblock.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
                                textblock.FontFamily = fci.FontFamily;
                                textblock.FontSize = fci.FontSize;
                                textblock.FontStyle = fci.FontStyle;
                                textblock.FontWeight = fci.FontWeight;
                                heightcounter += textblock.ActualHeight;

                            }
                            else
                            {
                                textblock.Text = "";
                            }
                        }
                    }

                }
                else
                {
                    if (rssfeed != null)
                    {
                        if (rssfeed.HasError)
                        {
                            titleTextBlock.Text = "Error";
                        }
                        else
                        {
                            titleTextBlock.Text = fci.Url;
                        }
                    }
                    else
                    {
                        titleTextBlock.Text = "Fetching...";
                    }
                    titleTextBlock.Margin = new Thickness(0, 0, 0, 0);
                    titleTextBlock.Foreground = textbrush.Clone();
                    if (rssfeed != null) { titleTextBlock.Tag = rssfeed.FeedUri; }
                    titleTextBlock.FontFamily = fci.TitleFontFamily;
                    titleTextBlock.FontSize = fci.TitleFontSize;
                    titleTextBlock.FontStyle = fci.TitleFontStyle;
                    titleTextBlock.FontWeight = fci.TitleFontWeight;
                    heightcounter += titleTextBlock.ActualHeight;
                    if (rssfeed!=null && !string.IsNullOrEmpty(rssfeed.ErrorMessage))
                    {
                        errormsg = rssfeed.ErrorMessage;
                    }
                    TextBlock1.Text = errormsg;
                    TextBlock1.TextTrimming = TextTrimming.CharacterEllipsis;
                    TextBlock1.Margin = new Thickness(0, heightcounter, 0, 0);
                    TextBlock1.Foreground = textbrush.Clone();
                    TextBlock1.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
                    TextBlock1.FontFamily = fci.FontFamily;
                    TextBlock1.FontSize = fci.FontSize;
                    TextBlock1.FontStyle = fci.FontStyle;
                    TextBlock1.FontWeight = fci.FontWeight;
                    heightcounter += TextBlock1.ActualHeight;

                }
                this.Height = heightcounter;
                this.InvalidateVisual();
            }
        }
        
        private void GetFeedType(object state)
        {
            try
            {
                FeedConfigItem fci = (FeedConfigItem)state;
                log4net.ThreadContext.Properties["myContext"] = string.Format("{0} GetFeedType",fci.Url);
                Log.Debug("Getting the Feed Type");
                XmlDocument document = (XmlDocument)FeedwinManager.Fetch(fci);
                foreach (IFeed feedplugin in FeedwinManager.thisinst.Plugins)
                {
                    Log.DebugFormat("Testing {0} to see if it can handle feed",feedplugin.PluginName);
                    if (feedplugin.CanHandle(document))
                    {
                        Log.DebugFormat("It can! Yay!");
                        IWebProxy reqproxy;
                        if (fci.Proxytype != ProxyType.Global) { reqproxy = fci.Proxy; }
                        else { reqproxy = FeedwinManager.GetGlobalProxy(); }
                        if (reqproxy != null)
                        {
                            Log.DebugFormat("Set Proxy for feed to {0}", reqproxy.GetProxy(new Uri(fci.Url)));
                        }
                        else
                        {
                            Log.Debug("Set Proxy for feed to nothing, nothing at all");
                        }
                        rssfeed = feedplugin.Factory(new Uri(fci.Url), fci.AuthType, fci.Username, fci.Password, reqproxy);
                        rssfeed.UpdateInterval = fci.UpdateInterval;
                        break;
                    }
                }
            }
            catch (XmlException ex) { errormsg = "Invalid XML Document"; Log.Error("XMLException thrown in parsing the feed",ex); }
            catch (UriFormatException ex) { errormsg = "Invalid URI";  Log.Error("URIException thrown in fetching the feed",ex);}
            catch (WebException ex) { errormsg = ex.Message;Log.Error("WebException thrown in fetching the feed",ex); }
            if (rssfeed == null)
            {
                Log.Debug("Didn't find a plugin to handle this feed");
                if (errormsg == string.Empty)
                {
                    errormsg = "No Plugin to handle feed";
                }
            }
            else
            {
                rssfeed.Updated += new EventHandler(rssfeed_Updated);
                Log.Debug("Kicking off the watcher thread");
                ThreadPool.QueueUserWorkItem(new WaitCallback(rssfeed.Watch));
            }
            RedrawWin();
        }
        private void FetchImage(object state)
        {
            log4net.ThreadContext.Properties["myContext"] = string.Format("{0} Fetch Image", fci.Url);
            Log.Debug("Going to try and fetch an image for feed");
            WebResponse wresp;
            try
            {
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(oldimageurl);
                wr.UserAgent = "Mozilla/5.0";
                wr.Timeout = 10000;
                wresp = wr.GetResponse();
                
                feedimage = System.Drawing.Image.FromStream(wresp.GetResponseStream());
                RedrawWin();
            }
            catch (Exception ex) { Log.Error("Exception thrown when fetching image for feed", ex); }
        }
        void rssfeed_Updated(object sender, EventArgs e)
        {
            if (rssfeed.ImageUrl != null && rssfeed.ImageUrl != oldimageurl)
            {
                oldimageurl = rssfeed.ImageUrl;
                ThreadPool.QueueUserWorkItem(new WaitCallback(FetchImage));
            }
            updatedcolor = fci.HoverColor;
            RedrawWin();
        }
        public void UpdateNow(object state)
        {
            Log.Debug("Received request to update feed");
            if (rssfeed == null)
            {
                GetFeedType(fci);
            }
            else
            {
                rssfeed.Update();
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
                    MessageBox.Show("An error occurred in trying to open this URL in the browser. Windows is stupid, so can't really say why. Try again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (pinned)
            {
                NativeMethods.SendWpfWindowBack(this);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
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
        private void PinToDesktop()
        {
            PinToDesktop(false);
        }
        private void PinToDesktop(bool setpos)
        {
            pinned = true;
            RedrawWin();
            if (setpos)
            {
                //Formmoved
            }
            this.Topmost = false;
            NativeMethods.SendWpfWindowBack(this);
            this.Cursor = Cursors.Arrow;
            
        }
        private void UnpinFromDesktop()
        {
            pinned = false;
            RedrawWin();
            this.Topmost = true;
            this.Cursor = Cursors.SizeAll;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Debug("Feedwin has been loaded");
            this.Width = fci.Width;
            this.Height = 200;
            if (FeedwinManager.thisinst.MoveMode)
            {
                UnpinFromDesktop();
            }
            else
            {
                PinToDesktop();
            }
            this.Left = fci.Position.X;
            this.Top = fci.Position.Y;
            FeedwinManager.thisinst.RedrawAll += new EventHandler(thisinst_RedrawAll);
            FeedwinManager.thisinst.ToggleMoveMode += new Action<bool>(thisinst_ToggleMoveMode);
            NativeMethods.SetWindowLongToolWindow(this);
        }

        void thisinst_ToggleMoveMode(bool obj)
        {
            if (!obj)
            {
                Log.Debug("Move mode toggled - pinning to desktop");
                PinToDesktop(true);
            }
            else
            {
                Log.Debug("Move mode toggled - unpinning from desktop");
                UnpinFromDesktop();
            }

        }
        void thisinst_RedrawAll(object sender, EventArgs e)
        {
            RedrawWin();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (pinned)
            {
                this.Left = fci.Position.X;
                this.Top = fci.Position.Y;
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.Left = NearestX(this.Left);
                this.Top = NearestX(this.Top);
                fci.Position = new Point(this.Left, this.Top);
            }
        }
        private static int NearestX(double input)
        {
            return Convert.ToInt32(Math.Round((double)input / Properties.Settings.Default.GridWidth) * Properties.Settings.Default.GridWidth);
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender != null && sender is TextBlock)
            {
                ((TextBlock)sender).Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fadein);
            }
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender != null && sender is TextBlock)
            {
                ((TextBlock)sender).Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fadeout);
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && sender is TextBlock && pinned)
            {
                if (((TextBlock)sender).Tag != null)
                {
                    Log.DebugFormat("Starting browser with url [{0}]", ((TextBlock)sender).Tag.ToString());
                    ThreadPool.QueueUserWorkItem(new WaitCallback(StartProcess), ((TextBlock)sender).Tag.ToString());
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!pinned)
            {
                NativeMethods.MakeWindowMovable(this);                
            }
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!pinned)
            {
                //rightstartpoint = e.Location;
                this.Cursor = Cursors.SizeWE;
            }
        }
    }
}
