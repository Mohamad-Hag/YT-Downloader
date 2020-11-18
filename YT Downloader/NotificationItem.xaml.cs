using MySql.Data.MySqlClient;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YT_Downloader.Classes;

namespace YT_Downloader
{
    /// <summary>
    /// Interaction logic for NotificationItem.xaml
    /// </summary>
    public partial class NotificationItem : UserControl
    {
        private int DismissAnimationDuration = 300;

        public int NotificationId
        {
            get { return (int)GetValue(NotificationIdProperty); }
            set { SetValue(NotificationIdProperty, value); }
        }
        public static readonly DependencyProperty NotificationIdProperty = DependencyProperty.Register("NotificationId", typeof(int), typeof(NotificationItem), new PropertyMetadata(0));
        public string Notification
        {
            get { return (string)GetValue(NotificationProperty); }
            set { SetValue(NotificationProperty, value); }
        }
        public static readonly DependencyProperty NotificationProperty = DependencyProperty.Register("Notification", typeof(string), typeof(NotificationItem), new PropertyMetadata("Notification should apears here..."));

        public DateTime NotificationDate
        {
            get { return (DateTime)GetValue(NotificationDateProperty); }
            set { SetValue(NotificationDateProperty, value); }
        }
        public static readonly DependencyProperty NotificationDateProperty = DependencyProperty.Register("NotificationDate", typeof(DateTime), typeof(NotificationItem), new PropertyMetadata(new DateTime(2020, 1, 1, 12, 0, 0)));
        public NotificationItem()
        {
            InitializeComponent();
        }
        private void UC_MouseEnter(object sender, MouseEventArgs e)
        {
            DismissB.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEE"));
        }

        private void UC_MouseLeave(object sender, MouseEventArgs e)
        {
            DismissB.Background = UC.Background;
        }

        private async void DismissB_Click(object sender, RoutedEventArgs e)
        {
            TranslateTransform tt = new TranslateTransform();
            this.RenderTransform = tt;
            StackPanel sp = this.Parent as StackPanel;
            DoubleAnimation dismissAnimation = new DoubleAnimation();
            dismissAnimation.To = sp.ActualWidth + this.ActualWidth;
            dismissAnimation.Duration = TimeSpan.FromMilliseconds(DismissAnimationDuration);
            dismissAnimation.AccelerationRatio = 0.5;
            tt.BeginAnimation(TranslateTransform.XProperty, dismissAnimation);
            await Task.Delay(DismissAnimationDuration);
            sp.Children.Remove(this);
            DatabaseConnectivity dc = new DatabaseConnectivity();
            dc.Connect();
            MySqlCommand command = new MySqlCommand();
            command.Parameters.AddWithValue("@id", NotificationId);
            dc.Execute(command, "DELETE FROM notifications WHERE NotificationId=@id");
            dc.Unconnect();
            if(sp.Children.Count == 0)
            {
                MainWindow.Instance.NotificationsG.Visibility = Visibility.Collapsed;
            }
        }

        private void UC_Loaded(object sender, RoutedEventArgs e)
        {
            DateLB.Content = DateTime.Now.ToString();
        }

        private void UC_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.NotificationsG.Visibility = Visibility.Collapsed;
            System.Diagnostics.Process.Start("https://www.google.com");
        }
    }
}
