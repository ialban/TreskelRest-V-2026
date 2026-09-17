using AccesoDatos;
using Entidades.modelo;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Entidades
{
    public class Coordenada : EntidadBase
    {
        public Coordenada(string strConnection) : base(strConnection)
        {
        }

        private void getFromCol(Random random, string coords, out string valor, out string lbl)
        {
            string[] cols = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
            string[] rows = { "1", "2", "3", "4", "5" };

            int rCol = random.Next(0, 10) + 1;
            int rRow = random.Next(0, 5) + 1;

            valor = "";
            lbl = "";

            trjCoordRow mRow = new trjCoordRow();
            mRow = JsonConvert.DeserializeObject<trjCoordRow>(coords);

            if (rRow == 1 && rCol == 1) { valor = mRow.f1.c01; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 2 && rCol == 1) { valor = mRow.f2.c01; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 3 && rCol == 1) { valor = mRow.f3.c01; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 4 && rCol == 1) { valor = mRow.f4.c01; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 5 && rCol == 1) { valor = mRow.f5.c01; lbl = cols[rCol - 1] + rows[rRow - 1]; }

            if (rRow == 1 && rCol == 2) { valor = mRow.f1.c02; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 2 && rCol == 2) { valor = mRow.f2.c02; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 3 && rCol == 2) { valor = mRow.f3.c02; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 4 && rCol == 2) { valor = mRow.f4.c02; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 5 && rCol == 2) { valor = mRow.f5.c02; lbl = cols[rCol - 1] + rows[rRow - 1]; }

            if (rRow == 1 && rCol == 3) { valor = mRow.f1.c03; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 2 && rCol == 3) { valor = mRow.f2.c03; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 3 && rCol == 3) { valor = mRow.f3.c03; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 4 && rCol == 3) { valor = mRow.f4.c03; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 5 && rCol == 3) { valor = mRow.f5.c03; lbl = cols[rCol - 1] + rows[rRow - 1]; }

            if (rRow == 1 && rCol == 4) { valor = mRow.f1.c04; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 2 && rCol == 4) { valor = mRow.f2.c04; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 3 && rCol == 4) { valor = mRow.f3.c04; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 4 && rCol == 4) { valor = mRow.f4.c04; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 5 && rCol == 4) { valor = mRow.f5.c04; lbl = cols[rCol - 1] + rows[rRow - 1]; }

            if (rRow == 1 && rCol == 5) { valor = mRow.f1.c05; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 2 && rCol == 5) { valor = mRow.f2.c05; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 3 && rCol == 5) { valor = mRow.f3.c05; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 4 && rCol == 5) { valor = mRow.f4.c05; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 5 && rCol == 5) { valor = mRow.f5.c05; lbl = cols[rCol - 1] + rows[rRow - 1]; }

            if (rRow == 1 && rCol == 6) { valor = mRow.f1.c06; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 2 && rCol == 6) { valor = mRow.f2.c06; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 3 && rCol == 6) { valor = mRow.f3.c06; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 4 && rCol == 6) { valor = mRow.f4.c06; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 5 && rCol == 6) { valor = mRow.f5.c06; lbl = cols[rCol - 1] + rows[rRow - 1]; }

            if (rRow == 1 && rCol == 7) { valor = mRow.f1.c07; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 2 && rCol == 7) { valor = mRow.f2.c07; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 3 && rCol == 7) { valor = mRow.f3.c07; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 4 && rCol == 7) { valor = mRow.f4.c07; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 5 && rCol == 7) { valor = mRow.f5.c07; lbl = cols[rCol - 1] + rows[rRow - 1]; }

            if (rRow == 1 && rCol == 8) { valor = mRow.f1.c08; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 2 && rCol == 8) { valor = mRow.f2.c08; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 3 && rCol == 8) { valor = mRow.f3.c08; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 4 && rCol == 8) { valor = mRow.f4.c08; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 5 && rCol == 8) { valor = mRow.f5.c08; lbl = cols[rCol - 1] + rows[rRow - 1]; }

            if (rRow == 1 && rCol == 9) { valor = mRow.f1.c09; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 2 && rCol == 9) { valor = mRow.f2.c09; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 3 && rCol == 9) { valor = mRow.f3.c09; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 4 && rCol == 9) { valor = mRow.f4.c09; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 5 && rCol == 9) { valor = mRow.f5.c09; lbl = cols[rCol - 1] + rows[rRow - 1]; }

            if (rRow == 1 && rCol == 10) { valor = mRow.f1.c10; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 2 && rCol == 10) { valor = mRow.f2.c10; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 3 && rCol == 10) { valor = mRow.f3.c10; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 4 && rCol == 10) { valor = mRow.f4.c10; lbl = cols[rCol - 1] + rows[rRow - 1]; }
            if (rRow == 5 && rCol == 10) { valor = mRow.f5.c10; lbl = cols[rCol - 1] + rows[rRow - 1]; }
        }

        private mCoordenada GenerarCoordenadas(string usuacod)
        {
            mCoordenada coord = new mCoordenada();

            Usuarios usr = new Usuarios(strConnection);
            usr.usuaCod = usuacod;
            usr.getById();

            var guid = Guid.NewGuid();
            var justNumbers = new String(guid.ToString().Where(Char.IsDigit).ToArray());
            var seed = int.Parse(justNumbers.Substring(0, 4));

            var random = new Random(seed);

            var coord1 = coord.coord1;
            var lcoor1 = coord.lcoor1;
            var coord2 = coord.coord2;
            var lcoor2 = coord.lcoor2;
            var coord3 = coord.coord3;
            var lcoor3 = coord.lcoor3;
            getFromCol(random, usr.usuacoord, out coord1, out lcoor1);
            getFromCol(random, usr.usuacoord, out coord2, out lcoor2);
            getFromCol(random, usr.usuacoord, out coord3, out lcoor3);

            coord.coord1 = coord1.Trim();
            coord.coord2 = coord2.Trim();
            coord.coord3 = coord3.Trim();
            coord.lcoor1 = lcoor1.Trim();
            coord.lcoor2 = lcoor2.Trim();
            coord.lcoor3 = lcoor3.Trim();

            return coord;
        }

        public mCoordenada Generar(string accion, int secuencial, string tipo, string origen, string secorigen, string usuacod, string estado)
        {
            DbConnection conn = new DbConnection(strConnection);
            mCoordenada coord = new mCoordenada();

            if (conn.conectar())
            {
                // Generando las coordenadas acorde a las coordenadas del usuario
                if (accion == "N" || accion == "R")
                {
                    coord = GenerarCoordenadas(usuacod);
                }

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("msgscl", secuencial));
                parametros.Add(new SqlParameter("msgtip", tipo));
                parametros.Add(new SqlParameter("tiporg", origen));
                parametros.Add(new SqlParameter("secorg", secorigen));
                parametros.Add(new SqlParameter("usucod", usuacod));
                parametros.Add(new SqlParameter("crd1lbl", coord.lcoor1 == null ? "" : coord.lcoor1));
                parametros.Add(new SqlParameter("crd2lbl", coord.lcoor2 == null ? "" : coord.lcoor2));
                parametros.Add(new SqlParameter("crd3lbl", coord.lcoor3 == null ? "" : coord.lcoor3));
                parametros.Add(new SqlParameter("crd1val", coord.coord1 == null ? "" : coord.coord1));
                parametros.Add(new SqlParameter("crd2val", coord.coord2 == null ? "" : coord.coord2));
                parametros.Add(new SqlParameter("crd3val", coord.coord3 == null ? "" : coord.coord3));
                parametros.Add(new SqlParameter("estado", estado == "" ? "A" : estado));
                
                string query = "SELECT * FROM fnGrabarMsgCoord(@accion, @msgscl, @msgtip, @tiporg, @secorg, @usucod, @crd1lbl, @crd2lbl, @crd3lbl, @crd1val, @crd2val, @crd3val, @estado);";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null && (accion == "N" || accion == "R"))
                {
                    coord.coordscl = int.Parse(dt.Rows[0]["msgscl"].ToString());
                }

                conn.cerrar();
            }

            return coord;
        }

        //public void grabar(string accion)
        //{
        //    Respuesta res = new Respuesta();
        //    DbConnection conn = new DbConnection(strConnection);

        //    if (conn.conectar())
        //    {
        //        List<SqlParameter> parametros = new List<SqlParameter>();
        //        parametros.Add(new SqlParameter("accion", accion));
        //        parametros.Add(new SqlParameter("msgid", msgscl));
        //        parametros.Add(new SqlParameter("tipo", msgtip));
        //        parametros.Add(new SqlParameter("origen", tiporg));
        //        parametros.Add(new SqlParameter("secuencial", secorg));
        //        parametros.Add(new SqlParameter("usuario", usucod));
        //        parametros.Add(new SqlParameter("pin", msgpin));
        //        parametros.Add(new SqlParameter("estado", msgatv));

        //        string query = "SELECT * FROM fnGrabarMsgCoord(@accion, @msgid, @tipo, @origen, @secuencial, @usuario, @pin, @estado);";

        //        DataTable dt = conn.ejecutaQuery(query, parametros);

        //        if (dt != null)
        //        {
        //            msgscl = int.Parse(dt.Rows[0]["msgscl"].ToString());
        //        }

        //        conn.cerrar();
        //    }
        //}

        public string CoordenadaActive(int scl)
        {
            string estado = "I";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("secuencial", scl));

                string query = "select msgatv from tbMsgCoord where msgscl = @secuencial";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    estado = dt.Rows[0]["msgatv"].ToString();
                }

                conn.cerrar();
            }

            return estado;
        }
    }
}
