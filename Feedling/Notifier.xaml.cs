/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Threading;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for Notifier.xaml
    /// </summary>
    public partial class Notifier : Window
    {
        public Notifier(string feedtitle, IEnumerable<Tuple<string, string>> newitemlist)
        {
            InitializeComponent();
            titlebox.Inlines.Clear();
            titlebox.Inlines.Add(string.Format("New items in {0}", feedtitle));

            foreach (var link in newitemlist)
            {
                titlebox.Inlines.Add(new LineBreak());
                var hyperlink = new Hyperlink { NavigateUri = new Uri(link.Item2) };
                hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
                hyperlink.Inlines.Add(link.Item1);
                titlebox.Inlines.Add(hyperlink);
            }

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                var workingArea = Screen.PrimaryScreen.WorkingArea;
                var presentationSource = PresentationSource.FromVisual(this);
                if (presentationSource != null && presentationSource.CompositionTarget != null)
                {
                    var transform = presentationSource.CompositionTarget.TransformFromDevice;
                    var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));
                    Left = corner.X - ActualWidth - 20;
                    Top = corner.Y - ActualHeight;
                }
            }));
            NativeMethods.HideFromAltTab(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
