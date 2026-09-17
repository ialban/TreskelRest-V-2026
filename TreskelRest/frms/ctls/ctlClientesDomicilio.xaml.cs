using Entidades;
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
    public partial class ctlClientesDomicilio : UserControl, IDisposable
    {
        public Window contenedor { get; set; }
        public ctlDelivery parent { get; set; } = null;

        public event AceptaCliente aceptaCliente;
        public delegate void AceptaCliente(ClienteDomicilio cliente);

        private List<ClienteDomicilio> clientes = new List<ClienteDomicilio>();
        private int selectedClient = -1;

        public ctlClientesDomicilio(List<ClienteDomicilio> clientes)
        {
            InitializeComponent();
            this.clientes = clientes;
            generar();
        }

        public ctlClientesDomicilio()
        {
            InitializeComponent();
        }

        public void generar()
        {
            for (int i = 0; i < clientes.Count; i++)
            {
                StackPanel pnl = new StackPanel();
                pnl.Orientation = Orientation.Horizontal;
                pnl.Width = 630;
                pnl.Tag = i;

                Button btn = new Button();
                btn.Width = 60;
                btn.Height = 60;
                btn.Tag = clientes[i];
                btn.Content = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/images/delete.png")),
                    Width = 40,
                    Height = 40,
                    VerticalAlignment = VerticalAlignment.Center
                };

                btn.Click += new RoutedEventHandler(btnButton_Click);

                ToggleButton tbtn = new ToggleButton();
                tbtn.Height = 60;
                tbtn.Width = 560;

                tbtn.FontSize = 16;

                tbtn.Tag= i.ToString();

                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.HorizontalAlignment = HorizontalAlignment.Left;
                sp.Width = 550;

                Image imagen = new Image();
                if (clientes[i].clieNid.Length == 10)
                {
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/client.png"));
                } else if (clientes[i].clieNid.Length == 13)
                {
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/discount.png"));
                }
                else
                {
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/passport.png"));
                }


                imagen.Width = 30;
                imagen.Height = 30;

                Label lbl = new Label();
                lbl.Width = 540;
                TextBlock txt = new TextBlock();

                txt.TextWrapping = TextWrapping.Wrap;
                txt.Text = clientes[i].clieNid+" -> " + clientes[i].clieNbr + " -> " + clientes[i].clieDir + " -> " + clientes[i].clieTlf;

                lbl.Content = txt;

                sp.Children.Add(imagen);
                sp.Children.Add(lbl);

                tbtn.Content = sp;

                tbtn.Checked += new RoutedEventHandler(ctlButton_Click);

                pnl.Children.Add(tbtn);
                pnl.Children.Add(btn);

                //clientesContainer.Children.Add(tbtn);
                clientesContainer.Children.Add(pnl);
            }
        }

        public void ctlButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(UIElement obj in clientesContainer.Children)
            {
                var cont = ((StackPanel)obj).Children[0];
                
                if (!((ToggleButton)cont).Tag.ToString().Equals(((ToggleButton)sender).Tag.ToString()))
                {
                    ((ToggleButton)cont).IsChecked = false;
                }                
            }

            ((ToggleButton)sender).IsChecked = true;

            selectedClient = int.Parse(((ToggleButton)sender).Tag.ToString());
        }

        private void btnButton_Click(object sender, RoutedEventArgs e)
        {
            // Desactivar dato del cliente.
            var clienteDomicilio = (ClienteDomicilio)((Button)sender).Tag;

            if (ObjComportamiento.showMsgYesNo("¿Desea eliminar los datos del cliente?") == MessageBoxResult.Yes)
            {
                bool result = clienteDomicilio.desactivar();

                if (result)
                {
                    var pnl = (StackPanel)((Button)sender).Parent;
                    var idx = int.Parse(pnl.Tag.ToString());

                    var cnt = (WrapPanel)pnl.Parent;
                    cnt.Children.RemoveAt(idx);
                }
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = ObjComportamiento.showMsgYesNo("Está seguro de seleccionar dicho cliente?");

            if (res == MessageBoxResult.Yes)
            {
                aceptaCliente(clientes[selectedClient]);
                contenedor.Close();
            }
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            aceptaCliente(new ClienteDomicilio("")
            {
                clieNid = clientes[0].clieNid,
                clieNbr = clientes[0].clieNbr,
                clieDir = "",
                clieTlf = "",
                clieRef = ""
            });
            contenedor.Close();
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
