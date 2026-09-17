using Entidades;
using LibKeys;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlBackup.xaml
    /// </summary>
    public partial class ctlBackup : UserControl, IDisposable
    {
        public Window contenedor { get; set; }
        private clsTextBox clsUrl;

        public ctlBackup()
        {
            InitializeComponent();

            Backup bkp = new Backup(General.connectionString);

            bkp.load();

            if (bkp.horaBackup != "") chkPrimer.IsChecked = true;
            if (bkp.horaBackup2 != "") chkSegundo.IsChecked = true;
            if (bkp.horaBackup3 != "") chkTercero.IsChecked = true;

            hInicio1.setValue(bkp.horaBackup  != "" ? bkp.horaBackup  : "00:00");
            hInicio2.setValue(bkp.horaBackup2 != "" ? bkp.horaBackup2 : "00:00");
            hInicio3.setValue(bkp.horaBackup3 != "" ? bkp.horaBackup3 : "00:00");

            cboCnt.SelectedIndex = (int.Parse(bkp.cntFiles)) - 1;
            chkMail.IsChecked = bkp.envMail;

            chkUpload.IsChecked = bkp.UploadFile;
            txtUrlServer.Text = bkp.server;
            txtUser.Text = bkp.usuario;
            txtClave.Password = bkp.clave;


            clsUrl = new clsTextBox(txtUrlServer, General.USA_TECLADO_PANTALLA);
            clsUrl.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.URL;
            clsUrl.setMaxLength(150);
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            string msgError = "";
            Backup bkp = new Backup(General.connectionString);

            if (chkPrimer.IsChecked ==false && chkSegundo.IsChecked == false && chkTercero.IsChecked == false)
            {
                msgError += "- Debe habilitar por lo menos una hora para realizar el backup";
            }

            bkp.horaBackup = hInicio1.getValue();
            bkp.horaBackup2 = chkSegundo.IsChecked == true ? hInicio2.getValue() : "";
            bkp.horaBackup3 = chkTercero.IsChecked == true ? hInicio3.getValue() : "";
            bkp.cntFiles = ((ComboBoxItem) cboCnt.SelectedValue).Content.ToString();
            bkp.envMail = chkMail.IsChecked == true? true : false;
            bkp.UploadFile = chkUpload.IsChecked == true ? true : false;

            bkp.server = txtUrlServer.Text;
            bkp.usuario = txtUser.Text;
            bkp.clave = txtClave.Password;

            if (bkp.UploadFile)
            {
                if (bkp.server.Length == 0) msgError = "- Debe especificar una dirección URL para el servidor" + System.Environment.NewLine;
                if (bkp.usuario.Length == 0) msgError = "- Debe especificar una dirección URL para el servidor" + System.Environment.NewLine;
                if (bkp.clave.Length == 0) msgError = "- Debe especificar una dirección URL para el servidor" + System.Environment.NewLine;
            }

            if (msgError != "")
            {
                msgError = "Se ha producido uno de los siguientes errores:" + System.Environment.NewLine + msgError;
                ObjComportamiento.showMsgError(msgError);
                return;
            }

            Respuesta resp = bkp.grabar();
            if (resp.resultado)
            {
                bkp.registraLog("Grabando configuración de Backup", General.USUARIO, General.CAJA);
            }

            ObjComportamiento.showMuestraMensaje(resp);
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void chkPrimer_Checked(object sender, RoutedEventArgs e)
        {
            if(hInicio1!=null)
                hInicio1.IsEnabled = true;
        }

        private void chkPrimer_Unchecked(object sender, RoutedEventArgs e)
        {
            hInicio1.IsEnabled = false;
        }

        private void chkSegundo_Checked(object sender, RoutedEventArgs e)
        {
            hInicio2.IsEnabled = true;
        }

        private void chkSegundo_Unchecked(object sender, RoutedEventArgs e)
        {
            hInicio2.IsEnabled = false;
        }

        private void chkTercero_Checked(object sender, RoutedEventArgs e)
        {
            hInicio3.IsEnabled = true;
        }

        private void chkTercero_Unchecked(object sender, RoutedEventArgs e)
        {
            hInicio3.IsEnabled = false;
        }

        private void ActivarUpload(Boolean value)
        {
            txtUrlServer.IsEnabled = value;
            txtUser.IsEnabled = value;
            txtClave.IsEnabled = value;
        }

        private void chkUpload_Checked(object sender, RoutedEventArgs e)
        {
            ActivarUpload(true);
        }

        private void chkUpload_Unchecked(object sender, RoutedEventArgs e)
        {
            ActivarUpload(false);
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            Backup bkp = new Backup(General.connectionString);

            int rest = bkp.StartBackup(General.CAJA);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (rest > 0)
            {
                bkp.SetStartOnDemand();

                bool procesando = true;

                BkpDemand demanda = bkp.GetBkpDemand();
                pgProgreso.Maximum = demanda.bkpCAc;

                while (procesando)
                {
                    txtProgreso.Text = demanda.bkpCmt;
                    pgProgreso.Value = demanda.bkpCPg;

                    if (demanda.bkpAtv) procesando = true;

                    demanda = bkp.GetBkpDemand();
                    TimeSpan ts = stopwatch.Elapsed;
                    if (pgProgreso.Value >= pgProgreso.Maximum || ts.Minutes > 5)
                    {
                        procesando = false;
                        stopwatch.Stop();
                    }
                }
            }
        }


        #region IDisposable Support
        private bool disposedValue = false; // Para detectar llamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: elimine el estado administrado (objetos administrados).
                }

                // TODO: libere los recursos no administrados (objetos no administrados) y reemplace el siguiente finalizador.
                // TODO: configure los campos grandes en nulos.

                disposedValue = true;
            }
        }

        // TODO: reemplace un finalizador solo si el anterior Dispose(bool disposing) tiene código para liberar los recursos no administrados.
        // ~ctlBackup() {
        //   // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
        //   Dispose(false);
        // }

        // Este código se agrega para implementar correctamente el patrón descartable.
        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
            Dispose(true);
            // TODO: quite la marca de comentario de la siguiente línea si el finalizador se ha reemplazado antes.
            // GC.SuppressFinalize(this);
        }

        #endregion

    }
}
