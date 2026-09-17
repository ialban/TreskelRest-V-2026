using System;

namespace LibFacturacionElectronica.cls
{
    public class GeneraClaveAcceso: TiposDatos
    {
        public int getVerificador(string valor)
        {
            var valores = valor.ToCharArray();
            int[] Valores = Array.ConvertAll(valores, s => int.Parse(s.ToString()));
            var multiplicadores = new int[valores.Length];

            int numero = 2;
            for (int i = valores.Length - 1; i >= 0; i--)
            {
                multiplicadores[i] = numero;
                numero++;

                if (numero > 7) numero = 2;
            }

            int suma = 0;
            for (int i = 0; i < valores.Length; i ++){
                Valores[i] = Valores[i] * multiplicadores[i];
                suma += Valores[i];
            }

            int modulo = suma % 11;

            if (modulo != 0)
            {
                int resta = 11 - modulo;

                if (resta == 10)
                {
                    resta = 1;
                }

                return resta;
            }
            else
                return 0;
        }

        /*
         Estructura de la Clave de Acceso

         FechaEmision = ddmmaaaa  06062022
         TipoComprobante = "01"
         NumeroRuc = 0925485675001
         TipoAmbiente 1
         Serie = 000001
         CodigoNumerico = 00000001
         TipoEmision = 1
         DigitoVerificador = 1 (Modulo 11)
         */
        public string getClaveAcceso(string ruc, DateTime fecha, string tipoComprobante,
            string ambiente, string NumeroSerie, string NumeroComprobante, string codigoNumerico,
            string emision)
        {
            string clave = "";

            int dia = fecha.Day;
            int mes = fecha.Month;
            int anio = fecha.Year;

            string sDia = dia.ToString().PadLeft(2, '0');
            string sMes = mes.ToString().PadLeft(2, '0');
            string sAnio = anio.ToString().PadLeft(4, '0');

            string sTipoComprobante = tipoComprobante;
            string sTipoAmbiente = ambiente;
            string sNumeroSerie = NumeroSerie;
            string sComprobante = NumeroComprobante.ToString();
            string sCodigo = codigoNumerico;
            string sTipoEmision = emision;

            var s = sNumeroSerie.Split('-');
            if (s.Length > 1)
            {
                sNumeroSerie = s[0] + s[1];
            }

            clave = sDia + sMes + sAnio + sTipoComprobante + ruc + sTipoAmbiente + sNumeroSerie + sComprobante + sCodigo + sTipoEmision;

            string sValidador = getVerificador(clave).ToString();

            clave = clave + sValidador;

            return clave;
        }
    }
}
