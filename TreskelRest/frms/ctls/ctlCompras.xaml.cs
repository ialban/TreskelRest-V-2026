using Entidades;
using Entidades.modelos;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlCompras.xaml
    /// </summary>
    public partial class ctlCompras : UserControl, IDisposable
    {
        private string codigo = "";
        private string unidad = "";
        private string producto = "";
        private bool ModProd = false;
        private mProducto ProdModificar = null;
        private List<mPago> Pagos = new List<mPago>();
        private double total = 0;
        private double ivaPct = 0;
        private double iva = 0;

        private clsTextBox clsFac;
        private clsTextBox clsCnt;
        private clsTextBox clsPrc;
        private clsTextBox clsSb0;
        private clsTextBox clsSbI;
        private clsTextBox clsDsc;
        private clsTextBox clsIva;
        private clsTextBox clsTot;

        public ctlCompras()
        {
            InitializeComponent();

            clsFac = new clsTextBox(NumFact, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS, 10);
            clsCnt = new clsTextBox(Cantidad, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS, 4);
            clsPrc = new clsTextBox(Precio, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.DECIMALES, 8);
            clsSb0 = new clsTextBox(SubTotal0, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.DECIMALES, 6);
            clsSbI = new clsTextBox(SubTotal12, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.DECIMALES, 6);
            clsDsc = new clsTextBox(Descuento, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.DECIMALES, 6);
            clsIva = new clsTextBox(Iva, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.DECIMALES, 6);
            clsTot = new clsTextBox(Total, General.USA_TECLADO_PANTALLA, clsTextBox.CARACTERES_PERMITIDOS.DECIMALES, 6);

            clsPrc.cantidaDecimales = 4;
            clsSb0.cantidaDecimales = 4;
            clsSbI.cantidaDecimales = 4;
            clsDsc.cantidaDecimales = 4;
            clsIva.cantidaDecimales = 4;
            clsTot.cantidaDecimales = 4;

            Fecha.SelectedDate = DateTime.Today;
            sbLimpiar();
            getEmpresas();
            getProductos("");

            ivaPct = General.IVA;

            SetTamanioMenuConfig();
        }

        public void sbLimpiar()
        {
            Cantidad.Text = "1";
            Precio.Text = "0.00";
            if (cboEmpresa.Items.Count > 0)
                cboEmpresa.SelectedIndex = 0;
            //----------------------
            SubTotal0.Text = "0.00";
            SubTotal12.Text = "0.00";
            Descuento.Text = "0.00";
            Iva.Text = "0.00";
            Total.Text = "0.00";
        }

        private void SetTamanioMenuConfig()
        {
            if (General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal)
            {
                GrpProductos.Height = 78;
                setTamanioBotones(btnCancelar, 70, 14, 30);
                setTamanioBotones(btnGrabar, 70, 14, 30);
                setTamanioBotones(btnPago, 70, 14, 30);
                setTamanioBotones(Todo, 70, 14, 30);
                setTamanioBotones(Vegetales, 70, 14, 30);
                setTamanioBotones(Carnes, 70, 14, 30);
                setTamanioBotones(Bebida, 70, 14, 30);
                setTamanioBotones(BAlcohol, 70, 14, 30);
                setTamanioBotones(Lacteos, 70, 14, 30);
                setTamanioBotones(Mariscos, 70, 14, 30);
                setTamanioBotones(Especias, 70, 14, 30);
                setTamanioBotones(Frutas, 70, 14, 30);
                setTamanioBotones(PanTorta, 70, 14, 30);
                setTamanioBotones(Cereales, 70, 14, 30);
                setTamanioBotones(Pastas, 70, 14, 30);
                setTamanioBotones(CajaChica, 70, 14, 30);
                setTamanioBotones(Herramientas, 70, 14, 30);
                setTamanioBotones(Materiales, 70, 14, 30);
                setTamanioBotones(Otros, 70, 14, 30);
            }
            else
            {
                GrpProductos.Height = 110;
                setTamanioBotones(btnCancelar, 50, 10, 25);
                setTamanioBotones(btnGrabar, 50, 10, 25);
                setTamanioBotones(btnPago, 50, 10, 25);
                setTamanioBotones(Todo, 50, 10, 25);
                setTamanioBotones(Vegetales, 50, 10, 25);
                setTamanioBotones(Carnes, 50, 10, 25);
                setTamanioBotones(Bebida, 50, 10, 25);
                setTamanioBotones(BAlcohol, 50, 10, 25);
                setTamanioBotones(Lacteos, 50, 10, 25);
                setTamanioBotones(Mariscos, 50, 10, 25);
                setTamanioBotones(Especias, 50, 10, 25);
                setTamanioBotones(Frutas, 50, 10, 25);
                setTamanioBotones(PanTorta, 50, 10, 25);
                setTamanioBotones(Cereales, 50, 10, 25);
                setTamanioBotones(Pastas, 50, 10, 25);
                setTamanioBotones(CajaChica, 50, 10, 25);
                setTamanioBotones(Herramientas, 50, 10, 25);
                setTamanioBotones(Materiales, 50, 10, 25);
                setTamanioBotones(Otros, 50, 10, 25);
            }
        }

        private void setTamanioBotones(object obj, int size, int fontSize, int imgSize)
        {
            if (obj.GetType() == typeof(Button))
            {
                var btn = (Button)obj;
                btn.Height = size;
                btn.Width = size;
                ((Image)((StackPanel)btn.Content).Children[0]).Width = imgSize;
                ((Image)((StackPanel)btn.Content).Children[0]).Height = imgSize;
                ((TextBlock)((StackPanel)btn.Content).Children[1]).FontSize= fontSize;
            }
            else
            {
                var btn = (ToggleButton)obj;
                btn.Height = size;
                btn.Width = size;
                ((Image)((StackPanel)btn.Content).Children[0]).Width = imgSize;
                ((Image)((StackPanel)btn.Content).Children[0]).Height = imgSize;
                ((TextBlock)((StackPanel)btn.Content).Children[1]).FontSize = fontSize;
            }
        }

        public void getEmpresas()
        {
            Proveedor proveedor = new Proveedor(General.connectionString);

            DataTable dt = proveedor.listar();

            foreach(DataRow dr in dt.Rows)
            {
                General.fillComboBoxFromDataTable(cboEmpresa, dt, "S");
            }
        }

        public void getProductos(string grupo)
        {
            Contenedor.Children.Clear();

            MateriaPrima productos = new MateriaPrima(General.connectionString);
            DataTable dt = productos.listarProductos(grupo);

            if (dt == null || dt.Rows.Count == 0) return;

            // 1. Determinar el tamaño una sola vez fuera del bucle
            bool esNormal = General.TAMANIO_BOTONES_PEDD == TamanioBotonesMenu.Normal;
            double height = esNormal ? 60 : 50;
            double width = esNormal ? 150 : 130;
            double fontSize = esNormal ? 14 : 10;

            // 2. Optimizar la renderización visual en WPF/UWP
            Contenedor.UpdateLayout();

            foreach (DataRow dr in dt.Rows)
            {
                TextBlock txtBlock = new TextBlock
                {
                    Text = dr["descripcion"]?.ToString(),
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                ToggleButton tbtn = new ToggleButton
                {
                    Height = height,
                    Width = width,
                    FontSize = fontSize,
                    Tag = dr["codigo"]?.ToString() + "-" + dr["unidad"]?.ToString(),
                    Content = txtBlock,
                    Background = Brushes.Honeydew
                };

                tbtn.Checked += ctlButton_Checked;
                tbtn.Unchecked += ctlButton_UnChecked;

                Contenedor.Children.Add(tbtn);
            }
        }

        private void cboEmpresa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                if (sender is ComboBox cbo && cbo.SelectedItem is ComboBoxItem itemSeleccionado)
                {
                    var valor = General.getTagFromComboBox(cboEmpresa);

                    if (valor == "9999999999999")
                    {
                        NumFact.Text = "999-999-999999999";
                    }
                }
            }
        }

        public void ctlButton_Checked(Object sender, RoutedEventArgs e)
        {
            foreach (UIElement obj in Contenedor.Children)
            {
                if(((ToggleButton)sender).Tag != ((ToggleButton)obj).Tag)
                {
                    ((ToggleButton)obj).IsChecked = false;
                    ((ToggleButton)sender).Background = Brushes.Honeydew;
                }  
            }

            var data = ((ToggleButton)sender).Tag.ToString().Split("-");
            this.codigo = data[0];
            this.unidad = data[1];
            
            var contenido = ((ToggleButton)sender).Content == null ? null : ((ToggleButton)sender).Content;
            if (contenido != null && contenido.GetType() == typeof(TextBlock))
            {
                this.producto = ((TextBlock)contenido).Text.ToString();
            }
            else
            {
                this.producto = "";
            }

            ((ToggleButton)sender).Background = Brushes.Goldenrod;

            if (cboUnidad != null)
            {
                General.selectedByTagCombo(cboUnidad, this.unidad);
            }
            
        }

        public void ctlButton_UnChecked(Object sender, RoutedEventArgs e)
        {
            this.codigo = "";
            this.producto = "";

            ((ToggleButton)sender).Background = Brushes.Honeydew;
        }

        public void ButtonGroup_Click(Object sender, RoutedEventArgs e)
        {
            getProductos(((Button) sender).Tag.ToString());
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            if (dgvProd.Items.Count == 0)
            {
                msg += "- No hay items ingresados " + ((char)13);
            }

            if (cboEmpresa.SelectedIndex == 0)
            {
                msg += "- No ha seleccionado un proveedor " + ((char)13);
            }

            if (NumFact.Text.Trim().Length == 0)
            {
                msg += "- No ha ingresado el número de la factura " + ((char)13);
            }

            if (this.Pagos.Count == 0)
            {
                msg += "- No ha ingresado ningún modo de pago " + ((char)13);
            }

            if (SubTotal0.Text == "" && SubTotal12.Text == "") {
                msg += "- Debe ingresar el Subtotal sin IVA o el Subtotal con IVA " + System.Environment.NewLine;
            }

            if (double.Parse(SubTotal0.Text) == 0 && double.Parse(SubTotal12.Text) == 0 && General.INCLUYE_PRECIOS_COMPRAS)
            {
                msg += "- Debe ingresar el Subtotal sin IVA o el Subtotal con IVA " + System.Environment.NewLine;
            }

            if (Iva.Text == "")
            {
                msg += "- No ha ingresado el valor del IVA " + System.Environment.NewLine;
            }

            if (Descuento.Text == "")
            {
                msg += "- No ha ingresado el valor del Descuento " + System.Environment.NewLine;
            }

            if (Total.Text != "")
            {
                if (double.Parse(Total.Text) == 0)
                {
                    msg += "- No ha ingresado el valor total de la compra " + System.Environment.NewLine;
                }
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            if (ObjComportamiento.showMsgYesNo("¿Está seguro de ingresar la compra?") == MessageBoxResult.Yes)
            {
                Compra compra = new Compra(General.connectionString);

                compra.codcaja = General.CAJA;
                compra.codempr = ((ComboBoxItem)cboEmpresa.SelectedItem).Tag.ToString();
                compra.numfact = NumFact.Text.ToString().PadLeft(6,'0');
                compra.fecha = string.Format("{0:yyyy-MM-dd}", Fecha.SelectedDate); //Fecha.Text.ToString();
                compra.subtot0 = double.Parse(SubTotal0.Text);
                compra.subtot = double.Parse(SubTotal12.Text);
                compra.iva = double.Parse(Iva.Text);
                compra.descto = double.Parse(Descuento.Text);
                compra.total = double.Parse(Total.Text);
                compra.usaCajaChica = CajaChica.IsChecked == true ? true : false;

                compra.detalle = new List<DetalleCompra>();

                foreach (mProducto item in dgvProd.Items)
                {
                    double precio = General.StringToDouble(item.precio);
                    double cantidad = double.Parse(item.cantidad);
                    double iva = precio * General.IVA_COMPRA / 100;

                    DetalleCompra detalle = new DetalleCompra
                    {
                        itemcnt = (int) cantidad,
                        itemcod = item.codigo,
                        itemund = item.unidad,
                        itemprc = precio,
                        itemiva = iva,
                        itempto = cantidad * precio + iva,
                        nbrprd  = item.item
                    };
                    
                    compra.detalle.Add(detalle);
                }

                compra.pago = this.Pagos;

                Respuesta respuesta = compra.grabar("N");

                ObjComportamiento.showMuestraMensaje(respuesta);
                
                if(respuesta.resultado)
                {
                    dgvProd.Items.Clear();
                    cboEmpresa.SelectedIndex = 0;
                    cboUnidad.SelectedIndex = 0;
                    NumFact.Text = "";
                    sbLimpiar();

                    ObjComportamiento.showMsgInfo(respuesta.mensaje);
                }
                else
                {
                    ObjComportamiento.showMsgError(respuesta.mensaje);
                }

                CajaChica.IsChecked = false;
            }
            
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        private void btnPago_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Pago de Factura";
            win.Width = 1010;
            win.Height = 550;

            //this.iva = Math.Round(this.total * this.ivaPct / 100, 2);

            ctlPagos crea = new ctlPagos(Total.Text);
            crea.contenedor = win;
            crea.pagoRealizado += new ctlPagos.PagoRealizado(Pago_Realizado);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void Pago_Realizado(List<mPago> pagos)
        {
            Double valor = pagos[0].valor;
            this.Pagos.Clear();
            
            this.Pagos = pagos;
        }

        private void Seleccionar_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";
            if (Cantidad.Text.Length == 0) msg += "Debe ingresar la cantidad de producto";
            if (Precio.Text.Length == 0 && General.INCLUYE_PRECIOS_COMPRAS) msg += "Debe ingresar el precio del producto";
            if (this.producto.Length == 0) msg += "Debe seleccionar un producto";

            if (msg != "")
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            if (this.ModProd)
            {
                this.total -= double.Parse(ProdModificar.cantidad) * double.Parse(ProdModificar.precio);

                this.ProdModificar.cantidad = Cantidad.Text.ToString();
                this.ProdModificar.unidad = ((ComboBoxItem)(cboUnidad.SelectedItem)).Tag.ToString();
                if (Precio.Text.Equals(""))
                {
                    this.ProdModificar.precio = String.Format("{0:0.00}", 0);
                }
                else
                {
                    this.ProdModificar.precio = String.Format("{0:0.00}", double.Parse(Precio.Text.ToString()));
                }
                

                dgvProd.Items.Refresh();
            }
            else
            {
                double cantidad = double.Parse(Cantidad.Text);
                double precio = double.Parse(string.IsNullOrEmpty(Precio.Text) ? "0.00" : Precio.Text.ToString());
                double iva = cantidad*precio * General.IVA_COMPRA/100;
                double total = cantidad*precio;

                mProducto producto = new mProducto
                {
                    codigo = this.codigo,
                    item = this.producto,
                    cantidad = Cantidad.Text.ToString(),
                    unidad = ((ComboBoxItem)(cboUnidad.SelectedItem)).Tag.ToString(),
                    precio = string.Format("{0:0.00}", precio),
                    iva = string.Format("{0:0.00}", iva),
                    total = string.Format("{0:0.00}", total),
                };

                dgvProd.Items.Add(producto);
            }            

            Cantidad.Text = "1";
            General.selectedByTagCombo(cboUnidad, "U");
            Precio.Text = "0.00";

            foreach(UIElement obj in Contenedor.Children)
            {
                ((ToggleButton)obj).IsChecked = false;                
            }

            if (dgvProd.Items.Count > 0)
            {
                SubTotal0.IsEnabled = true;
                SubTotal12.IsEnabled = true;
                Descuento.IsEnabled = true;
                Iva.IsEnabled = true;
                Total.IsEnabled = true;
            }

            Calcular();
        }

        private void Calcular()
        {
            double subtotal0 = 0;
            double subtotal1 = 0;
            double iva = 0;

            foreach(mProducto data in dgvProd.Items)
            {
                subtotal1 += double.Parse(data.total);
                iva += double.Parse(data.iva);
            }

            SubTotal12.Text = string.Format("{0: 0.00}", subtotal1);
            Iva.Text = string.Format("{0: 0.00}", iva);
            Total.Text = string.Format("{0: 0.00}", subtotal1 + iva);
        }

        private void btnDelItem_Click(object sender, RoutedEventArgs e)
        {
            mProducto row = (mProducto)((FrameworkElement)sender).DataContext;
            String codigo = row.codigo;
            dgvProd.Items.Remove(row);
        
            dgvProd.Items.Refresh();

            if (dgvProd.Items.Count == 0)
            {
                SubTotal0.IsEnabled = false;
                SubTotal12.IsEnabled = false;
                Descuento.IsEnabled = false;
                Iva.IsEnabled = false;
                Total.IsEnabled = false;
            }

            Calcular();
        }

        private void btnEditItem_Click(object sender, RoutedEventArgs e)
        {
            this.ProdModificar = (mProducto)((FrameworkElement)sender).DataContext;
            
            foreach (UIElement obj in Contenedor.Children)
            {
                if (((ToggleButton)obj).Tag.ToString() == ProdModificar.codigo)
                {
                    ((ToggleButton)obj).IsChecked = true;
                    ((ToggleButton)obj).Background = Brushes.Goldenrod;
                }else
                {
                    ((ToggleButton)obj).IsChecked = false;
                    ((ToggleButton)obj).Background = Brushes.Honeydew;
                }
            }

            this.codigo = ProdModificar.codigo;
            this.producto = ProdModificar.item;
                        
            Cantidad.Text = ProdModificar.cantidad;
            General.selectedByTagCombo(cboUnidad, ProdModificar.unidad.Substring(0, 1));
            Precio.Text = string.Format("{0:0.00}", General.StringToDouble(ProdModificar.precio));

            this.ModProd = true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ctlCompras() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

       
    }
}
