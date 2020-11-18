using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using YT_Downloader.Classes;

namespace YT_Downloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int NumberOfWordsIn(string statement)
        {
            statement = statement.Trim();
            statement = Regex.Replace(statement, @"\s+", " ");
            int number = -1;
            foreach (char c in statement)
            {
                if (c == ' ')
                {
                    number++;
                }
            }
            if (number == -1)
            {
                number = 1;
            }
            return number;
        }
        private void ViewPasswordTBl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            PasswordBox pb = (PasswordBox)tb.TemplatedParent;
            var template = pb.Template;
            TextBox tb1 = (TextBox)template.FindName("PasswordShownLB", pb);
            EntryWindow.ShowHidePassword(pb);
            if (tb.Foreground != Brushes.White)
            {
                tb.Foreground = Brushes.AliceBlue;
                tb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e90ff"));
                tb1.Text = pb.Password;
                tb1.Focus();
            }
            else
            {
                tb.Background = Brushes.White;
                tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e90ff"));
                pb.Password = tb1.Text; tb1.Focus();
            }
        }

        private void PasswordShownLB_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void PasswordShownLB_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            PasswordBox pb = (PasswordBox)tb.TemplatedParent;
            pb.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e90ff")); ;
            tb.Focus();
            tb.CaretIndex = tb.Text.Length;
        }

        private void MainApp_Startup(object sender, StartupEventArgs e)
        {
            string email = YT_Downloader.Properties.Settings.Default.Email;
            string password = YT_Downloader.Properties.Settings.Default["Password"].ToString();
            if (email.Equals(string.Empty) || password.Equals(string.Empty))
                return;
            Current.StartupUri = new Uri("/MainWindow.xaml", UriKind.Relative);
        }
    }
}
