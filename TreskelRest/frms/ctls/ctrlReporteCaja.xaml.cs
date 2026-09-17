using LibReports;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctrlReporteCaja.xaml
    /// </summary>
    public partial class ctrlReporteCaja : UserControl
    {
        public Window contenedor { get; set; }
        private winGeneral win = new winGeneral();
        private string opcion = "D";

        public ctrlReporteCaja()
        {
            InitializeComponent();

            txtAnio.ValorMaximo = 3000;
            txtAnio.ValorMinimo = 1990;

            txtAnio.setValue( DateTime.Now.Year.ToString());

            General.selectedByTagCombo(cboMes, DateTime.Now.Month.ToString());

            win.WindowStyle = WindowStyle.None;
            win.iniciaReporte += new winGeneral.IniciaReporte(EjecutaReporte);
            win.Width = 300;
            win.Height = 175;

            ctlEsperar esperar = new ctlEsperar();
            win.Contenido.Children.Add(esperar);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.Topmost = true;

            dtpDia.SelectedDate = DateTime.Now;
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            win.ShowDialog();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void HabilitaMensualOpcion(bool opcion)
        {
            if (txtAnio != null) txtAnio.IsEnabled = opcion;
            if (cboMes != null) cboMes.IsEnabled = opcion;
            if (chkDetalle != null) chkDetalle.IsEnabled = opcion;
        }

        private void HabilitaDiarioOpcion(bool opcion)
        {
            if (dtpDia != null) dtpDia.IsEnabled = opcion;
        }

        private void rbMensual_Checked(object sender, RoutedEventArgs e)
        {
            opcion = "M";
            HabilitaMensualOpcion(true);
        }

        private void rbMensual_Unchecked(object sender, RoutedEventArgs e)
        {
            HabilitaMensualOpcion(false);
        }

        private void rbDiario_Checked(object sender, RoutedEventArgs e)
        {
            opcion = "D";
            HabilitaDiarioOpcion(true);
        }

        private void rbDiario_Unchecked(object sender, RoutedEventArgs e)
        {
            HabilitaDiarioOpcion(false);
        }

        private void EjecutaReporte()
        {
            int anio = 0;
            int mes = 0;
            int dia = 0;

            if (opcion == "M")
            {
                anio = int.Parse(txtAnio.getValue().ToString());
                mes = int.Parse(((ComboBoxItem)(cboMes.SelectedItem)).Tag.ToString());
            }
            else
            {
                DateTime fecha = DateTime.Parse(dtpDia.Text);

                anio = fecha.Year;
                mes = fecha.Month;
                dia = fecha.Day;
            }

            bool detalle = chkDetalle.IsChecked == true ? true : false;

            Thread t = new Thread(() => { ReporteCaja(win, opcion, anio, mes, dia, detalle); });

            t.Start();
        }
        public void ReporteCaja(winGeneral win, string opcion, int anio, int mes, int dia, bool detalle)
        {
            RptMovimientoCaja rpt = new RptMovimientoCaja(General.connectionString, General.TEMA_REPORTE);

            rpt.NombreEmpresa = General.EMPRESA;
            rpt.FormatoNumerico = General.FORMATO_NUMERICO_REPORTE;

            Boolean resp = rpt.crear(opcion, anio, mes, dia, detalle);

            this.Dispatcher.Invoke(() =>
            {
                win.Visibility = Visibility.Hidden;
                General.PROCESANDO = false;

                if (!resp)
                {
                    ObjComportamiento.showMsgError("El reporte no pudo ser generado, verifique que no se encuentre abierto!");
                }
            });
        }

    }
}
