using Entidades;
using Entidades.modelos;
using LibKeys;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlCreaCarrier.xaml
    /// </summary>
    public partial class ctlCreaCarrier : UserControl
    {
        public Window contenedor { get; set; }
        public ctlCarriers parent { get; set; } = null;
        public String accion { get; set; } = "N";

        private clsTextBox clsVal;
        private clsTextBox clsAcro;
        private clsTextBox clsNbr;
        private byte[] _imagen = null;

        public ctlCreaCarrier()
        {
            InitializeComponent();

            clsVal = new clsTextBox(txtValor, General.USA_TECLADO_PANTALLA);
            clsVal.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;
            clsVal.setMaxLength(8);

            clsAcro = new clsTextBox(txtAcronimo, General.USA_TECLADO_PANTALLA);
            clsAcro.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;
            clsAcro.permiteEspacios = false;
            clsAcro.setMaxLength(3);

            clsNbr = new clsTextBox(txtNombre, General.USA_TECLADO_PANTALLA);
            clsNbr.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;
            clsNbr.permiteEspacios = true;
            clsNbr.setMaxLength(100);

            imgUploader.NewSize = new Size(100, 100);
        }

        public void setCodigo(string codigo)
        {
            _imagen = null;
            Carriers carriers = new Carriers(General.connectionString);

            mCarrier carrier =  carriers.getCarrierById(codigo);

            if(carrier != null)
            {
                txtNid.Text = carrier.codigo;
                txtNombre.Text = carrier.carrier;
                txtValor.Text = carrier.valor.ToString().Replace(',','.');
                txtAcronimo.Text = carrier.icono;
                chkAtv.IsChecked = carrier.estado == "S" ? true : false;
                chkPctj.IsChecked = carrier.porcentaje == "S" ? true : false;
                chkAplica.IsChecked = carrier.aplica == "T" ? true : false;

                if(carrier.imagen != null)
                {
                    imgUploader.setImagen = carrier.imagen;
                    _imagen = carrier.imagen;
                }
                else
                {
                    switch (carrier.icono)
                    {
                        case "UBR":
                            imgUploader.setImage = new BitmapImage(new Uri("pack://application:,,,/images/uber-eats.png"));
                            break;
                        case "GLV":
                            imgUploader.setImage = new BitmapImage(new Uri("pack://application:,,,/images/glovo.png"));
                            break;
                        case "RPI":
                            imgUploader.setImage = new BitmapImage(new Uri("pack://application:,,,/images/rappi.png"));
                            break;
                        case "TPT":
                            imgUploader.setImage = new BitmapImage(new Uri("pack://application:,,,/images/tipti.png"));
                            break;
                        case "CBF":
                            imgUploader.setImage = new BitmapImage(new Uri("pack://application:,,,/images/cabify.png"));
                            break;
                        case "SES":
                            imgUploader.setImage = new BitmapImage(new Uri("pack://application:,,,/images/super-easy.png"));
                            break;
                        case "PEY":
                            imgUploader.setImage = new BitmapImage(new Uri("pack://application:,,,/images/pedidosya.png"));
                            break;
                        case "DID":
                            imgUploader.setImage = new BitmapImage(new Uri("pack://application:,,,/images/DiDiFood.png"));
                            break;
                    }
                }
            }
            
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";

            if (txtValor.Text.Length == 0)
            {
                msg += "El valor de recargo por el carrier no puede estar en blanco";
            }

            if (General.StringToDouble(txtValor.Text.ToString()) == 0)
            {
                msg += "El valor de recargo por el carrier no puede ser 0";
            }

            if(msg != "")
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            Carriers carrier = new Carriers(General.connectionString);

            if (_imagen != null)
                carrier.imagen = _imagen;
            
            carrier.nombre = txtNombre.Text.ToUpper().ToString();
            carrier.precio = General.StringToDouble(txtValor.Text.ToString());
            carrier.esPorcentaje = chkPctj.IsChecked == true ? true : false;
            carrier.activo = chkAtv.IsChecked == true ? true : false;
            carrier.aplica = chkAplica.IsChecked == true ? "T" : "I";
            carrier.codigo = txtNid.Text.ToString();
            carrier.acronimo = txtAcronimo.Text.ToUpper().ToString();

            var result = carrier.grabar();
            
            if (result.resultado)
            {
                ObjComportamiento.showMsgInfo("Datos del carrier fueron actualizados correctamente");
            }else
            {
                ObjComportamiento.showMsgError("Datos del carrier no pudieron ser actualizados");
            }
        }

        private void imgUploader__ctlImageUploader(byte[] imagen)
        {
            _imagen = imagen;
        }
    }
}
