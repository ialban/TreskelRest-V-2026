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

namespace LibControls.radiobutton
{
    /// <summary>
    /// Lógica de interacción para ctlRadioAsButton.xaml
    /// </summary>
    public partial class ctlRadioAsButton : UserControl
    {
        private Color _colorChecked = Color.FromRgb(255, 255, 255);
        private Color _colorUnChecked = Color.FromRgb(0, 0, 0);

        public Color ColorChecked
        {
            set
            {
                _colorChecked = value;
            }
        }

        public Color ColorUnChecked
        {
            set
            {
                _colorUnChecked = value;
            }
        }

        public bool? IsChecked
        {
            get
            {
                return RbButton.IsChecked;
            }
            set
            {
                if (IsChecked == true)
                {
                    objGrid.Background = new SolidColorBrush(Colors.Black);
                    lblRadioText.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    objGrid.Background = new SolidColorBrush(Colors.Gray);
                    lblRadioText.Foreground = new SolidColorBrush(Colors.Black);
                }
                RbButton.IsChecked = value;
            }
        }

        public double Width
        {
            get
            {
                return this.Width;
            }
            set
            {
                this.Width = value;
                RbButton.Width = this.Width - 10;
            }
        }

        public double Height
        {
            get
            {
                return this.Height;
            }
            set
            {
                this.Height = value;
                RbButton.Height = this.Height - 10;
            }
        }

        public string Texto
        {
            get
            {
                return lblRadioText.Content.ToString();
            }
            set
            {
                lblRadioText.Content = value;
            }
        }

        public ctlRadioAsButton()
        {
            InitializeComponent();
        }
    }
}
