using Entidades;
using LibKeys;
using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para ctlCreaFeriado.xaml
    /// </summary>
    public partial class ctlCreaFeriado : UserControl
    {
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        public ctlFeriados parent { get; set; } = null;

        public delegate void CreacionFeriado(string descripcion, string codigo, DateTime desde, DateTime hasta, bool activo);
        public event CreacionFeriado creacionFeriado;

        private clsTextBox clsDsc = null;

        public void setNid(string nid)
        {
            Feriado feriado = new Feriado(General.connectionString);

            feriado.frdoNid = int.Parse(nid);
            if (feriado.getById() == true)
            {
                txtNid.Text = feriado.frdoNid.ToString();
                txtDescripcion.Text = feriado.frdoDsc;
                desde.SelectedDate = feriado.frdoFin;
                hasta.SelectedDate = feriado.frdoFfn;
                chkIvaFeriado.IsChecked = feriado.frdoAif;
                chkActivo.IsChecked = feriado.frdoAtv;
                
                txtNid.IsEnabled = false;
            }
            else
            {
                txtNid.Text = feriado.frdoNid.ToString();
            }
        }

        public ctlCreaFeriado()
        {
            InitializeComponent();

            txtNid.IsEnabled = false;

            clsDsc = new clsTextBox(txtDescripcion, General.USA_TECLADO_PANTALLA);

            clsDsc.setMaxLength(180);
            clsDsc.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsDsc.permiteEspacios = true;

            chkActivo.IsChecked = true;
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            
            if (txtDescripcion.Text.Trim().Length == 0)
            {
                msg += "- El campo Descripción no puede estar en blanco" + ((char)13);
            }

            if (desde.SelectedDate == null)
            {
                msg += "- Debe seleccionar una fecha de inicio" + ((char)13);
            }

            if (hasta.SelectedDate == null)
            {
                msg += "- Debe seleccionar una fecha de fin" + ((char)13);
            }

            if (hasta.SelectedDate < desde.SelectedDate)
            {
                msg += "- El fecha de Fin del feriado no puede ser menor a la de Inicio" + ((char)13);
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Desar grabar los datos del Feriado?");
            if (res == MessageBoxResult.Yes)
            {
                Feriado feriado = new Feriado(General.connectionString);

                feriado.frdoNid = txtNid.Text=="" ? 0 : int.Parse( txtNid.Text);
                feriado.frdoDsc = txtDescripcion.Text;
                feriado.frdoFin = (DateTime) desde.SelectedDate;
                feriado.frdoFfn = (DateTime) hasta.SelectedDate;
                feriado.frdoAif = chkIvaFeriado.IsChecked == true ? true : false;
                feriado.frdoAtv = chkActivo.IsChecked == true ? true : false;
                
                if (feriado.grabar(this.accion))
                {
                    ObjComportamiento.showMsgInfo("Datos del Feriado grabados correctamente");
                    if (this.accion.Equals("M"))
                    {
                        this.parent.Actualiza_Trigger();
                    }

                    
                    contenedor.Close();
                }
                else
                {
                    ObjComportamiento.showMsgError("Los datos del Feriado no pudieron ser grabados!, " + ((char)13) + "comuniquese con el administrador!");
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }
    }
}
