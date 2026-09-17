using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreskelRest.cls
{
    public class Luhn
    {   
        public int generaValidador(string numero)
        {
            int val = 0;
            int sum = this.obtieneSuma(numero);

            if (sum % 11 != 0)
            {
                val = 11 - (sum % 11);
            }

            return val;
        }

        private int obtieneSuma(string numero)
        {
            int[] mult = new int[numero.Length];

            int val = 2;
            for (int i = mult.Length - 1; i >= 0; i--)
            {
                mult[i] = val;

                if (val < 7)
                    val++;
                else
                    val = 2;
            }

            int num = 0;
            int sum = 0;
            val = 0;
            for (int i = 0; i < numero.Length; i++)
            {
                num = int.Parse(numero.Substring(i, 1));
                sum += num * mult[i];
            }

            return sum;
        }

        public Boolean validarMod10(string numero, int validador)
        {
            Boolean valido = false;
            int val = 0;

            int mul = 0;
            int num = 0;
            int sum = 0;

            for (int i=0; i< numero.Length; i++)
            {
                mul = i % 2 == 0 ? 2 : 1;
                num = int.Parse(numero.Substring(i, 1));

                int op = num * mul;
                num = op >9 ? op - 9 : op;
                sum += num;
            }

            if (sum % 10 != 0)
            {
                val = 10 - sum % 10;
            }

            if (val == validador) valido = true;

            return valido;
        }

        public Boolean validarMod11(string numero, int validador)
        {
            Boolean valido = false;
            
            int val = this.generaValidador(numero);
            
            if (val == validador) valido = true;                

            return valido;
        }
    }
}
