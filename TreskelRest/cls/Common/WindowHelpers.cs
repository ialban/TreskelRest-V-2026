using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace TreskelRest.cls.Common
{
    public static class WindowHelpers
    {
        public static Screen CurrentScreen(this Window window)
        {
            return Screen.FromPoint(new System.Drawing.Point((int)window.Left, (int)window.Top));
        }
    }
}
