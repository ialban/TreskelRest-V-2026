using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class PlanCuentas : EntidadBase
    {
        public int pcuesec { get; set; }
        public string pcuecod { get; set; }
        public string pcuedsc { get; set; }
        public int pcuenvl { get; set; }
        public int pcuepad { get; set; }
        public string tcuesec { get; set; }
        public string pcuetip { get; set; }
        public bool pcueres { get; set; }
        public bool pcueajs { get; set; }
        public string pcuestd { get; set; }
        public string pcuepre { get; set; }

        public PlanCuentas(string strConnection) : base(strConnection)
        {
        }

        public List<mPlanCuenta> buscar(string descripcion, string estado)
        {
            List<mPlanCuenta> data = new List<mPlanCuenta>();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "SELECT  pcuesec, pcuecod, pcuedsc, pcuenvl, pcuepad, tcuesec, " +
                                "       pcuetip, pcueres, pcueajs, " +
                                "       CASE pcuestd " +
                                "           WHEN 'A' THEN 'ACTIVO' " +
                                "           ELSE 'INACTIVO' " +
                                "       END AS pcuestd, pcuepre " + 
                               "FROM    tbAdsiPCue " + 
                               "WHERE   pcuedsc LIKE '%@descripcion%' " + 
                               "        AND pcuestd = @estado " + 
                               "ORDER BY pcuesec, pcuenvl";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("@descripcion", descripcion));
                parametros.Add(new SqlParameter("@estado", estado));

                DataTable dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        data.Add(new mPlanCuenta
                        {
                            pcuesec = int.Parse(dr["pcuesec"].ToString()),
                            pcuecod = dr["pcuecod"].ToString(),
                            pcuedsc = dr["pcuedsc"].ToString(),
                            pcuenvl = int.Parse(dr["pcuenvl"].ToString()),
                            pcuepad = int.Parse(dr["pcuepad"].ToString()),
                            tcuesec = dr["tcuesec"].ToString(),
                            pcuetip = dr["pcuetip"].ToString(),
                            pcueres = (bool) dr["pcueres"],
                            pcueajs = (bool) dr["pcueajs"],
                            pcuestd = dr["pcuestd"].ToString(),
                            pcuepre = dr["pcuepre"].ToString(),
                        });
                    }
                }
            }

            return data;
        }
    }
}
