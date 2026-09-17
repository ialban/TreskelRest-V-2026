namespace LibHttpClient.Model
{
    public class ResultadoNotaCredito
    {
        public long id { get; set; }
        public string empresaid { get; set; }
        public string ntcrsec { get; set; }
        public byte[] xmlfile { get; set; }
        public byte[] pdffile { get; set; }
        public string estado { get; set; }
        public string estadomail { get; set; }
        public string error { get; set; }
        public string mensaje { get; set; }
    }
}
