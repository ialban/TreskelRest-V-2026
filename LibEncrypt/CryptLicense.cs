using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LibEncrypt
{
    public enum LicenseType
    {
        TRIAL = 0,
        ANUAL = 1,
        FULL = 2,
    }

    public class LicenseInfo
    {
        public LicenseType licenseType { get; set; } = LicenseType.TRIAL;
        public string ExpirationDate { get; set; } = DateTime.Now.AddDays(30).Date.ToString();
        public string Version { get; set; } = "1.0.0";
        public string ProductCode { get; set; } = "";
        public string OperatingSystem { get; set; } = "";
        public string PCName { get; set; } = "";
        public string LicenseKey { get; set; } = "";

        public bool error { get; set; } = false;
        public string errorMessage { get; set; } = "";
    }

    public class CryptLicense()
    {
        public string ClaveActivacion { get; set; } = "";

        public bool generar(LicenseInfo license, string key)
        {
            try
            {
                license.ProductCode = key;
                var licencia = Newtonsoft.Json.JsonConvert.SerializeObject(license);

                CryptUtil crypt = new CryptUtil(key.Replace("-",""));
                string encriptado = crypt.Encriptar(licencia);

                ClaveActivacion = FormatKeyWithDashes(GeneraHexHash(encriptado));

                if (File.Exists("TreskelSoft.lic"))
                {
                    File.Delete("TreskelSoft.lic");
                }

                File.WriteAllText("TreskelSoft.lic", encriptado);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public LicenseInfo leer(string key)
        {
            LicenseInfo license = new LicenseInfo();
            CryptUtil crypt = new CryptUtil(key.Replace("-", ""));

            try
            {
                if (!File.Exists("TreskelSoft.lic"))
                {
                    license.error = true;
                    license.errorMessage = "Archivo de licencia no encontrado.";
                }

                string textoEncriptado = File.ReadAllText("TreskelSoft.lic");
                string textoLimpio = crypt.Dencriptar(textoEncriptado);

                license = Newtonsoft.Json.JsonConvert.DeserializeObject<LicenseInfo>(textoLimpio);

                string ClaveActivacion = FormatKeyWithDashes(GeneraHexHash(textoEncriptado));

                license.LicenseKey = ClaveActivacion;
            }
            catch(Exception ex)
            {
                
            }

            return license;
        }

        public LicenseInfo validar(string key)
        {
            LicenseInfo license = new LicenseInfo();
            CryptUtil crypt = new CryptUtil(key.Replace("-",""));

            try
            {
                if (!File.Exists("TreskelSoft.lic"))
                {
                    license.error = true;
                    license.errorMessage = "Archivo de licencia no encontrado.";
                    return license;
                }

                string textoEncriptado = File.ReadAllText("TreskelSoft.lic");
                string textoLimpio = crypt.Dencriptar(textoEncriptado);

                license = Newtonsoft.Json.JsonConvert.DeserializeObject<LicenseInfo>(textoLimpio);

                if (key != license.ProductCode)
                {
                    license.error = true;
                    license.errorMessage = "Clave del producto con coincide";
                    return license;
                }

                if (license.ExpirationDate != null && license.ExpirationDate == DateTime.MinValue.ToString())
                {
                    license.error = true;
                    license.errorMessage = "Fecha de expiración inválida.";
                    return license;
                }

                if (license.ExpirationDate != null && DateTime.Parse(license.ExpirationDate) <  DateTime.Now)
                {
                    license.error = true;
                    license.errorMessage = "Su periodo de prueba o de licenciamiento finalizó.";
                    return license;
                }
                
                return license;
            }
            catch (Exception ex)
            {
                license.error = true;
                license.errorMessage = ex.Message;
                return license;
            }
        }

        private string GeneraHexHash(string encriptado)
        {
            byte[] inputBytes;
            inputBytes = Convert.FromBase64String(encriptado);

            byte[] hashBytes;
            using (SHA256 sha256 = SHA256.Create())
            {
                hashBytes = sha256.ComputeHash(inputBytes);
            }

            string hexHash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();

            return hexHash;
        }

        public string GenerateFormattedKeyFromBase64(string base64Input)
        {
            // 1. Decodificar la cadena Base64 a bytes
            byte[] inputBytes;
            try
            {
                inputBytes = Convert.FromBase64String(base64Input);
            }
            catch (FormatException)
            {
                return "Error: Formato Base64 inválido.";
            }

            // 2. Calcular un hash (SHA256) de los bytes para obtener una salida de longitud fija
            byte[] hashBytes;
            using (SHA256 sha256 = SHA256.Create())
            {
                hashBytes = sha256.ComputeHash(inputBytes);
            }

            // 3. Convertir los bytes del hash a una cadena hexadecimal
            // Obtenemos 64 caracteres hexadecimales de longitud
            string hexHash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();

            // 4. Tomar los primeros 30 caracteres (necesitamos 6 grupos de 5)
            string rawKey = hexHash.Substring(0, 30);

            // 5. Formatear la cadena bruta con guiones
            return FormatKeyWithDashes(rawKey);
        }

        private string FormatKeyWithDashes(string rawKey)
        {
            if (rawKey.Length != 64)
            {
                throw new ArgumentException("La clave bruta debe tener exactamente 64 caracteres para este formato.", nameof(rawKey));
            }

            var sb = new StringBuilder();
            int[] groupLengths = { 8, 8, 8, 8, 8, 8, 8, 8 };
            int currentIndex = 0;

            foreach (int length in groupLengths)
            {
                sb.Append(rawKey.Substring(currentIndex, length));
                currentIndex += length;

                if (currentIndex < rawKey.Length)
                {
                    sb.Append('-');
                }
            }

            return sb.ToString();
        }
    }
}
