using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YT_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void WindowZoom(Window targetWindow)
        {
            if (targetWindow.WindowState == WindowState.Maximized)
                targetWindow.WindowState = WindowState.Normal;
            else
                targetWindow.WindowState = WindowState.Maximized;
        }
        private void CheckVideo()
        {
            MessageBox.Show("");
        }
        private void WindowZoomB_Click(object sender, RoutedEventArgs e)
        {
            WindowZoom(this);
        }

        private void TitleBarDP_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
            if (e.ClickCount == 2)
                WindowZoom(this);
        }
        private void UrlTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UrlTB.Text != string.Empty)
            {
                CheckB.IsEnabled = true;
            }
            else
            {
                CheckB.IsEnabled = false;
            }
        }

        private void Check_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckVideo();
        }

        private void CheckB_Click(object sender, RoutedEventArgs e)
        {
            CheckVideo();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
                WindowZoomB.Content = "\uE922";
            else
                WindowZoomB.Content = "\uE923";
        }
    }
}
