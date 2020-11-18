using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace YT_Downloader
{
    static class TheControl
    {
        public static bool IsMax = false;
        public static bool IsFull = false;
        public static Point OldLocation;
        public static Point DefaultLocation;
        public static Size OldSize;
        public static Size DefaultSize;

        public static void SetInitials(Window window)
        {
            OldSize = new Size(window.Width, window.Height);
            OldLocation = new Point(window.Top, window.Left);
            DefaultSize = new Size(window.Width, window.Height);
            DefaultLocation = new Point(window.Top, window.Left);
        }

        public static void DoMaximize(Window window)
        {
            if(!IsMax)
            {
                OldSize = new Size(window.Width, window.Height);
                OldLocation = new Point(window.Width, window.Height);
                Maximize(window);
                IsMax = true;
                IsFull = false;
            }
            else
            {
                if(OldSize.Width >= SystemParameters.WorkArea.Width || OldSize.Height >= SystemParameters.WorkArea.Height)
                {
                    window.Top = DefaultLocation.Y;
                    window.Left = DefaultLocation.X;
                    window.Width = DefaultSize.Width;
                    window.Height = DefaultSize.Height;
                }
                else
                {
                    window.Top = OldLocation.Y;
                    window.Left = OldLocation.X;
                    window.Width = OldSize.Width;
                    window.Height = OldSize.Height;
                }
                IsMax = IsFull = false;
            }
        }

        public static void DoFullScreen(Window window)
        {
            if (!IsMax)
            {
                OldSize = new Size(window.Width, window.Height);
                OldLocation = new Point(window.Width, window.Height);
                FullScreen(window);
                IsMax = false;
                IsFull = true;
            }
            else
            {
                window.Top = OldLocation.Y;
                window.Left = OldLocation.X;
                window.Width = OldSize.Width;
                window.Height = OldSize.Height;
                IsMax = IsFull = false;
            }
        }
        private static void FullScreen(Window window)
        {
            if (window.WindowState == WindowState.Normal)
                window.WindowState = WindowState.Maximized;
            else
                window.WindowState = WindowState.Normal;
        }

        private static void Maximize(Window window)
        {
            double x = SystemParameters.WorkArea.Width;
            double y = SystemParameters.WorkArea.Height;
            window.WindowState = WindowState.Normal;
            window.Top = 0;
            window.Left = 0;
            window.Width = x;
            window.Width = y;
        }

        public static void Minimize(Window window)
        {
            window.WindowState = WindowState.Minimized;
        }

        public static void Shutdown()
        {

        }
    }
}
