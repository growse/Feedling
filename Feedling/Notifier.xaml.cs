/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Feedling.Classes;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for Notifier.xaml
    /// </summary>
    public partial class Notifier
    {
        private static Notifier thisinst;
        public static Notifier GetNotifier()
        {
            return thisinst ?? (thisinst = new Notifier());
        }

        private readonly Storyboard board;
        private PresentationSource presentationSource;
        public Notifier()
        {
            InitializeComponent();
            Opacity = 0;
            board = (Storyboard)FindResource("fader");
        }

        public void ShowNotifier(string feedtitle, IEnumerable<Tuple<string, string>> newitemlist)
        {
            if (Opacity > 0)
            {
                board.Stop(this);
            }
            else
            {
                titlebox.Inlines.Clear();
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(FadeInWindowThingieLikeAnEpicAwesomeToaster));
            }
            titlebox.Inlines.Add(string.Format("New items in {0}", feedtitle));

            foreach (var link in newitemlist)
            {
                titlebox.Inlines.Add(new LineBreak());
                Uri linkuri;
                Uri.TryCreate(link.Item2, UriKind.Absolute, out linkuri);
                var hyperlink = new Hyperlink { NavigateUri = linkuri };
                hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
                hyperlink.Inlines.Add(link.Item1);
                titlebox.Inlines.Add(hyperlink);
            }
            PositionWindow();
        }

        private void FadeInWindowThingieLikeAnEpicAwesomeToaster()
        {
            Opacity = 0;
            Show();
            Visibility = Visibility.Visible;
            PositionWindow();
            board.Begin(this);
        }

        private void PositionWindow()
        {
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            if (presentationSource != null && presentationSource.CompositionTarget != null)
            {
                var transform = presentationSource.CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));
                Left = corner.X - ActualWidth - 20;
                Top = corner.Y - ActualHeight;
            }
        }

        private void DismissButtonClick(object sender, RoutedEventArgs e)
        {
            //Visibility = Visibility.Collapsed;
            //Opacity = 0;
            ShowNotifier("sdflijlij", new List<Tuple<string, string>> { new Tuple<string, string>("dsfl", "sddf") });
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            presentationSource = PresentationSource.FromVisual(this);
            NativeMethods.HideFromAltTab(this);
        }
    }
}
