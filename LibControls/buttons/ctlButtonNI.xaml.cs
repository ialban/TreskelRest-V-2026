using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace LibControls.buttons
{
    /// <summary>
    /// Interaction logic for ctlButtonNI.xaml
    /// </summary>
    public partial class ctlButtonNI : UserControl
    {
        private String _codigo;
        private String _titulo;
        private String _tag;

        private double _ancho;
        private double _alto;

        private Boolean _ShowCodigo = true;
        private Boolean _CentrarTexto = false;
        private Boolean _muestraEdicion = false;

        private string _TipoBotones = "ST";
        private byte[] _imagen = null;

        public delegate void ctlButton_Click(object sender, RoutedEventArgs e, string tipo);
        public event ctlButton_Click _ctlButtonClick;

        public delegate void ctlButton_DobleClick(object sender, MouseButtonEventArgs e, string tipo);
        public event ctlButton_DobleClick _ctlButtonDobleClick;

        public delegate void ctlButton_EditClick(object sender, RoutedEventArgs e);
        public event ctlButton_EditClick _ctlButtonEditClick;

        public enum tamanioButton
        {
            Normal = 1,
            Small = 2
        }

        private tamanioButton _setTamanioButton = tamanioButton.Normal;

        public ctlButtonNI()
        {
            InitializeComponent();
        }

        public tamanioButton SetTamanioButton
        {
            set
            {
                _setTamanioButton = value;
                if (value == tamanioButton.Normal)
                {
                    codigo.FontSize = 14;
                    titulo.FontSize = 12;
                }
                else
                {
                    codigo.FontSize = 12;
                    titulo.FontSize = 10;
                }
            }
        }

        public Boolean MuestraEdicion
        {
            set
            {
                _muestraEdicion = value;
            }
        }

        public Boolean CentrarTexto
        {
            get
            {
                return _CentrarTexto;
            }
            set
            {
                _CentrarTexto = value;
                if (_CentrarTexto)
                {
                    titulo.TextAlignment = TextAlignment.Center;
                }
                else
                {
                    titulo.TextAlignment = TextAlignment.Left;
                }

            }
        }

        public String TipoBotones
        {
            get
            {
                return _TipoBotones;
            }
            set
            {
                _TipoBotones = value;

                if (_TipoBotones == "ST")
                {
                    ShowCodigo = false;
                    CentrarTexto = true;

                    imagen.Height = 0;
                    imagen.Visibility = Visibility.Hidden;

                    titulo.Height = Double.NaN;

                    this.Width = _ancho + 10; // 120;
                    this.Height = _alto + 10; // 70;

                    //Boton.Width = this.Width -10;
                    //Boton.Height = this.Height - 10;
                }
                else if (_TipoBotones == "STE")
                {
                    this.Width = _ancho + 10; //150;
                    this.Height = _alto + 10; // 70;

                    ShowCodigo = false;
                    CentrarTexto = true;

                    if (_muestraEdicion)
                    {
                        Boton.Width = this.Width - 30;
                    }
                    else
                    {
                        Boton.Width = this.Width - 8;
                    }
                    
                    Boton.Height = this.Height - 10;

                    stackTexto.HorizontalAlignment = HorizontalAlignment.Left;
                    titulo.HorizontalAlignment = HorizontalAlignment.Left;
                    titulo.Width = Boton.Width - 10;
                    //titulo.FontSize = 11;

                    if (_muestraEdicion)
                    {
                        Editar.Visibility = Visibility.Visible;
                        Editar.Height = this.Height - 10;
                    }
                }
                else if (_TipoBotones == "SI")
                {
                    this.Width = 100;
                    this.Height = 100;

                    ShowCodigo = false;
                    titulo.Visibility = Visibility.Hidden;
                    titulo.Height = 0;
                    titulo.Width = 0;

                    imagen.Height = this.Height - 32;
                    imagen.Width = this.Width - 32;
                    imagen.HorizontalAlignment = HorizontalAlignment.Center;
                    imagen.VerticalAlignment = VerticalAlignment.Center;
                    imagen.Margin = new Thickness(0, 0, 0, 0);

                    Boton.Width = this.Width - 15;
                    Boton.Height = this.Height - 10;

                    stackTexto.Width = Boton.Width - 3;
                    stackTexto.Margin = new Thickness(0, 0, 0, 0);
                    stackTexto.HorizontalAlignment = HorizontalAlignment.Center;
                }
                else if (_TipoBotones == "SIE")
                {
                    this.Width = 120;
                    this.Height = 100;

                    ShowCodigo = false;
                    titulo.Visibility = Visibility.Hidden;
                    titulo.Height = 0;
                    titulo.Width = 0;
                    CentrarTexto = true;

                    imagen.Height = this.Height - 32;
                    imagen.Width = this.Width - 32;
                    imagen.HorizontalAlignment = HorizontalAlignment.Left;
                    imagen.VerticalAlignment = VerticalAlignment.Center;
                    imagen.Margin = new Thickness(5, 0, 0, 0);

                    if (_muestraEdicion)
                    {
                        Boton.Width = this.Width - 30;
                    }
                    else
                    {
                        Boton.Width = this.Width - 8;
                    }

                    Boton.Height = this.Height - 10;
                    Boton.Width = this.Width - 38;

                    stackTexto.HorizontalAlignment = HorizontalAlignment.Left;
                    titulo.HorizontalAlignment = HorizontalAlignment.Left;
                    titulo.FontSize = 11;

                    if (_muestraEdicion)
                    {
                        Editar.Visibility = Visibility.Visible;
                        Editar.Height = this.Height - 10;
                    }
                }
                else if (_TipoBotones == "IT")
                {
                    this.Width = 100;
                    this.Height = 140;

                    ShowCodigo = false;

                    imagen.Height = this.Width - 32;
                    imagen.Width = this.Width - 32;
                    imagen.HorizontalAlignment = HorizontalAlignment.Center;
                    imagen.VerticalAlignment = VerticalAlignment.Top;
                    imagen.Margin = new Thickness(0, 3, 0, 0);

                    Boton.Width = this.Width - 15;
                    Boton.Height = this.Height - 10;

                    stackTexto.Width = Boton.Width - 3;
                    stackTexto.Margin = new Thickness(0, 0, 0, 0);
                    stackTexto.HorizontalAlignment = HorizontalAlignment.Center;
                    stackTexto.VerticalAlignment = VerticalAlignment.Top;

                    titulo.FontSize = 10;
                    titulo.Height = 60;
                }
                else if (_TipoBotones == "ITE")
                {
                    this.Width = 120;
                    this.Height = 140;

                    ShowCodigo = false;

                    imagen.Height = this.Width - 44;
                    imagen.Width = this.Width - 44;
                    imagen.HorizontalAlignment = HorizontalAlignment.Center;
                    imagen.VerticalAlignment = VerticalAlignment.Top;
                    imagen.Margin = new Thickness(0, 3, 0, 0);

                    if (_muestraEdicion)
                    {
                        Boton.Width = this.Width - 30;
                    }
                    else
                    {
                        Boton.Width = this.Width - 8;
                    }

                    Boton.Width = this.Width - 38;
                    Boton.Height = this.Height - 10;

                    stackTexto.Width = Boton.Width - 3;
                    stackTexto.Margin = new Thickness(0, 0, 0, 0);
                    stackTexto.HorizontalAlignment = HorizontalAlignment.Center;
                    stackTexto.VerticalAlignment = VerticalAlignment.Top;

                    titulo.FontSize = 10;
                    titulo.Height = 60;

                    if (_muestraEdicion)
                    {
                        Editar.Visibility = Visibility.Visible;
                        Editar.Height = this.Height - 10;
                    }
                }
            }
        }

        public byte[] Imagen
        {
            get
            {
                return _imagen;
            }
            set
            {
                _imagen = value;

                if(_TipoBotones == "SI" || _TipoBotones == "IT" || _TipoBotones == "SIE" || _TipoBotones == "ITE")
                {
                    if(_imagen != null)
                        imagen.Source = LoadImage(_imagen);
                }
            }
        }

        public Boolean ShowCodigo
        {
            get
            {
                return _ShowCodigo;
            }
            set
            {
                _ShowCodigo = value;
                if (_ShowCodigo)
                {
                    codigo.Visibility = Visibility.Visible;
                }
                else
                {
                    codigo.Visibility = Visibility.Hidden;
                    codigo.Height = 0;
                    codigo.Width = 0;
                }

            }
        }

        public double Ancho
        {
            get
            {
                _ancho = Boton.Width;
                return _ancho;
            }
            set
            {
                _ancho = value;
                Boton.Width = _ancho;
                stackTexto.Width = Boton.Width - 5;
                this.Width = _ancho + 15;
            }
        }

        public double Alto
        {
            get
            {
                _alto = Boton.Height;
                return _alto;
            }
            set
            {
                _alto = value;
                Boton.Height = _alto;
                this.Height = _alto + 15;
            }
        }

        public String Codigo
        {
            get
            {
                _codigo = codigo.Content.ToString();
                return _codigo;
            }
            set
            {
                _codigo = value;
                codigo.Content = _codigo;
            }
        }

        public String Titulo
        {
            get
            {
                _titulo = titulo.Text.ToString();
                return _titulo;
            }
            set
            {
                _titulo = value;
                titulo.Text = _titulo;                
            }
        }

        public String CodigoAlterno
        {
            get
            {
                _tag = Boton.Tag.ToString();
                return _tag;
            }
            set
            {
                _tag = value;
                Boton.Tag = _tag;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this._ctlButtonClick(sender, e, _codigo);
        }

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //this._ctlButtonDobleClick(sender, e, _codigo);
        }

        private async void Boton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (await TouchHold(Boton, TimeSpan.FromSeconds(2)))
            {
                if (_muestraEdicion)
                    this._ctlButtonDobleClick(sender, e, _codigo);
            }
        }

        public Task<bool> TouchHold(FrameworkElement element, TimeSpan duration)
        {
            DispatcherTimer timer = new DispatcherTimer();
            TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
            timer.Interval = duration;

            MouseButtonEventHandler touchUpHandler = delegate
            {
                timer.Stop();
                if (task.Task.Status == TaskStatus.Running)
                {
                    task.SetResult(false);
                }
            };

            element.PreviewMouseUp += touchUpHandler;

            timer.Tick += delegate
            {
                element.PreviewMouseUp -= touchUpHandler;
                timer.Stop();
                task.SetResult(true);
            };

            timer.Start();
            return task.Task;
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if(_muestraEdicion)
                this._ctlButtonEditClick(Boton, e);
        }
    
        public void setTituloBold(bool value)
        {
            if (value)
                titulo.FontWeight = FontWeights.Bold;
            else
                titulo.FontWeight = FontWeights.Normal;
        }

        public void setTituloColor(bool value)
        {
            if (value)
                titulo.Foreground = Brushes.Red;
            else
                titulo.Foreground = Brushes.Black;
        }
    }
}
