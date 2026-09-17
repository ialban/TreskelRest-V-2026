using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LibEncrypt
{
    public class CryptUtil : IDisposable
    {
        private byte[] key;
        private byte[] iv;

        private String strKey = "Tr3sk3lS0ftK3yTh3NewV3rs10nAESCK";
        private String strIv = "I4lb4n03A3s#-86c";

        public CryptUtil()
        {
            key = UTF8Encoding.UTF8.GetBytes(strKey);
            iv = UTF8Encoding.UTF8.GetBytes(strIv);
        }

        public CryptUtil(string key)
        {
            if (!(key.Length > 0 && key.Length == 32))
            {
                key = strKey;
            }

            this.key = UTF8Encoding.UTF8.GetBytes(key);
            iv = UTF8Encoding.UTF8.GetBytes(strIv);
        }

        public string Encriptar(string mensaje)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(mensaje);

            byte[] encripted;

            Aes cripto = Aes.Create();
            cripto.KeySize = 256;
            cripto.Mode = CipherMode.CBC;
            cripto.Padding = PaddingMode.PKCS7;

            //RijndaelManaged cripto = new RijndaelManaged();

            using (MemoryStream ms = new MemoryStream(inputBytes.Length))
            {
                using (CryptoStream objCryptoStream =
                    new CryptoStream(ms, cripto.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
                    objCryptoStream.FlushFinalBlock();
                    objCryptoStream.Close();
                }
                encripted = ms.ToArray();
            }

            return Convert.ToBase64String(encripted);
        }

        public string Dencriptar(string mensaje)
        {
            byte[] inputBytes = Convert.FromBase64String(mensaje);

            byte[] resultBytes = new byte[inputBytes.Length];

            string textoLimpio = String.Empty;

            //RijndaelManaged cripto = new RijndaelManaged();
            Aes cripto = Aes.Create();
            cripto.KeySize = 256;
            cripto.Mode = CipherMode.CBC;
            cripto.Padding = PaddingMode.PKCS7;

            using (MemoryStream ms = new MemoryStream(inputBytes))
            {

                using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateDecryptor(key, iv),
                    CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(objCryptoStream, true))
                    {
                        textoLimpio = sr.ReadToEnd();
                    }
                }
            }
            return textoLimpio;
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
        // ~CryptUtil() {
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
