using AccesoDatos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Grupos : EntidadBase
    {
        public string grpCod { get; set; }
        public string grpNbr { get; set; }
        public Boolean grpAtv { get; set; }

        public Grupos(string strConnection) : base(strConnection)
        {
        }

        public Respuesta grabar(String accion)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();                

                parametros.Add(new SqlParameter("grpCod", this.grpCod));
                parametros.Add(new SqlParameter("grpNbr", this.grpNbr));
                parametros.Add(new SqlParameter("grpAtv", this.grpAtv));

                if (accion.Equals("N"))
                {
                    List<SqlParameter> parametros1 = new List<SqlParameter>();
                    parametros1.Add(new SqlParameter("grpCod", this.grpCod));

                    String query = "SELECT * FROM tbIvtrGrp WHERE grpCod = @grpCod";

                    DataTable dt = conn.ejecutaQuery(query, parametros1);

                    if (dt != null)
                    {
                        res.resultado = false;
                        res.mensaje = "Datos de Grupo de productos no se pudieron grabar, " + ((char)13) +
                            "El código ingresado ya existe";
                        return res;
                    }

                    conn.cerrar();

                    if (conn.conectar())
                    {
                        query = "INSERT INTO tbIvtrGrp VALUES (@grpCod, @grpNbr, @grpAtv)";

                        var result = conn.updateQuery(query, parametros);
                        res.resultado = result.resultado;
                        res.mensaje = result.detalle;

                        if (res.resultado)
                        {
                            res.mensaje = "Datos del Grupo de productos grabados correctamente";
                        }
                        else
                        {
                            res.mensaje = "Los datos del Grupo de producto no pudieron ser grabados!, " + ((char)13) + "comuniquese con el administrador!";
                        }
                    }
                }
                else
                {
                    String query = "UPDATE tbIvtrGrp SET grpNbr=@grpNbr, grpAtv=@grpAtv WHERE grpCod=@grpCod";

                    var result = conn.updateQuery(query, parametros);
                    res.resultado = result.resultado;
                    res.mensaje = result.detalle;

                    if (res.resultado)
                    {
                        res.mensaje = "Datos del Grupo de productos grabados correctmente";
                    }
                    else
                    {
                        res.mensaje = "Los datos del Grupo de producto no pudieron ser grabados!, " + ((char)13) + "comuniquese con el administrador!";
                    }                    
                }

                conn.cerrar();
            }

            return res;
        }

        public DataTable listar(int estado = 0)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                string query = "SELECT grpCod AS codigo, grpNbr as descripcion, (CASE WHEN grpAtv = TRUE THEN 'ACTIVO' ELSE 'INACTIVO' END) AS estado FROM tbIvtrGrp ";
                
                switch(estado)
                {
                    case 1:
                        query += "WHERE grpAtv = true ORDER BY grpCod";
                        break;
                    case 2:
                        query += "WHERE grpAtv = false ORDER BY grpCod";
                        break;
                    default:
                        query += "ORDER BY grpCod";
                        break;
                }

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

        public Boolean getById()
        {
            Boolean res = false;
            DbConnection conn = new DbConnection(strConnection); ;
            if (conn.conectar())
            {
                String query = "SELECT * FROM tbIvtrGrp WHERE grpCod = @grpCod";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("grpCod", this.grpCod));

                DataTable dt = null;

                dt = conn.ejecutaQuery(query, parametros);
                dt.TableName = "Listado";
                conn.cerrar();

                if (dt.Rows.Count > 0)
                {
                    this.grpCod = dt.Rows[0]["grpCod"].ToString();
                    this.grpNbr = dt.Rows[0]["grpNbr"].ToString();
                    this.grpAtv = (Boolean) dt.Rows[0]["grpAtv"];

                    res = true;
                }
            }

            return res;
        }
    }
}
