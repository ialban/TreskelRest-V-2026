using FoxLearn.License;
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

namespace TreskelKeys
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class generador : Window
    {
        private const int ProductCode = 1;

        public class Codigo
        {
            public int num1 { get; set; }
            public int num2 { get; set; }
        };

        private string[][] TablaKes = {
            new string[]{ "82", "19", "02", "35", "21", "86", "05", "15", "67", "09"},
            new string[]{ "42", "84", "57", "34", "56", "79", "24", "63", "49", "53" },
            new string[]{ "96", "41", "55", "52", "24", "42", "57", "07", "96", "97" },
            new string[]{ "59", "16", "25", "56", "37", "50", "31", "16", "19", "69" },
            new string[]{ "78", "37", "31", "73", "45", "29", "52", "54", "37", "19" }
        };

        public Codigo getNumKey(int fila, int columna)
        {
            int numero = int.Parse(TablaKes[fila][columna]);

            Codigo codigo = new Codigo();

            codigo.num1 = numero / 10;
            codigo.num2 = numero % 10;

            return codigo;
        }

        public generador()
        {
            InitializeComponent();
        }

        private int getFila(string letra)
        {
            int pos = -1;
            string[] letras = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
            for (int i= 0; i< letras.Length; i++)
            {
                if(letras[i] == letra)
                {
                    pos = i;
                    break;
                }
            }

            return pos;
        }

        private void generaLicencia()
        {
            KeyManager km = new KeyManager(txtProdID.Text);
            KeyValuesClass kv;
            string productKey = String.Empty;

            if (cboTipo.SelectedIndex == 0)
            {
                kv = new KeyValuesClass()
                {
                    Type = LicenseType.TRIAL,
                    Header = Convert.ToByte(9),
                    Footer = Convert.ToByte(6),
                    ProductCode = (byte)ProductCode,
                    Edition = Edition.ENTERPRISE,
                    Version = 1,
                    Expiration = DateTime.Now.Date.AddDays(Convert.ToInt32(30))
                };

                if (!km.GenerateKey(kv, ref productKey))
                {
                    showMsgError("No fue posible generar la clave");
                }
            }
            else
            {
                kv = new KeyValuesClass()
                {
                    Type = LicenseType.FULL,
                    Header = Convert.ToByte(9),
                    Footer = Convert.ToByte(6),
                    ProductCode = (byte)ProductCode,
                    Edition = Edition.ENTERPRISE,
                    Version = 1
                };

                if (!km.GenerateKey(kv, ref productKey))
                {
                    showMsgError("No fue posible generar la clave");
                }
            }
            txtProdKey.Text = productKey != null ? productKey : "";
        }

        public static void showMsgError(String mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnGenerar1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cargaImagen(Image icono, int valor)
        {
            switch (valor)
            {
                case 0:
                    icono.Source = new BitmapImage(new Uri("pack://application:,,,/images/PIC_0.png"));
                    break;
                case 1:
                    icono.Source = new BitmapImage(new Uri("pack://application:,,,/images/PIC_1.png"));
                    break;
                case 2:
                    icono.Source = new BitmapImage(new Uri("pack://application:,,,/images/PIC_2.png"));
                    break;
                case 3:
                    icono.Source = new BitmapImage(new Uri("pack://application:,,,/images/PIC_3.png"));
                    break;
                case 4:
                    icono.Source = new BitmapImage(new Uri("pack://application:,,,/images/PIC_4.png"));
                    break;
                case 5:
                    icono.Source = new BitmapImage(new Uri("pack://application:,,,/images/PIC_5.png"));
                    break;
                case 6:
                    icono.Source = new BitmapImage(new Uri("pack://application:,,,/images/PIC_6.png"));
                    break;
                case 7:
                    icono.Source = new BitmapImage(new Uri("pack://application:,,,/images/PIC_7.png"));
                    break;
                case 8:
                    icono.Source = new BitmapImage(new Uri("pack://application:,,,/images/PIC_8.png"));
                    break;
                case 9:
                    icono.Source = new BitmapImage(new Uri("pack://application:,,,/images/PIC_9.png"));
                    break;
            }
        }

        private void btnGenerar2_Click(object sender, RoutedEventArgs e)
        {
            int fila1 = 0; int colum1 = 0;
            int fila2 = 0; int colum2 = 0;
            int fila3 = 0; int colum3 = 0;

            string letra = txtCodigo1.Text.Substring(0, 1).ToUpper();
            fila1 = int.Parse(txtCodigo1.Text.Substring(1))-1;
            colum1 = getFila(letra);

            letra = txtCodigo2.Text.Substring(0, 1).ToUpper();
            fila2 = int.Parse(txtCodigo2.Text.Substring(1))-1;
            colum2 = getFila(letra);

            letra = txtCodigo3.Text.Substring(0, 1).ToUpper();
            fila3 = int.Parse(txtCodigo3.Text.Substring(1))-1;
            colum3 = getFila(letra);

            Codigo num = getNumKey(fila1, colum1);

            int numero1 = num.num1;
            int numero2 = num.num2;

            num = getNumKey(fila2, colum2);
            
            int numero3 = num.num1;
            int numero4 = num.num2;

            num = getNumKey(fila3, colum3);

            int numero5 = num.num1;
            int numero6 = num.num2;

            cargaImagen(image1, numero1);
            cargaImagen(image2, numero2);
            cargaImagen(image3, numero3);
            cargaImagen(image4, numero4);
            cargaImagen(image5, numero5);
            cargaImagen(image6, numero6);
        }

        private void btnCopiar1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCopiar2_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
