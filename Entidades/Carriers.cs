using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Carriers : EntidadBase
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string acronimo { get; set; }
        public double precio { get; set; }
        public Boolean esPorcentaje { get; set; }
        public Boolean activo { get; set; }
        public string aplica { get; set; }
        public byte[] imagen { get; set; }

        public Carriers(string strConnection) : base(strConnection)
        {

        }  
        
        public mCarrier getCarrierById(string codigo)
        {
            mCarrier carrier = null;

            DbConnection conn = new DbConnection(strConnection);
            if (conn.conectar())
            {
                String query = "SELECT 	crrinid AS codigo, crrinbr AS carrier, crriicn AS icono, crriprc AS valor, " +
                           "        CASE crripct " +
                           "            WHEN TRUE THEN 'S' " +
                           "            ELSE 'N' " +
                           "        END AS porcentaje,  " +
                           "        CASE crriatv " +
                           "            WHEN TRUE THEN 'S' " +
                           "            ELSE 'N' " +
                           "        END AS estado, crriapi AS aplica, " +
                           "        crriimg AS imagen " +
                           "FROM    tbAdsiCrrie " +
                           "WHERE   crriNid = @codigo ";

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("codigo", codigo));

                DataTable dt = conn.ejecutaQuery(query, parametros);
                
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        carrier = new mCarrier();

                        carrier.codigo = codigo;
                        carrier.carrier = dr["carrier"].ToString();
                        carrier.icono = dr["icono"].ToString();
                        carrier.valor = Utils.Helper.StringToDouble(dr["valor"].ToString());
                        carrier.porcentaje = dr["porcentaje"].ToString();
                        carrier.aplica = dr["aplica"].ToString();
                        carrier.estado = dr["estado"].ToString();
                        
                        if (dr["imagen"].Equals(DBNull.Value))
                        {
                            carrier.imagen = null;
                        }
                        else
                        {
                            carrier.imagen = (byte[])dr["imagen"];
                        }
                    }
                }

                conn.cerrar();
            }

            return carrier;
        }

        public List<mCarrier> buscar(String nombre, string estado)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            List<mCarrier> carriers = new List<mCarrier>();

            if (conn.conectar())
            {
                String query = "SELECT * FROM fnLstCarriers(@nombre, @estado) ORDER BY carrier";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("nombre", '%' + nombre.ToUpper() + '%'));
                parametros.Add(new SqlParameter("estado", estado));

                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    dt.TableName = "Listado";

                    foreach(DataRow dr in dt.Rows)
                    {
                        carriers.Add(new mCarrier() {
                            codigo = dr["codigo"].ToString(),
                            carrier = dr["carrier"].ToString(),
                            icono = dr["icono"].ToString(),
                            valor = Utils.Helper.StringToDouble( dr["valor"].ToString()),
                            porcentaje = dr["porcentaje"].ToString(),
                            estado = dr["estado"].ToString(),
                            imagen = dr["imagen"] != System.DBNull.Value ? (byte[]) dr["imagen"] : null
                        });
                    }
                }
                else
                {
                    dt = new DataTable("Listado");
                }
            }

            return carriers;
        }

        public Respuesta grabar()
        {
            DbConnection conn = new DbConnection(strConnection);
            Respuesta respuesta = new Respuesta
            {
                resultado = false,
                mensaje = ""
            };

            if (conn.conectar())
            {
                String query = "";

                if (codigo == "")
                {
                    query = "SELECT LPAD(COALESCE(MAX(crriNid::int)+1,1)::text,4,'0') AS codigo FROM tbAdsiCrrie";

                    DataTable dt = conn.ejecutaQuery(query);

                    foreach(DataRow dr in dt.Rows)
                    {
                        codigo = dr["codigo"].ToString();
                    }

                    query = "INSERT INTO tbAdsiCrrie (crriNid, crriNbr, crriIcn, crriPrc, crriPct, crriAtv, crriApi";
                    if (this.imagen != null)
                    {
                        query += ",crriImg) ";
                    }
                    else
                    {
                        query += ") ";
                    }

                    query += "VALUES(@codigo, @nombre, @acronimo, @precio, @porcentaje, @estado, @aplica";

                    if (this.imagen != null)
                    {
                        query += ", @imagen) ";
                    }
                    else
                    {
                        query += ") ";
                    }
                }
                else
                {
                    query = "UPDATE  tbAdsiCrrie " +
                            "SET     crriprc = @precio, " +
                            "        crripct = @porcentaje, " +
                            "        crriNbr = @nombre, " +
                            "        crriapi = @aplica, " +
                            "        crriatv = @estado, " +
                            "        crriicn = @acronimo ";

                    if (this.imagen != null)
                    {
                        query += ",crriimg = @imagen ";
                    }

                    query += "WHERE   crriNid = @codigo";
                }
                
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codigo", codigo));
                parametros.Add(new SqlParameter("nombre", nombre));
                parametros.Add(new SqlParameter("porcentaje", esPorcentaje));
                parametros.Add(new SqlParameter("precio", precio));
                parametros.Add(new SqlParameter("aplica", aplica));
                parametros.Add(new SqlParameter("estado", activo));
                parametros.Add(new SqlParameter("acronimo", acronimo));
                if (this.imagen != null)
                {
                    parametros.Add(new SqlParameter("imagen", this.imagen));
                }

                var result = conn.updateQuery(query, parametros);

                respuesta.resultado = result.resultado;
                respuesta.mensaje = result.detalle;
            }

            conn.cerrar();

            return respuesta;
        }
    }
}
