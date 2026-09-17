namespace Entidades.modelos
{
    public class mBancos
    {
        public int bcosnid { get; set; }
        public string bcosnbr { get; set; }
        public string bcosacr { get; set; }
        public string bcosnct { get; set; }
        public string bcostip { get; set; }
        public string bcosuso { get; set; }
        public bool bcospri { get; set; }
        public bool bcosatv { get; set; }
    }

    public class lBancos
    {
        public int codigo { get; set; }
        public string nombre { get; set; }
        public string cuenta { get; set; }
        public string acronimo { get; set; }
        public string tipo { get; set; }
        public string uso { get; set; }
        public string estado { get; set; }
        public string imagenUso { get; set; }
    }
}
