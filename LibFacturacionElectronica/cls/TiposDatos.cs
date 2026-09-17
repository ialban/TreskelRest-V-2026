using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LibFacturacionElectronica.cls
{
    public class TiposDatos
    {
        public enum TipoAmbiente
        {
            Pruebas = 1,
            Produccion = 2
        }

        // Debe retornarse como string de dos digitos Ejm: "01"
        public enum TipoComprobante
        {
            Factura = 1,
            LiquidacionComprasBienesPrestacionServicios = 3,
            NotaCredito = 4,
            NotaDebito = 5,
            GuiaRemision = 6,
            ComprobanteRetencion = 7
        }

        public enum TipoEmision
        {
            EmisionNormal = 1
        }

        public enum TipoIdentificador
        {
            Ruc = 1,
            CedulaIdentidad = 2,
            Pasaporte = 3
        }

        public enum AgenteRetencion
        {
            SI = 1,
            NO = 2
        }

        public enum LLevaContabilidad
        {
            SI = 1,
            NO = 2
        }

        public enum TipoIdentificadorComprador
        {
            Ruc = 4,
            Cedula = 5,
            Pasaporte = 6,
            CosumidorFinal = 7,
            IdentificacionDelExterior = 8
        }

        public enum TipoImpuesto
        {
            IVA = 2,
            ICE = 3,
            IRBPNR = 5
        }

        public enum TarifaIVA
        {
            Cero = 0,
            Doce = 2,
            Catorce = 3,
            Quince = 4,
            Cinco = 5,
            NoObjetoDeImpuesto = 6,
            ExcentoDeIVA = 7,
            IvaDiferenciado = 8,
            Trece = 10
        }

        public enum TipoDeImpuesto
        {
            RENTA = 1,
            IVA = 2,
            ISD = 6
        }

        public enum RentencionIVA
        {
            Diez = 9,
            Veinte = 10,
            Treinta = 1,
            Ciencuenta = 11,
            Setenta = 2,
            Cien = 3,
            Cero = 7,
            NoProcedeRetencion = 8
        }

        public enum EstadoTramite
        {
            PPR = 1, // En Procesamiento
            AUT = 2, // Autorizado
            NAT = 3, // No Autorizado
        }

        public enum FormasDePago
        {
            SinSistemaFinanciero = 1,
            CompensacionDeudas = 15,
            TarjetaDebito = 16,
            DineroElectronico = 17,
            TarjetaPrepago = 18,
            TarjetaCredito = 19,
            OtrosConSistemaFinanciero = 20,
            EndosoTitulos = 21
        }

        public enum TiempoPlazo
        {
            [Description("N/A")]
            Ninguno = 0,
            [Description("días")]
            Dias = 1,
            [Description("meses")]
            Meses = 2,
            [Description("años")]
            Anios = 3
        }

        public enum Moneda
        {
            DOLAR = 1
        }

        public class TotalImpuesto{
            public TipoDeImpuesto codigo { get; set; }
            public TarifaIVA CodigoPorcentaje { get; set; }
            public decimal BaseImponible { get; set; }
            public decimal Valor { get; set; }
        }

        public class Pago
        {
            public FormasDePago formaDePago { get; set; }
            public decimal Total { get; set; }
            public int Plazo { get; set; }
            public TiempoPlazo TiempoPlazo {get; set;}
        }

        public class Impuesto: TotalImpuesto
        {
            public double tarifa { get; set; }
        }

        public class Detalle
        {
            public string CodigoPrincipal { get; set; }
            public string CodigoAuxiliar { get; set; }
            public string Descripcion { get; set; }
            public int Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal Descuento { get; set; }
            public decimal PrecioTotalSinImpuesto { get; set; }
            public List<Impuesto> Impuesto { get; set; }
        }

        public class InformacionAdicional
        {
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}
