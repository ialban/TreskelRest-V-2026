using LibElectSign.model;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

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
                int resultado = await EjecutarTreskelSign(xmlPath, firmaPath, CertificadoClave, environment);
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
                }
                
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
    }    
}
