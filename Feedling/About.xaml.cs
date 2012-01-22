/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About
    {
        public About()
        {
            InitializeComponent();
            versionlabel.Content = string.Format(
                "Version {0}",
                Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
                
            );
            copyrightlabel.Content = ((AssemblyCopyrightAttribute)(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0])).Copyright;
        }

        private void okbtn_Click(object sender, RoutedEventArgs e)
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
