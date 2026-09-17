using Entidades;
using LibKeys;
using System;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlValeIngEgr.xaml
    /// </summary>
    public partial class ctlValeIngEgr : UserControl
    {
        private String tipo = "I";
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";

        private clsTextBox clsDet;
        private clsTextBox clsVal;

        public void limpiar()
        {
            txtDetalle.Text = "";
            txtValor.Text = "0.00";
            dtpFecha.Text = DateTime.Today.ToString();
        }

        public ctlValeIngEgr(String tipo, String caja)
        {
            this.tipo = tipo;
            InitializeComponent();

            clsDet = new clsTextBox(txtDetalle, General.USA_TECLADO_PANTALLA);
            clsVal = new clsTextBox(txtValor, General.USA_TECLADO_PANTALLA);

            clsDet.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsDet.otrosCaracteresPermitidos = ".-()$/,;:";
            clsDet.permiteEspacios = true;
            clsDet.setMaxLength(50);

            clsVal.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;
            clsVal.setMaxLength(6);

            this.limpiar();
            lblCaja.Content = caja;
        }
        
        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            if(txtDetalle.Text.Trim().Length == 0)
            {
                msg += "- Debe ingresar el detalle o motivo del " + (this.tipo.Equals("I") == true ? "Ingreso" : "Egreso");
            }

            if(txtValor.Text.Trim() == "0.00")
            {
                msg += "- Debe ingresar un valor";
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            MovimientoCaja MovCaja = new MovimientoCaja(General.connectionString);

            MovCaja.caja = General.CAJA;
            MovCaja.detalle = txtDetalle.Text.Trim();

            MovCaja.fecha = DateTime.Parse("11/04/2023");
            MovCaja.fecha = DateTime.Parse(dtpFecha.Text);
            MovCaja.tipo = this.tipo;
            MovCaja.usuarioCrea = General.CODUSUARIO;
            MovCaja.valor = txtValor.Text.Trim();

            Respuesta respuesta = MovCaja.grabar(this.accion);

            if (respuesta.resultado)
            {
                this.limpiar();
            }

            ObjComportamiento.showMuestraMensaje(respuesta);
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void btnMotivos_Click(object sender, RoutedEventArgs e)
        {
            MovimientoCaja MovCaja = new MovimientoCaja(General.connectionString);

            winGeneral win = new winGeneral();
            win.Title = "Listado de Motivos para Generar un Vale de " + (this.tipo.Equals("I") ? "Ingreso" : "Egreso");
            win.Width = 500;
            win.Height = 400;

            ctlMotivos crea = new ctlMotivos();
            crea.contenedor = win;
            crea.tipo = this.tipo;
            crea.data = MovCaja.getMotivos(this.tipo);
            crea.generar();

            crea.aceptaMotivo += new ctlMotivos.AceptaMotivo(Muestra_Motivo);

            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        public void Muestra_Motivo(String secuencial, String mensaje)
        {
            txtDetalle.Text = mensaje;
        }
    }
}
