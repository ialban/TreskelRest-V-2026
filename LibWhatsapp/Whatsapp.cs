using System;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace LibWhatsapp
{
    public enum ServiciosWhatsApp
    {
        Twilio = 1,

    }

    public class Whatsapp
    {
        public string accountSid { get; set; }
        public string authToken { get; set; }
        public string origenPN { get; set; } = "+14155238886";
        public string destinoPN { get; set; } // +593984990123
        public ServiciosWhatsApp servicio { get; set; } = ServiciosWhatsApp.Twilio;

        public Whatsapp()
        {

        }

        public void Send(string mensaje)
        {
            switch (servicio)
            {
                case ServiciosWhatsApp.Twilio:
                    SendFromTwilio(mensaje);
                    break;
            }
        }

        private void SendFromTwilio(string mensaje)
        {
            try
            {
                TwilioClient.Init(accountSid, authToken);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                

                var messageOptions = new CreateMessageOptions(
                  new PhoneNumber("whatsapp:" + destinoPN));
                messageOptions.From = new PhoneNumber("whatsapp:" + origenPN);
                messageOptions.Body = mensaje;


                var message = MessageResource.Create(messageOptions);
            }
            catch(Exception e)
            {

            }            
        }
    }
}
