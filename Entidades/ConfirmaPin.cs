using AccesoDatos;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class ConfirmaPin : EntidadBase
    {
        public int msgscl { get; set; }
        public string msgtip{ get; set; }
        public string tiporg { get; set; }
        public string secorg { get; set; }
        public string usucod { get; set; }
        public string msgpin { get; set; }
        public string msgatv { get; set; } = "A";

        public ConfirmaPin(string strConnection) : base(strConnection)
        {
        }

        public void grabar(string accion)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("msgid", msgscl));
                parametros.Add(new SqlParameter("tipo", msgtip));
                parametros.Add(new SqlParameter("origen", tiporg));
                parametros.Add(new SqlParameter("secuencial", secorg));
                parametros.Add(new SqlParameter("usuario", usucod));
                parametros.Add(new SqlParameter("pin", msgpin));
                parametros.Add(new SqlParameter("estado", msgatv));

                string query = "SELECT * FROM fnGrabarMsgWhat(@accion, @msgid, @tipo, @origen, @secuencial, @usuario, @pin, @estado);";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    msgscl = int.Parse(dt.Rows[0]["msgscl"].ToString());
                }

                conn.cerrar();
            }
        }

        public string PinActive(int scl, bool usaWhatsapp = true)
        {
            string estado = "I";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("secuencial", scl));

                string query = "";
                if (usaWhatsapp)
                    query = "select msgatv from tbMsgWhat where msgscl = @secuencial";
                else
                    query = "select msgatv from tbMsgEmail where msgscl = @secuencial";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    estado = dt.Rows[0]["msgscl"].ToString();
                }

                conn.cerrar();
            }

            return estado;
        }
    }
}
