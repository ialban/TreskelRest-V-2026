using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace TreskelRest.cls
{
    public class WpfScreen
    {
        private readonly Screen screen;

        public static IEnumerable<WpfScreen> AllScreens()
        {
            foreach(Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                yield return new WpfScreen(screen);
            }
        }

        public static WpfScreen GetScreenForm(Window window)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(window);
            Screen screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
            WpfScreen wpfScreen = new WpfScreen(screen);
            return wpfScreen; 
        }

        public static WpfScreen GetScreenFrom(System.Windows.Point point)
        {
            int x = (int) Math.Round(point.X);
            int y = (int) Math.Round(point.Y);

            System.Drawing.Point drawingPoint = new System.Drawing.Point(x, y);
            Screen screen = System.Windows.Forms.Screen.FromPoint(drawingPoint);
            WpfScreen wpfScreen = new WpfScreen(screen);
            return wpfScreen;
        }

        public static WpfScreen Primary
        {
            get { return new WpfScreen(System.Windows.Forms.Screen.PrimaryScreen); }
        }

        internal WpfScreen(System.Windows.Forms.Screen screen)
        {
            this.screen = screen;
        }

        public Rect DevicecBounds
        {
            get { return this.GetRect(this.screen.Bounds); }
        }

        public Rect WorkingArea
        {
            get { return this.GetRect(this.screen.WorkingArea); }
        }

        public Rect GetRect(Rectangle value)
        {
            return new Rect
            {
                X = value.X,
                Y = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }

        public bool IsPrimary
        {
            get { return this.screen.Primary; }
        }

        public string DeviceName
        {
            get { return this.screen.DeviceName; }
        }
    }
}
