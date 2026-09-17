using LibControls.buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlChgMesa.xaml
    /// </summary>
    public partial class ctlChgMesa : UserControl
    {
        public Window contenedor { get; set; }
        public String mesa { get; set; }

        public event CambiaMesa cambiaMesa;
        public delegate void CambiaMesa(object sender, RoutedEventArgs e, String mesa);        

        public ctlChgMesa()
        {
            InitializeComponent();
            for (int i = 1; i <= General.CANTIDAD_MESAS; i++)
            {
                ctlToggleButton btn = new ctlToggleButton();
                btn.Height = 60;
                btn.Width = 90;
                btn.Imagen = "pack://application:,,,/images/table.png";
                btn.FontSize = 16;

                btn.Codigo = "M";
                btn.Contenido = i.ToString().PadLeft(3, '0');
                btn.CodigoAlterno = btn.Contenido;

                btn._ctlButtonClick += new ctlToggleButton.ctlButton_Click(ctlButton_Click);

                mesasContainer.Children.Add(btn);                
            }
        }

        public void ctlButton_Click(object sender, RoutedEventArgs e, string respuesta)
        {
            foreach (UIElement obj in mesasContainer.Children)
            {
                if (!((ctlToggleButton)obj).CodigoAlterno.ToString().Equals(((ToggleButton)sender).Tag.ToString()))
                {
                    ((ctlToggleButton)obj).Checked = false;
                }
            }

            ((ToggleButton)sender).IsChecked = true;
            
            mesa = respuesta;
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            cambiaMesa(sender, e, mesa);
            contenedor.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }
    }
}
