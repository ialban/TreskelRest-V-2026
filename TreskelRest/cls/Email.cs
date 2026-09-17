using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TreskelRest.cls
{
    public class Email : IDisposable
    {
        public String host { get; set; } = "";
        public String port { get; set; } = "";
        public String user { get; set; } = "";
        public String pswd { get; set; } = "";
        public String auth { get; set; } = "";
        public String utls { get; set; } = "";
        public String sucu { get; set; } = "";

        private SmtpClient smtpServer = new SmtpClient();

        public String subject { get; set; } = "";
        public String body { get; set; } = "";

        public Email(String host, String port, String user, String passwd, String auth, String utls, String sucu)
        {
            this.host = host;
            this.user = user;
            this.port = port;
            this.pswd = passwd;
            this.auth = auth;
            this.utls = utls;
            this.sucu = sucu;

            prepareServer();
        }

        private void prepareServer()
        {
            smtpServer.Port = int.Parse(this.port);
            smtpServer.Host = this.host;
            smtpServer.Credentials = new NetworkCredential(this.user, this.pswd);
            smtpServer.EnableSsl = this.utls.Equals("1") ? true : false;
            //smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smtpServer.UseDefaultCredentials = false;
        }

        public void solicitarLicencia()
        {
            try
            {
                if (PcNetworkInfo.IsConnectedToInternet())
                {
                    MailMessage mail = new MailMessage();

                    if (!host.Equals("") && !user.Equals(""))
                    {
                        mail.From = new MailAddress(user);
                        mail.To.Add("treskelsoft@gmail.com");
                        mail.CC.Add("ignacio.alban@gmail.com");

                        mail.Subject = subject;
                        mail.Body = body;
                        mail.IsBodyHtml = true;

                        smtpServer.Send(mail);
                    }
                }
            }
            catch (System.Net.Mail.SmtpException e)
            {
                String msg = e.Message;
            }
        }

        public void enviar()
        {
            try
            {
                MailMessage mail = new MailMessage();

                if (PcNetworkInfo.InternetConnectionStatus())
                {
                    if (!host.Equals("") && !user.Equals(""))
                    {
                        mail.From = new MailAddress(user);
                        if (General.CORREO1 != "")
                        {
                            mail.To.Add(General.CORREO1);
                        }
                        else
                        {
                            mail.To.Add(user);
                        }

                        if (General.COPIAR_CORREOS)
                        {
                            if (General.CORREO2 != "") mail.CC.Add(General.CORREO2);
                            if (General.CORREO3 != "") mail.CC.Add(General.CORREO3);
                            if (General.CORREO4 != "") mail.CC.Add(General.CORREO4);
                        }


                        mail.Subject = subject;
                        body = body.Replace("[Sucursal]", " [ " + sucu + " ]");
                        mail.Body = body;
                        mail.IsBodyHtml = true;

                        smtpServer.Send(mail);
                    }
                }
            }
            catch (System.Net.Mail.SmtpException e)
            {
                String msg = e.Message;
                ObjComportamiento.showMsgError(msg);
            }
            catch(Exception e)
            {
                string msg = e.Message;
                ObjComportamiento.showMsgError(msg);
            }            
        }

        public void enviar(string mensaje, string correo1, string copia = "")
        {
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(user);
                mail.To.Add(correo1);

                if (copia != "") mail.CC.Add(copia);

                mail.Subject = subject;
                body = mensaje;
                mail.Body = body;
                mail.IsBodyHtml = false;
                

                smtpServer.Send(mail);
            }
            catch (System.Net.Mail.SmtpException e)
            {
                String msg = e.Message;
                ObjComportamiento.showMsgError(msg);
            }
            catch (Exception e)
            {
                string msg = e.Message;
                ObjComportamiento.showMsgError(msg);
            }
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
        // ~Email() {
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
