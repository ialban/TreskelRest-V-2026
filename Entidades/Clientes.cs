using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Clientes : EntidadBase
    {
        public string nidNid { get; set; }
        public string nidTip { get; set; } = "C";
        public string nidNbr { get; set; } = "";
        public string nidDir { get; set; } = "";
        public string nidTlf { get; set; } = "";
        public string nidEml { get; set; } = "";
        public string nidStdo { get; set; } = "A";
        public bool clieCdto { get; set; } = false;
        public string rucEmpr { get; set; } = null;
        public string emprNbr { get; set; } = null;
        public bool emprPaga { get; set; } = false;

        public Clientes(string strConnection) : base(strConnection)
        {

        }

        public Boolean grabar(String accion)
        {
            Boolean res = false;
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("nidNid", this.nidNid));
                parametros.Add(new SqlParameter("nidTip", this.nidTip));
                parametros.Add(new SqlParameter("nidNbr", this.nidNbr.ToUpper()));
                parametros.Add(new SqlParameter("nidDir", this.nidDir));
                parametros.Add(new SqlParameter("nidTlf", this.nidTlf));
                parametros.Add(new SqlParameter("nidEml", this.nidEml));
                parametros.Add(new SqlParameter("nidStdo", this.nidStdo));
                parametros.Add(new SqlParameter("clieCdto", this.clieCdto));
                parametros.Add(new SqlParameter("emprPaga", this.emprPaga));

                string query = string.Empty;

                if (accion.Equals("N"))
                {
                    if (!string.IsNullOrEmpty(rucEmpr))
                    {
                        parametros.Add(new SqlParameter("rucEmpr", this.rucEmpr));

                        query = "INSERT INTO tbAdsiNid " + 
                                "   (nidnid, nidtip, nidnbr, niddir, nidtlf, nideml, nidstdo, cliecdto, nidempr, emprpaga) " +
                                "VALUES (@nidNid, @nidTip, @nidNbr, @nidDir, @nidTlf, @nidEml, @nidStdo, @cliecdto, @rucEmpr, @emprPaga)";
                    }
                    else
                    {
                        query = "INSERT INTO tbAdsiNid " +
                                "   (nidnid, nidtip, nidnbr, niddir, nidtlf, nideml, nidstdo, cliecdto, emprpaga) " +
                                "VALUES (@nidNid, @nidTip, @nidNbr, @nidDir, @nidTlf, @nidEml, @nidStdo, @cliecdto, @emprPaga)";
                    }
                    

                    var result = conn.updateQuery(query, parametros);
                    res = result.resultado;
                }
                else
                {
                    if (!string.IsNullOrEmpty(rucEmpr))
                    {
                        query = "UPDATE  tbAdsiNid " +
                                "       SET nidTip = @nidTip, nidNbr = @nidNbr, nidDir = @nidDir, " +
                                "       nidTlf = @nidTlf, nidEml = @nidEml, nidStdo = @nidStdo, " +
                                "       cliecdto = @clieCdto, nidempr = @rucEmpr, emprpaga = @emprPaga " +
                                "WHERE nidNid=@nidNid";

                        parametros.Add(new SqlParameter("rucEmpr", this.rucEmpr));
                    }
                    else
                    {
                        query = "UPDATE  tbAdsiNid " +
                                "       SET nidTip = @nidTip, nidNbr = @nidNbr, nidDir = @nidDir, " +
                                "       nidTlf = @nidTlf, nidEml = @nidEml, nidStdo = @nidStdo, " +
                                "       cliecdto = @clieCdto, emprpaga = @emprPaga " +
                                "WHERE nidNid=@nidNid";
                    }
                        

                    var result = conn.updateQuery(query, parametros);
                    res = result.resultado;
                }

                conn.cerrar();
            }

            return res;
        }

        public DataTable buscar(String apellido, string tipo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT * FROM fnLstNid(@apellido, @tipo) ORDER BY cliente";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("apellido", '%' + apellido + '%'));
                parametros.Add(new SqlParameter("tipo", tipo));

                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    dt.TableName = "Listado";
                }
                else
                {
                    dt = new DataTable("Listado");
                }
            }

            return dt;
        }

        public Boolean getById()
        {
            Boolean res = false;
            DbConnection conn = new DbConnection(strConnection); ;
            if (conn.conectar())
            {
                String query = "SELECT c.*, e.nidnid AS rucempr, e.nidnbr AS emprnbr " +
                               "FROM tbAdsiNid c " +
                               "     LEFT OUTER JOIN tbAdsiNid e ON e.nidnid = c.nidempr " +
                               "WHERE c.nidNid = @nidNid";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("nidNid", this.nidNid));

                DataTable dt = null;

                dt = conn.ejecutaQuery(query, parametros);
                if (dt != null)
                {
                    dt.TableName = "Listado";
                    conn.cerrar();

                    if (dt.Rows.Count > 0)
                    {
                        this.nidNid = dt.Rows[0]["nidNid"].ToString();
                        this.nidTip = dt.Rows[0]["nidTip"].ToString();
                        this.nidNbr = dt.Rows[0]["nidNbr"].ToString();
                        this.nidDir = dt.Rows[0]["nidDir"].ToString();
                        this.nidTlf = dt.Rows[0]["nidTlf"].ToString();
                        this.nidEml = dt.Rows[0]["nidEml"].ToString();
                        this.nidStdo = dt.Rows[0]["nidstdo"].ToString();
                        this.clieCdto = (bool) dt.Rows[0]["cliecdto"];
                        this.rucEmpr = dt.Rows[0]["rucempr"].ToString();
                        this.emprNbr = dt.Rows[0]["emprnbr"].ToString();
                        this.emprPaga = (bool)dt.Rows[0]["emprpaga"];

                        res = true;
                    }
                }
                
            }

            return res;
        }

        public List<mClientes> getByIdOrName(string filtro)
        {
            List<mClientes> respuesta = new List<mClientes>();

            DbConnection conn = new DbConnection(strConnection); ;
            if (conn.conectar())
            {
                string query = "SELECT * FROM fnBscClieByIdOrName(@filtro)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("filtro", filtro));

                DataTable dt = null;

                dt = conn.ejecutaQuery(query, parametros);
                if (dt != null)
                {
                    dt.TableName = "Listado";
                    conn.cerrar();

                    foreach(DataRow dr in dt.Rows)
                    {
                        respuesta.Add(new mClientes
                        {
                            nidNid = dr["nidnid"].ToString(),
                            nidNbr = dr["nidnbr"].ToString(),
                            nidDir = dr["niddir"].ToString(),
                            nidTlf = dr["nidtlf"].ToString(),
                            nidEml = dr["nideml"].ToString(),
                            nidStdo = dr["nidstdo"].ToString(),
                            nidTip = dr["nidtip"].ToString(),
                            nidEmpr = dr["nidempr"].ToString(),
                            emprNbr = dr["emprnbr"].ToString(),
                            cliecdto = (bool) dr["cliecdto"],
                            emprPaga = (bool) dr["emprpaga"]
                        });
                    }
                }
            }

            return respuesta;
        }

        public List<mClientes> getAll(string filtro)
        {
            DbConnection conn = new DbConnection(strConnection);
            List<mClientes> clientes = new List<mClientes>();
            DataTable dt = null;
            filtro = "%" + filtro.ToUpper() + "%";
            if (conn.conectar())
            {
                String query = "SELECT * FROM tbAdsiNid WHERE nidnbr LIKE @filtro AND cliecdto = TRUE ORDER BY nidnid ASC, nidtip DESC";

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("filtro", filtro));

                dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        clientes.Add(new mClientes
                        {
                            nidNid = dr["nidnid"].ToString(),
                            nidTip = dr["nidtip"].ToString(),
                            nidNbr = dr["nidnbr"].ToString()
                        });
                    }
                }

                conn.cerrar();
            }

            return clientes;
        }

        public List<mClientes> getClientesConCredito(int tipo)
        {
            DbConnection conn = new DbConnection(strConnection);
            List<mClientes> clientes = new List<mClientes>();
            DataTable dt = null;
            
            if (conn.conectar())
            {
                String query = string.Empty;

                if (tipo == 0)
                {
                    //query = "SELECT nidnid AS codigo, nidnbr AS nombre FROM tbAdsiNid WHERE cliecdto = TRUE and emprpaga = false ORDER BY nidnid ASC, nidtip DESC";
                    query = "SELECT DISTINCT " +
                            "        c.nidnid AS codigo, c.nidnbr AS nombre " +
                            "FROM    tbAdsiNid c " +
                            "        INNER JOIN tbPeddMCdto cl ON cl.nidpers = c.nidnid " +
                            "        INNER JOIN tbPeddDCdto em ON em.niddeud = cl.nidpers " +
                            "WHERE c.cliecdto = TRUE  " +
                            "ORDER BY c.nidnbr ASC ";
                }
                else
                {
                    //query = "SELECT DISTINCT " +
                    //        "    e.nidnid AS codigo, e.nidnbr AS nombre " +
                    //        "FROM tbAdsiNid c " +
                    //        "    INNER JOIN " +
                    //        "    tbAdsiNid e ON e.nidnid = c.nidempr " +
                    //        "WHERE c.cliecdto = TRUE AND c.emprpaga = true " +
                    //        "ORDER BY e.nidnbr ASC";
                    query = "SELECT 	DISTINCT " + 
                            "        c.nidnid AS codigo, c.nidnbr AS nombre " +
                            "FROM    tbAdsiNid c " +
                            "        INNER JOIN tbPeddDCdto em ON em.niddeud = c.nidnid " +
                            "WHERE c.nidnid NOT IN(SELECT d.nidpers FROM tbPeddMCdto d WHERE d.cdtoscl = em.cdtoscl) " +
                            "ORDER BY c.nidnbr ASC";
                }
                

                List<SqlParameter> parametros = new List<SqlParameter>();

                dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        clientes.Add(new mClientes
                        {
                            nidNid = dr["codigo"].ToString(),
                            nidNbr = dr["nombre"].ToString()
                        });
                    }
                }

                conn.cerrar();
            }

            return clientes;
        }

        public List<mDeuda> GetDeuda(string clieNid)
        {
            DbConnection conn = new DbConnection(strConnection);
            List<mDeuda> deuda = new List<mDeuda>();
            DataTable dt = null;

            if (conn.conectar())
            {
                string query = "SELECT  cdtoscl, " +
                               "        CASE " +
                               "            WHEN cdtopdo LIKE 'Q%' THEN " +
                               "                CASE " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 1 THEN 'PRIMER QUIMESTRE DE ENERO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 2 THEN 'SEGUNDO QUIMESTRE DE ENERO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 3 THEN 'PRIMER QUIMESTRE DE FEBRERO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 4 THEN 'SEGUNDO QUIMESTRE DE FEBRERO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 5 THEN 'PRIMER QUIMESTRE DE MARZO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 6 THEN 'SEGUNDO QUIMESTRE DE MARZO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 7 THEN 'PRIMER QUIMESTRE DE ABRIL' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 8 THEN 'SEGUNDO QUIMESTRE DE ABRIL' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 9 THEN 'PRIMER QUIMESTRE DE MAYO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 10 THEN 'SEGUNDO QUIMESTRE DE MAYO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 11 THEN 'PRIMER QUIMESTRE DE JUNIO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 12 THEN 'SEGUNDO QUIMESTRE DE JUNIO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 13 THEN 'PRIMER QUIMESTRE DE JULIO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 14 THEN 'SEGUNDO QUIMESTRE DE JULIO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 15 THEN 'PRIMER QUIMESTRE DE AGOSTO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 16 THEN 'SEGUNDO QUIMESTRE DE AGOSTO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 17 THEN 'PRIMER QUIMESTRE DE SEPTIEMBRE' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 18 THEN 'SEGUNDO QUIMESTRE DE SEPTIEMBRE' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 19 THEN 'PRIMER QUIMESTRE DE OCTUBRE' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 20 THEN 'SEGUNDO QUIMESTRE DE OCTUBRE' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 21 THEN 'PRIMER QUIMESTRE DE NOVIEMBRE' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 22 THEN 'SEGUNDO QUIMESTRE DE NOVIEMBRE' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 23 THEN 'PRIMER QUIMESTRE DE DICIEMBRE' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 24 THEN 'SEGUNDO QUIMESTRE DE DICIEMBRE' " +
                               "                END " +
                               "            ELSE " +
                               "                CASE " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 1 THEN 'MES DE ENERO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 2 THEN 'MES DE FEBRERO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 3 THEN 'MES DE MARZO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 4 THEN 'MES DE ABRIL' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 5 THEN 'MES DE MAYO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 6 THEN 'MES DE JUNIO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 7 THEN 'MES DE JULIO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 8 THEN 'MES DE AGOSTO' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 9 THEN 'MES DE SEPTIEMBRE' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 10 THEN 'MES DE OCTUBRE' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 11 THEN 'MES DE NOVIEMBRE' " +
                               "                    WHEN(SUBSTRING(cdtopdo, 2)::INT) = 12 THEN 'MES DE DICIEMBRE' " +
                               "                END " +
                               "            END AS cdtopdo, " +
                               "        cdtoval " +
                               "FROM    tbPeddDCdto " +
                               "WHERE   niddeud = @cliente AND cdtostd = 'N'";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("cliente", clieNid));

                dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        deuda.Add(new mDeuda
                        {
                            cdtoscl = dr["cdtoscl"].ToString(),
                            cdtopdo = dr["cdtopdo"].ToString(),
                            cdtoval = Utils.Helper.StringToDouble(dr["cdtoval"].ToString())
                        });
                    }
                }

                conn.cerrar();
            }

            return deuda;
        }

        public bool PagaDeuda(string deuda, double valor)
        {
            bool respuesta = false;
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("vcodigo", deuda));
                parametros.Add(new SqlParameter("vvalor", valor));


                string query = string.Empty;

                query = "SELECT * FROM fngrabaPagoDeuda(@vcodigo::varchar, @vvalor::numeric(10,6));";

                var dt = conn.ejecutaQuery(query, parametros);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (int.Parse(dr["codigo"].ToString()) == 1)
                        {
                            respuesta = true;
                        }
                    }
                }

                conn.cerrar();
            }

            return respuesta;
        }
    }
}
