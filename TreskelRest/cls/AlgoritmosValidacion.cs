using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreskelRest.cls
{
    public class GeneradorID
    {
        public string numero { get; set; }
        public string validador { get; set; }
    }

    public class AlgoritmosValidacion
    {
        private string numBase = "0914556634001";
        private string[] carBase = { "R", "A", "Z", "O", "W", "L", "Q", "V", "M", "Y", "K" };

        private int busca(string letra)
        {
            int pos = -1;
            for (int i = 0; i < 10; i++)
            {
                if (carBase[i] == letra) pos = i;
            }
            return pos;
        }

        public string generar(string numero)
        {
            string nuevo = "";

            if (numero.Length > this.numBase.Length)
            {
                for (int i = 0; i < numero.Length - this.numBase.Length; i++)
                {
                    this.numBase = "0" + this.numBase;
                }
            }

            for (int i = 0; i < numero.Length; i++)
            {
                int num = int.Parse(numero.Substring(i, 1));
                int nbs = num + int.Parse(numBase.Substring(i, 1));

                nuevo += nbs > 9 ? nbs - 9 : nbs;
            }

            return nuevo;
        }

        public Boolean verificaValidadorCard(string numero, string ruc, string validador)
        {
            Boolean valido = false;

            string oNumero = this.generar(ruc);

            if (oNumero != numero) return false;

            Luhn luhn = new Luhn();
            string oValidador = this.carBase[luhn.generaValidador(numero)];

            if (oValidador == validador) return true;

            return valido;
        }

        public GeneradorID generaValidador(string numero)
        {
            Luhn luhn = new Luhn();
            string nuevo = this.generar(numero);
            string validador = this.carBase[luhn.generaValidador(nuevo)];

            return new GeneradorID()
            {                
                numero = nuevo,
                validador = validador
            };
        }

        public Boolean validaRuc(string numero)
        {
            Boolean valido = false;

            if (numero.Length < 13)
                return false;

            numero = numero.Substring(0, numero.Length - 3);                        

            int codProv = int.Parse(numero.Substring(0, 2));
            if (!(codProv > 0 && codProv <= General.CANTIDAD_PROVINCIAS))
                return false;

            int tdigito = int.Parse(numero.Substring(2, 1));

            int validador = 0;

            Luhn luhn = new Luhn();
            switch (tdigito)
            {
                case 9:
                    validador = int.Parse(numero.Substring(numero.Length - 1));
                    numero = numero.Substring(0, numero.Length - 1);                    
                    valido = luhn.validarMod11(numero, validador);
                    break;
                case 6:
                    validador = int.Parse(numero.Substring(numero.Length - 2, 1));
                    numero = numero.Substring(0, numero.Length - 2);                    
                    valido = luhn.validarMod11(numero, validador);
                    break;
                default:
                    validador = int.Parse(numero.Substring(numero.Length - 1));
                    numero = numero.Substring(0, numero.Length - 1);                    
                    valido = luhn.validarMod10(numero, validador);
                    break;
            }

            return valido;
        }
    }
}
