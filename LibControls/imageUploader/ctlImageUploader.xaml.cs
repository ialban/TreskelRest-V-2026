using LibControls.objects;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LibControls.imageUploader
{
    /// <summary>
    /// Lógica de interacción para imageUploader.xaml
    /// </summary>
    public partial class ctlImageUploader : UserControl
    {
        private String _img = null;
        private BitmapSource _bmp = null;
        private byte[] _imagen = null;
        private Size _newSize;

        public delegate void ctlImageUploader_load(byte[] imagen);
        public event ctlImageUploader_load _ctlImageUploader;

        public ctlImageUploader()
        {
            InitializeComponent();
        }

        public Size NewSize
        {
            set
            {
                _newSize = value;
            }
        }

        public byte[] setImagen
        {
            get
            {
                return _imagen;
            }
            set
            {
                _imagen = value;

                icono.Source = BtyeArrayToBitmapImage(_imagen);
            }
        }

        public BitmapImage setImage
        {
            set
            {
                _imagen = null;
                icono.Source = value;
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

        private BitmapImage BtyeArrayToBitmapImage(byte[] _img)
        {
            using (MemoryStream ms = new MemoryStream(_img))
            {
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = null;
                bi.StreamSource = ms;
                bi.EndInit();
                bi.Freeze();

                return bi;
            }
        }

        public String getImageFromFile(Image img)
        {
            String _img = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Selección de Imágenes";
            openFileDialog.Filter = "Imagenes (*.png;*.jpeg)|*.png;*.jpeg";
            if (openFileDialog.ShowDialog() == true)
            {
                _img = openFileDialog.FileName;
                img.Source = new BitmapImage(new Uri(_img));
            }

            return _img;
        }

        private void btnNoImage_Click(object sender, RoutedEventArgs e)
        {
            _img = null;
            _bmp = null;
            _imagen = null;
            try
            {
                icono.Source = new BitmapImage(new Uri("pack://application:,,,/images/noimage.png"));
            }
            catch (Exception er)
            {
                Console.Write(er.Message);
            }
        }

        private void mnuPegar_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                _img = null;
                _bmp = Clipboard.GetImage();
                icono.Source = _bmp;

                ImagenToByteArray();

                _ctlImageUploader(_imagen);
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            _bmp = null;
            _img = getImageFromFile(icono);

            ImagenToByteArray();

            _ctlImageUploader(_imagen);
        }

        private void mnuLimpiar_Click(object sender, RoutedEventArgs e)
        {
            btnNoImage_Click(sender, new RoutedEventArgs());
        }
    }
}
