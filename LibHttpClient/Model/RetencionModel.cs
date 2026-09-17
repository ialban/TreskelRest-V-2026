namespace LibHttpClient.Model
{
    public class RetencionModel
    {
        public long? id { get; set; }
        public string retesec { get; set; }
        public string claveAcceso { get; set; }
        public string ruc { get; set; }
        public string empresaid { get; set; }
        public string serial { get; set; }
        public string codigoLocal { get; set; }
        public string nombreLocal { get; set; }
        public string direccionLocal { get; set; }
        public string idProveedor { get; set; }
        public string nombreProveedor { get; set; }
        public string emailProveedor { get; set; }
        public string data { get; set; }
        public byte[] bData { get; set; }
        public string pcName { get; set; }
        public string ipAddress { get; set; }
        public string claveDownload { get; set; }
    }
}
