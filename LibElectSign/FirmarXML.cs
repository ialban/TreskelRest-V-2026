using FirmaXadesNet;
using FirmaXadesNet.Signature.Parameters;
using LibElectSign.model;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xades;

namespace LibElectSign
{
    public class FirmarXML
    {
        public string xmlPath;
        public string CertificadoClave = "";
        public string firmaPath = "";
        public string environment = "P";

        public async Task<SignResponse> SignXMLAsync()
        {
            SignResponse respuesta = new SignResponse
            {
                success = false,
                message = string.Empty
            };

            try
            {
                respuesta = await firmar(xmlPath, firmaPath, CertificadoClave, environment);
                /*int resultado = await EjecutarTreskelSign(xmlPath, firmaPath, CertificadoClave, environment);
                switch (resultado)
                {
                    case 1:
                        respuesta = new SignResponse { 
                            success = true,
                            message = string.Empty
                        };
                        break;
                    case 2:
                        respuesta = new SignResponse
                        {
                            success = false,
                            message = $"No existe el archivo XML en la ruta {xmlPath}"
                        };
                        break;
                    case 3:
                        respuesta = new SignResponse
                        {
                            success = false,
                            message = $"No existe el certificado en la ruta {firmaPath}"
                        };
                        break;
                    case 4:
                        respuesta = new SignResponse
                        {
                            success = false,
                            message = $"No fue posible firmar el archivo XML, revise los log en el EVENT VIEWER"
                        };
                        break;
                }*/

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error en SignXMLAsync: " + e.Message);
            }
            return respuesta;
        }

        private async Task<int> EjecutarTreskelSign(string xml, string firma, string pass, string env)
        {
            string argumentos = $"-e {env} -x {xml.Replace("\\\\","\\")} -f {firma.Replace(" ", "")} -p {pass}";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                // Path en el que se encuentra el archivo de firmado de la factura
                FileName = "C:\\TreskelSign\\TreskelSign.exe",
                // Argumentos que se pasarán al archivo firmador
                Arguments = argumentos,
                // Requerido para capturar salida y evitar shell
                UseShellExecute = false,
                // true = No mostrar la ventana negra de consola, false = Muestra la ventana de consola
                CreateNoWindow = true,
                // Opcional: Para leer mensajes del programa
                RedirectStandardOutput = false,
                // Opcional: Para leer errores de sistema
                RedirectStandardError = false
            };

            try
            {
                return await Task.Run(() =>
                {
                    using (Process proceso = Process.Start(startInfo))
                    {
                        bool terminado = proceso.WaitForExit(10000);

                        if (!terminado)
                        {
                            try
                            {
                                proceso.Kill();
                            }
                            catch { }
                            return -3; // El proceso se colgó o tardó demasiado
                        }

                        return proceso.ExitCode;
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return -1;
            }
        }
    
        private async Task<SignResponse> firmar(string xml, string firma, string pass, string env)
        {
            SignResponse respuesta = new SignResponse { success = false, message = string.Empty };

            try
            {
                // 1. Validaciones previas de archivos existentes
                if (!File.Exists(xml))
                {
                    respuesta.message = $"No existe el archivo XML en la ruta {xml}";
                    return respuesta;
                }
                if (!File.Exists(firma))
                {
                    respuesta.message = $"No existe el certificado en la ruta {firma}";
                    return respuesta;
                }

                // 2. Cargar el certificado de forma segura (.NET 10)
                var certificado = X509CertificateLoader.LoadPkcs12FromFile(firma, pass);

                // 3. Configurar los parámetros exactos exigidos por el SRI
                var parametros = new SignatureParameters
                {
                    SignaturePolicyInfo = null,
                    SignaturePackaging = SignaturePackaging.ENVELOPED,

                    // CORRECCIÓN: Se asigna el URI estándar W3C para SHA-256 como string
                    DigestMethod = FirmaXadesNet.Crypto.DigestMethod.SHA256,
                    Signer = new FirmaXadesNet.Crypto.Signer(certificado)
                };

                var firmador = new XadesService();

                // 4. Procesar el firmado usando streams sin corromper el archivo original
                // Leemos el archivo original en memoria para evitar conflictos de bloqueo de escritura
                byte[] xmlOriginalBytes = File.ReadAllBytes(xml);

                using (MemoryStream msOriginal = new MemoryStream(xmlOriginalBytes))
                using (FileStream fsFirmado = new FileStream(xml, FileMode.Create, FileAccess.Write))
                {
                    // CORRECCIÓN: El método .Sign() procesa el stream de lectura, inyecta los parámetros y escribe en el de salida
                    var documentoFirmado = firmador.Sign(msOriginal, parametros);
                    documentoFirmado.Save(fsFirmado);
                }

                respuesta.success = true;
            }
            catch (Exception ex)
            {
                respuesta.success = false;
                respuesta.message = $"Error en el proceso criptográfico: {ex.Message}";
                System.Diagnostics.Debug.WriteLine("Error en método firmar: " + ex.ToString());
            }

            return respuesta;
        }
    }    
}
