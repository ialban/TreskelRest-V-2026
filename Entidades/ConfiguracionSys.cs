using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class ConfiguracionSys : EntidadBase
    {
        public ConfiguracionSys(string strConnection) : base(strConnection)
        {
        }

        public DataTable load()
        {
            DataTable dt = null;

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "SELECT confNid, confDsc, desencryptar(confVal) as confVal FROM tbconfparam WHERE confAtv=true ";
                query += "UNION SELECT * FROM (SELECT 9988 AS confNid, 'Logotipo Factura' AS confDsc, CASE WHEN emprlofc IS NULL then '' else encode(emprlofc,'base64') end as confVal FROM tbadsiempr LIMIT 1) c ";
                query += "UNION SELECT * FROM (SELECT 9989 AS confNid, 'UsaContabilidad' AS confDsc, CASE WHEN desencryptar(emprcont) IS NULL THEN '0' ELSE desencryptar(emprcont) END AS confVal FROM tbadsiempr LIMIT 1) d ";
                query += "UNION SELECT * FROM (SELECT 9990 AS confNid, 'usaRetencion' AS confDsc, CASE WHEN desencryptar(emprretc) IS NULL THEN '0' ELSE desencryptar(emprretc) END AS confVal FROM tbadsiempr LIMIT 1) e ";
                query += "UNION SELECT * FROM (SELECT 9991 AS confNid, 'Razón Social' AS confDsc, CASE WHEN desencryptar(emprrsoc) IS NULL THEN '' ELSE desencryptar(emprrsoc) END AS confVal FROM tbadsiempr LIMIT 1) f ";
                query += "UNION SELECT * FROM (SELECT 9992 AS confNid, 'Establecimiento' AS confDsc, CASE WHEN desencryptar(emprecod) IS NULL THEN '' ELSE desencryptar(emprecod) END AS confVal FROM tbadsiempr LIMIT 1) g ";
                query += "UNION SELECT * FROM (SELECT 9993 AS confNid, 'Dirección Matriz' AS confDsc, CASE WHEN desencryptar(emprdirm) IS NULL THEN '' ELSE desencryptar(emprdirm) END AS confVal FROM tbadsiempr LIMIT 1) h ";
                query += "UNION SELECT * FROM (SELECT 9994 AS confNid, 'Dirección Local' AS confDsc, CASE WHEN desencryptar(emprdirl) IS NULL THEN '' ELSE desencryptar(emprdirl) END AS confVal FROM tbadsiempr LIMIT 1) i ";
                query += "UNION SELECT * FROM (SELECT 9995 AS confNid, 'Logotipo Empresa' AS confDsc, CASE WHEN emprimg IS NULL then '' else encode(emprimg,'base64') end as confVal FROM tbadsiempr LIMIT 1) c ";
                query += "UNION SELECT * FROM (SELECT 9999 AS confNid, 'Nombre de la empresa' AS confDsc, desencryptar(emprnbr) as confVal FROM tbadsiempr LIMIT 1) a ";
                query += "UNION SELECT * FROM (SELECT 9998 AS confNid, 'Ruc de la empresa' AS confDsc, desencryptar(emprnid) as confVal FROM tbadsiempr LIMIT 1) b ";
                query += "ORDER BY confNid";

                dt = conn.ejecutaQuery(query);

                conn.cerrar();
            }

            return dt;
        }

        public DataTable loadConfigBase()
        {
            DataTable dt = null;

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "SELECT confNid, confDsc, desencryptar(confVal) as confVal " +
                               "FROM tbconfparam " +
                               "WHERE confNid IN (3, 13, 20, 21, 161, 162, 163, 164)";

                dt = conn.ejecutaQuery(query);
            }

            return dt;
        }

        public Respuesta grabar(List<mConfig> valores)
        {
            Respuesta respuesta = new Respuesta();

            respuesta.resultado = false;
            respuesta.mensaje = "La configuración no pudo ser grabada correctamente";

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT * FROM fnConfiguracion(@data)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("data", JsonConvert.SerializeObject(valores)));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    respuesta.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1 ? true : false;
                    respuesta.mensaje = dt.Rows[0]["descripcion"].ToString();
                }
            }
            return respuesta;
        }
    }
}
