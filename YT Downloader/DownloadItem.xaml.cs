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
    /// Interaction logic for DownloadItem.xaml
    /// </summary>
    public partial class DownloadItem : UserControl
    {
        public string DownloadName
        {
            get { return (string)GetValue(DownloadNameProperty); }
            set { SetValue(DownloadNameProperty, value); }
        }
        public static readonly DependencyProperty DownloadNameProperty = DependencyProperty.Register("DownloadName", typeof(string), typeof(DownloadItem), new PropertyMetadata("Youtube video name"));

        public string DownloadSize
        {
            get { return (string)GetValue(DownloadSizeProperty); }
            set { SetValue(DownloadSizeProperty, value); }
        }
        public static readonly DependencyProperty DownloadSizeProperty = DependencyProperty.Register("DownloadSizeProperty", typeof(string), typeof(DownloadItem), new PropertyMetadata("0 KB/s"));

        public int DownloadPercentage
        {
            get { return (int)GetValue(DownloadPercentageProperty); }
            set { SetValue(DownloadPercentageProperty, value); }
        }
        public static readonly DependencyProperty DownloadPercentageProperty = DependencyProperty.Register("DownloadPercentage", typeof(int), typeof(DownloadItem), new PropertyMetadata(0));

        public bool IsDownloaded
        {
            get { return (bool)GetValue(IsDownloadedProperty); }
            set { SetValue(IsDownloadedProperty, value); }
        }
        public static readonly DependencyProperty IsDownloadedProperty = DependencyProperty.Register("IsDownloaded", typeof(bool), typeof(DownloadItem), new PropertyMetadata(false));
        public DownloadItem()
        {
            InitializeComponent();
        }

        private void UC_Loaded(object sender, RoutedEventArgs e)
        {            
            if (IsDownloaded)
            {
                Pause_Countinue_OpenDownloadB.Content = "\uED25";
                DownloadSizeLB.Content = "Downloaded";
                DownloadPB.Visibility = Visibility.Collapsed;
                DownloadPercentageLB.Content = DateTime.Now.ToString();
                CheckLB.Content = "\uE73E";
            }
            else
            {
                Pause_Countinue_OpenDownloadB.Content = "\uE769";
                CheckLB.Content = "";
            }
        }

        private void DownloadPB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DownloadPercentageLB.Content = e.NewValue.ToString() + "%";
        }

        private void UC_Initialized(object sender, EventArgs e)
        {

        }

        private void UC_LayoutUpdated(object sender, EventArgs e)
        {
        }

        private void UC_MouseEnter(object sender, MouseEventArgs e)
        {
            ContainerBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF006DD8"));
        }

        private void UC_MouseLeave(object sender, MouseEventArgs e)
        {
            ContainerBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCC"));
        }

        private void Pause_Countinue_OpenDownloadB_Click(object sender, RoutedEventArgs e)
        {
            if((string)Pause_Countinue_OpenDownloadB.Content != "\uED25")
            {
                if ((string)Pause_Countinue_OpenDownloadB.Content == "\uE769")
                {
                    Pause_Countinue_OpenDownloadB.Content = "\uE768";
                }
                else
                {
                    Pause_Countinue_OpenDownloadB.Content = "\uE769";
                }
            }
        }

        private void RemoveDownloadB_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent == DownloadsPage.Instance.DownloadedSP)
            {
                DownloadsPage.Instance.DownloadedSP.Children.Remove(this);
                MainWindow.Instance.ShowMessage("Item removed from downloaded list.", MessageType.Success);

            }        
        }
    }
}
