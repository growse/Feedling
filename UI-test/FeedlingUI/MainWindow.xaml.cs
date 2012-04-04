using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FeedlingUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
		}
		private void lb_click(object sender,RoutedEventArgs e)
		{
			object clicked=(e.OriginalSource as FrameworkElement).DataContext;
			var lbi=lb.ItemContainerGenerator.ContainerFromItem(clicked) as ListBoxItem;
			System.Windows.MessageBox.Show(e.OriginalSource.ToString());
//			lbi.IsSelected=true;
		}
	}
}