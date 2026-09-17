using Entidades;
using Entidades.modelos;
using AccessDatos;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;
using LibHttpClient;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlViewFacturas.xaml
    /// </summary>
    public partial class ctlViewFacturas : UserControl, IDisposable
    {
        private bool disposedValue;
        private Ventas ventas = new Ventas(General.connectionString);
        private clsTextBox clsNid;
        private clsTextBox clsPed;

        public ctlViewFacturas()
        {
            InitializeComponent();

            clsNid = new clsTextBox(txtCliente, General.USA_TECLADO_PANTALLA);
            clsPed = new clsTextBox(txtPedido, General.USA_TECLADO_PANTALLA);

            clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsNid.permiteEspacios = false;
            clsNid.MaxLength = 20;

            clsPed.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsPed.permiteEspacios = false;
            clsPed.MaxLength = 10;

            var fecha = DateTime.Now;

            desde.SelectedDate = fecha;
            hasta.SelectedDate = fecha;
        }

        private void btnSpin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            List<mDocSriAutorizados> resp = ventas.getFacturaSriPorCliente(txtCliente.Text, txtPedido.Text, (DateTime)desde.SelectedDate, (DateTime)hasta.SelectedDate);

            Resultado.ItemsSource = null;

            if (resp.Count > 0)
            {
                Resultado.ItemsSource = resp;
            }
            else
            {
                ObjComportamiento.showMsgInfo("No existe resultado alguno para el criterio de búsqueda");
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        private async void btnMostrar_Click(object sender, RoutedEventArgs e)
        {
            await Descargar(sender);
        }

        private async Task Descargar(object sender)
        {
            WebBrowser wb = new WebBrowser();

            mDocSriAutorizados row = null;
            if (sender.GetType() == typeof(mDocSriAutorizados))
            {
                row = (mDocSriAutorizados)sender;
            }
            else
            {
                row = (mDocSriAutorizados)((FrameworkElement)sender).DataContext;
            }

            HttpClienteAPI client = new HttpClienteAPI();
            client.TrsklUrl = General.URL_WS;
            client.PathDownload = General.RUTA_XML_GENERADOS + "\\Descargados";

            string anio = row.autosri.Substring(4, 4);
            string mes = row.autosri.Substring(2, 2);

            switch (row.tipo.Replace("ó", "o"))
            {
                case "Factura":
                    client.PathDownload += "\\Facturacion\\" + anio + "\\" + mes;
                    break;
                case "Retencion":
                    client.PathDownload += "\\Retencion\\" + anio + "\\" + mes;
                    break;
                case "NotaCredito":
                    client.PathDownload += "\\NotasCredito\\" + anio + "\\" + mes;
                    break;
                case "NotaDebito":
                    client.PathDownload += "\\NotasDebito\\" + anio + "\\" + mes;
                    break;
            }

            string empresaid = General.EMPRESA_GUID;

            string respuesta = await client.downloadFactura(General.RUC_EMPRESA, empresaid, row.autosri, row.secPeddRet, row.secuencial, row.tipo.Replace("ó","o"));

            if (respuesta != "OK")
            {
                ObjComportamiento.showMsgError(respuesta);
            }
            else
            {
                try
                {
                    string filename = "";
                    switch (row.tipo)
                    {
                        case "Retención":
                            filename = "RET-" + row.autosri + ".pdf";
                            break;
                        case "NotaCredito":
                            filename = "NCR-" + row.autosri + ".pdf";
                            break;
                        default:
                            filename = row.autosri + ".pdf";
                            break;
                    }
                    
                    string path = client.PathDownload + "\\" + filename;

                    ObjComportamiento.showMsgInfo("El archivo fue descargado en " + General.RUTA_XML_GENERADOS +
                        "\\Descargados, con el nombre " + System.Environment.NewLine +
                        filename);

                    Process.Start(path);
                }
                catch (Exception e)
                {
                    ObjComportamiento.showMsgError("Ha ocurrido un error durante al intentar abrir el archivo descargado, verifique que este exista y que no existe errores en el archivo");
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminar el estado administrado (objetos administrados)
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~ctlViewFacturas()
        // {
        //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void btnCliFnl_Click(object sender, RoutedEventArgs e)
        {
            txtCliente.Text = "9999999999999";
        }
    }
}
