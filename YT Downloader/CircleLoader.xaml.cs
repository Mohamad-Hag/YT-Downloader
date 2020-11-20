using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace YT_Downloader
{
    /// <summary>
    /// Interaction logic for CircleLoader.xaml
    /// </summary>
    public partial class CircleLoader : UserControl
    {
        private DoubleAnimation loadAnimation = new DoubleAnimation();
        private RotateTransform loadRotation = new RotateTransform();
        public CircleLoader()
        {
            InitializeComponent();
        }

        public void Stop()
        {
            loadRotation.BeginAnimation(RotateTransform.AngleProperty, null);
        }
        public void Start()
        {
            MainG.RenderTransformOrigin = new Point(0.5, 0.5);
            MainG.RenderTransform = loadRotation;
            loadAnimation.To = 360;
            loadAnimation.RepeatBehavior = RepeatBehavior.Forever;
            loadAnimation.Duration = TimeSpan.FromMilliseconds(500);
            loadRotation.BeginAnimation(RotateTransform.AngleProperty, loadAnimation);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
