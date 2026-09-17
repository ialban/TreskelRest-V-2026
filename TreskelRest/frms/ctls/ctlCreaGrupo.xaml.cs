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
    /// Interaction logic for ctlCreaGrupo.xaml
    /// </summary>
    public partial class ctlCreaGrupo : UserControl
    {
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        private int estado { get; set; } = 0;
        public ctlGrupos parent { get; set; } = null;

        private clsTextBox clsNid;
        private clsTextBox clsNbr;
        
        public ctlCreaGrupo()
        {
            InitializeComponent();

            clsNid = new clsTextBox(txtNid, General.USA_TECLADO_PANTALLA);
            clsNbr = new clsTextBox(txtNombre, General.USA_TECLADO_PANTALLA);

            clsNid.setMaxLength(4);
            clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;

            clsNbr.setMaxLength(50);
            clsNbr.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsNbr.permiteEspacios = true;
            clsNbr.otrosCaracteresPermitidos = "./,()-:";
        }

        public void setCod(string cod)
        {
            Grupos grupos = new Grupos(General.connectionString);

            grupos.grpCod = cod;
            if (grupos.getById() == true)
            {
                txtNid.Text = grupos.grpCod;
                txtNombre.Text = grupos.grpNbr;
                chkAtv.IsChecked = grupos.grpAtv;

                txtNid.IsEnabled = false;
            }
        }

        public void setEstado(int estado)
        {
            Grupos grupos = new Grupos(General.connectionString);

            this.estado = estado;
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            if (txtNid.Text.Trim().Length == 0)
            {
                msg += "- El campo Código no puede estar en blanco" + ((char)13);
            }

            if (txtNombre.Text.Trim().Length == 0)
            {
                msg += "- El campo Nombre no puede estar en blanco" + ((char)13);
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Desar grabar los datos del Grupo de productos?");
            if (res == MessageBoxResult.Yes)
            {
                Grupos grupos = new Grupos(General.connectionString);

                grupos.grpCod = txtNid.Text.PadLeft(4,'0');
                grupos.grpNbr = txtNombre.Text;
                grupos.grpAtv = chkAtv.IsChecked == true ? true : false;

                Respuesta resultado = grupos.grabar(this.accion);
                if (resultado.resultado)
                {
                    ObjComportamiento.showMsgInfo(resultado.mensaje);
                    this.parent.Listar(estado);
                    contenedor.Close();
                }
                else
                {
                    ObjComportamiento.showMsgError(resultado.mensaje);
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }
    }
}
