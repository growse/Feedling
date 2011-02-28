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
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;
using FeedHanderPluginInterface;
using NLog;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for FeedWin.xaml
    /// </summary>
    public partial class FeedWin : Window
    {
        #region Vars and Consts
        private FeedConfigItem fci;
        private bool selected = false;
        private Color updatedcolor;
        private Hashtable hotrects = new Hashtable();
        private IFeed rssfeed;
        private bool updating = true;
        private bool pinned;
        private string errormsg = "Fetching...";
        SolidColorBrush textbrush = new SolidColorBrush();
        SolidColorBrush titletextbrush = new SolidColorBrush();
        private ColorAnimation fadein, fadeout;
        private Logger Log = LogManager.GetCurrentClassLogger();
        public FeedConfigItem FeedConfig
        {
            get { return fci; }
            set { fci = value; RedrawWin(); }
        }
        #endregion

        /// <summary>
        /// Constructor! Hurrah!
        /// </summary>
        /// <param name="feeditem"></param>
        public FeedWin(FeedConfigItem feeditem)
        {
            Log.Debug("Constructing feedwin. GUID:{0} URL: {1}", feeditem.Guid, feeditem.Url);
            InitializeComponent();

            fci = feeditem;
            updatedcolor = fci.DefaultColor;
            //Kick of thread to figure out what sort of plugin to load for this sort of feed.
            ThreadPool.QueueUserWorkItem(new WaitCallback(GetFeedType), fci);

            //Set up the animations for the mouseovers
            fadein = new ColorAnimation() { AutoReverse = false, From = fci.DefaultColor, To = fci.HoverColor, Duration = new Duration(TimeSpan.FromMilliseconds(200)), RepeatBehavior = new RepeatBehavior(1) };
            fadeout = new ColorAnimation() { AutoReverse = false, To = fci.DefaultColor, From = fci.HoverColor, Duration = new Duration(TimeSpan.FromMilliseconds(200)), RepeatBehavior = new RepeatBehavior(1) };

            textbrush = new SolidColorBrush(feeditem.DefaultColor);

            //Add the right number of textblocks to the form, depending on what the user asked for.
            for (int ii = 0; ii < fci.DisplayedItems; ii++)
            {
                maingrid.RowDefinitions.Add(new RowDefinition());
                TextBlock textblock = new TextBlock();

                textblock.Style = (Style)FindResource("linkTextStyle");
                textblock.Name = string.Format("TextBlock{0}", ii + 1);
                this.RegisterName(textblock.Name, textblock);
                textblock.TextTrimming = TextTrimming.CharacterEllipsis;
                textblock.Foreground = textbrush.Clone();
                textblock.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
                textblock.FontFamily = fci.FontFamily;
                textblock.FontSize = fci.FontSize;
                textblock.FontStyle = fci.FontStyle;
                textblock.FontWeight = fci.FontWeight;
                textblock.Visibility = Visibility.Collapsed;
                textblock.SetValue(Grid.ColumnSpanProperty, 2);
                maingrid.Children.Add(textblock);
                Grid.SetRow(textblock, ii + 1);
            }

            titletextbrush = new SolidColorBrush();
            titletextbrush.Color = fci.DefaultColor;
        }

        #region Methods

        /// <summary>
        /// Called by FeedWinManager when the feed is selected. Forces a redraw that paints the background a solid colour.
        /// </summary>
        public void Select()
        {
            selected = true;
            RedrawWin();
        }

        /// <summary>
        /// Called by FeedWinManager when the feed is selected.Forces a redraw that removes the background a solid colour.
        /// </summary>
        public void Deselect()
        {
            selected = false;
            RedrawWin();
        }

        private delegate void RedrawWinCallback();
        /// <summary>
        /// Thread-safe window drawing method. Decides basically what colour everything should be, depending on the state of the window (selected, pinned etc.)
        /// Also sets the text of the textblocks to either the feed items, some sort of error, or random mumblings from Hamlet.
        /// </summary>
        private void RedrawWin()
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                RedrawWinCallback d = new RedrawWinCallback(RedrawWin);
                this.Dispatcher.Invoke(d, null);
            }
            else
            {
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

                //Set textblock styling
                titleTextBlock.FontFamily = fci.TitleFontFamily;
                titleTextBlock.FontSize = fci.TitleFontSize;
                titleTextBlock.FontStyle = fci.TitleFontStyle;
                titleTextBlock.FontWeight = fci.TitleFontWeight;
                for (int n = 1; n <= fci.DisplayedItems; n++)
                {
                    TextBlock textblock = ((TextBlock)FindName(string.Format("TextBlock{0}", n)));
                    if (textblock != null)
                    { textblock.Foreground = textbrush.Clone(); }
                }

                //Figure out what to display
                if (rssfeed != null && rssfeed.Loaded)
                {
                    if (rssfeed.HasError)
                    {
                        Log.Debug("Feed has error, so setting title error: {0}", rssfeed.ErrorMessage);
                        //Error has been discovered loading this feed.
                        titleTextBlock.Text = "Error";
                        TextBlock leadingtextblock = ((TextBlock)FindName("TextBlock1"));
                        if (leadingtextblock != null)
                        {
                            if (string.IsNullOrEmpty(rssfeed.ErrorMessage))
                            {
                                leadingtextblock.Text = "No error message was given";
                            }
                            else
                            {
                                leadingtextblock.Text = rssfeed.ErrorMessage;
                            }
                            leadingtextblock.Visibility = Visibility.Visible;
                        }
                        for (int n = 2; n <= fci.DisplayedItems; n++)
                        {
                            TextBlock textblock = ((TextBlock)FindName(string.Format("TextBlock{0}", n)));
                            if (textblock != null)
                            {
                                textblock.Text = "";
                                textblock.Tag = null;
                                textblock.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                    else
                    {
                        Log.Debug("No error, get on with the headlines for: {0}", rssfeed.Title);
                        //No error, get one with putting the headlines out.
                        titleTextBlock.Text = System.Web.HttpUtility.HtmlDecode(rssfeed.Title);
                        titleTextBlock.Tag = rssfeed.FeedUri;
                        for (int n = 1; n <= fci.DisplayedItems; n++)
                        {
                            TextBlock textblock = ((TextBlock)FindName(string.Format("TextBlock{0}", n)));
                            if (textblock != null)
                            {
                                if (rssfeed.FeedItems.Count >= n)
                                {
                                    textblock.Text = System.Web.HttpUtility.HtmlDecode(rssfeed.FeedItems[n - 1].Title);
                                    textblock.Tag = rssfeed.FeedItems[n - 1].Link;
                                    textblock.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    textblock.Text = "";
                                    textblock.Tag = null;
                                    textblock.Visibility = Visibility.Collapsed;
                                }
                            }
                        }
                    }
                }
                else if (rssfeed != null && !rssfeed.Loaded)
                {
                    //Still fetching the feed. Let the user know.
                    titleTextBlock.Text = "Fetching...";
                    TextBlock textblock = ((TextBlock)FindName("TextBlock1"));
                    if (textblock != null)
                    {
                        textblock.Text = fci.Url;
                        textblock.Visibility = Visibility.Visible;
                    }
                }
                else if (rssfeed == null)
                {
                    titleTextBlock.Text = errormsg;
                    TextBlock textblock = ((TextBlock)FindName("TextBlock1"));
                    if (textblock != null)
                    {
                        textblock.Text = fci.Url;
                        textblock.Visibility = Visibility.Visible;
                    }
                }
                this.InvalidateVisual();
            }
        }

        /// <summary>
        /// Takes a given FeedConfiItem and iterates through all the available plugins to see if any of them can handle it.
        /// </summary>
        /// <param name="state"></param>
        private void GetFeedType(object state)
        {
            try
            {
                FeedConfigItem fci = (FeedConfigItem)state;
                Log.Debug("Getting the Feed Type");
                XmlDocument document = (XmlDocument)FeedwinManager.Fetch(fci);
                foreach (IPlugin feedplugin in FeedwinManager.thisinst.Plugins)
                {
                    Log.Debug("Testing {0} to see if it can handle feed", feedplugin.PluginName);
                    if (feedplugin.CanHandle(document))
                    {
                        Log.Debug("It can! Yay!");
                        IWebProxy reqproxy;
                        if (fci.ProxyType != ProxyType.Global) { reqproxy = fci.Proxy; }
                        else { reqproxy = FeedwinManager.GetGlobalProxy(); }
                        if (reqproxy != null)
                        {
                            Log.Debug("Set Proxy for feed to {0}", reqproxy.GetProxy(new Uri(fci.Url)));
                        }
                        else
                        {
                            Log.Debug("Set Proxy for feed to nothing, nothing at all");
                        }
                        rssfeed = feedplugin.AddFeed(new Uri(fci.Url), fci.AuthType, fci.UserName, fci.Password, reqproxy);
                        rssfeed.UpdateInterval = fci.UpdateInterval;
                        break;
                    }
                }
            }
            catch (XmlException ex) { errormsg = "Invalid XML Document"; Log.Error("XMLException thrown in parsing the feed", ex); }
            catch (UriFormatException ex) { errormsg = "Invalid URI"; Log.Error("URIException thrown in fetching the feed", ex); }
            catch (WebException ex) { errormsg = ex.Message; Log.Error("WebException thrown in fetching the feed", ex); }
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
            updating = false;
            RedrawWin();
        }

        /// <summary>
        /// Called to request the feed to update itself.
        /// </summary>
        /// <param name="state">Unused. Useless. Pointless.</param>
        public void UpdateNow(object state)
        {
            Log.Debug("Received request to update feed");
            if (rssfeed == null)
            {
                GetFeedType(fci);
            }
            else
            {
                updating = true;
                RedrawWin();
                rssfeed.Update();
            }
        }

        /// <summary>
        /// Called to load the browser and pass it the given url.
        /// </summary>
        /// <param name="url">URL string to load</param>
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

        /// <summary>
        /// Takes a given coordinate and turns it into the nearest grid X position
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static int NearestX(double input)
        {
            return Convert.ToInt32(Math.Round((double)input / Properties.Settings.Default.GridWidth) * Properties.Settings.Default.GridWidth);
        }

        /// <summary>
        /// Sends window to the bottom.
        /// </summary>
        private void PinToDesktop()
        {
            pinned = true;
            RedrawWin();

            this.Topmost = false;
            NativeMethods.SendWpfWindowBack(this);
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Brings window back to the top.
        /// </summary>
        private void UnpinFromDesktop()
        {
            pinned = false;
            RedrawWin();
            this.Topmost = true;
            this.Cursor = Cursors.SizeAll;
        }
        #endregion

        #region Events
        void rssfeed_Updated(object sender, EventArgs e)
        {
            Log.Debug("Updated event fired");
            updating = false;
            updatedcolor = fci.HoverColor;
            RedrawWin();
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
                PinToDesktop();
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
                    Log.Debug("Starting browser with url [{0}]", ((TextBlock)sender).Tag.ToString());
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
        #endregion

    }
}
