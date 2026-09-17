using LibReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Lógica de interacción para ctlReporteSRI.xaml
    /// </summary>
    public partial class ctlReporteSRI : UserControl
    {
        private winGeneral win = new winGeneral();
        public Window contenedor { get; set; }

        public ctlReporteSRI()
        {
            InitializeComponent();

            DateTime fch = DateTime.Now;
            int mes = fch.Month;

            txtAnio.setValue(fch.Year.ToString());
            txtAnio.ValorMaximo = 5000;

            fecha.SelectedDate =fch;
            General.selectedByTagCombo(cboMes, mes.ToString());

            win.WindowStyle = WindowStyle.None;
            win.iniciaReporte += new winGeneral.IniciaReporte(EjecutaReporte);
            win.Width = 300;
            win.Height = 175;

            ctlEsperar esperar = new ctlEsperar();
            win.Contenido.Children.Add(esperar);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.Topmost = true;

            Mensual.IsEnabled = false;
            Diario.IsEnabled = true;
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            win.ShowDialog();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void EjecutaReporte()
        {
            int anio = 0; //int.Parse(txtAnio.getValue().ToString());
            int mes = 0; //int.Parse(((ComboBoxItem)(cboMes.SelectedItem)).Tag.ToString());
            int dia = 0;

            if (chkMensual.IsChecked == true)
            {
                anio = int.Parse(txtAnio.getValue());
                mes = int.Parse(((ComboBoxItem)(cboMes.SelectedItem)).Tag.ToString());
            }
            else
            {
                DateTime fch = (DateTime)fecha.SelectedDate;

                anio = fch.Year;
                mes = fch.Month;
                dia = fch.Day;
            }

            Thread t = new Thread(() => { ReporteSRI(win, anio, mes, dia); });

            t.Start();
        }

        public void ReporteSRI(winGeneral win, int anio, int mes, int dia)
        {
            RptSRI rpt = new RptSRI(General.connectionString, General.TEMA_REPORTE);
            rpt.NombreEmpresa = General.EMPRESA;
            rpt.FormatoNumerico = General.FORMATO_NUMERICO_REPORTE;

            Boolean resp = rpt.crear(anio, mes, dia);

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

        private void chkMensual_Checked(object sender, RoutedEventArgs e)
        {
            Mensual.IsEnabled = true;
            Diario.IsEnabled = false;
        }

        private void chkMensual_Unchecked(object sender, RoutedEventArgs e)
        {
            Mensual.IsEnabled = false;
            Diario.IsEnabled = true;
        }
    }
}
