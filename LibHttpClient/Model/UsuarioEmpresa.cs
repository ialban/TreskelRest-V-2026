namespace LibHttpClient.Model
{
    public class UsuarioEmpresa
    {
        public string Ruc { get; set; }
        public string EmpresaId { get; set; } // GUID
        public string Nombre { get; set; }
        public bool Bloqueado { get; set; }
        public bool Activo { get; set; }
        public string Environment { get; set; }
        public byte[] logotipo { get; set; }
    }
}
