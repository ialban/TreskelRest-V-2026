using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    public class Elastic
    {
        public List<Recipient> Recipients;
        public Content content;
    }

    public class Content
    {
        public List<Body> body;
        public string EnvelopeFrom { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
    }

    public class Body
    {
        public string ContentType { get; set; } = "HTML"; // PlaintText
        public string Content = "";
        public string Charset = "";
    }

    public class Recipient
    {
        public string email { get; set; } = "";

    }

    /// <summary>
    /// Lógica de interacción para ctlTestEmail.xaml
    /// </summary>
    public partial class ctlTestEmail : UserControl
    {
        public ctlTestEmail()
        {
            InitializeComponent();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            string host = txtServer.Text;
            string puerto = txtPuerto.Text;
            string usuario = txtUsuario.Text;
            string clave = txtClave.Password;
            string utls = chkTls.IsChecked == true ? "1" : "0";
            string mensaje = txtMensaje.Text;
            string origen = txtCorreo1.Text;

            Email cls = new Email(host, puerto, usuario, clave, "", utls, "");

            cls.enviar(mensaje, origen);
        }

        private void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Elastic elastic = new Elastic
                {
                    Recipients = new List<Recipient>
                {
                    new Recipient
                    {
                        email = txtCorreo1.Text
                    },
                    new Recipient
                    {
                        email = "nelson.mejiap@gmail.com"
                    }
                },
                    content = new Content
                    {
                        body = new List<Body>
                    {
                        new Body
                        {
                            Content = txtMensaje.Text,
                            ContentType = "PlainText"
                        }
                    },
                        From = txtUsuario.Text,
                        Subject = "Prueba API"
                    }
                };

                string token = "8AF89E84F640357015B69392EAEAD0AD5240EFCC7600BD48559C5D5427956A01620732A7586C8D1139B66CBE258948BA";

                //var client = new RestClient(txtServer.Text)
                //{
                //    ThrowOnAnyError = true,
                //    Timeout = 5000
                //};


                //var request = new RestSharp.RestRequest();

                //request.AddHeader("content-type", "application/json");
                //request.AddHeader("ApiKey", token);

                //string contenido = JsonConvert.SerializeObject(elastic);

                //request.AddParameter("application/json", contenido, ParameterType.RequestBody);


                //var response = client.Post(request);
            }
            catch(Exception ex)
            {
                ObjComportamiento.showMsgError(ex.Message);
            }
            
        }
    }
}
