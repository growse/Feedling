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
    public partial class Notifier:Window
    {
        private static Notifier thisinst;
        public static Notifier GetNotifier()
        {
            return thisinst ?? (thisinst = new Notifier());
        }

        private PresentationSource presentationSource;
        public Notifier()
        {
            InitializeComponent();
            Opacity = 0;
        }
        public void ShowNotifier(string feedtitle, IEnumerable<Tuple<string, string>> newitemlist)
        {
            if (Opacity > 0)
            {
                //TODO: Some cunning sort of queuing system
            }
            else
            {
                titlebox.Inlines.Clear();
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
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(FadeInWindowThingieLikeAnEpicAwesomeToaster));
            }
        }

        private void FadeInWindowThingieLikeAnEpicAwesomeToaster()
        {
            Opacity = 0;
            Show();
            Visibility = Visibility.Visible;
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            if (presentationSource != null && presentationSource.CompositionTarget != null)
            {
                var transform = presentationSource.CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));
                Left = corner.X - ActualWidth - 20;
                Top = corner.Y - ActualHeight;
            }

            var board = (Storyboard)FindResource("fader");
            board.Begin(this);
        }

        private void DismissButtonClick(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            Opacity = 0;
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
