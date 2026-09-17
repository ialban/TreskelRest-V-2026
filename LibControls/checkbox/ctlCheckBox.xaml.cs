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

namespace LibControls.checkbox
{
    /// <summary>
    /// Lógica de interacción para ctlCheckBox.xaml
    /// </summary>
    public partial class ctlCheckBox : UserControl
    {
        public bool? isChecked
        {
            get {
                return this.chkEstado.IsChecked;
            }

            set
            {
                this.chkEstado.IsChecked = value;
            }
        }

        public bool isEnabled
        {
            get
            {
                return this.chkEstado.IsEnabled;
            }

            set
            {
                this.chkEstado.IsEnabled = value;
            }
        }

        public event CheckBoxChecked ctlChecked;
        public delegate void CheckBoxChecked(object sender, RoutedEventArgs e);

        public event CheckBoxUnChecked ctlUnChecked;
        public delegate void CheckBoxUnChecked(object sender, RoutedEventArgs e);

        public ctlCheckBox()
        {
            InitializeComponent();

        }

        private void chkEstado_Checked(object sender, RoutedEventArgs e)
        { 
            if (ctlChecked != null)
                ctlChecked(sender, e);
        }

        private void chkEstado_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ctlUnChecked != null)
                ctlUnChecked(sender, e);
        }
    }
}
