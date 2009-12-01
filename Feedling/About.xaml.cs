using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            versionlabel.Content = string.Format(
                "Version {0}.{1}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString(System.Globalization.CultureInfo.InvariantCulture)
            );
            copyrightlabel.Content = ((AssemblyCopyrightAttribute)(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0])).Copyright;
        }

        private void okbtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
