namespace Entidades.modelos
{
    public class mItemInv
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public int cantidad { get; set; }
    }

    public class mNoIngred
    {
        public int id { get; set; }
        public string codigo { get; set; }
    }

    public class rMovItems
    {
        public string codigo { get; set; }
        public string producto { get; set; }
        public string descripcion { get; set; }
        public string movimiento { get; set; }
        public string fecha { get; set; }
        public double cantidad { get; set; }
        public string unidad { get; set; }
    }

    public class rMovItemsMes
    {
        public string producto { get; set; }
        public int anio { get; set; }
        public double inicioMes { get; set; }
        public double ingresosMes { get; set; }
        public double egresosMes { get; set; }
        public double saldosMes { get; set; }
    }

    public class rMovItemsKrdx
    {
        public string producto { get; set; }
        public int anio { get; set; }
        public double inicioEnero { get; set; }
        public double ingresosEnero { get; set; }
        public double egresosEnero { get; set; }
        public double saldosEnero { get; set; }

        public double inicioFebrero { get; set; }
        public double ingresosFebrero { get; set; }
        public double egresosFebrero { get; set; }
        public double saldosFebrero { get; set; }

        public double inicioMarzo { get; set; }
        public double ingresosMarzo { get; set; }
        public double egresosMarzo { get; set; }
        public double saldosMarzo { get; set; }

        public double inicioAbril { get; set; }
        public double ingresosAbril { get; set; }
        public double egresosAbril { get; set; }
        public double saldosAbril { get; set; }

        public double inicioMayo { get; set; }
        public double ingresosMayo { get; set; }
        public double egresosMayo { get; set; }
        public double saldosMayo { get; set; }

        public double inicioJunio { get; set; }
        public double ingresosJunio { get; set; }
        public double egresosJunio { get; set; }
        public double saldosJunio { get; set; }

        public double inicioJulio { get; set; }
        public double ingresosJulio { get; set; }
        public double egresosJulio { get; set; }
        public double saldosJulio { get; set; }

        public double inicioAgosto { get; set; }
        public double ingresosAgosto { get; set; }
        public double egresosAgosto { get; set; }
        public double saldosAgosto { get; set; }

        public double inicioSeptiembre { get; set; }
        public double ingresosSeptiembre { get; set; }
        public double egresosSeptiembre { get; set; }
        public double saldosSeptiembre { get; set; }

        public double inicioOctubre { get; set; }
        public double ingresosOctubre { get; set; }
        public double egresosOctubre { get; set; }
        public double saldosOctubre { get; set; }

        public double inicioNoviembre { get; set; }
        public double ingresosNoviembre { get; set; }
        public double egresosNoviembre { get; set; }
        public double saldosNoviembre { get; set; }

        public double inicioDiciembre { get; set; }
        public double ingresosDiciembre { get; set; }
        public double egresosDiciembre { get; set; }
        public double saldosDiciembre { get; set; }
    }
}
