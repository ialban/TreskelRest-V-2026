using Entidades;
using LibKeys;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlCreaItems.xaml
    /// </summary>
    public partial class ctlCreaItems : UserControl
    {
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        public ctlItems parent { get; set; } = null;

        private clsTextBox clsNid = null;
        private clsTextBox clsPrecio = null;
        private clsTextBox clsProd = null;

        private byte[] _imagen = null;
        private string _barcode = "";

        public ctlCreaItems()
        {
            InitializeComponent();

            clsNid = new clsTextBox(txtNid, General.USA_TECLADO_PANTALLA);
            clsProd = new clsTextBox(txtNombre, General.USA_TECLADO_PANTALLA);
            clsPrecio = new clsTextBox(txtPrecio, General.USA_TECLADO_PANTALLA);

            txtNid.IsEnabled = true;

            Grupos grupo = new Grupos(General.connectionString);
            MateriaPrima mprima = new MateriaPrima(General.connectionString);

            General.fillComboBoxFromDataTable(cboGrupo, grupo.listar(), "S");
            General.fillComboBoxFromDataTable(cboMPrima, mprima.listarProductosFinales(), "N", true);

            clsNid.setMaxLength(4);
            clsProd.setMaxLength(100);
            clsPrecio.setMaxLength(6);

            clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsProd.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsProd.otrosCaracteresPermitidos = "./,()-:";
            clsPrecio.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;

            clsProd.permiteEspacios = true;

            imgUploader.NewSize = new Size(120, 120);

            chkEstado.IsChecked = true;

            this.KeyUp += CtlCreaItems_KeyUp;
        }

        private void CtlCreaItems_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            _barcode += e.Key;
            Console.WriteLine(_barcode);
            if (_barcode.EndsWith("J"))
            {
                _barcode = "";
            }
        }

        public void setCodigo(string nid)
        {
            Items item = new Items(General.connectionString);

            item.itemCod = nid;
            if (item.getById() == true)
            {
                txtNid.Text = item.itemCod;

                General.selectedByTagCombo(cboGrupo, item.grpCod);
                General.selectedByTagCombo(cboSubgrupo, item.sgrpCod);
                General.selectedByTagCombo(cboMPrima, item.prdCod);                

                txtNombre.Text = item.itemNbr;
                txtPrecio.Text = item.itemPrc.ToString();
                chkGravanIva.IsChecked = item.itemGravaIva;
                chkIva.IsChecked = item.itemIva;
                chkCocina.IsChecked = item.itemCoc;
                chkParrilla.IsChecked = item.itemParr;
                chkBar.IsChecked = item.itemBar;
                chkDelCocina.IsChecked = item.itemDelCoc;
                chkEstado.IsChecked = item.itemAtv;
                chkShowSbgrp.IsChecked = item.itemSbg;
                txtBarcode.Text = item.itemBcde;
                txtAlias.Text = item.itemals;
                switch (item.itemUnd)
                {
                    case "UND":
                        cboUnidad.SelectedIndex = 0;
                        break;
                    case "KG":
                        cboUnidad.SelectedIndex = 1;
                        break;
                    case "LB":
                        cboUnidad.SelectedIndex = 2;
                        break;
                    case "GR":
                        cboUnidad.SelectedIndex = 3;
                        break;
                }

                if (item.itemImg != null)
                {
                    imgUploader.setImagen = item.itemImg;
                    _imagen = item.itemImg;
                }

                cboDestino.SelectedIndex = item.itemPara;

                txtNid.IsEnabled = false;
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            if (txtNid.Text.Trim().Length == 0 && accion != "N")
            {
                msg += "- El campo Código no puede estar en blanco" + ((char)13);
            }

            if (txtNombre.Text.Trim().Length == 0)
            {
                msg += "- El campo Producto no puede estar en blanco" + ((char)13);
            }

            if (txtPrecio.Text.Trim().Length == 0)
            {
                msg += "- El campo Precio no puede estar en blanco" + ((char)13);
            }

            if (((ComboBoxItem)cboSubgrupo.SelectedItem).Tag.ToString().Equals(""))
            {
                msg += "- Debe seleccionar un Subgrupo" + ((char)13);
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Desar grabar los datos del productos?");
            if (res == MessageBoxResult.Yes)
            {
                Items item = new Items(General.connectionString);

                item.itemCod = txtNid.Text.PadLeft(4, '0');
                item.grpCod = ((ComboBoxItem)(cboGrupo.SelectedItem)).Tag.ToString();
                item.sgrpCod = ((ComboBoxItem)(cboSubgrupo.SelectedItem)).Tag.ToString();
                item.prdCod = ((ComboBoxItem)(cboMPrima.SelectedItem)).Tag.ToString();
                item.itemGravaIva = chkGravanIva.IsChecked == true ? true : false;
                item.itemIva = chkIva.IsChecked == true ? true : false;
                item.itemCoc = chkCocina.IsChecked == true ? true : false;
                item.itemParr = chkParrilla.IsChecked == true ? true : false;
                item.itemBar = chkBar.IsChecked == true ? true : false;
                item.itemSbg = chkShowSbgrp.IsChecked == true ? true : false;
                item.itemDelCoc = chkDelCocina.IsChecked == true ? true : false;
                item.itemals = txtAlias.Text.ToUpper().ToString();
                var unidad = cboUnidad.SelectedIndex;
                switch (unidad)
                {
                    case 0:
                        item.itemUnd = "UND"; // Unidad
                        break;
                    case 1:
                        item.itemUnd = "KG"; // Kilogramo
                        break;
                    case 2:
                        item.itemUnd = "LB"; // Libra
                        break;
                    case 3:
                        item.itemUnd = "GR"; // Gramo
                        break;
                }

                item.itemBcde = txtBarcode.Text;

                item.itemPrc = Math.Round(General.StringToDouble(txtPrecio.Text), 2);

                if (chkGravanIva.IsChecked == true)
                {
                    if (chkIva.IsChecked == true)
                    {
                        item.itemPiv = Math.Round(item.itemPrc / (1 + General.IVA / 100), 6);
                    }
                    else
                    {
                        item.itemPiv = Math.Round(item.itemPrc + item.itemPrc * (General.IVA / 100), 2);
                    }
                }
                else
                {
                    item.itemPiv = 0;
                }
                
                item.itemNbr = txtNombre.Text;

                if (_imagen != null)
                    item.itemImg = _imagen;

                item.itemAtv = chkEstado.IsChecked == true ? true : false;
                item.itemPara = cboDestino.SelectedIndex;


                Respuesta respuesta = item.grabar(this.accion);
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

        private void imgUploader__ctlImageUploader(byte[] imagen)
        {
            _imagen = imagen;
        }

        private void cboGrupo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SubGrupos subgrupo = new SubGrupos(General.connectionString);

            DataTable dt = subgrupo.listarByGroup(((ComboBoxItem)(cboGrupo.SelectedItem)).Tag.ToString());

            General.fillComboBoxFromDataTable(cboSubgrupo, dt, "S");

        }

        private void chkGravanIva_Checked(object sender, RoutedEventArgs e)
        {
            if (chkIva != null)
            {
                chkIva.IsEnabled = true;
            }
        }

        private void chkGravanIva_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkIva != null)
            {
                chkIva.IsEnabled = false;
                chkIva.IsChecked = false;
            }
        }
    }
}
