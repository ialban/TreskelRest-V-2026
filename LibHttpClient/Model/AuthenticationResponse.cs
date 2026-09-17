namespace LibHttpClient.Model
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string Ruc { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Token { get; set; }
        public string environment { get; set; }
    }
}
