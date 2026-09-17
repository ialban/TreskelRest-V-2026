using Flurl;
using Flurl.Http;
using LibHttpClient.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LibHttpClient
{
    public class HttpClienteAPI
    {
        public string TrsklUrl { get; set; }

        public string Token { get; set; }
        public string PathDownload { get; set; }

        public async Task<AuthenticationResponse> Login(string usuario, string clave, string ruc, string empresaGuid)
        {
            try
            {
                var response = await TrsklUrl
                .AppendPathSegment("usuario")
                .AppendPathSegment("login")
                .PostJsonAsync(
                    new AuthenticationRequest
                    {
                        Ruc = ruc,
                        EmpresaId = empresaGuid,
                        Usuario = usuario,
                        Clave = clave
                    }
                )
                .ReceiveJson<AuthenticationResponse>();

                if (response != null)
                {
                    return response;
                }
                else
                {
                    return new AuthenticationResponse
                    {
                        Id = 0,
                        Nombre = "",
                        Ruc = "",
                        Token = "",
                        Usuario = ""
                    };
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<FacturaRequest> sendFactura(FacturaModel values)
        {
            FacturaRequest respuesta = new FacturaRequest();

            try
            {
                var response = await TrsklUrl
                .AppendPathSegment("facturacion")
                .AppendPathSegment("declaracion")
                .SetQueryParam("token", $"Bearer {Token}")
                .WithTimeout(20)
                .PostJsonAsync(
                    values
                )
                .ReceiveJson<ComprobanteResponse>();

                if (response != null)
                {
                    if (response.Estado == "AUTORIZADO")
                    {
                        Archivos archivos = (Archivos)response.archivos;

                        respuesta.estado = "AUTORIZADO";
                        respuesta.estadoMail = !response.EstadoMail ? "NO ENVIADO" : "CORREO ENVIADO";
                        respuesta.tipo = response.Tipo;
                        respuesta.mensaje = response.Mensaje;
                    }
                    else
                    {
                        if (response.Estado == "D") respuesta.estado = "DEVUELTA";
                        if (response.Estado == "P") respuesta.estado = "PENDIENTE";
                        if (response.Estado == "N") respuesta.estado = "NO PROCESADO";
                        if (response.Estado == "v") respuesta.estado = "VALIDADO";

                        respuesta.estadoMail = "NO ENVIADO/NO EXISTE";
                        respuesta.tipo = response.Tipo;
                        respuesta.mensaje = response.Mensaje;
                    }
                }
                else
                {

                    respuesta.estado = "NO PROCESADO";
                    respuesta.estadoMail = "NO ENVIADO/NO EXISTE";
                    respuesta.tipo = "ERROR";
                    respuesta.mensaje = "HUBO UN PROBLEMA NO SE PROCESÓ LA FACTURA";
                }
            }
            catch (Exception e)
            {
                respuesta.estado = "NO PROCESADO";
                respuesta.estadoMail = "NO ENVIADO/NO EXISTE";
                respuesta.tipo = "ERROR";
                respuesta.mensaje = "HUBO UN PROBLEMA NO SE PROCESÓ LA FACTURA";
            }

            return respuesta;
        }

        public async Task sendMail(string Token, List<string> destino, string titulo, string mensaje)
        {
            OpcionesMail opcMail = new OpcionesMail
            {
                destino = destino,
                titulo = titulo, 
                mensaje = mensaje
            };

            try
            {
                var response = await TrsklUrl
                .AppendPathSegment("mensajes")
                .AppendPathSegment("sendmail")
                .SetQueryParam("token", Token)
                .WithTimeout(10)
                .PostJsonAsync(
                    opcMail
                );
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }   
        }

        public async Task sendWhatsapp(string token)
        {
            OpcionesMail opcMail = new OpcionesMail();
            try
            {
                var response = await TrsklUrl
                .AppendPathSegment("mensajes")
                .AppendPathSegment("whatsapp")
                .SetQueryParam("token", token)
                .WithTimeout(10)
                .PostJsonAsync(
                    opcMail
                );
            }
            catch (FlurlHttpException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }

        public async Task<string> downloadFactura(string ruc, string empresaid, string autorizacion, string ventscl, string secuencial, string tipo)
        {
            string motivo = "OK";

            if (Directory.Exists(PathDownload) == false)
            {
                Directory.CreateDirectory(PathDownload);
            }

            FacturaModel values = new FacturaModel { 
                ruc = ruc, empresaid = empresaid,claveAcceso=autorizacion, ventasec = ventscl
            };

            try
            {
                using var response = await TrsklUrl
                .AppendPathSegment("documentos")
                .AppendPathSegment("descargar")
                .AppendPathSegment("Factura")
                .WithTimeout(30)
                .PostJsonAsync(
                    values
                );

                await using var stream = await response.GetStreamAsync();

                string nombreArchivo = (tipo == "Factura" ? autorizacion :
                                       (tipo == "Retencion" ? "RET-" + autorizacion :
                                       (tipo == "NotaCredito" ? "NCR-" + autorizacion :
                                       "NDB-" + autorizacion))) + ".pdf";

                using var file = File.Create(Path.Combine(PathDownload, nombreArchivo));

                await stream.CopyToAsync(file);
            }
            catch (FlurlHttpException e)
            {
                try
                {
                    motivo = await e.GetResponseStringAsync();

                    switch (motivo)
                    {
                        case "ArchivosNoEncontrados":
                        case "NoEncontrado":
                            motivo = "Archivo no encontrado";
                            break;
                        case "TipoNoValido":
                            motivo = "Tipo de Documento no encontrado";
                            break;
                        case "FacTransaccionNoEncontrado":
                            motivo = "Transacción de facturación no encontrada";
                            break;
                        case "ReteTransaccionNoEncontrado":
                            motivo = "Transaccion de retención no encontrada";
                            break;
                        case "NCredTransaccionNoEncontrado":
                            motivo = "Transaccion de nota de crédito no encontrada";
                            break;
                        case "EmpresaNoValida":
                            motivo = "Datos de la empresa no válidos";
                            break;
                        default:
                            motivo = "Hubo un error al conectar al API " + TrsklUrl + ", revise su conexión de Internet";
                            break;
                    }
                }
                catch(Exception ex)
                {

                }   
            }
            catch (Exception e)
            {
                motivo = "ErrorHttp";
            }

            return motivo;
        }

        public async Task<RetencionRequest> sendRetencion(RetencionModel values)
        {
            RetencionRequest respuesta = new RetencionRequest();

            try
            {
                var response = await TrsklUrl
                .AppendPathSegment("retencion")
                .AppendPathSegment("declaracion")
                .SetQueryParam("token", $"Bearer {Token}")
                .WithTimeout(10)
                .PostJsonAsync(
                    values
                )
                .ReceiveJson<ComprobanteResponse>();

                if (response != null)
                {
                    if (response.Estado == "A")
                    {
                        respuesta.estado = "AUTORIZADO";
                        respuesta.estadoMail = !response.EstadoMail ? "NO ENVIADO" : "CORREO ENVIADO";
                        respuesta.tipo = response.Tipo;
                        respuesta.mensaje = response.Mensaje;
                    }
                    else
                    {
                        if (response.Estado == "D") respuesta.estado = "DEVUELTA";
                        if (response.Estado == "P") respuesta.estado = "PENDIENTE";
                        if (response.Estado == "N") respuesta.estado = "NO PROCESADO";
                        if (response.Estado == "V") respuesta.estado = "VALIDADO";

                        respuesta.estadoMail = "NO ENVIADO/NO EXISTE";
                        respuesta.tipo = response.Tipo;
                        respuesta.mensaje = response.Mensaje;
                    }
                }
                else
                {

                    respuesta.estado = "NO PROCESADO";
                    respuesta.estadoMail = "NO ENVIADO/NO EXISTE";
                    respuesta.tipo = "ERROR";
                    respuesta.mensaje = "HUBO UN PROBLEMA NO SE PROCESÓ LA FACTURA";
                }
            }
            catch (Exception e)
            {
                // Lanzar error 
                respuesta.estado = "NO PROCESADO";
                respuesta.estadoMail = "NO ENVIADO/NO EXISTE";
                respuesta.tipo = "ERROR";
                respuesta.mensaje = "HUBO UN PROBLEMA NO SE PROCESÓ LA FACTURA";
            }

            return respuesta;
        }

        public async Task<ComprobanteResponse> obtieneFactura(FacturaModel values)
        {
            FacturaModel tempo = new FacturaModel();
            tempo.claveAcceso = values.claveAcceso;
            tempo.id = values.id;
            tempo.empresaid = values.empresaid;
            tempo.ventasec = values.ventasec;
            tempo.serial = values.serial;
            tempo.ruc = values.ruc;

            ComprobanteResponse respuesta = new ComprobanteResponse();
            int count = 0;
            Thread.Sleep(3000);

            ResultadoFacturacion resultado = new ResultadoFacturacion();
            resultado.estado = "N";
            resultado.estadomail = "N";
            resultado.ventasec = values.ventasec;
            resultado.empresaid = values.empresaid;
            resultado.error = "";

            while (count <= 30)
            {
                try
                {
                    var response = await TrsklUrl
                    .AppendPathSegment("facturacion")
                    .AppendPathSegment("verificacion")
                    .SetQueryParam("token", $"Bearer {Token}")
                    .WithTimeout(20)
                    .PostJsonAsync(
                        tempo
                    )
                    .ReceiveJson<ComprobanteResponse>();

                    if (response != null)
                    {
                        if(response.Estado != null)
                        {
                            resultado.estado = response.Estado;
                            resultado.estadomail = response.EstadoMail == true ? "A" : "N";
                            resultado.error = response.Estado != "A" ? "Error" : response.Estado;
                            resultado.ventasec = response.Secuencial;
                            resultado.mensaje = response.Mensaje;

                            break;
                        } 
                    }

                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
                Thread.Sleep(1000);
                count++;
            }

            if (resultado != null)
            {
                respuesta.Secuencial = resultado.ventasec;
                respuesta.Estado = resultado.estado;
                respuesta.EstadoMail = resultado.estadomail == "A" ? true: false;
                respuesta.Mensaje = resultado.mensaje;
            }
            else
            {
                respuesta.Estado = "NO PROCESADO";
                respuesta.EstadoMail = resultado.estadomail == "A" ? true : false;
                respuesta.Tipo = "ERROR";
                respuesta.Mensaje = "";
            }

            return respuesta;
        }

        public async Task<RetencionRequest> obtieneRetencion(RetencionModel values)
        {
            RetencionModel tempo = new RetencionModel();
            tempo.claveAcceso = values.claveAcceso;
            tempo.id = values.id;
            tempo.empresaid = values.empresaid;
            tempo.retesec = values.retesec;
            tempo.serial = values.serial;
            tempo.ruc = values.ruc;

            RetencionRequest respuesta = new RetencionRequest();
            int count = 0;
            Thread.Sleep(3000);

            ResultadoRetencion resultado = new ResultadoRetencion();
            resultado.estado = "N";
            resultado.estadomail = "N";
            resultado.retesec = values.retesec;
            resultado.empresaid = values.empresaid;
            resultado.error = "";

            while (count <= 30)
            {
                try
                {
                    var response = await TrsklUrl
                    .AppendPathSegment("retencion")
                    .AppendPathSegment("verificacion")
                    .SetQueryParam("token", $"Bearer {Token}")
                    .WithTimeout(10)
                    .PostJsonAsync(
                        tempo
                    )
                    .ReceiveJson<ResultadoRetencion>();

                    if (response != null)
                    {
                        if (response.estado != null)
                        {
                            resultado = response;
                            break;
                        }
                    }

                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
                Thread.Sleep(1000);
                count++;
            }

            if (resultado != null)
            {
                respuesta.reteslc = resultado.retesec;
                respuesta.estado = resultado.estado;
                respuesta.estadoMail = resultado.estadomail;
                respuesta.mensaje = resultado.mensaje;
                respuesta.pdf = resultado.pdffile;
                respuesta.xml = resultado.xmlfile;
            }
            else
            {
                respuesta.estado = "NO PROCESADO";
                respuesta.estadoMail = "NO ENVIADO/NO EXISTE";
                respuesta.tipo = "ERROR";
                respuesta.mensaje = "";
            }

            return respuesta;
        }

        public async Task<FacturaRequest> VerificaFactura(FacturaModel values)
        {
            FacturaRequest respuesta = null;
            int count = 0;
            Thread.Sleep(1000);

            ResultadoFacturacion resultado = new ResultadoFacturacion();

            while (count <= 30)
            {
                try
                {
                    var response = await TrsklUrl
                    .AppendPathSegment("facturacion")
                    .AppendPathSegment("verificacion")
                    .SetQueryParam("token", $"Bearer {Token}")
                    .WithTimeout(10)
                    .PostJsonAsync(
                        values
                    )
                    .ReceiveJson<ResultadoFacturacion>();

                    if (response != null)
                    {
                        resultado = response;
                        break;
                    }

                }
                catch (Exception err)
                {

                }
                Thread.Sleep(1000);
                count++;
            }

            if (resultado != null)
            {
                respuesta = new FacturaRequest();

                if (resultado.empresaid != null)
                {
                    respuesta.ventslc = resultado.ventasec;
                    respuesta.estado = resultado.estado;
                    respuesta.estadoMail = resultado.estadomail;
                    respuesta.mensaje = resultado.mensaje;
                    respuesta.pdf = resultado.pdffile;
                    respuesta.xml = resultado.xmlfile;
                }
                
            }

            return respuesta;
        }

        public async Task<RetencionRequest> VerificaRetencion(RetencionModel values)
        {
            RetencionRequest respuesta = null;
            int count = 0;
            Thread.Sleep(1000);

            ResultadoRetencion resultado = new ResultadoRetencion();

            while (count <= 30)
            {
                try
                {
                    var response = await TrsklUrl
                    .AppendPathSegment("retencion")
                    .AppendPathSegment("verificacion")
                    .SetQueryParam("token", $"Bearer {Token}")
                    .WithTimeout(10)
                    .PostJsonAsync(
                        values
                    )
                    .ReceiveJson<ResultadoRetencion>();

                    if (response != null)
                    {
                        resultado = response;
                        break;
                    }

                }
                catch (Exception err)
                {

                }
                Thread.Sleep(1000);
                count++;
            }

            if (resultado != null)
            {
                respuesta = new RetencionRequest();

                if (resultado.empresaid != null)
                {
                    respuesta.reteslc = resultado.retesec;
                    respuesta.estado = resultado.estado;
                    respuesta.estadoMail = resultado.estadomail;
                    respuesta.mensaje = resultado.mensaje;
                    respuesta.pdf = resultado.pdffile;
                    respuesta.xml = resultado.xmlfile;
                }

            }

            return respuesta;
        }

        public async Task<NotaCreditoRequest> VerificaNotaCredito(NotaCreditoModel values)
        {
            NotaCreditoRequest respuesta = null;
            int count = 0;
            Thread.Sleep(1000);

            ResultadoNotaCredito resultado = new ResultadoNotaCredito();

            while (count <= 30)
            {
                try
                {
                    var response = await TrsklUrl
                    .AppendPathSegment("notacredito")
                    .AppendPathSegment("verificacion")
                    .SetQueryParam("token", $"Bearer {Token}")
                    .WithTimeout(10)
                    .PostJsonAsync(
                        values
                    )
                    .ReceiveJson<ResultadoNotaCredito>();

                    if (response != null)
                    {
                        resultado = response;
                        break;
                    }

                }
                catch (Exception err)
                {

                }
                Thread.Sleep(1000);
                count++;
            }

            if (resultado != null)
            {
                respuesta = new NotaCreditoRequest();

                if (resultado.empresaid != null)
                {
                    respuesta.ntcrslc = resultado.ntcrsec;
                    respuesta.estado = resultado.estado;
                    respuesta.estadoMail = resultado.estadomail;
                    respuesta.mensaje = resultado.mensaje;
                    respuesta.pdf = resultado.pdffile;
                    respuesta.xml = resultado.xmlfile;
                }

            }

            return respuesta;
        }

        public async Task<NotaCreditoRequest> sendNotaCredito(NotaCreditoModel values)
        {
            NotaCreditoRequest respuesta = new NotaCreditoRequest();

            try
            {
                var response = await TrsklUrl
                .AppendPathSegment("notacredito")
                .AppendPathSegment("declaracion")
                .SetQueryParam("token", $"Bearer {Token}")
                .WithTimeout(10)
                .PostJsonAsync(
                    values
                )
                .ReceiveJson<ComprobanteResponse>();

                if (response != null)
                {
                    if (response.Estado == "A")
                    {
                        respuesta.estado = "AUTORIZADO";
                        respuesta.estadoMail = !response.EstadoMail ? "NO ENVIADO" : "CORREO ENVIADO";
                        respuesta.tipo = response.Tipo;
                        respuesta.mensaje = response.Mensaje;
                    }
                    else
                    {
                        if (response.Estado == "D") respuesta.estado = "DEVUELTA";
                        if (response.Estado == "P") respuesta.estado = "PENDIENTE";
                        if (response.Estado == "N") respuesta.estado = "NO PROCESADO";
                        if (response.Estado == "V") respuesta.estado = "VALIDADO";

                        respuesta.estadoMail = "NO ENVIADO/NO EXISTE";
                        respuesta.tipo = response.Tipo;
                        respuesta.mensaje = response.Mensaje;
                    }
                }
                else
                {
                    string errorMsg = "Hubo un error al conectar al API en " + TrsklUrl + ", revise su conexión a Internet";

                    throw new Exception(errorMsg);
                }
            }
            catch (Exception e)
            {

            }

            return respuesta;
        }

        public async Task<NotaCreditoRequest> obtieneNotaCredito(NotaCreditoModel values)
        {
            NotaCreditoModel tempo = new NotaCreditoModel();
            tempo.claveAcceso = values.claveAcceso;
            tempo.id = values.id;
            tempo.empresaid = values.empresaid;
            tempo.ntcrsec = values.ntcrsec;
            tempo.serial = values.serial;
            tempo.ruc = values.ruc;

            NotaCreditoRequest respuesta = new NotaCreditoRequest();
            int count = 0;
            Thread.Sleep(3000);

            ResultadoNotaCredito resultado = new ResultadoNotaCredito();
            resultado.estado = "N";
            resultado.estadomail = "N";
            resultado.ntcrsec = values.ntcrsec;
            resultado.empresaid = values.empresaid;
            resultado.error = "";

            while (count <= 30)
            {
                try
                {
                    var response = await TrsklUrl
                    .AppendPathSegment("api/notacredito")
                    .AppendPathSegment("verificacion")
                    .SetQueryParam("token", $"Bearer {Token}")
                    .WithTimeout(10)
                    .PostJsonAsync(
                        tempo
                    )
                    .ReceiveJson<ResultadoNotaCredito>();

                    if (response != null)
                    {
                        if (response.estado != null)
                        {
                            resultado = response;
                            break;
                        }
                    }

                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
                Thread.Sleep(1000);
                count++;
            }

            if (resultado != null)
            {
                respuesta.ntcrslc = resultado.ntcrsec;
                respuesta.estado = resultado.estado;
                respuesta.estadoMail = resultado.estadomail;
                respuesta.mensaje = resultado.mensaje;
                respuesta.pdf = resultado.pdffile;
                respuesta.xml = resultado.xmlfile;
            }
            else
            {
                respuesta.estado = "NO PROCESADO";
                respuesta.estadoMail = "NO ENVIADO/NO EXISTE";
                respuesta.tipo = "ERROR";
                respuesta.mensaje = "";
            }

            return respuesta;
        }

        public async Task sendLogoFactura(UsuarioEmpresa payload)
        {
            try
            {
                var response = await TrsklUrl
                .AppendPathSegment("usuario")
                .AppendPathSegment("actualiza")
                .SetQueryParam("token", $"Bearer {Token}")
                .WithTimeout(15)
                .PostJsonAsync(
                    payload
                )
                .ReceiveJson<StandardJsonResponse>();
            }
            catch (Exception e)
            {

            }
        }
    }
}
