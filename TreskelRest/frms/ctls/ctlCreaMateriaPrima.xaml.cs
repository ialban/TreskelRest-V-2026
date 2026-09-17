using Entidades;
using LibControls.objects;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlCreaMateriaPrima.xaml
    /// </summary>
    public partial class ctlCreaMateriaPrima : UserControl
    {
        private string _img = null;
        private BitmapSource _bmp = null;
        private byte[] _imagen = null;
        private Size _newSize = new Size(210, 210);

        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        public ctlMateriaPrima parent { get; set; } = null;

        public ctlCreaMateriaPrima()
        {
            InitializeComponent();

            txtNid.IsEnabled = true;
        }

        public Size NewSize
        {
            set
            {
                _newSize = value;
            }
        }

        public void setCodigo(string nid)
        {
            MateriaPrima materia = new MateriaPrima(General.connectionString);

            materia.prdCod = nid;
            if (materia.getById() == true)
            {
                txtNid.Text = materia.prdCod;

                General.selectedByTagCombo(cboUnidad, materia.prdUnd);
                General.selectedByTagCombo(cboGrupo, materia.prdGrp);
                txtNombre.Text = materia.prdNbr;
                chkFinal.IsChecked = materia.prdFnl;
                chkActivo.IsChecked = materia.prdAtv;
                txtBarCode.Text = materia.prdBcd;

                if(materia.prdImg != null)
                {
                    imgProducto.Source = General.BtyeArrayToBitmapImage(materia.prdImg);
                }
                
                txtNid.IsEnabled = false;
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";

            if (txtNombre.Text.Trim().Length == 0)
            {
                msg += "- El campo Producto no puede estar en blanco" + ((char)13);
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Desar grabar los datos del productos?");
            if (res == MessageBoxResult.Yes)
            {
                MateriaPrima materia = new MateriaPrima(General.connectionString);

                materia.prdCod = txtNid.Text.PadLeft(6, '0');
                materia.prdUnd = ((ComboBoxItem)(cboUnidad.SelectedItem)).Tag.ToString();
                materia.prdNbr = txtNombre.Text;
                materia.prdGrp = ((ComboBoxItem)(cboGrupo.SelectedItem)).Tag.ToString();
                materia.prdAtv = chkActivo.IsChecked == true ? true : false;
                materia.prdGiv = chkUsaIva.IsChecked == true ? true : false;
                materia.prdBcd = txtBarCode.Text;
                materia.prdFnl = chkFinal.IsChecked == true ? true : false;

                if (this._img != null)
                {
                    materia.prdImg = General.bitmapImageToBytes(this._img);
                }
                else if (this._imagen != null)
                {
                    materia.prdImg = this._imagen;
                }
                else
                {
                    materia.prdImg = null;
                }        

                Respuesta respuesta = materia.grabar(this.accion);
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

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            _img = General.getImageFromFile(imgProducto);
        }

        private void mnuPegar_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                _img = null;
                _bmp = Clipboard.GetImage();
                imgProducto.Source = _bmp;

                ImagenToByteArray();
                //_ctlImageUploader(_imagen);
            }
        }

        private void mnuLimpiar_Click(object sender, RoutedEventArgs e)
        {
            btnNoImage_Click(sender, new RoutedEventArgs());
        }

        private void btnNoImage_Click(object sender, RoutedEventArgs e)
        {
            _img = null;
            try
            {
                imgProducto.Source = new BitmapImage(new Uri("pack://application:,,,/images/noimage.png"));
            }
            catch(Exception er)
            {
                Console.Write(er.Message);
            }
        }

        private void ImagenToByteArray()
        {
            if (this._img != null)
            {
                clsImages clsImg = new clsImages();
                clsImg.LoadFromFile(this._img);
                _imagen = clsImg.ImageToByteArrayWithResize((int)_newSize.Width, (int)_newSize.Height);
                clsImg.Dispose();
            }

            if (this._bmp != null)
            {
                clsImages clsImg = new clsImages();
                clsImg.LoadFromBitmapSource(this._bmp);
                _imagen = clsImg.ImageToByteArrayWithResize((int)_newSize.Width, (int)_newSize.Height);
                clsImg.Dispose();
            }
        }
    }
}
