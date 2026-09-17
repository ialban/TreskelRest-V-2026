using LibControls.buttons;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ctlMotivos.xaml
    /// </summary>
    public partial class ctlMotivos : UserControl, IDisposable
    {
        public Window contenedor { get; set; }
        public String secuencial { get; set; }
        public DataTable data { get; set; }
        public String tipo { get; set; } = "N";

        public String mensaje { get; set; }

        public event AceptaMotivo aceptaMotivo;
        public delegate void AceptaMotivo(String secuencial, String mensaje);

        public ctlMotivos()
        {
            InitializeComponent();
        }

        public void generar()
        {
            foreach (DataRow dr in data.Rows)
            {
                ToggleButton tbtn = new ToggleButton();
                tbtn.Height = 60;
                tbtn.Width = 440;

                tbtn.FontSize = 16;

                tbtn.Tag= dr["codigo"].ToString();

                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.HorizontalAlignment = HorizontalAlignment.Left;
                sp.Width = 450;

                Image imagen = new Image();
                if (tipo.Equals("M"))
                {
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/message.png"));
                } else if (tipo.Equals("D"))
                {
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/discount.png"));
                }
                else if (tipo.Equals("A"))
                {
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/cancelbill.png"));
                }
                else if (tipo.Equals("G"))
                {
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/gift.png"));
                }
                else if (tipo.Equals("I"))
                {
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/cajain.png"));
                }
                else if (tipo.Equals("E"))
                {
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/cajaout.png"));
                }
                else
                {
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/rightchevron.png"));
                }


                imagen.Width = 30;
                imagen.Height = 30;

                Label lbl = new Label();
                lbl.Content = dr["descripcion"].ToString();

                sp.Children.Add(imagen);
                sp.Children.Add(lbl);

                tbtn.Content = sp;

                //tbtn.Content = dr["descripcion"].ToString();
                
                tbtn.Checked += new RoutedEventHandler(ctlButton_Click);

                motivosContainer.Children.Add(tbtn);                
            }
        }

        public void ctlButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(UIElement obj in motivosContainer.Children)
            {
                if (!((ToggleButton)obj).Tag.ToString().Equals(((ToggleButton)sender).Tag.ToString()))
                {
                    ((ToggleButton)obj).IsChecked = false;
                }                
            }

            ((ToggleButton)sender).IsChecked = true;
            //mensaje = ((ToggleButton)sender).Content.ToString();

            mensaje = ((Label)((StackPanel)((ToggleButton)sender).Content).Children[1]).Content.ToString();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            if (this.tipo.Equals("A"))
            {
                MessageBoxResult res =ObjComportamiento.showMsgYesNo("Está seguro de anular la factura con el secuencial " + this.secuencial);
                if(res == MessageBoxResult.Yes)
                {
                    aceptaMotivo(secuencial, mensaje);
                    contenedor.Close();
                }
            } else if (this.tipo.Equals("G"))
            {
                var msgAlert = "";
                if(string.IsNullOrEmpty(this.secuencial))
                {
                    msgAlert = "Está seguro de convertir en REGALO el pedido";
                }
                else
                {
                    msgAlert = "Está seguro de convertir en REGALO el pedido con secuencial " + this.secuencial;
                }
                MessageBoxResult res = ObjComportamiento.showMsgYesNo(msgAlert);
                if (res == MessageBoxResult.Yes)
                {
                    aceptaMotivo(secuencial, mensaje);
                    contenedor.Close();                    
                }
            }else
            {
                aceptaMotivo(secuencial, mensaje);
                contenedor.Close();
            }            
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ctlMotivos() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
