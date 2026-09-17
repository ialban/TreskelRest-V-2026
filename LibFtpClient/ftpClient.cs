using Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;
using System.Security.Authentication;
using System.Net.Security;

namespace LibFtpClient
{
    public class ftpClient
    {
        public string server { get; set; } = "";
        public string user { get; set; } = "";
        public string clave { get; set; } = "";
        public string empresa { get; set; } = "";

        public int puerto { get; set; } = 21;

        public ftpClient()
        {

        }

        public ftpClient(string server, string user, string clave)
        {
            this.server = server;
            this.user = user;
            this.clave = clave;
        }

        public ftpClient(string server, string user, string clave, int puerto)
        {
            this.server = server;
            this.user = user;
            this.clave = clave;
            this.puerto = puerto;
        }

        public bool DirectoryExists(FtpWebRequest request, string directorio)
        {
            try
            {
                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
        }

        public Respuesta Upload(string file, string directorio)
        {
            Respuesta respuesta = new Respuesta {
                resultado = false,
                mensaje = ""
            };

            FtpClient ftpClient = new FtpClient(this.server, this.user, this.clave, this.puerto);
            try
            {
                FileInfo fileInfo = new FileInfo(file);
                FtpProfile ftpProfile = new FtpProfile()
                {
                    Protocols = SslProtocols.Tls,
                    Timeout = 0,
                    RetryAttempts = 3,
                    Encoding = Encoding.UTF8,
                    Host = this.server,
                    Encryption = FtpEncryptionMode.Auto,
                    Credentials = new NetworkCredential()
                };

                ftpProfile.Credentials.UserName = this.user;
                ftpProfile.Credentials.Password = this.clave;
                //ftpClient.ValidateAnyCertificate = true;
                ftpClient.ValidateCertificate += (control, e) => {
                    e.Accept = true;
                };
                ftpClient.AutoConnect();

                if (!ftpClient.DirectoryExists("/BkpDB/" + directorio))
                    ftpClient.CreateDirectory("/BkpDB/" + directorio);

                int num = (int)ftpClient.UploadFile(file, $"/BkpDB/{directorio}/{fileInfo.Name}", FtpRemoteExists.Overwrite, false, FtpVerify.None, (Action<FtpProgress>)null);
                respuesta.resultado = true;
            }
            catch(Exception e)
            {
                respuesta.resultado = false;
                respuesta.mensaje = e.Message;
            }

            return respuesta;
        }
    }
}
