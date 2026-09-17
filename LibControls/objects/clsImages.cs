using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LibControls.objects
{
    public class clsImages: IDisposable
    {
        private Image image { get; set; } = null;

        public clsImages() { }

        public clsImages(Image image)
        {
            this.image = image;
        }

        public Bitmap resize(int newHeigth, int newWidth)
        {
            Bitmap bmp = new Bitmap(newWidth, newHeigth);
            if (image != null)
                bmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
           
            Graphics grImg = Graphics.FromImage(bmp);

            grImg.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grImg.SmoothingMode = SmoothingMode.HighQuality;
            grImg.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var destRect = new Rectangle(0, 0, newWidth, newHeigth);

            if(image != null)
                grImg.DrawImage(image, destRect, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            return bmp;
        }

        public void LoadFromFile(string archivo)
        {
            image = Image.FromFile(archivo);
        }

        public void LoadFromUrl(string url)
        {
            //image = Image.FromStream
        }

        public void LoadFromBitmapSource(BitmapSource bmp)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bmp));
                enc.Save(outStream);

                image = new System.Drawing.Bitmap(outStream);
            }
        }

        private byte[] ImageToBytesArray(Image imagen)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(imagen, typeof(byte[]));
        }

        public byte[] BitmapToBytesArray(Bitmap bitmap)
        {
            return ImageToBytesArray(bitmap);
        }

        public byte[] ImageToByteArrayWithResize(int newHeigth, int newWidth)
        {
            Bitmap bmp = resize(newHeigth, newWidth);
            return BitmapToBytesArray(bmp);
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar llamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: elimine el estado administrado (objetos administrados).
                }

                // TODO: libere los recursos no administrados (objetos no administrados) y reemplace el siguiente finalizador.
                // TODO: configure los campos grandes en nulos.

                disposedValue = true;
            }
        }

        // TODO: reemplace un finalizador solo si el anterior Dispose(bool disposing) tiene código para liberar los recursos no administrados.
        // ~clsImages() {
        //   // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
        //   Dispose(false);
        // }

        // Este código se agrega para implementar correctamente el patrón descartable.
        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
            Dispose(true);
            // TODO: quite la marca de comentario de la siguiente línea si el finalizador se ha reemplazado antes.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
