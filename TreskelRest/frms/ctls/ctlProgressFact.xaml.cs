using Entidades.modelos;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlProgressFact.xaml
    /// </summary>
    public partial class ctlProgressFact : UserControl
    {
        public Window contenedor { get; set; }

        public enum EnumEstados
        {
            NoProcesado = 0,
            Autorizado = 1,
            Pendiente = 2,
            Devuelta = 3,
            Procesando = 4,
            Validado = 5
        }

        private EnumEstados _estados = EnumEstados.NoProcesado;
        private DocElectronico doc = new DocElectronico();

        public EnumEstados Estado
        {
            set
            {
                _estados = value;
            }
        }

        public ctlProgressFact()
        {
            InitializeComponent();
            doc._retornaEstado += RetornaEstado;
            doc._Finalizado += Finalizado;
            doc._disparaError += DisparaError;
        }

        private void Finalizado()
        {
            contenedor.Close();
        }

        private void RetornaEstado(string estado)
        {
            switch (estado)
            {
                case "":
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/connecting.png"));
                    contenido.Text = "Conectando...";
                    break;
                case "C":
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/connected.png"));
                    contenido.Text = "Conectado...";
                    break;
                case "G":
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/generando.png"));
                    contenido.Text = "Conectando...";
                    break;
                case "W":
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/toprocess.png"));
                    contenido.Text = "Procesando...";
                    break;
                case "A":
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/authorize.png"));
                    contenido.Text = "Factura Autorizada!";
                    break;
                case "P":
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/pending.png"));
                    contenido.Text = "Pendiente de Autorización!";
                    break;
                case "V":
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/verified.png"));
                    contenido.Text = "Verificada pero no Autorizada!";
                    break;
                case "R":
                case "D":
                    imagen.Source = new BitmapImage(new Uri("pack://application:,,,/images/rejected.png"));
                    contenido.Text = "Rechazada o Devuelta!";
                    break;
            }
            System.Windows.Forms.Application.DoEvents();
        }

        public void Iniciar(mFacturasSRI data)
        {
            _ = doc.Procesar(data);
        }

        private void DisparaError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            contenedor.Close();
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
