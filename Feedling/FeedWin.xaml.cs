/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
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
        private bool selected;
        private IFeed rssfeed;
        private bool pinned;
        private string errormsg = "Fetching...";
        private SolidColorBrush textbrush = new SolidColorBrush();
        private readonly Image movehandle;
        private readonly ColorAnimation fadein;
        private readonly ColorAnimation fadeout;
        private readonly Logger Log = LogManager.GetCurrentClassLogger();
        private string CurrentTopStory = "";
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
            Width = fci.Width;
            Left = fci.Position.X;
            Top = fci.Position.Y;
            //Kick of thread to figure out what sort of plugin to load for this sort of feed.
            ThreadPool.QueueUserWorkItem(GetFeedType, fci);

            //Set up the animations for the mouseovers
            fadein = new ColorAnimation { AutoReverse = false, From = fci.DefaultColor, To = fci.HoverColor, Duration = new Duration(TimeSpan.FromMilliseconds(200)), RepeatBehavior = new RepeatBehavior(1) };
            fadeout = new ColorAnimation { AutoReverse = false, To = fci.DefaultColor, From = fci.HoverColor, Duration = new Duration(TimeSpan.FromMilliseconds(200)), RepeatBehavior = new RepeatBehavior(1) };

            textbrush = new SolidColorBrush(feeditem.DefaultColor);

            //Add the right number of textblocks to the form, depending on what the user asked for.
            for (var ii = 0; ii < fci.DisplayedItems; ii++)
            {
                maingrid.RowDefinitions.Add(new RowDefinition());
                var textblock = new TextBlock
                                    {
                                        Style = (Style)FindResource("linkTextStyle"),
                                        Name = string.Format("TextBlock{0}", ii + 1),
                                        TextTrimming = TextTrimming.CharacterEllipsis,
                                        Foreground = textbrush.Clone(),
                                        Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)),
                                        FontFamily = fci.FontFamily,
                                        FontSize = fci.FontSize,
                                        FontStyle = fci.FontStyle,
                                        FontWeight = fci.FontWeight,
                                        Visibility = Visibility.Collapsed
                                    };


                textblock.SetValue(Grid.ColumnSpanProperty, 2);
                RegisterName(textblock.Name, textblock);
                maingrid.Children.Add(textblock);
                Grid.SetRow(textblock, ii + 1);
            }


            maingrid.RowDefinitions.Add(new RowDefinition());
            movehandle = new Image();
            movehandle.Width = movehandle.Height = 16;
            movehandle.Name = "movehandle";

            movehandle.HorizontalAlignment = HorizontalAlignment.Left;

            movehandle.Cursor = Cursors.SizeAll;
            movehandle.Source = new BitmapImage(new Uri("/Feedling;component/Resources/move-icon.png", UriKind.Relative));
            movehandle.SetValue(Grid.ColumnSpanProperty, 2);
            RegisterName(movehandle.Name, movehandle);
            maingrid.Children.Add(movehandle);
            movehandle.MouseDown += movehandle_MouseDown;
            movehandle.Visibility = Visibility.Collapsed;
            Grid.SetRow(movehandle, maingrid.RowDefinitions.Count);


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
        /// Called by FeedWinManager when the feed is selected. Forces a redraw that removes the background a solid colour.
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
        internal void RedrawWin()
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                RedrawWinCallback d = RedrawWin;
                Dispatcher.Invoke(d, null);
            }
            else
            {
                if (selected)
                {
                    Background = new SolidColorBrush(Colors.White);
                    textbrush = new SolidColorBrush(Colors.Black);
                    fadein.To = fadein.From = Colors.Black;
                    fadeout.To = fadeout.From = Colors.Black;
                }
                else
                {
                    Background = new SolidColorBrush(Colors.Transparent);
                    textbrush = new SolidColorBrush(fci.DefaultColor);
                    fadein.To = fadeout.From = fci.HoverColor;
                    fadeout.To = fadein.From = fci.DefaultColor;
                    movehandle.Visibility = !pinned ? Visibility.Visible : Visibility.Collapsed;
                }

                //Set textblock styling
                titleTextBlock.FontFamily = fci.TitleFontFamily;
                titleTextBlock.FontSize = fci.TitleFontSize;
                titleTextBlock.FontStyle = fci.TitleFontStyle;
                titleTextBlock.FontWeight = fci.TitleFontWeight;
                titleTextBlock.Foreground = textbrush.Clone();
                for (var n = 1; n <= fci.DisplayedItems; n++)
                {
                    var textblock = ((TextBlock)FindName(string.Format("TextBlock{0}", n)));
                    if (textblock == null) continue;
                    textblock.Foreground = textbrush.Clone();
                    textblock.FontFamily = fci.FontFamily;
                    textblock.FontSize = fci.FontSize;
                    textblock.FontWeight = fci.FontWeight;
                    textblock.FontStyle = fci.FontStyle;
                }

                //Figure out what to display
                if (rssfeed != null && rssfeed.Loaded)
                {
                    if (rssfeed.HasError)
                    {
                        Log.Debug("Feed has error, so setting title error: {0}", rssfeed.ErrorMessage);
                        //Error has been discovered loading this feed.
                        titleTextBlock.Text = "Error";
                        var leadingtextblock = ((TextBlock)FindName("TextBlock1"));
                        if (leadingtextblock != null)
                        {
                            leadingtextblock.Text = string.IsNullOrEmpty(rssfeed.ErrorMessage) ? "No error message was given" : rssfeed.ErrorMessage;
                            leadingtextblock.Visibility = Visibility.Visible;
                        }
                        for (var n = 2; n <= fci.DisplayedItems; n++)
                        {
                            var textblock = ((TextBlock)FindName(string.Format("TextBlock{0}", n)));
                            if (textblock == null) continue;
                            textblock.Text = "";
                            textblock.Tag = null;
                            textblock.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        Log.Debug("No error, get on with the headlines for: {0}", rssfeed.Title);
                        //No error, get one with putting the headlines out.
                        titleTextBlock.Text = rssfeed.Title;
                        titleTextBlock.Tag = rssfeed.FeedUri;
                        for (var n = 1; n <= fci.DisplayedItems; n++)
                        {
                            var textblock = ((TextBlock)FindName(string.Format("TextBlock{0}", n)));
                            if (textblock == null) continue;
                            if (rssfeed.FeedItems.Count >= n)
                            {
                                textblock.Text = rssfeed.FeedItems[n - 1].Title;
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
                else if (rssfeed != null && !rssfeed.Loaded)
                {
                    //Still fetching the feed. Let the user know.
                    titleTextBlock.Text = "Fetching...";
                    var textblock = ((TextBlock)FindName("TextBlock1"));
                    if (textblock != null)
                    {
                        textblock.Text = fci.Url;
                        textblock.Visibility = Visibility.Visible;
                    }
                }
                else if (rssfeed == null)
                {
                    titleTextBlock.Text = errormsg;
                    var textblock = ((TextBlock)FindName("TextBlock1"));
                    if (textblock != null)
                    {
                        textblock.Text = fci.Url;
                        textblock.Visibility = Visibility.Visible;
                    }
                }
                InvalidateVisual();
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
                var fci = (FeedConfigItem)state;
                Log.Debug("Getting the Feed Type");
                var document = (XmlDocument)FeedwinManager.Fetch(fci);
                foreach (var feedplugin in FeedwinManager.thisinst.Plugins)
                {
                    Log.Debug("Testing {0} to see if it can handle feed", feedplugin.PluginName);
                    if (!feedplugin.CanHandle(document)) continue;
                    Log.Debug("It can! Yay!");
                    var reqproxy = fci.ProxyType != ProxyType.Global ? fci.Proxy : FeedwinManager.GetGlobalProxy();
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
                rssfeed.Updated += rssfeed_Updated;
                Log.Debug("Kicking off the watcher thread");
                var t = new Thread(rssfeed.Watch);
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
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
                RedrawWin();
                rssfeed.Update();
            }
        }

        /// <summary>
        /// Called to load the browser and pass it the given url.
        /// </summary>
        /// <param name="url">URL string to load</param>
        private static void StartProcess(object url)
        {
            if (!url.ToString().StartsWith("http://") && !url.ToString().StartsWith("https://")) return;
            try
            {
                System.Diagnostics.Process.Start(url.ToString());
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show(
                    "An error occurred in trying to open this URL in the browser. Windows is stupid, so can't really say why. Try again",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Takes a given coordinate and turns it into the nearest grid X position
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static int NearestX(double input)
        {
            return Convert.ToInt32(Math.Round(input / Properties.Settings.Default.GridWidth) * Properties.Settings.Default.GridWidth);
        }

        /// <summary>
        /// Sends window to the bottom.
        /// </summary>
        private void PinToDesktop()
        {
            pinned = true;
            RedrawWin();

            Topmost = false;
            NativeMethods.SendWpfWindowBack(this);
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Brings window back to the top.
        /// </summary>
        private void UnpinFromDesktop()
        {
            pinned = false;
            RedrawWin();
            Topmost = true;
        }
        #endregion

        #region Events
        void rssfeed_Updated(object sender, EventArgs e)
        {
            Log.Debug("Updated event fired");
            FeedConfig.FeedLabel = rssfeed.Title;
            if (FeedConfig.NotifyOnNewItem && rssfeed.FeedItems.Count > 0 && !rssfeed.FeedItems[0].Title.Equals(CurrentTopStory) && !string.IsNullOrEmpty(CurrentTopStory))
            {
                var newitemlist = rssfeed.FeedItems.TakeWhile(item => !item.Title.Equals(CurrentTopStory)).Select(item => new Tuple<string, string>(item.Title, item.Link.ToString())).ToList();
                var notifier = new Notifier(rssfeed.Title, newitemlist);
                notifier.Show();
                //I'm sure there's a good reason why this works. Notifier doens't show up otherwise, as we're on our own Thread.
                System.Windows.Threading.Dispatcher.Run();
            }
            CurrentTopStory = rssfeed.FeedItems[0].Title;
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
                Width = fci.Width;
            }
            else
            {
                fci.Width = Width;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Debug("Feedwin has been loaded");
            Width = fci.Width;
            Height = 200;
            if (FeedwinManager.thisinst.MoveMode)
            {
                UnpinFromDesktop();
            }
            else
            {
                PinToDesktop();
            }
            Left = fci.Position.X;
            Top = fci.Position.Y;
            FeedwinManager.thisinst.ToggleMoveMode += thisinst_ToggleMoveMode;
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

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (pinned)
            {
                //Left = fci.Position.X;
                //Top = fci.Position.Y;
                //WindowState = WindowState.Normal;
            }
            else
            {
                Left = NearestX(Left);
                Top = NearestX(Top);
                fci.Position = new Point(Left, Top);
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
            if (sender == null || !(sender is TextBlock) || !pinned) return;
            if (((TextBlock)sender).Tag == null) return;
            Log.Debug("Starting browser with url [{0}]", ((TextBlock)sender).Tag.ToString());
            ThreadPool.QueueUserWorkItem(StartProcess, ((TextBlock)sender).Tag.ToString());
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!pinned)
            {
                //NativeMethods.MakeWindowMovable(this);
            }
        }

        private double startpoint;
        private double initialwidth;

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (pinned) return;
            initialwidth = Width;
            startpoint = PointToScreen(Mouse.GetPosition(this)).X;
            Cursor = Cursors.SizeWE;
        }
        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!pinned)
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!pinned && e.RightButton == MouseButtonState.Pressed)
            {
                Width = initialwidth - (startpoint - PointToScreen(Mouse.GetPosition(this)).X);
            }
        }
        void movehandle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        #endregion
    }
}
