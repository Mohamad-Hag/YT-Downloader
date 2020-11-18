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
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;
using YT_Downloader.Classes;
using System.Windows.Threading;

namespace YT_Downloader
{
    /// <summary>
    /// Interaction logic for EntryWindow.xaml
    /// </summary>
    public partial class EntryWindow : Window
    {
        private int TransitionDuration = 150;
        public static EntryWindow Instance;

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

        private bool IsGoSignUpRunning = false;
        private bool IsGoSignInRunning = false;
        private bool IsGoBackRunning = false;

        private DispatcherTimer MessageTimer = new DispatcherTimer();
        private DispatcherTimer HideMessageTimer = new DispatcherTimer();

        Task t1, t2;

        public EntryWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        #region Custom Methods
        private async void Enter()
        {
            MessageBorder.Opacity = 0;
            MessageBorder.Visibility = Visibility.Collapsed;
            WindowTitleTBl.Text = "Loading...";
            BackB.Visibility = Visibility.Collapsed;
            LoadG.Visibility = Visibility.Visible;
            LoadG.Opacity = 0.8;
            LoadCL.Start();

            Account account = new Account(SignInEmailTB.Text, SignInPasswordTB.Password);
            await account.Enter();
            await Task.Delay(500);

            BackB.Visibility = Visibility.Visible;
            WindowTitleTBl.Text = "Sign In";
            LoadG.Visibility = Visibility.Collapsed;
            LoadG.Opacity = 1;
            LoadCL.Stop();

            if (account.GetMessage().Equals(""))
            {
                MainWindow mw = new MainWindow();
                mw.Show();
                Close();

                Properties.Settings.Default.Email = account.GetEmail();
                Properties.Settings.Default.Password = account.GetPassword();
                Properties.Settings.Default.Username = account.GetUsername();
                Properties.Settings.Default.Id = account.GetId();
                Properties.Settings.Default.Save();
                mw.UsernameRN.Text = Properties.Settings.Default.Username;
                mw.UserIdLB.Content = Properties.Settings.Default.Id;
            }
            ShowMessage(account.GetMessage(), MessageType.Error);

        }
        private async void Register()
        {
            MessageBorder.Opacity = 0;
            MessageBorder.Visibility = Visibility.Collapsed;
            WindowTitleTBl.Text = "Loading...";
            BackB.Visibility = Visibility.Collapsed;
            LoadG.Visibility = Visibility.Visible;
            LoadG.Opacity = 0.8;
            LoadCL.Start();

            Account account = new Account(SignUpUsernameTB.Text, SignUpEmailTB.Text, SignUpPasswordTB.Password);
            await account.Create();
            await Task.Delay(500);

            BackB.Visibility = Visibility.Visible;
            WindowTitleTBl.Text = "Sign Up";
            LoadG.Visibility = Visibility.Collapsed;
            LoadG.Opacity = 1;
            LoadCL.Stop();
            if (!account.ShowMessage())
            {
                MainWindow mw = new MainWindow();
                mw.Show();
                Close();

                Properties.Settings.Default.Email = account.GetEmail();
                Properties.Settings.Default.Password = account.GetPassword();
                Properties.Settings.Default.Username = account.GetUsername();
                Properties.Settings.Default.Id = account.GetId();
                Properties.Settings.Default.Save();
                mw.UsernameRN.Text = Properties.Settings.Default.Username;
                mw.UserIdLB.Content = Properties.Settings.Default.Id;
                return;
            }
            ShowMessage(account.GetMessage(), MessageType.Error);

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
        public static void ShowHidePassword(PasswordBox targetPasswordBox)
        {
            var template = targetPasswordBox.Template;
            TextBox lb = (TextBox)template.FindName("PasswordShownLB", targetPasswordBox);
            TextBlock tb = (TextBlock)template.FindName("ViewPasswordTBl", targetPasswordBox);
            if (tb.Foreground != Brushes.White)
            {
                lb.Visibility = Visibility.Visible;
                tb.Focus();
            }
            else
            {
                lb.Visibility = Visibility.Collapsed;
                targetPasswordBox.Focus();
            }
        }
        #endregion
        private void TitleBarG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadG.Visibility = Visibility.Visible;
            LoadCL.Start();
            await Task.Delay(3500);
            LoadCL.Stop();
            LoadG.Visibility = Visibility.Collapsed;
            IntroductionG.Visibility = Visibility.Visible;
            WindowTitleTBl.Text = "Welcome";

            DoubleAnimation introAnimation = new DoubleAnimation();
            introAnimation.To = 1;
            introAnimation.Duration = TimeSpan.FromMilliseconds(350);
            introAnimation.AccelerationRatio = 0.5;
            IntroductionG.BeginAnimation(OpacityProperty, introAnimation);
        }

        private async void SignInIntroB_Click(object sender, RoutedEventArgs e)
        {
            if (!IsGoSignInRunning)
            {
                IsGoSignInRunning = true;
                IntroductionG.Visibility = Visibility.Collapsed;
                SignUpG.Visibility = Visibility.Collapsed;
                SignInG.Visibility = Visibility.Visible;
                BackB.Visibility = Visibility.Visible;
                SignInEmailTB.Focus();
                WindowTitleTBl.Text = "Sign In";
                Task t1 = new Task(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ThicknessAnimation transitionAnimation = new ThicknessAnimation();
                        transitionAnimation.To = new Thickness(0, 0, 0, 0);
                        transitionAnimation.Duration = TimeSpan.FromMilliseconds(TransitionDuration);
                        transitionAnimation.AccelerationRatio = 0.5;
                        SignInG.BeginAnimation(MarginProperty, transitionAnimation);
                    });
                });
                t1.Start();
                await Task.Delay(TransitionDuration);
                IsGoSignInRunning = false;
            }
        }

        private async void SignUpIntroB_Click(object sender, RoutedEventArgs e)
        {
            if (!IsGoSignUpRunning)
            {
                IsGoSignUpRunning = true;
                IntroductionG.Visibility = Visibility.Collapsed;
                SignInG.Visibility = Visibility.Collapsed;
                SignUpG.Visibility = Visibility.Visible;
                BackB.Visibility = Visibility.Visible;
                WindowTitleTBl.Text = "Sign Up";
                SignUpEmailTB.Focus();
                Task t1 = new Task(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ThicknessAnimation transitionAnimation = new ThicknessAnimation();
                        transitionAnimation.To = new Thickness(0, 0, 0, 0);
                        transitionAnimation.Duration = TimeSpan.FromMilliseconds(TransitionDuration);
                        transitionAnimation.AccelerationRatio = 0.5;
                        SignUpG.BeginAnimation(MarginProperty, transitionAnimation);
                    });
                });
                t1.Start();
                await Task.Delay(TransitionDuration);
                IsGoSignUpRunning = false;
            }
        }

        private async void BackB_Click(object sender, RoutedEventArgs e)
        {
            if (!IsGoBackRunning)
            {
                IsGoBackRunning = true;
                BackB.Visibility = Visibility.Collapsed;
                WindowTitleTBl.Text = "Welcome";
                IntroductionG.Visibility = Visibility.Visible;
                Task t = new Task(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (SignInG.Margin == new Thickness(400, 0, 0, 0))
                        {
                            SignUpEmailTB.Text = SignUpUsernameTB.Text = SignUpPasswordTB.Password = string.Empty;
                            Task t1 = new Task(() =>
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    ThicknessAnimation transitionAnimation = new ThicknessAnimation();
                                    transitionAnimation.To = new Thickness(400, 0, 0, 0);
                                    transitionAnimation.Duration = TimeSpan.FromMilliseconds(TransitionDuration);
                                    transitionAnimation.AccelerationRatio = 0.5;
                                    SignUpG.BeginAnimation(MarginProperty, transitionAnimation);
                                });
                            });
                            t1.Start();
                        }
                        else
                        {
                            SignInEmailTB.Text = SignInPasswordTB.Password = string.Empty;
                            Task t2 = new Task(() =>
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    ThicknessAnimation transitionAnimation = new ThicknessAnimation();
                                    transitionAnimation.To = new Thickness(400, 0, 0, 0);
                                    transitionAnimation.Duration = TimeSpan.FromMilliseconds(TransitionDuration);
                                    transitionAnimation.AccelerationRatio = 0.5;
                                    SignInG.BeginAnimation(MarginProperty, transitionAnimation);
                                });
                            });
                            t2.Start();
                        }

                    });
                });
                t.Start();
                await Task.Delay(TransitionDuration);
                IsGoBackRunning = false;
            }
        }

        private void SignInB_Click(object sender, RoutedEventArgs e)
        {
            Enter();
        }

        private void SignUpB_Click(object sender, RoutedEventArgs e)
        {
            Register();
        }

        private void LoadG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void SignInPasswordTB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var template = SignInPasswordTB.Template;
            TextBlock tb = (TextBlock)template.FindName("ViewPasswordTBl", SignInPasswordTB);
            TextBox lb = (TextBox)template.FindName("PasswordShownLB", SignInPasswordTB);
            lb.Text = SignInPasswordTB.Password;
            if (SignInPasswordTB.Password == string.Empty)
            {
                tb.Visibility = Visibility.Collapsed;
            }
            else
            {
                tb.Visibility = Visibility.Visible;
            }
        }

        private void SignUpEmailTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Register();
        }

        private void SignInEmailTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Enter();
        }

        private void SignUpPasswordTB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var template = SignUpPasswordTB.Template;
            TextBlock tb = (TextBlock)template.FindName("ViewPasswordTBl", SignUpPasswordTB);
            if (SignUpPasswordTB.Password == string.Empty)
            {
                tb.Visibility = Visibility.Collapsed;
            }
            else
            {
                tb.Visibility = Visibility.Visible;
            }
        }
    }
}
