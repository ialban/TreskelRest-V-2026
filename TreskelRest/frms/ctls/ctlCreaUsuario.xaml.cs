using Entidades;
using Entidades.modelos;
using LibKeys;
using LibPrint;
using Newtonsoft.Json;
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
    public class trjCoordCol
    {
        public string c01 { get; set; } = "";
        public string c02 { get; set; } = "";
        public string c03 { get; set; } = "";
        public string c04 { get; set; } = "";
        public string c05 { get; set; } = "";
        public string c06 { get; set; } = "";
        public string c07 { get; set; } = "";
        public string c08 { get; set; } = "";
        public string c09 { get; set; } = "";
        public string c10 { get; set; } = "";
    }
    public class trjCoordRow
    {
        public trjCoordCol f1 { get; set; }
        public trjCoordCol f2 { get; set; }
        public trjCoordCol f3 { get; set; }
        public trjCoordCol f4 { get; set; }
        public trjCoordCol f5 { get; set; }
    }

    /// <summary>
    /// Interaction logic for ctlCreaUsuario.xaml
    /// </summary>
    public partial class ctlCreaUsuario : UserControl
    {
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        public ctlUsuarios parent { get; set; } = null;

        private List<mPermiso> permisos = new List<mPermiso>();

        private clsTextBox clsNid;
        private clsTextBox clsDsc;
        private clsTextBox clsUsr;
        private clsTextBox clsClv;
        private clsTextBox clsClvApr;

        private string coordenadas;

        public ctlCreaUsuario()
        {
            InitializeComponent();

            clsNid = new clsTextBox(txtCodigo, General.USA_TECLADO_PANTALLA);
            clsDsc = new clsTextBox(txtDescripcion, General.USA_TECLADO_PANTALLA);
            clsUsr = new clsTextBox(txtUsuario, General.USA_TECLADO_PANTALLA);
            clsClv = new clsTextBox(txtClave, General.USA_TECLADO_PANTALLA);
            clsClvApr = new clsTextBox(txtClvAprob, General.USA_TECLADO_PANTALLA);

            clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsDsc.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;
            clsUsr.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.USUARIOS;
            if (General.USA_CLAVE_NUMERICA)
            {
                clsClv.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            }
            else
            {
                clsClv.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.CLAVES;
            }

            clsNid.setMaxLength(6);
            clsDsc.setMaxLength(50);
            clsUsr.setMaxLength(20);
            clsClv.setMaxLength(General.CANTIDAD_CARACTERES_PASSWORD);
            clsClvApr.setMaxLength(20);
            clsClvApr.setMinLength(10);

            clsDsc.permiteEspacios = true;

            this.getEmpleados();
            this.getPermisos();

            if (General.MODO_VALIDACION_ANULA_ELIMINA != "G")
            {
                grbTjtaClv.Visibility = Visibility.Hidden;
            }

            this.actualizaPermisos();
        }

        private void actualizaPermisos()
        {
            foreach (GroupBox grb in grdPermisos.Children)
            {
                foreach (Control control in ((Grid)grb.Content).Children)
                {
                    if (control.GetType() == typeof(CheckBox))
                    {
                        if (this.permisos.Count > 0)
                        {
                            CheckBox chk = (CheckBox)control;

                            mPermiso existeMTK = this.permisos.Where(x => x.tag == chk.Tag.ToString()).FirstOrDefault();

                            chk.IsChecked = existeMTK != null ? existeMTK.estado : false;

                            chk.Checked += new RoutedEventHandler(chk_Checked);
                            chk.Unchecked += new RoutedEventHandler(chk_UnChecked);
                        }                        
                    }
                }
            }
        }

        private void getPermisos()
        {
            Usuarios usuarios = new Usuarios(General.connectionString);

            this.permisos = usuarios.getPermisos();
        }

        private void DefinePermisos(string tag, Boolean estado)
        {
            try
            {
                var stdo = this.permisos.Find(x => x.tag == tag);
                if (stdo !=  null)
                    this.permisos.Find(x => x.tag == tag).estado = estado;
            }
            catch(Exception e)
            {

            }            
                        
        }

        private void chk_Checked(object sender, RoutedEventArgs e)
        {
            this.DefinePermisos(((CheckBox)sender).Tag.ToString(), true);
        }

        private void chk_UnChecked(object sender, RoutedEventArgs e)
        {
            this.DefinePermisos(((CheckBox)sender).Tag.ToString(), false);
        }

        public void limpiar()
        {
            txtCodigo.Text = "";
            txtDescripcion.Text = "";
            txtUsuario.Text = "";
            txtClave.Password = "";

            General.selectedByTagCombo(cboTipo, "");
            General.selectedByTagCombo(cboEmpleado, "");

            chkDelPedido.IsChecked = false;
            chkDescuento.IsChecked = false;
            chkGrps.IsChecked = false;
            chkInventario.IsChecked = false;
            chkPedidos.IsChecked = false;
            chkProductos.IsChecked = false;
            chkRegalos.IsChecked = false;
            chkReportes.IsChecked = false;
            chkUsuarios.IsChecked = false;
            chkValEgr.IsChecked = false;
            chkValIng.IsChecked = false;
            chkVentas.IsChecked = false;
            chkBackup.IsChecked = false;
            chkCaja.IsChecked = false;
            chkCompras.IsChecked = false;
            chkConfig.IsChecked = false;
            chkAnulaFact.IsChecked = false;
            chkMarket.IsChecked = false;

            tabControl.SelectedIndex = 0;


            chkEstado.IsChecked = true;

            if (General.MODO_VALIDACION_ANULA_ELIMINA != "G")
            {
                lblClvAprob.Visibility = Visibility.Hidden;
                txtClvAprob.Visibility = Visibility.Hidden;
                btnGenerar.Visibility = Visibility.Hidden;
                TarjetaCoords.Visibility = Visibility.Hidden;
            }
        }

        public void getEmpleados()
        {
            Empleados empleados = new Empleados(General.connectionString);

            DataTable dt = empleados.lista();

            foreach (DataRow dr in dt.Rows)
            {
                General.fillComboBoxFromDataTable(cboEmpleado, dt, "S");
            }
        }

        public void setRowCoord(string fila, trjCoordCol col)
        {
            foreach(var lbls in TarjetaCoords.Children)
            {
                if (lbls.GetType() == typeof(Label))
                {
                    if (((Label)lbls).Name.Contains("f" + fila + "c"))
                    {
                        if (col != null)
                        {
                            if (((Label)lbls).Name.Contains("c01")) ((Label)lbls).Content = col.c01;
                            if (((Label)lbls).Name.Contains("c02")) ((Label)lbls).Content = col.c02;
                            if (((Label)lbls).Name.Contains("c03")) ((Label)lbls).Content = col.c03;
                            if (((Label)lbls).Name.Contains("c04")) ((Label)lbls).Content = col.c04;
                            if (((Label)lbls).Name.Contains("c05")) ((Label)lbls).Content = col.c05;
                            if (((Label)lbls).Name.Contains("c06")) ((Label)lbls).Content = col.c06;
                            if (((Label)lbls).Name.Contains("c07")) ((Label)lbls).Content = col.c07;
                            if (((Label)lbls).Name.Contains("c08")) ((Label)lbls).Content = col.c08;
                            if (((Label)lbls).Name.Contains("c09")) ((Label)lbls).Content = col.c09;
                            if (((Label)lbls).Name.Contains("c10")) ((Label)lbls).Content = col.c10;
                        }
                    }
                }
            }
            
        }

        public void setCodigo(string nid)
        {
            Usuarios usuario = new Usuarios(General.connectionString);

            usuario.usuaCod = nid;
            if (usuario.getById() == true)
            {
                txtCodigo.Text = usuario.usuaCod;                

                txtDescripcion.Text = usuario.usuaNbr;
                txtUsuario.Text = usuario.usuaUsr;
                
                txtClave.Password = usuario.usuaClv;

                General.selectedByTagCombo(cboTipo, usuario.usuaTip);
                General.selectedByTagCombo(cboEmpleado, usuario.empdNid);

                txtCodigo.IsEnabled = false;
                txtUsuario.IsEnabled = false;

                // ------------------
                coordenadas = usuario.usuacoord;
                trjCoordRow trj = JsonConvert.DeserializeObject<trjCoordRow>(usuario.usuacoord);

                if (trj != null)
                {
                    setRowCoord("1", trj.f1);
                    setRowCoord("2", trj.f2);
                    setRowCoord("3", trj.f3);
                    setRowCoord("4", trj.f4);
                    setRowCoord("5", trj.f5);
                }
                // ------------------

                this.permisos= usuario.getPermisoUsuario(nid);
                this.actualizaPermisos();
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            if (txtDescripcion.Text.Trim().Equals(""))
            {
                msg += "Debe ingresar una descripción" + ((char) 13);
            }

            if (txtUsuario.Text.Trim().Equals(""))
            {
                msg += "Debe ingresar un usuario" + ((char)13);
            }

            if (txtClave.Password.Trim().Equals(""))
            {
                msg += "Debe ingresar una contraseña" + ((char)13); 
            }

            if(cboTipo.SelectedIndex <= 0) {
                msg += "Debe seleccionar un tipo de usuario" + ((char)13);
            }

            if(cboEmpleado.SelectedIndex <= 0)
            {
                msg += "Debe seleccionar un empleado" + ((char)13);
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            if (ObjComportamiento.showMsgYesNo("¿Está seguro de grabar la información del usuario?") == MessageBoxResult.Yes)
            {
                Usuarios usuarios = new Usuarios(General.connectionString);

                usuarios.usuaAtv = chkEstado.IsChecked == true ? "A" : "I";
                usuarios.usuaCad = General.CODUSUARIO;
                usuarios.usuaClv = txtClave.Password.ToString();
                usuarios.usuaCod = txtCodigo.Text;
                usuarios.usuaNbr = txtDescripcion.Text;
                usuarios.usuaTip = ((ComboBoxItem)cboTipo.SelectedItem).Tag.ToString();
                usuarios.empdNid = ((ComboBoxItem)cboEmpleado.SelectedItem).Tag.ToString();
                usuarios.usuaUsr = txtUsuario.Text;

                usuarios.permisos = this.permisos;

                usuarios.usuacoord = coordenadas;

                Respuesta respuesta = usuarios.grabar(this.accion);

                ObjComportamiento.showMuestraMensaje(respuesta);

                if(accion.Equals("N") && respuesta.resultado)
                {
                    this.limpiar();
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void cboTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboTipo != null && txtClvAprob != null)
            {
                txtClvAprob.IsEnabled = false;
                txtClvAprob.Password = "";
                clearAllCheckbox(grpPuede);
                clearAllCheckbox(grpAcciones);


                chkAllPue.IsChecked = false;
                chkAllAcc.IsChecked = false;

                chkUsuarios.IsEnabled = false;
                chkConfig.IsEnabled = false;
                chkBackup.IsEnabled = false;
                chkDescuento.IsEnabled = false;
                chkRegalos.IsEnabled = false;
                chkDelPedido.IsEnabled = false;
                chkAnulaFact.IsEnabled = false;

                //if (cboTipo.SelectedIndex == 0)
                //{

                //}

                if (cboTipo.SelectedIndex == 1)
                {
                    chkPedidos.IsChecked = true;
                    chkReportes.IsChecked = true;

                    chkDelPedido.IsEnabled = true;
                    chkVentas.IsEnabled = false;
                    chkCaja.IsEnabled = false;
                    chkProductos.IsEnabled = false;
                    chkCompras.IsEnabled = false;
                    chkInventario.IsEnabled = false;
                    chkValIng.IsEnabled = false;
                    chkValEgr.IsEnabled = false;
                }

                if (cboTipo.SelectedIndex == 2)
                {
                    chkPedidos.IsChecked = true;
                    chkVentas.IsChecked = true;
                    chkCaja.IsChecked = true;
                    chkCompras.IsChecked = true;
                    chkReportes.IsChecked = true;
                    chkValIng.IsChecked = true;
                    chkValEgr.IsChecked = true;
                    chkDescuento.IsChecked = true;
                    chkRegalos.IsChecked = true;

                    chkDescuento.IsEnabled = true;
                    chkRegalos.IsEnabled = true;
                    chkDelPedido.IsEnabled = true;
                    chkAnulaFact.IsEnabled = true;

                    chkVentas.IsEnabled = true;
                    chkCaja.IsEnabled = true;
                    chkProductos.IsEnabled = true;
                    chkCompras.IsEnabled = true;
                    chkInventario.IsEnabled = true;
                    chkValIng.IsEnabled = true;
                    chkValEgr.IsEnabled = true;
                }

                if (cboTipo.SelectedIndex == 3)
                {
                    chkAllPue.IsChecked = true;
                    chkAllAcc.IsChecked = true;

                    txtClvAprob.IsEnabled = true;
                    
                    chkUsuarios.IsEnabled = true;
                    chkConfig.IsEnabled = true;
                    chkBackup.IsEnabled = true;

                    chkDescuento.IsEnabled = true;
                    chkRegalos.IsEnabled = true;
                    chkDelPedido.IsEnabled = true;
                    chkAnulaFact.IsEnabled = true;
                }
            }
        }

        private void clearAllCheckbox(GroupBox grp)
        {
            var ob = (Grid) grp.Content;

            foreach(object obj in ob.Children)
            {
                if (obj.GetType() == typeof(CheckBox))
                {
                    ((CheckBox)obj).IsChecked = false;
                }
            }
        }

        private void CheckUncheckAllCheckbox(GroupBox grp, bool seleccionado)
        {
            var ob = (Grid)grp.Content;

            foreach (object obj in ob.Children)
            {
                if (obj.GetType() == typeof(CheckBox))
                {
                    CheckBox chk = (CheckBox)obj;
                    if (chk.IsEnabled)
                    {
                        chk.IsChecked = seleccionado;
                    }
                }
            }
        }

        private void chkAllPue_Checked(object sender, RoutedEventArgs e)
        {
            if (chkAllPue != null)
            {
                CheckUncheckAllCheckbox(grpPuede, true);
            }
        }

        private void chkAllPue_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkAllPue != null)
            {
                CheckUncheckAllCheckbox(grpPuede, false);
            }
        }

        private void chkAllAcc_Checked(object sender, RoutedEventArgs e)
        {
            if (chkAllAcc != null)
            {
                CheckUncheckAllCheckbox(grpAcciones, true);
            }
        }

        private void chkAllAcc_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkAllAcc != null)
            {
                CheckUncheckAllCheckbox(grpAcciones, false);
            }
        }

        private trjCoordCol generaColumnas()
        {
            trjCoordCol col = new trjCoordCol();

            for (int i = 0; i < 10; i++) // Columnas
            {
                int numero = 0;
                do
                {
                    numero = General.generaNumeroAleatorio() * 10 +
                             General.generaNumeroAleatorio();
                } while (numero == 0);
                
                if (i == 0) col.c01 = string.Format("{0: 00}", numero);
                if (i == 1) col.c02 = string.Format("{0: 00}", numero);
                if (i == 2) col.c03 = string.Format("{0: 00}", numero);
                if (i == 3) col.c04 = string.Format("{0: 00}", numero);
                if (i == 4) col.c05 = string.Format("{0: 00}", numero);
                if (i == 5) col.c06 = string.Format("{0: 00}", numero);
                if (i == 6) col.c07 = string.Format("{0: 00}", numero);
                if (i == 7) col.c08 = string.Format("{0: 00}", numero);
                if (i == 8) col.c09 = string.Format("{0: 00}", numero);
                if (i == 9) col.c10 = string.Format("{0: 00}", numero);
            }

            return col;
        }


        private void btnGenerar_Click(object sender, RoutedEventArgs e)
        {
            trjCoordRow row = new trjCoordRow();

            for (int j = 0; j < 5; j++) // Filas
            {
                trjCoordCol col = new trjCoordCol();

                if (j == 0) row.f1 = generaColumnas();
                if (j == 1) row.f2 = generaColumnas();
                if (j == 2) row.f3 = generaColumnas();
                if (j == 3) row.f4 = generaColumnas();
                if (j == 4) row.f5 = generaColumnas();
            }

            setRowCoord("1", row.f1);
            setRowCoord("2", row.f2);
            setRowCoord("3", row.f3);
            setRowCoord("4", row.f4);
            setRowCoord("5", row.f5);

            coordenadas = JsonConvert.SerializeObject(row);
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            EposPrint cls = new EposPrint();

            Respuesta resp = cls.setPrinter(General.PRINTER_FACTURA);

            cls.TipoImpresora = General.TIPO_IMPRESORAS;
            cls.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;
            cls.DefineSeparador = char.Parse(General.SEPARADOR);
            cls.ImprimeNombreEmpresa = General.IMPRIME_NOMBRE_EMPRESA;
            cls.ImprimeDireccionEmpresa = General.IMPRIME_DIRECCION_EMPRESA;
            cls.ImprimeRucEmpresa = General.IMPRIME_RUC_EMPRESA;
            cls.CabeceraNegrita = General.NEGRITA_EN_CABECERA;
            cls.AlineacionCabecera = General.ALINEACION_CABECERA;
            cls.CantidadLineasSaltarEndTabla = General.CANTIDAD_LINEAS_FINAL_ITEMS;

            var row = JsonConvert.DeserializeObject<trjCoordRow>(coordenadas);

            LibPrint.modelo.trjCoordRow pRow = new LibPrint.modelo.trjCoordRow();

            pRow.f1 = new LibPrint.modelo.trjCoordCol
            {
                c01 = row.f1.c01,
                c02 = row.f1.c02,
                c03 = row.f1.c03,
                c04 = row.f1.c04,
                c05 = row.f1.c05,
                c06 = row.f1.c06,
                c07 = row.f1.c07,
                c08 = row.f1.c08,
                c09 = row.f1.c09,
                c10 = row.f1.c10
            };

            pRow.f2 = new LibPrint.modelo.trjCoordCol
            {
                c01 = row.f2.c01,
                c02 = row.f2.c02,
                c03 = row.f2.c03,
                c04 = row.f2.c04,
                c05 = row.f2.c05,
                c06 = row.f2.c06,
                c07 = row.f2.c07,
                c08 = row.f2.c08,
                c09 = row.f2.c09,
                c10 = row.f2.c10
            };

            pRow.f3 = new LibPrint.modelo.trjCoordCol
            {
                c01 = row.f3.c01,
                c02 = row.f3.c02,
                c03 = row.f3.c03,
                c04 = row.f3.c04,
                c05 = row.f3.c05,
                c06 = row.f3.c06,
                c07 = row.f3.c07,
                c08 = row.f3.c08,
                c09 = row.f3.c09,
                c10 = row.f3.c10
            };

            pRow.f4 = new LibPrint.modelo.trjCoordCol
            {
                c01 = row.f4.c01,
                c02 = row.f4.c02,
                c03 = row.f4.c03,
                c04 = row.f4.c04,
                c05 = row.f4.c05,
                c06 = row.f4.c06,
                c07 = row.f4.c07,
                c08 = row.f4.c08,
                c09 = row.f4.c09,
                c10 = row.f4.c10
            };

            pRow.f5 = new LibPrint.modelo.trjCoordCol
            {
                c01 = row.f5.c01,
                c02 = row.f5.c02,
                c03 = row.f5.c03,
                c04 = row.f5.c04,
                c05 = row.f5.c05,
                c06 = row.f5.c06,
                c07 = row.f5.c07,
                c08 = row.f5.c08,
                c09 = row.f5.c09,
                c10 = row.f5.c10
            };

            cls.coordenadas(txtDescripcion.Text, pRow,
                            General.EMPRESA, General.DIRECCION_LOCAL, General.RUC_EMPRESA);

            cls.sbIpr();
            cls.Dispose();
        }
    }
}
