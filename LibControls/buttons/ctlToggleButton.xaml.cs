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

namespace LibControls.buttons
{
    /// <summary>
    /// Interaction logic for ctlToggleButton.xaml
    /// </summary>
    public partial class ctlToggleButton : UserControl
    {
        public delegate void ctlButton_Click(object sender, RoutedEventArgs e, string respuesta);
        public delegate void ctlButton_UnCheck(object sender, RoutedEventArgs e, string respuesta);
        public event ctlButton_Click _ctlButtonClick;
        public event ctlButton_UnCheck _ctlButtonUnCheck;
        
        public object CodigoAlterno
        {
            get
            {
                return Boton.Tag;
            }
            set
            {
                Boton.Tag = value;
            }
        }

        public object Codigo
        {
            get
            {
                return codigo.Content;
            }
            set
            {
                codigo.Content = value;
            }
        }

        public Boolean Checked
        {
            get
            {
                return Boton.IsChecked == true? true:false;
            }
            set
            {
                Boton.IsChecked = value;
            }
        }

        public string Contenido
        {
            get
            {                
                return titulo.Text;
            }
            set
            {
                titulo.Text = value;
            }
        }
        
        public String Imagen
        {
            set
            {
                imagen.Visibility = Visibility.Visible;
                imagen.Source = new BitmapImage(new Uri(value));
            }
        }

        public ctlToggleButton()
        {
            InitializeComponent();
        }

        private void Boton_Checked(object sender, RoutedEventArgs e)
        {
            this._ctlButtonClick(sender, e, titulo.Text);
        }

        private void Boton_Unchecked(object sender, RoutedEventArgs e)
        {
            _ctlButtonUnCheck(sender, e, titulo.Text);
        }
    }
}
