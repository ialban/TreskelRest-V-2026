using System;

namespace LibKeys
{
    public class GridKeys
    {
        private string[][] TablaKes = {
            new string[]{ "82", "19", "02", "35", "21", "86", "05", "15", "67", "09"},
            new string[]{ "42", "84", "57", "34", "56", "79", "24", "63", "49", "53" },
            new string[]{ "96", "41", "55", "52", "24", "42", "57", "07", "96", "97" },
            new string[]{ "59", "16", "25", "56", "37", "50", "31", "16", "19", "69" },
            new string[]{ "78", "37", "31", "73", "45", "29", "52", "54", "37", "19" }
        };

        public bool validar(string[] rotulos, string[] valores)
        {
            bool correcto = true;
            string caracteres = "ABCDEFGHIJ";

            for(int i=0; i<3; i++)
            {
                string rotulo = rotulos[i].Substring(0, 1);                
                int col = caracteres.IndexOf(rotulo);
                int row = int.Parse(rotulos[i].Substring(1));

                if(valores[i] != TablaKes[row-1][col])
                {
                    correcto = false;
                }
            }
            return correcto;
        }
        
    }
}
