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
    /// Lógica de interacción para ctlCheckBox.xaml
    /// </summary>
    public partial class ctlRadioButton : UserControl
    {
        public bool? isChecked
        {
            get {
                //this.rbOpcion.IsChecked;
                return (bool)GetValue(IsCheckedProperty);
                //return this.rbOpcion.IsChecked;
            }

            set
            {
                SetValue(IsCheckedProperty, value);
                this.rbOpcion.IsChecked = value;
                //this.rbOpcion.IsChecked = value;
            }
        }

        public bool isEnabled
        {
            get
            {
                return this.rbOpcion.IsEnabled;
            }

            set
            {
                this.rbOpcion.IsEnabled = value;
            }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(
                "IsChecked",
                typeof(bool?),
                typeof(ctlRadioButton),
                new PropertyMetadata(null));

        

        public event RadioButtonChecked ctlRadioChecked;
        public delegate void RadioButtonChecked(object sender, RoutedEventArgs e);

        public event RadioButtonUnChecked ctlRadioUnChecked;
        public delegate void RadioButtonUnChecked(object sender, RoutedEventArgs e);

        public ctlRadioButton()
        {
            InitializeComponent();

        }

        private void rbOpcion_Checked(object sender, RoutedEventArgs e)
        {
            ctlRadioChecked(sender, e);
        }

        private void rbOpcion_Unchecked(object sender, RoutedEventArgs e)
        {
            ctlRadioUnChecked(sender, e);
        }
    }
}
