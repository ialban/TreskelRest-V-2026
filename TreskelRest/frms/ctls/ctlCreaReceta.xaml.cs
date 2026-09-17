using Entidades;
using Entidades.modelos;
using LibKeys;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlCreaReceta.xaml
    /// </summary>
    public partial class ctlCreaReceta : UserControl
    {
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        private String codigo { get; set; }
        private mProducto rowSel;

        private clsTextBox clsCnt;

        public void setNid(String codigo)
        {
            this.codigo = codigo;

            General.selectedByTagCombo(cboItem, codigo);
            Items items = new Items(General.connectionString);

            DataTable dt = items.getRecetaDetalle(codigo);

            foreach(DataRow dr in dt.Rows)
            {
                mProducto producto = new mProducto();

                producto.codigo = dr["prdcod"].ToString();
                producto.cantidad = dr["rctacnt"].ToString();
                producto.item = dr["prdnbr"].ToString();
                switch (dr["rctaund"].ToString())
                {
                    case "U":
                        producto.unidad = "Unidad";
                        break;
                    case "Q":
                        producto.unidad = "Quintal";
                        break;
                    case "K":
                        producto.unidad = "Kilos";
                        break;
                    case "L":
                        producto.unidad = "Libras";
                        break;
                    case "G":
                        producto.unidad = "Gramos";
                        break;
                    case "O":
                        producto.unidad = "Onzas";
                        break;
                    case "P":
                        producto.unidad = "Porciones";
                        break;
                }
                                
                producto.inventario = (Boolean)dr["rctainv"] == true ? "SI" : "NO";
                dgvProductos.Items.Add(producto);
            }            
            
            dgvProductos.Items.Refresh();
            cboItem.IsEnabled = false;
        }

        public ctlCreaReceta()
        {
            InitializeComponent();

            clsCnt = new clsTextBox(txtCantidad, General.USA_TECLADO_PANTALLA);

            clsCnt.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsCnt.setMaxLength(3);

            Items items = new Items(General.connectionString);
            MateriaPrima productos = new MateriaPrima(General.connectionString);

            DataTable dt = items.getAllItems();
            DataTable mp = productos.listarProductos();

            General.fillComboBoxFromDataTable(cboItem, dt, "S");
            General.fillComboBoxFromDataTable(cboProducto, mp, "S");
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            mProducto row = (mProducto)((FrameworkElement)sender).DataContext;
            
            dgvProductos.Items.Remove(row);
            
            dgvProductos.Items.Refresh();
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            rowSel = (mProducto)((FrameworkElement)sender).DataContext;

            General.selectedByTagCombo(cboProducto, rowSel.codigo);
            txtCantidad.Text = rowSel.cantidad;
            General.selectedByTagCombo(cboUnidad, rowSel.unidad.Substring(0, 1));
            chkInv.IsChecked = rowSel.inventario.Equals("SI") ? true : false;

            cboProducto.IsEnabled = false;
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            if (((ComboBoxItem)(cboItem.SelectedItem)).Tag.ToString().Equals("S"))
            {
                msg = "- Debe seleccionar una comida";
            }

            if (dgvProductos.Items.Count == 0)
            {
                msg = "- No ha seleccionado ingredientes para la comida";
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            Items item = new Items(General.connectionString);

            item.itemCod = ((ComboBoxItem)(cboItem.SelectedItem)).Tag.ToString();

            foreach(mProducto prd in dgvProductos.Items)
            {
                item.receta.Add(new Receta
                {
                    prdcod = prd.codigo,
                    rctacnt = General.StringToDouble(prd.cantidad),
                    rctaund = prd.unidad.Substring(0, 1),
                    rctainv = prd.inventario.Equals("SI") ? true : false
                });
            }

            Respuesta respuesta = item.grabarReceta(accion);
            ObjComportamiento.showMuestraMensaje(respuesta);
            btnCancelar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void addToList()
        {
            mProducto producto = new mProducto();

            producto.codigo = ((ComboBoxItem)(cboProducto.SelectedItem)).Tag.ToString();

            foreach (mProducto prd in dgvProductos.Items)
            {
                if (producto.codigo.Equals(prd.codigo))
                {
                    ObjComportamiento.showMsgInfo("El producto seleccionado ya existe en la receta");
                    return;
                }
            }

            producto.item = ((ComboBoxItem)(cboProducto.SelectedItem)).Content.ToString();

            if (txtCantidad.Text.ToString().Trim().Equals("")){
                producto.cantidad = "1";
            }
            else
            {
                if (General.StringToDouble(txtCantidad.Text.ToString()) == 0)
                {
                    producto.cantidad = "1";
                }
                else
                {
                    producto.cantidad = txtCantidad.Text.ToString().Replace(".", ",");
                }
            }
            
            
            if (cboUnidad.SelectedItem != null)
            {
                producto.unidad = ((ComboBoxItem)(cboUnidad.SelectedItem)).Content.ToString();
            }
            else
            {
                producto.unidad = "Unidad";
            }
            producto.inventario = chkInv.IsChecked == true ? "SI" : "NO";

            dgvProductos.Items.Add(producto);
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            if (this.accion.Equals("N"))
            {
                addToList();
            }else
            {
                if (rowSel == null)
                {
                    addToList();
                }
                else
                {
                    if (txtCantidad.Text.ToString().Trim().Equals(""))
                    {
                        rowSel.cantidad = "1";
                    }
                    else
                    {
                        if (General.StringToDouble(txtCantidad.Text.ToString()) == 0)
                        {
                            rowSel.cantidad = "1";
                        }
                        else
                        {
                            rowSel.cantidad = txtCantidad.Text.ToString().Replace(".",",");
                        }                        
                    }
                    
                    if (cboUnidad.SelectedItem != null)
                    {
                        rowSel.unidad = ((ComboBoxItem)(cboUnidad.SelectedItem)).Content.ToString();
                    }
                    else
                    {
                        rowSel.unidad = "Unidad";
                    }
                    rowSel.inventario = chkInv.IsChecked == true ? "SI" : "NO";

                    cboProducto.IsEnabled = true;
                    rowSel = null;
                    dgvProductos.SelectedIndex = -1;
                }
            }
            
            dgvProductos.Items.Refresh();

            cboProducto.SelectedIndex = 0;
            txtCantidad.Text = "";
        }

        private void cboProducto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String codigo=((ComboBoxItem)(cboProducto.SelectedItem)).Tag.ToString();
            cboUnidad.Items.Clear();

            if (!(codigo.Equals("S") || codigo.Equals("")))
            {
                MateriaPrima productos = new MateriaPrima(General.connectionString);

                productos.prdCod = codigo;
                productos.getById();

                if (!productos.prdUnd.Equals("U"))
                {                    
                    String[] data = {"Quintal","Kilos","Gramo","Libra","Onza","Porción" };
                    int i = 0;

                    foreach(String dato in data)
                    {
                        ComboBoxItem cbi = new ComboBoxItem();

                        cbi.Tag = dato.Substring(0, 1);
                        cbi.Content = dato;

                        if (i == 0)
                        {
                            cbi.IsSelected = true;
                        }

                        cboUnidad.Items.Add(cbi);
                        i++;
                    }                    
                }
            }
        }        
    }
}
