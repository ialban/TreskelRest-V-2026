using AccesoDatos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades 
{
    public class Proveedor : EntidadBase
    {
        public String provNid { get; set; }
        public String provNbr { get; set; }
        public String provDir { get; set; }
        public String provTlf { get; set; } = "";
        public String provEml { get; set; } = "";
        public String provCto { get; set; } = "";
        public Boolean provAtv { get; set; } = true;


        public Proveedor(string strConnection) : base(strConnection)
        {
        }

        public Respuesta grabar(String accion)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("provNid", this.provNid));
                parametros.Add(new SqlParameter("provNbr", this.provNbr));
                parametros.Add(new SqlParameter("provDir", this.provDir));
                parametros.Add(new SqlParameter("provCto", this.provCto));
                parametros.Add(new SqlParameter("provTlf", this.provTlf));
                parametros.Add(new SqlParameter("provEml", this.provEml));
                parametros.Add(new SqlParameter("provAtv", this.provAtv));

                if (accion.Equals("N"))
                {
                    List<SqlParameter> parametros1 = new List<SqlParameter>();
                    parametros1.Add(new SqlParameter("provNid", this.provNid));

                    String query = "SELECT * FROM tbIvtrProv WHERE provNid = @provNid";

                    DataTable dt = conn.ejecutaQuery(query, parametros1);

                    if (dt != null)
                    {
                        res.resultado = false;
                        res.mensaje = "Datos del Proveedodr no se pudieron grabar, " + ((char)13) +
                            "El Ruc ingresado ya existe";
                        return res;
                    }

                    conn.cerrar();

                    if (conn.conectar())
                    {
                        query = "INSERT INTO tbIvtrProv VALUES (@provNid, @provNbr, @provDir, @provTlf, @provEml, @provCto, @provAtv)";

                        var result = conn.updateQuery(query, parametros);
                        res.resultado = result.resultado;
                        if (res.resultado)
                        {
                            res.mensaje = "Datos del Proveedor grabados correctamente";
                        }
                        else
                        {
                            res.mensaje = "Los datos del Proveedor no pudieron ser grabados!, " + ((char)13) + "comuniquese con el administrador!";
                        }
                    }
                }
                else
                {
                    String query = "UPDATE tbIvtrProv SET provNbr=@provNbr, provDir=@provDir, provTlf=@provTlf, provEml=@provEml, provCto=@provCto, provAtv=@provAtv WHERE provNid=@provNid";

                    var result = conn.updateQuery(query, parametros);
                    res.resultado = result.resultado;
                    if (res.resultado)
                    {
                        res.mensaje = "Datos del Proveedor grabados correctmente";
                    }
                    else
                    {
                        res.mensaje = "Los datos del Proveedor no pudieron ser grabados!, " + ((char)13) + "comuniquese con el administrador!";
                    }
                }
            }

            return res;
        }

        public DataTable listar()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                string query = "SELECT provNid AS codigo, provNbr AS descripcion FROM tbIvtrProv WHERE provAtv = true ORDER BY provNbr";

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

        public DataTable buscar(String nombre, string estado)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT * FROM fnLstProv(@provNbr, @provAtv) ORDER BY proveedor";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("provNbr", '%' + nombre + '%'));
                parametros.Add(new SqlParameter("provAtv", estado));

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
                String query = "SELECT * FROM tbIvtrProv WHERE provNid = @provNid AND provatv = true";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("provNid", this.provNid));

                DataTable dt = null;

                dt = conn.ejecutaQuery(query, parametros);
                if (dt == null) return false;

                dt.TableName = "Listado";
                conn.cerrar();

                if (dt.Rows.Count > 0)
                {
                    this.provNid = dt.Rows[0]["provNid"].ToString();
                    this.provNbr = dt.Rows[0]["provNbr"].ToString();
                    this.provDir = dt.Rows[0]["provDir"].ToString();
                    this.provTlf = dt.Rows[0]["provTlf"].ToString();
                    this.provCto = dt.Rows[0]["provCto"].ToString();
                    this.provEml = dt.Rows[0]["provEml"].ToString();
                    this.provAtv = ((Boolean)dt.Rows[0]["provAtv"]);                    

                    res = true;
                }
            }

            return res;
        }
    }
}
