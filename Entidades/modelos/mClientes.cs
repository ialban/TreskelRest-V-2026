namespace Entidades.modelos
{
    public class mClientes
    {
        public string nidNid { get; set; }
        public string nidTip { get; set; } = "C";
        public string nidNbr { get; set; } = "";
        public string nidDir { get; set; } = "";
        public string nidTlf { get; set; } = "";
        public string nidEml { get; set; } = "";
        public string nidStdo { get; set; } = "A";
        //----------------------
        public bool cliecdto { get; set; } = false;
        public string nidEmpr { get; set; }
        public string emprNbr { get; set; }
        public bool emprPaga { get; set; } = false;
    }
}
