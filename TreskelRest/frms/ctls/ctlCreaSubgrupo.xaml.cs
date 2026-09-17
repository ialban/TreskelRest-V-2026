using Entidades;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ctlCreaSubgrupo.xaml
    /// </summary>
    public partial class ctlCreaSubgrupo : UserControl
    {
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        public ctlSubgrupos parent { get; set; } = null;

        private clsTextBox clsNid;
        private clsTextBox clsSgr;

        public ctlCreaSubgrupo()
        {
            InitializeComponent();

            clsNid = new clsTextBox(txtNid, General.USA_TECLADO_PANTALLA);
            clsSgr = new clsTextBox(txtNombre, General.USA_TECLADO_PANTALLA);

            clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsSgr.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;

            clsNid.setMaxLength(4);
            clsSgr.setMaxLength(50);

            clsSgr.permiteEspacios = true;
            clsSgr.otrosCaracteresPermitidos = "./,()-:";

            Grupos grupos = new Grupos(General.connectionString);

            DataTable dt = grupos.listar();

            General.fillComboBoxFromDataTable(cboGrupo, dt, "S");

            txtNid.IsEnabled = true;
        }
        
        public void setCodigo(string nid)
        {
            SubGrupos subgrupos = new SubGrupos(General.connectionString);

            subgrupos.sgrpCod = nid;
            if (subgrupos.getById() == true)
            {
                txtNid.Text = subgrupos.sgrpCod;

                General.selectedByTagCombo(cboGrupo, subgrupos.grpCod);

                txtNombre.Text = subgrupos.sgrpNbr;
                
                txtNid.IsEnabled = false;
            }
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
                msg += "- El campo Subgrupo no puede estar en blanco" + ((char)13);
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Desar grabar los datos del Subgrupo de productos?");
            if (res == MessageBoxResult.Yes)
            {
                SubGrupos subgrupos = new SubGrupos(General.connectionString);

                subgrupos.sgrpCod = txtNid.Text.PadLeft(4, '0');
                subgrupos.grpCod = ((ComboBoxItem)(cboGrupo.SelectedItem)).Tag.ToString();
                subgrupos.sgrpNbr = txtNombre.Text;

                Respuesta respuesta = subgrupos.grabar(this.accion);
                if (respuesta.resultado)
                {
                    ObjComportamiento.showMsgInfo(respuesta.mensaje);
                    if (this.accion.Equals("M"))
                    {
                        this.parent.Actualiza_Trigger();
                    }
                    contenedor.Close();
                }
                else
                {
                    ObjComportamiento.showMsgError(respuesta.mensaje);
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }
    }
}
