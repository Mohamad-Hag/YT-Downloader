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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class DownloadsPage : Page
    {
        public static DownloadsPage Instance;
        public DownloadsPage()
        {
            InitializeComponent();
            Instance = this;
        }

        private void GoHomeB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ins = MainWindow.Instance;
            ins.TransitionMove(false);
            ins.LastClickedButtons.Clear();

            var template1 = ins.ViewDownloadsB.Template;
            Border border1 = (Border)template1.FindName("border", ins.ViewDownloadsB);
            border1.BorderThickness = new Thickness(1);
            border1.BorderBrush = Brushes.White;

            var template2 = ins.ViewSettingsB.Template;
            Border border2 = (Border)template2.FindName("border", ins.ViewSettingsB);
            border2.BorderThickness = new Thickness(1);
            border2.BorderBrush = Brushes.White;
        }
    }
}
