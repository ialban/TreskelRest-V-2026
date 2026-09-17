using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Parametros : EntidadBase
    {
        public int confNid { get; set; }
        public String confDsc { get; set; }
        public String confVal { get; set; }
        public Boolean confAtv { get; set; }

        public Parametros(string strConnection) : base(strConnection)
        {
        }

        public DataTable listar()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT confNid AS codigo, confDsc as descripcion, desencryptar(confVal) AS valor FROM tbConfParam WHERE confAtv=true " +
                               "UNION " +
                               "SELECT 9989 AS codigo,'Usa Retenciones' AS descripcion, desencryptar(emprretc) AS valor FROM tbAdsiEmpr " +
                               "UNION " +
                               "SELECT 9990 AS codigo,'Lleva contabilidad' AS descripcion, desencryptar(emprcont) AS valor FROM tbAdsiEmpr " +
                               "UNION " +
                               "SELECT 9991 AS codigo,'Codigo Empresa' AS descripcion, desencryptar(emprecod) AS valor FROM tbAdsiEmpr " +
                               "UNION " +
                               "SELECT 9992 AS codigo,'Direccion local' AS descripcion, desencryptar(emprdirl) AS valor FROM tbAdsiEmpr " +
                               "UNION " +
                               "SELECT 9993 AS codigo,'Direccion Matriz' AS descripcion, desencryptar(emprdirm) AS valor FROM tbAdsiEmpr " +
                               "UNION " +
                               "SELECT 9994 AS codigo,'Razon Social' AS descripcion, desencryptar(emprrsoc) AS valor FROM tbAdsiEmpr " +
                               "UNION " +
                               "SELECT 9995 AS codigo,'Logotipo Empresa' AS descripcion, case emprimg when null then '' else encode(emprimg,'base64') end AS valor FROM tbAdsiEmpr " +
                               "UNION " +
                               "SELECT 9996 AS codigo,'Treskel Credencial' AS descripcion, case emprcrd when '' then '' else desencryptar(emprcrd) end AS valor FROM tbAdsiEmpr " +
                               "UNION " +
                               "SELECT 9997 AS codigo,'Validador Credencial' AS descripcion, emprvld AS valor FROM tbAdsiEmpr " +
                               "UNION " +
                               "SELECT 9998 AS codigo,'Ruc Empresa' AS descripcion, desencryptar(emprNid) AS valor FROM tbAdsiEmpr "+
                               "UNION " +
                               "SELECT 9999 AS codigo, 'Nombre Empresa' AS descripcion, desencryptar(emprNbr) AS valor FROM tbAdsiEmpr";

                dt = conn.ejecutaQuery(query);
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

        public DataTable ParametrosBackup()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT  confNid AS codigo, confDsc as descripcion, desencryptar(confVal) AS valor " + 
                               "FROM    tbConfParam " + 
                               "WHERE   confNid IN (29,30,31,33,34,35,36,37,38,43) AND confAtv=true";

                dt = conn.ejecutaQuery(query);
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

        public DataTable PgDumpLocation()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "select * from fnSetPgDumpLocation() AS descripcion";

                dt = conn.ejecutaQuery(query);
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
            conn.cerrar();
            return dt;
        }

        public bool SetEjecutaBackup(bool valor)
        {
            DbConnection conn = new DbConnection(strConnection);
            
            if (conn.conectar())
            {
                String query = "";

                if (valor)
                    query = "UPDATE tbConfParam SET confVal = encryptar('1'::text) WHERE confNid = 43";
                else
                    query = "UPDATE tbConfParam SET confVal = encryptar('0'::text) WHERE confNid = 43";

                var result = conn.updateQuery(query, new List<Microsoft.Data.SqlClient.SqlParameter>());

                conn.cerrar();
                return result.resultado;
            }

            return false;
        }
    }
}
