namespace Entidades.modelos
{
    public class mEmpleado
    {
        public string empdNid { get; set; }
        public string empdTip { get; set; }
        public string empdNom { get; set; }
        public string empdApe { get; set; }
        public string empdDir { get; set; } = "";
        public string empdTlf { get; set; } = "";
        public string empdCel { get; set; } = "";
        public string empdFna { get; set; } = null;
        public int empdChj { get; set; } = 0;
        public bool empdAtv { get; set; } = true;
    }

    public class msEmpleado
    {
        public string codigo { get; set; }
        public string tipo { get; set; }
        public string empleado { get; set; }
        public string telefono { get; set; }
        public string estado { get; set; }
    }

    public class mEmpleadoSalario
    {
        public string codigo { get; set; }
        public double salario { get; set; }
        public double descuento { get; set; }
        public double valor { get; set; }
        public double saldo { get; set; }
        public double saldoPrevio { get; set; }
    }
}
