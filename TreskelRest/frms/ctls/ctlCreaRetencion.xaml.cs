using Entidades;
using Entidades.modelos;
using LibKeys;
using System;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlCreaRetencion.xaml
    /// </summary>
    public partial class ctlCreaRetencion : UserControl
    {
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        public ctlClientes parent { get; set; } = null;


        private clsTextBox clsDsc = null;
        private clsTextBox clsVal = null;
        private clsTextBox clsSri = null;

        private bool activo = false;
        private string tipo = "I";
        public ctlCreaRetencion()
        {
            InitializeComponent();

            txtNid.IsEnabled = false;
            txtSri.IsEnabled = true;

            clsDsc = new clsTextBox(txtNombre, General.USA_TECLADO_PANTALLA);

            clsDsc.setMaxLength(200);
            clsDsc.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsDsc.permiteEspacios = true;
            clsDsc.permitePorcentaje = true;
            clsDsc.permitePunto = true;

            clsVal = new clsTextBox(txtValor, General.USA_TECLADO_PANTALLA);

            clsVal.setMaxLength(6);
            clsVal.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;
            clsVal.permiteEspacios = false;

            clsSri = new clsTextBox(txtSri, General.USA_TECLADO_PANTALLA);

            clsSri.setMaxLength(10);
            clsSri.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            
        }

        public void setCodigo(string codigo)
        {
            PctRetencion retencion = new PctRetencion(General.connectionString);

            mRetencion mretencion = retencion.GetById(codigo);

            if (retencion != null)
            {
                txtNid.Text = mretencion.codigo;
                txtSri.Text = mretencion.codsri;
                txtNombre.Text = mretencion.descripcion;
                txtValor.Text = mretencion.valor.ToString(General.FORMATO_NUMERICO_REPORTE);
                General.selectedByTagCombo(cboTipo, mretencion.tipo);
                chkAtv.IsChecked = mretencion.estado == "A" ? true : false;
            }
        }

        private void chkAtv_Checked(object sender, RoutedEventArgs e)
        {
            activo = true;
        }

        private void chkAtv_Unchecked(object sender, RoutedEventArgs e)
        {
            activo = false;
        }

        private void cboTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tipo = ((ComboBoxItem)(cboTipo.SelectedItem)).Tag.ToString();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";

            if (txtNombre.Text.Trim().Length == 0)
            {
                msg += "- El campo Nombre no puede estar en blanco" + ((char)13);
            }

            if (txtValor.Text.Trim().Length == 0)
            {
                msg += "- El campo Valor no puede estar en blanco" + ((char)13);
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Desar grabar los datos de la Retención?");
            if (res == MessageBoxResult.Yes)
            {
                PctRetencion retencion = new PctRetencion(General.connectionString);

                retencion.codigo = txtNid.Text;
                retencion.codsri = txtSri.Text;
                retencion.descripcion = txtNombre.Text;
                retencion.valor = General.StringToDouble(txtValor.Text);
                retencion.tipo = tipo;
                retencion.activo = activo;

                Respuesta respuesta = retencion.Grabar(this.accion);

                ObjComportamiento.showMuestraMensaje(respuesta);
            }
        }
    }
}
