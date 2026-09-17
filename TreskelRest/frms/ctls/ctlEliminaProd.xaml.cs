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
    public partial class ctlEliminaProd : UserControl, IDisposable
    {
        public Window contenedor { get; set; }

        public ProdPedd row { get; set; }
        public int cantidad { get; set; }

        public String mensaje { get; set; }
        private string pedd = "";
        private string prod = "";
        private List<ProdPedd> prods = new List<ProdPedd>();
        private bool memoria = false;
        private int indexToDelete = -1;
        private bool selecc = false;

        public event AceptaEliminacionProducto aceptaEliminacion;
        public delegate void AceptaEliminacionProducto(string peddscl, int cantidad, ProdPedd producto, int index, bool memoria);

        public ctlEliminaProd(string pedd, ref List<ProdPedd> prods, string prod, bool memoria)
        {
            InitializeComponent();

            this.pedd = pedd;
            this.prod = prod;
            this.prods = prods;
            this.memoria = memoria;
        }

        public void generar()
        {
            for(int i=0; i<prods.Count; i++)
            {
                if(prods[i].codigo == prod)
                {
                    ToggleButton tbtn = new ToggleButton();
                    tbtn.Height = 60;
                    tbtn.Width = 440;

                    tbtn.FontSize = 16;

                    tbtn.Tag = i.ToString();

                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;
                    sp.HorizontalAlignment = HorizontalAlignment.Left;
                    sp.Width = 450;

                    Label cnt = new Label();
                    cnt.Content = prods[i].cantidad.ToString();
                    cnt.FontSize = 14;
                    cnt.FontWeight = FontWeights.Bold;

                    Label lbl = new Label();
                    if (prods[i].observacion == null || prods[i].observacion == "")
                    {
                        lbl.Content = prods[i].producto;
                    }
                    else
                    {
                        lbl.Content = prods[i].producto + " (" + prods[i].observacion + ")"; 
                    }


                    sp.Children.Add(cnt);
                    sp.Children.Add(lbl);

                    tbtn.Content = sp;

                    tbtn.Checked += new RoutedEventHandler(ctlButton_Click);

                    motivosContainer.Children.Add(tbtn);
                }
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
            selecc = true;
            indexToDelete = int.Parse(((ToggleButton)sender).Tag.ToString());
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            if (selecc)
            {
                aceptaEliminacion(pedd, this.cantidad, this.row, indexToDelete, memoria);
            }
            
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
