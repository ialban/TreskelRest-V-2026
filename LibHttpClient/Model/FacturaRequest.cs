namespace LibHttpClient.Model
{
    public class FacturaRequest
    {
        public string ventslc { get; set; }
        public byte[] pdf { get; set; }
        public byte[] xml { get; set; }
        public string estado { get; set; }
        public string estadoMail { get; set; }
        public string mensaje { get; set; }
        public string tipo { get; set; }
    }
}
