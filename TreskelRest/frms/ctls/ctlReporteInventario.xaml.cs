using LibKeys;
using LibReports;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlReportePedidos.xaml
    /// </summary>
    public partial class ctlReporteInventario : UserControl
    {
        public Window contenedor { get; set; }
        private winGeneral win = new winGeneral();

        private int opcionRpt = 0;
        private clsTextBox clsVal;

        public ctlReporteInventario()
        {
            InitializeComponent();

            clsVal = new clsTextBox(Anio, General.USA_TECLADO_PANTALLA);
            clsVal.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsVal.setMaxLength(4);

            Anio.Text=DateTime.Now.Year.ToString();

            win.WindowStyle = WindowStyle.None;
            win.iniciaReporte += new winGeneral.IniciaReporte(EjecutaReporte);
            win.Width = 300;
            win.Height = 175;

            ctlEsperar esperar = new ctlEsperar();
            win.Contenido.Children.Add(esperar);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.Topmost = true;
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            RptInventario rpt = new RptInventario(General.connectionString, General.TEMA_REPORTE);
            RptInvOpciones rptOpc = null;

            string msg = "";
            if (opcionRpt == 0)
            {
                if (desde.Text == "") msg += "- Debe ingresar una fecha de inicio \n";
                if (hasta.Text == "") msg += "- Debe ingresar una fecha de fin \n";
            }
            else
            {
                if (Anio.Text == "") msg += "- Debe ingresar un Anio \n";
            }

            if (msg != "")
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            win.ShowDialog();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void habilitaFechas(bool valor)
        {
            if (desde != null) desde.IsEnabled = valor;
            if (hasta != null) hasta.IsEnabled = valor;
        }

        private void habilitaAnioMeses(bool valor)
        {
            if (Anio != null) Anio.IsEnabled = valor;
            if (cboMes != null) cboMes.IsEnabled = valor;
        }

        private void rbMovimientos_Checked(object sender, RoutedEventArgs e)
        {
            opcionRpt = 0;
            habilitaFechas(true);
        }

        private void rbMovimientos_Unchecked(object sender, RoutedEventArgs e)
        {
            habilitaFechas(false);
        }

        private void rbKardex_Checked(object sender, RoutedEventArgs e)
        {
            opcionRpt = 1;
            habilitaAnioMeses(true);
        }

        private void rbKardex_Unchecked(object sender, RoutedEventArgs e)
        {
            habilitaAnioMeses(false);
        }

        private void EjecutaReporte()
        {
            string Desde = "";
            string Hasta = "";
            string Tipo = "K";
            int anio = int.Parse(Anio.Text);
            int mes = int.Parse(((ComboBoxItem)cboMes.SelectedItem).Tag.ToString());
            if (opcionRpt == 0)
            {
                Desde = desde.Text;
                Hasta = hasta.Text;
                Tipo = "M";
                anio = 0;
                mes = 0;
            }
            
            Thread t = new Thread(() => { ReporteInventario(win, Tipo, Desde, Hasta, anio, mes); });

            t.Start();
        }

        public void ReporteInventario(winGeneral win, string tipo, string desde, string hasta, int anio, int mes)
        {
            RptInventario rpt = new RptInventario(General.connectionString, General.TEMA_REPORTE);
            RptInvOpciones rptOpc = null;

            rptOpc = new RptInvOpciones
            {
                desde = desde,
                hasta = hasta,
                tipo = tipo,
                anio = anio,
                mes = mes
            };

            rpt.NombreEmpresa = General.EMPRESA;
            rpt.FormatoNumerico = General.FORMATO_NUMERICO_REPORTE;

            Boolean resp = rpt.crear(rptOpc);

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
