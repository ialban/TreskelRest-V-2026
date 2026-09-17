namespace LibHttpClient.Model
{
    public class ResultadoFacturacion
    {
        public long id { get; set; }
        public string empresaid { get; set; }
        public string ventasec { get; set; }
        public byte[] xmlfile { get; set; }
        public byte[] pdffile { get; set; }
        public string estado { get; set; }
        public string estadomail { get; set; }
        public string error { get; set; }
        public string mensaje { get; set; }
    }
}
