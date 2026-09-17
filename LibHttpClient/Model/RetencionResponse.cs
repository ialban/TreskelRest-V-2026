namespace LibHttpClient.Model
{
    public class RetencionResponse
    {
        public EstadoResponse Estado { get; set; } // NoProcesado, Pendiente, Devuelta, Validado, Autorizado
        public EstadoEmail EstadoMail { get; set; } //CorreoNoExiste, Enviado, CorreoInvalido
        public string Tipo { get; set; }
        public string Mensaje { get; set; }
        public object archivos { get; set; }
    }   
}
