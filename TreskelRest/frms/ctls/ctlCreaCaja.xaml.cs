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
    /// Lógica de interacción para ctlCreaCaja.xaml
    /// </summary>
    public partial class ctlCreaCaja : UserControl
    {
        public Window contenedor { get; set; }
        public ctlCajas parent { get; set; } = null;

        private clsTextBox clsCaja;

        public ctlCreaCaja()
        {
            InitializeComponent();

            clsCaja = new clsTextBox(txtNombre, General.USA_TECLADO_PANTALLA);

            clsCaja.setMaxLength(20);
            clsCaja.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsCaja.permiteEspacios = true;
            clsCaja.otrosCaracteresPermitidos = ".-";
        }

        public void setCodigo(string codigo)
        {
            Caja caja = new Caja(General.connectionString);

            var result = caja.getById(codigo);
            if (result != null)
            {
                txtNid.Text = result.codigo;
                txtNombre.Text = result.nombre;
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            string accion = "N";

            if (txtNombre.Text == "")
            {
                ObjComportamiento.showMsgError("El campo nombre es obligatorio");
                return;
            }

            Caja caja = new Caja(General.connectionString);
            caja.codigo = txtNid.Text;
            caja.nombre = txtNombre.Text;

            if (txtNid.Text != "")
            {
                accion = "M";
            }

            var resultado = caja.Grabar(accion);
            ObjComportamiento.showMuestraMensaje(resultado);
            this.parent.Actualiza_Trigger();
            if (resultado.resultado)
            {
                btnCancelar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }
    }
}
