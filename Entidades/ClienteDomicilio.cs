using AccesoDatos;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class ClienteDomicilio : EntidadBase
    {
        public string peddScl { get; set; }
        public string clieNid { get; set; }
        public string clieNbr { get; set; }
        public string clieDir { get; set; }
        public string clieTlf { get; set; }
        public string clieRef { get; set; }

        public ClienteDomicilio(string strConnection) : base(strConnection)
        {

        }

        public List<ClienteDomicilio> getClienteByNid(string clieNid)
        {
            List<ClienteDomicilio> clientes = new List<ClienteDomicilio>();

            DbConnection conn = new DbConnection(strConnection);
            if (conn.conectar())
            {
                string query = "SELECT  clienid, clienbr,cliedir,clietlf " + 
                               "FROM    tbpedddatcli " + 
                               "WHERE   clieNid = @clieNid " +
                               "        AND clieact = true " +
                               "GROUP BY clienid, clienbr,cliedir,clietlf";

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("clieNid", clieNid));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ClienteDomicilio cliente = new ClienteDomicilio(this.strConnection);

                        cliente.peddScl = ""; // dr["peddScl"].ToString();
                        cliente.clieNid = dr["clieNid"].ToString();
                        cliente.clieNbr = dr["clieNbr"].ToString();
                        cliente.clieDir = dr["clieDir"].ToString();
                        cliente.clieTlf = dr["clieTlf"].ToString();
                        cliente.clieRef = ""; // dr["clieRef"].ToString();

                        clientes.Add(cliente);
                    }
                }

                conn.cerrar();
            }

            return clientes;
        }

        public List<ClienteDomicilio> getClienteByTlf(string clieTlf)
        {
            List<ClienteDomicilio> clientes = new List<ClienteDomicilio>();

            DbConnection conn = new DbConnection(strConnection);
            if (conn.conectar())
            {
                string query =  "SELECT clienid, clienbr,cliedir,clietlf " + 
                                "FROM   tbpedddatcli " + 
                                "WHERE  clieTlf = @clieTlf " + 
                                "       AND clieact = true " +
                                "GROUP BY clienid, clienbr,cliedir,clietlf";

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("clieTlf", clieTlf));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ClienteDomicilio cliente = new ClienteDomicilio(this.strConnection);

                        cliente.peddScl = ""; // dr["peddScl"].ToString();
                        cliente.clieNid = dr["clieNid"].ToString();
                        cliente.clieNbr = dr["clieNbr"].ToString();
                        cliente.clieDir = dr["clieDir"].ToString();
                        cliente.clieTlf = dr["clieTlf"].ToString();
                        cliente.clieRef = ""; // dr["clieRef"].ToString();

                        clientes.Add(cliente);
                    }
                }

                conn.cerrar();
            }

            return clientes;
        }

        public bool desactivar()
        {
            DbConnection conn = new DbConnection(strConnection);
            if (conn.conectar())
            {
                string query = "UPDATE tbpedddatcli " + 
                               "SET     clieact = false " +
                               "WHERE   clienid = @clienid AND " + 
                               "        clienbr = @clienbr AND " + 
                               "        cliedir = @cliedir AND " + 
                               "        clieTlf = @clieTlf ";

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("clienid", clieNid));
                parametros.Add(new SqlParameter("clienbr", clieNbr));
                parametros.Add(new SqlParameter("cliedir", clieDir));
                parametros.Add(new SqlParameter("clietlf", clieTlf));

                var res = conn.updateQuery(query, parametros);

                conn.cerrar();
                return res.resultado;
            }

            return false;
        }
    }
}
