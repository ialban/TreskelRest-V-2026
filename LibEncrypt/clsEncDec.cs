using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LibEncrypt
{
    public class clsEncDec : IDisposable
    {
        public enum enuTipEnc
        {
            DES,            // Key = 8 Bytes, Vector = 8 Bytes
            RC2,            // Key = 8 Bytes, Vector = 8 Bytes
            TripleDES,      // Key = 16 Bytes, Vector = 16 Bytes
            Rijndael        // Key = 16 Bytes, Vector = 16 Bytes
        }

        private enum _enuAnc
        {
            ENC,            // Encriptar
            DESC            // Desencriptar
        }

        private enuTipEnc _TipEnc;
        private string _sKey;
        private string _sIV;

        public clsEncDec()
        {
            this.sbCfgEncDec();
        }

        public void sbCfgEncDec(enuTipEnc TipEnc =enuTipEnc.Rijndael, string sKey = null, string sIV = null)
        {
            this._TipEnc = enuTipEnc.Rijndael;
            if (TipEnc != enuTipEnc.Rijndael) this._TipEnc = TipEnc;
            if (sKey != null)
            {
                this._sKey = sKey;
            }else
            {
                this._sKey = "12345678abcdefgh";
                if (TipEnc == enuTipEnc.DES || TipEnc == enuTipEnc.RC2)
                {
                    this._sKey = "1234abcd";
                }
            }

            if (sIV != null)
            {
                this._sIV = sIV;
            }else
            {
                this._sIV = "abcdefgh12345678";
                if (TipEnc == enuTipEnc.DES || TipEnc == enuTipEnc.RC2)
                {
                    this._sIV = "abcd1234";
                }
            }
        }

        public string encriptar(string texto)
        {
            try
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(texto);
                byte[] encripted;

                byte[] key = this._MakeKeyByteArray();
                byte[] iv = this._MakeIvByteArray();

                MemoryStream ms = new MemoryStream(inputBytes.Length * 2);
                ICryptoTransform transform = this._GetServiceProvider(key, iv, _enuAnc.ENC);

                CryptoStream cryptoStream = new CryptoStream(ms, transform, CryptoStreamMode.Write);
                cryptoStream.Write(inputBytes, 0, inputBytes.Length);

                cryptoStream.Close();

                encripted = ms.ToArray();
                
                return Convert.ToBase64String(encripted);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public string desencriptar(string texto)
        {
            try
            {
                byte[] inputBytes = Convert.FromBase64String(texto);
                byte[] resultBytes;

                byte[] key = this._MakeKeyByteArray();
                byte[] iv = this._MakeIvByteArray();
                
                MemoryStream ms = new MemoryStream(texto.Length);
                ICryptoTransform transform = this._GetServiceProvider(key, iv, _enuAnc.DESC);

                CryptoStream cryptoStream = new CryptoStream(ms, transform, CryptoStreamMode.Write);
                cryptoStream.Write(inputBytes, 0, inputBytes.Length);

                cryptoStream.Close();
                resultBytes = ms.ToArray();

                return Encoding.UTF8.GetString(resultBytes);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        private byte[] _MakeKeyByteArray()
        {
            // Dependiendo del algoritmo utilizado.
            switch (this._TipEnc)
            {
                case clsEncDec.enuTipEnc.DES:
                case clsEncDec.enuTipEnc.RC2:
                    if(this._sKey.Length < 8) this._sKey = this._sKey.PadRight(8);
                    if (this._sKey.Length > 8) this._sKey = this._sKey.Substring(0, 8);
                    break;
                case clsEncDec.enuTipEnc.TripleDES:
                case clsEncDec.enuTipEnc.Rijndael:
                    if (this._sKey.Length < 16) this._sKey = this._sKey.PadRight(16);
                    if (this._sKey.Length > 16) this._sKey = this._sKey.Substring(0, 16);
                    break;
            }


            // Utilizando los métodos del namespace System.Text, convertimos la cadena de caracteres en
            // un arreglo de bytes mediante el método GetBytes() del sistema de codificación UTF.
            return Encoding.UTF8.GetBytes(this._sKey);
        }

        private byte[] _MakeIvByteArray()
        {
            // Dependiendo del algoritmo utilizado.
            switch (this._TipEnc)
            {
                case clsEncDec.enuTipEnc.DES:
                case clsEncDec.enuTipEnc.RC2:
                    if (this._sIV.Length < 8) this._sKey = this._sIV.PadRight(8);
                    if (this._sIV.Length > 8) this._sKey = this._sIV.Substring(0, 8);
                    break;
                case clsEncDec.enuTipEnc.TripleDES:
                case clsEncDec.enuTipEnc.Rijndael:
                    if (this._sIV.Length < 16) this._sKey = this._sIV.PadRight(16);
                    if (this._sIV.Length > 16) this._sKey = this._sIV.Substring(0, 16);
                    break;
            }


            // Utilizando los métodos del namespace System.Text, convertimos la cadena de caracteres en
            // un arreglo de bytes mediante el método GetBytes() del sistema de codificación UTF.
            return Encoding.UTF8.GetBytes(this._sIV);
        }

        private ICryptoTransform _GetServiceProvider(byte[] Key, byte[] IV , _enuAnc enuAcc)
        {
            ICryptoTransform transform = null;

            switch (this._TipEnc)
            {
                case enuTipEnc.DES:
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    if (enuAcc == _enuAnc.ENC)
                    {
                        transform = des.CreateEncryptor(Key, IV);
                    }else
                    {
                        transform = des.CreateDecryptor(Key, IV);
                    }
                    break;
                case enuTipEnc.RC2:
                    RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
                    if (enuAcc == _enuAnc.ENC)
                    {
                        transform = rc2.CreateEncryptor(Key, IV);
                    }
                    else
                    {
                        transform = rc2.CreateDecryptor(Key, IV);
                    }
                    break;
                case enuTipEnc.Rijndael:
                    RijndaelManaged rijndael = new RijndaelManaged();
                    if (enuAcc == _enuAnc.ENC)
                    {
                        transform = rijndael.CreateEncryptor(Key, IV);
                    }
                    else
                    {
                        transform = rijndael.CreateDecryptor(Key, IV);
                    }
                    break;
                case enuTipEnc.TripleDES:
                    TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider();
                    if (enuAcc == _enuAnc.ENC)
                    {
                        transform = triple.CreateEncryptor(Key, IV);
                    }
                    else
                    {
                        transform = triple.CreateDecryptor(Key, IV);
                    }
                    break;
            }

            return transform;
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
        // ~clsEncDec() {
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
