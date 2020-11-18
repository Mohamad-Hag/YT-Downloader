using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using YT_Downloader.Classes;
using MySql.Data.MySqlClient;
using System.Net;
using System.Diagnostics;

namespace YT_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public enum MessageType
    {
        Error, Success, Warning, Information
    }
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        private string MessageErrorBackground = "#FFF9C3CB";
        private string MessageErrorForeground = "#FFBF0818";
        private string MessageErrorIcon = "\uEA39";

        private string MessageWarningBackground = "#FFF9EFC3";
        private string MessageWarningForeground = "#FFEEB800";
        private string MessageWarningIcon = "\uE7BA";

        private string MessageInformationBackground = "#FFC3F4F9";
        private string MessageInformationForeground = "#FF008C9B";
        private string MessageInformationIcon = "\uE946";

        private string MessageSuccessBackground = "#FFC3F9C3";
        private string MessageSuccessForeground = "#FF00AE00";
        private string MessageSuccessIcon = "\uE73E";

        private DispatcherTimer MessageTimer = new DispatcherTimer();
        private DispatcherTimer HideMessageTimer = new DispatcherTimer();

        private Task t1, t2;

        private int TransitionTime = 200;
        private Task TransitionTask;

        public SettingsPage sp = new SettingsPage();
        public DownloadsPage dp = new DownloadsPage();

        public List<Button> LastClickedButtons = new List<Button>();

        private int ClearAllAnimationDuration = 300;
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            Button b1 = new Button();
            Button b2 = new Button();
            LastClickedButtons.Add(b1);
            LastClickedButtons.Add(b2);
            UsernameRN.Text = Properties.Settings.Default.Username;
            UserIdLB.Content = Properties.Settings.Default.Id;       
        }

        #region Custom Methods
        private static bool IsInternetAvailable()
        {
            WebRequest request = WebRequest.Create("https://www.google.com");
            WebResponse response;
            try
            {
                response = request.GetResponse();
                response.Close();
                request = null;
                return true;
            }
            catch (Exception ex)
            {
                request = null;
                return false;
            }
        }
        private async void CheckNotificationsCount()
        {
            if (IsInternetAvailable())
            {
                try
                {
                    while (true)
                    {
                        int count = 0;
                        await Task.Delay(200);
                        DatabaseConnectivity dc = new DatabaseConnectivity();
                        dc.Connect();
                        MySqlCommand command = new MySqlCommand();
                        command.Parameters.AddWithValue("@id", UserIdLB.Content);
                        dc.ExecuteReader(command, "SELECT * FROM notifications WHERE AccountId=@id and IsRead=0");
                        MySqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            count++;
                        }
                        var template = ViewNotificationsB.Template;
                        TextBlock notificationCount = (TextBlock)template.FindName("NotificationCountTBl", ViewNotificationsB);
                        Border notificationBorder = (Border)template.FindName("NotificationBorder", ViewNotificationsB);
                        if (count > 0)
                        {
                            notificationBorder.Visibility = Visibility.Visible;
                            notificationCount.Text = count.ToString();
                        }
                        else
                        {
                            if (notificationBorder.Visibility == Visibility.Visible)
                                notificationBorder.Visibility = Visibility.Collapsed;
                        }
                        dc.Unconnect();
                    }

                }
                catch
                {
                    ShowMessage("Check your internet connection and try again", MessageType.Error);
                }

            }
            else
            {
                ShowMessage("Check your internet connection and try again", MessageType.Error);
            }
        }
        public void ShowMessage(string message)
        {
            MessageTimer.Stop();
            HideMessageTimer.Stop();
            MessageBorder.Visibility = Visibility.Visible;
            message.Trim();
            MessageLB.Content = message;
            new Thread(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    DoubleAnimation errorAnimation = new DoubleAnimation();
                    errorAnimation.From = 0;
                    errorAnimation.To = 1;
                    errorAnimation.AccelerationRatio = 0.5;
                    errorAnimation.Duration = TimeSpan.FromMilliseconds(300);
                    MessageBorder.BeginAnimation(OpacityProperty, errorAnimation);
                });
            }).Start();

            MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageInformationBackground));
            MessageBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageInformationForeground));
            MessageIconLB.Content = MessageInformationIcon;
            MessageTimer.Interval = TimeSpan.FromMilliseconds((500 * App.NumberOfWordsIn(message)) + 2000);
            MessageTimer.Tick += ((sender, e) =>
            {
                new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DoubleAnimation errorAnimation = new DoubleAnimation();
                        errorAnimation.To = 0;
                        errorAnimation.AccelerationRatio = 0.5;
                        errorAnimation.Duration = TimeSpan.FromMilliseconds(300);
                        MessageBorder.BeginAnimation(OpacityProperty, errorAnimation);
                        HideMessageTimer.Interval = TimeSpan.FromMilliseconds(300);
                        HideMessageTimer.Tick += ((sender1, e1) =>
                        {
                            MessageBorder.Visibility = Visibility.Collapsed;
                        });
                        HideMessageTimer.Start();
                    });
                }).Start();
            });
            MessageTimer.Start();
        }
        public void ShowMessage(MessageType type)
        {
            MessageTimer.Stop();
            HideMessageTimer.Stop();

            MessageBorder.Visibility = Visibility.Visible;
            t1 = new Task(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    DoubleAnimation errorAnimation = new DoubleAnimation();
                    errorAnimation.From = 0;
                    errorAnimation.To = 1;
                    errorAnimation.AccelerationRatio = 0.5;
                    errorAnimation.Duration = TimeSpan.FromMilliseconds(300);
                    MessageBorder.BeginAnimation(OpacityProperty, errorAnimation);
                });
            });
            t1.Start();
            if (type == MessageType.Error)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageErrorBackground));
                MessageBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageErrorForeground));
                MessageIconLB.Content = MessageErrorIcon;
                MessageLB.Content = "Something went wrong";
            }
            else if (type == MessageType.Warning)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageWarningBackground));
                MessageBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageWarningForeground));
                MessageIconLB.Content = MessageWarningIcon;
                MessageLB.Content = "Something may went wrong";
            }
            else if (type == MessageType.Information)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageInformationBackground));
                MessageBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageInformationForeground));
                MessageIconLB.Content = MessageInformationIcon;
                MessageLB.Content = "Contact support if you need some help";
            }
            else if (type == MessageType.Success)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageSuccessBackground));
                MessageBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageSuccessForeground));
                MessageIconLB.Content = MessageSuccessIcon;
                MessageLB.Content = "Process completed successfully";
            }
            MessageTimer.Interval = TimeSpan.FromMilliseconds((500 * App.NumberOfWordsIn(MessageLB.Content.ToString())) + 2000);
            MessageTimer.Tick += (async (sender, e) =>
            {
                t2 = new Task(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DoubleAnimation errorAnimation = new DoubleAnimation();
                        errorAnimation.To = 0;
                        errorAnimation.AccelerationRatio = 0.5;
                        errorAnimation.Duration = TimeSpan.FromMilliseconds(300);
                        MessageBorder.BeginAnimation(OpacityProperty, errorAnimation);
                        HideMessageTimer.Interval = TimeSpan.FromMilliseconds(300);
                        HideMessageTimer.Tick += ((sender1, e1) =>
                        {
                            MessageBorder.Visibility = Visibility.Collapsed;
                        });
                        HideMessageTimer.Start();
                    });
                });
                t2.Start();
                await Task.WhenAll(t1, t2);
            });
            MessageTimer.Start();

        }
        public void ShowMessage(string message, MessageType type)
        {
            MessageTimer.Stop();
            HideMessageTimer.Stop();
            MessageBorder.Visibility = Visibility.Visible;
            message.Trim();
            MessageLB.Content = message;
            new Thread(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    DoubleAnimation errorAnimation = new DoubleAnimation();
                    errorAnimation.From = 0;
                    errorAnimation.To = 1;
                    errorAnimation.AccelerationRatio = 0.5;
                    errorAnimation.Duration = TimeSpan.FromMilliseconds(300);
                    MessageBorder.BeginAnimation(OpacityProperty, errorAnimation);
                });
            }).Start();
            if (type == MessageType.Error)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageErrorBackground));
                MessageBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageErrorForeground));
                MessageIconLB.Content = MessageErrorIcon;
            }
            else if (type == MessageType.Warning)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageWarningBackground));
                MessageBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageWarningForeground));
                MessageIconLB.Content = MessageWarningIcon;
            }
            else if (type == MessageType.Information)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageInformationBackground));
                MessageBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageInformationForeground));
                MessageIconLB.Content = MessageInformationIcon;
            }
            else if (type == MessageType.Success)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageSuccessBackground));
                MessageBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MessageSuccessForeground));
                MessageIconLB.Content = MessageSuccessIcon;
            }
            MessageTimer.Interval = TimeSpan.FromMilliseconds((500 * App.NumberOfWordsIn(message)) + 2000);
            MessageTimer.Tick += ((sender, e) =>
            {
                new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DoubleAnimation errorAnimation = new DoubleAnimation();
                        errorAnimation.To = 0;
                        errorAnimation.AccelerationRatio = 0.5;
                        errorAnimation.Duration = TimeSpan.FromMilliseconds(300);
                        MessageBorder.BeginAnimation(OpacityProperty, errorAnimation);
                        HideMessageTimer.Interval = TimeSpan.FromMilliseconds(300);
                        HideMessageTimer.Tick += ((sender1, e1) =>
                        {
                            MessageBorder.Visibility = Visibility.Collapsed;
                        });
                        HideMessageTimer.Start();
                    });
                }).Start();
            });
            MessageTimer.Start();
        }
        private void WindowZoom(Window targetWindow)
        {
            if (targetWindow.WindowState == WindowState.Maximized)
                targetWindow.WindowState = WindowState.Normal;
            else
                targetWindow.WindowState = WindowState.Maximized;
        }
        private async void CheckVideo(TextBox urlTextBox)
        {
            string url = urlTextBox.Text;
            if (url != string.Empty)
            {
                string youtubeUrlPattern = @"^(https?\:\/\/)?(www\.youtube\.com|youtu\.?be)\/.+$";
                Regex r = new Regex(youtubeUrlPattern);
                if (r.IsMatch(url.Trim().Replace("\\", "/").Replace(" ", "")))
                {
                    int caret = urlTextBox.CaretIndex;
                    urlTextBox.Text = url.Trim().Replace("\\", "/").Replace(" ", "");
                    urlTextBox.CaretIndex = caret;
                    CheckCL.Visibility = Visibility.Visible;
                    CheckCL.Start();
                    await Task.Run(() =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                        });

                    });
                    await Task.Delay(1500);
                    CheckCL.Visibility = Visibility.Collapsed;
                    CheckCL.Stop();
                }
                else
                {
                    ShowMessage("Enter a valid youtube video Url", MessageType.Error);
                }
            }
        }

        #endregion
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
            CheckVideo(UrlTB);
        }

        private void CheckB_Click(object sender, RoutedEventArgs e)
        {
            CheckVideo(UrlTB);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
                WindowZoomB.Content = "\uE922";
            else
                WindowZoomB.Content = "\uE923";
            if (MainFrame.Background == Brushes.White)
            {
                TranslateTransform tt = new TranslateTransform();
                tt.X = -e.NewSize.Width;
                MainFrame.RenderTransform = tt;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UrlTB.Focus();
            CheckNotificationsCount();
        }


        private void UrlTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CheckVideo(UrlTB);
            }
        }

        private void BrowseB_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BrowseTB.Text = fbd.SelectedPath;
            }
        }
        public void TransitionMove(bool IsOpen)
        {
            if (TransitionTask.Status != TaskStatus.Running)
            {
                new Thread(() =>
                {
                    TransitionTask = new Task(() =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            DoubleAnimation da = new DoubleAnimation();
                            TranslateTransform tt = new TranslateTransform();
                            if (IsOpen && LastClickedButtons.Count >= 2)
                            {
                                if (LastClickedButtons[LastClickedButtons.Count - 2] != LastClickedButtons[LastClickedButtons.Count - 1])
                                {
                                    MainFrame.Background = null;
                                    CoverG.Visibility = Visibility.Visible;
                                    tt.X = -ActualWidth;
                                    MainFrame.RenderTransform = tt;
                                    da.Duration = TimeSpan.FromMilliseconds(TransitionTime);
                                    da.To = 0;
                                    da.AccelerationRatio = 0.5;
                                    tt.BeginAnimation(TranslateTransform.XProperty, da);
                                }
                            }
                            else if (IsOpen && MainFrame.Background == Brushes.White)
                            {
                                MainFrame.Background = null;
                                CoverG.Visibility = Visibility.Visible;
                                tt.X = -ActualWidth;
                                MainFrame.RenderTransform = tt;
                                da.Duration = TimeSpan.FromMilliseconds(TransitionTime);
                                da.To = 0;
                                da.AccelerationRatio = 0.5;
                                tt.BeginAnimation(TranslateTransform.XProperty, da);
                            }
                            else if (!IsOpen)
                            {
                                MainFrame.Background = Brushes.White;
                                CoverG.Visibility = Visibility.Collapsed;
                                MainFrame.RenderTransform = tt;
                                da.Duration = TimeSpan.FromMilliseconds(TransitionTime);
                                da.To = -ActualWidth;
                                da.AccelerationRatio = 0.5;
                                tt.BeginAnimation(TranslateTransform.XProperty, da);
                            }
                        });
                    });
                    TransitionTask.Start();

                }).Start();
            }
        }
        private void ViewDownloadsB_Click(object sender, RoutedEventArgs e)
        {
            var template1 = ViewSettingsB.Template;
            Border border1 = (Border)template1.FindName("border", ViewSettingsB);
            border1.BorderThickness = new Thickness(1);
            border1.BorderBrush = Brushes.White;

            var template2 = ViewDownloadsB.Template;
            Border border2 = (Border)template2.FindName("border", ViewDownloadsB);
            border2.BorderThickness = new Thickness(0, 0, 0, 2);
            border2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e90ff"));

            TransitionMove(true);
            MainFrame.Content = dp;
            LastClickedButtons.Add(ViewDownloadsB);

        }

        private void ViewSettingsB_Click(object sender, RoutedEventArgs e)
        {
            var template1 = ViewDownloadsB.Template;
            Border border1 = (Border)template1.FindName("border", ViewDownloadsB);
            border1.BorderThickness = new Thickness(1);
            border1.BorderBrush = Brushes.White;

            var template2 = ViewSettingsB.Template;
            Border border2 = (Border)template2.FindName("border", ViewSettingsB);
            border2.BorderThickness = new Thickness(0, 0, 0, 2);
            border2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e90ff"));

            TransitionMove(true);
            MainFrame.Content = sp;
            LastClickedButtons.Add(ViewSettingsB);

        }

        private void AcconutPerferencesTBl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PerferencesG.Visibility = Visibility.Visible;
        }

        private void PerferencesOpacityG_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PerferencesG.Visibility = Visibility.Collapsed;
        }

        private void LogoutB_Click(object sender, RoutedEventArgs e)
        {
            PerferencesG.Visibility = Visibility.Collapsed;
            if (MessageBox.Show("Do you want really to exit your account?", "Log out", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Properties.Settings.Default.Id = 0;
                Properties.Settings.Default.Username = "";
                Properties.Settings.Default.Email = "";
                Properties.Settings.Default.Password = "";
                Properties.Settings.Default.Save();
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void PerferencesOpacityG_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void AcconutPerferencesTBl_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void PerferencesOpacityG_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void EditProfileB_Click(object sender, RoutedEventArgs e)
        {
            PerferencesG.Visibility = Visibility.Collapsed;
            ShowMessage("This feature will be added soon.", MessageType.Information);
        }

        private void DeleteAccountB_Click(object sender, RoutedEventArgs e)
        {
            PerferencesG.Visibility = Visibility.Collapsed;
            ShowMessage("This feature will be added soon.", MessageType.Information);
        }

        private void ClearDataB_Click(object sender, RoutedEventArgs e)
        {
            PerferencesG.Visibility = Visibility.Collapsed;
            ShowMessage("This feature will be added soon.", MessageType.Information);
        }

        private void NotificationsOpacityG_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NotificationsLoaderCL.Stop();
            NotificationsG.Visibility = Visibility.Collapsed;
        }

        private async void ClearAllNotificationB_Click(object sender, RoutedEventArgs e)
        {
            if (NotificationsSP.Children.Count != 0)
            {
                TranslateTransform tt = new TranslateTransform();
                NotificationsSP.RenderTransform = tt;
                DoubleAnimation clearAllAnimation = new DoubleAnimation();
                clearAllAnimation.To = NotificationsSP.ActualWidth * 2;
                clearAllAnimation.AccelerationRatio = 0.5;
                clearAllAnimation.Duration = TimeSpan.FromMilliseconds(ClearAllAnimationDuration);
                tt.BeginAnimation(TranslateTransform.XProperty, clearAllAnimation);
                await Task.Delay(ClearAllAnimationDuration);
                NotificationsSP.Children.Clear();
                DatabaseConnectivity dc = new DatabaseConnectivity();
                dc.Connect();
                MySqlCommand command = new MySqlCommand();
                command.Parameters.AddWithValue("@id", UserIdLB.Content);
                dc.Execute(command, "DELETE FROM notifications WHERE AccountId=@id");
                dc.Unconnect();

            }
            NotificationsG.Visibility = Visibility.Collapsed;
        }

        private void Window_Activated(object sender, EventArgs e)
        {

        }

        private async void DownloadB_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.To = 40;
            da.Duration = TimeSpan.FromMilliseconds(200);
            da.AccelerationRatio = 0.5;
            ViewDownloadsB.BeginAnimation(FontSizeProperty, da);
            await Task.Delay(200);
            da.To = 25;
            da.Duration = TimeSpan.FromMilliseconds(200);
            da.AccelerationRatio = 0.5;
            ViewDownloadsB.BeginAnimation(FontSizeProperty, da);
            await Task.Delay(200);
            da.To = 40;
            da.Duration = TimeSpan.FromMilliseconds(200);
            da.AccelerationRatio = 0.5;
            ViewDownloadsB.BeginAnimation(FontSizeProperty, da);
            await Task.Delay(200);
            da.To = 25;
            da.Duration = TimeSpan.FromMilliseconds(200);
            da.AccelerationRatio = 0.5;
            ViewDownloadsB.BeginAnimation(FontSizeProperty, da);


        }

        private async void ViewNotificationsB_Click(object sender, RoutedEventArgs e)
        {
            NotificationsSP.Children.Clear();
            TranslateTransform tt = new TranslateTransform();
            tt.X = 0;
            tt.Y = 0;
            NotificationsSP.RenderTransform = tt;
            NotificationsLoaderCL.Visibility = Visibility.Visible;
            NotificationsG.Visibility = Visibility.Visible;
            NotificationsLoaderCL.Start();
            int count = 0;
            try
            {
                await Task.Run(new Action(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            DatabaseConnectivity dc = new DatabaseConnectivity();
                            dc.Connect();
                            MySqlCommand command = new MySqlCommand();
                            command.Parameters.AddWithValue("@id", UserIdLB.Content);
                            dc.ExecuteReader(command, "SELECT * FROM notifications WHERE AccountId=@id");
                            MySqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                count++;
                                NotificationItem ni = new NotificationItem();
                                ni.Padding = new Thickness(10);
                                ni.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF9F9F9"));
                                ni.Notification = reader["Message"].ToString();
                                ni.NotificationDate = Convert.ToDateTime(reader["Date"]);
                                ni.NotificationId = Convert.ToInt32(reader["NotificationId"]);
                                NotificationsSP.Children.Add(ni);
                                DatabaseConnectivity dc1 = new DatabaseConnectivity();
                                dc1.Connect();
                                dc1.ExecuteNonQuery("UPDATE notifications Set IsRead=1 WHERE AccountId=" + UserIdLB.Content);
                                dc1.Unconnect();
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                    });
                }));
            }
            catch
            {
                NotificationsLoaderCL.Visibility = Visibility.Collapsed;
                NotificationsLoaderCL.Stop();
                ClearAllNotificationB.Visibility = Visibility.Visible;
                NotificationsG.Visibility = Visibility.Collapsed;
                ShowMessage("Check your internet connection and try again", MessageType.Error);
            }
            NotificationsLoaderCL.Visibility = Visibility.Collapsed;
            NotificationsLoaderCL.Stop();
            ClearAllNotificationB.Visibility = Visibility.Visible;
            if (count == 0)
            {
                TextBlock tb = new TextBlock();
                tb.Padding = new Thickness(25);
                tb.FontSize = 15;
                tb.Text = "No Notification...";
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.FontFamily = new FontFamily(new Uri(@"pack:\\,,,\Resources\Fonts\Nunito-SemiBold.ttf"), "Nunito SemiBold");
                NotificationsSP.Children.Add(tb);
                ClearAllNotificationB.Visibility = Visibility.Collapsed;
            }
        }
    }
}
