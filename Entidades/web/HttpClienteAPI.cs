using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Entidades.web
{
    public class HttpClienteAPI
    {
        public string TrsklUrl { get; set; }

        public string Token { get; set; }

        public async Task<FacturaRequest> sendFactura(FacturaModel values)
        {
            try
            {
                var response = await TrsklUrl
                .AppendPathSegment("api/usuario")
                .AppendPathSegment("facturaelectronica")
                .WithOAuthBearerToken(Token)
                .PostJsonAsync(
                    values
                )
                .ReceiveJson<StandardJsonResponse>();

                if (response != null)
                {
                    return JsonConvert.DeserializeObject<FacturaRequest>(response.data.ToString());
                }
            }
            catch(Exception e)
            {

            }

            return null;
        }

    }
}
