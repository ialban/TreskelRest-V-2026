using LibControls.buttons;
using System;
using System.Collections.Generic;
using System.Data;
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
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlChgTipPedd.xaml
    /// </summary>
    public partial class ctlChgTipPedd : UserControl, IDisposable
    {
        public Window contenedor { get; set; }
        private DataTable dtTipPedd = new datasets.dsPedidos.tiposDataTable();

        public event AceptaCambioTipo aceptaCambioTipo;
        public delegate void AceptaCambioTipo(String tipo);

        private String _tipo;

        public ctlChgTipPedd()
        {
            InitializeComponent();

            DataRow dr = dtTipPedd.NewRow();
            dr["codigo"] = "L";
            dr["descripcion"] = "LOCAL";

            dtTipPedd.Rows.Add(dr);

            dr = dtTipPedd.NewRow();
            dr["codigo"] = "D";
            dr["descripcion"] = "DOMICILIO";

            dtTipPedd.Rows.Add(dr);

            dr = dtTipPedd.NewRow();
            dr["codigo"] = "C";
            dr["descripcion"] = "LLEVAR";

            dtTipPedd.Rows.Add(dr);

            creaBotones();
        }

        private void creaBotones()
        {
            foreach (DataRow dr in this.dtTipPedd.Rows)
            {
                ctlButton btn = new ctlButton();

                btn.Codigo = dr["codigo"].ToString();
                btn.Titulo = dr["descripcion"].ToString();

                btn.Imagen = General.sbDefineImagen(btn.Codigo);

                btn._ctlButtonClick += new ctlButton.ctlButton_Click(ctlButton_Click);

                PedidosContainer.Children.Add(btn);
            }
        }
        
        public void ctlButton_Click(object sender, RoutedEventArgs e, string tipo)
        {
            this._tipo = tipo;
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            if(ObjComportamiento.showMsgYesNo("Está seguro de cambiar el tipo de pedido?") == MessageBoxResult.Yes)
            {
                aceptaCambioTipo(_tipo);
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
        // ~ctlChgTipPedd() {
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
