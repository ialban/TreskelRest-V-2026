using AccesoDatos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class SubGrupos : EntidadBase
    {
        public string sgrpCod { get; set; }
        public string sgrpNbr { get; set; }
        public string grpCod { get; set; }

        public SubGrupos(string strConnection) : base(strConnection)
        {
        }

        public Respuesta grabar(String accion)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("sgrpCod", this.sgrpCod));
                parametros.Add(new SqlParameter("sgrpNbr", this.sgrpNbr));
                parametros.Add(new SqlParameter("grpCod", this.grpCod));

                if (accion.Equals("N"))
                {
                    List<SqlParameter> parametros1 = new List<SqlParameter>();
                    parametros1.Add(new SqlParameter("sgrpCod", this.sgrpCod));

                    String query = "SELECT * FROM tbIvtrSgrp WHERE grpCod = @sgrpCod ORDER BY sgrpCod";

                    DataTable dt = conn.ejecutaQuery(query, parametros1);

                    if (dt != null)
                    {
                        res.resultado = false;
                        res.mensaje = "Datos de SubGrupo de productos no se pudieron grabar, " + ((char)13) +
                            "El código ingresado ya existe";
                        return res;
                    }

                    conn.cerrar();

                    if (conn.conectar())
                    {
                        query = "INSERT INTO tbIvtrSgrp VALUES (@sgrpCod, @sgrpNbr, @grpCod)";

                        var result = conn.updateQuery(query, parametros);

                        res.resultado = result.resultado;

                        if (res.resultado)
                        {
                            res.mensaje = "Datos del SubGrupo de productos grabados correctamente";
                        }
                        else
                        {
                            res.mensaje = "Los datos del SubGrupo de producto no pudieron ser grabados!, " + ((char)13) + "comuniquese con el administrador!";
                        }
                    }
                }
                else
                {
                    String query = "UPDATE tbIvtrSgrp SET sgrpNbr=@sgrpNbr, grpCod=@grpCod WHERE sgrpCod=@sgrpCod";

                    var result = conn.updateQuery(query, parametros);

                    res.resultado = result.resultado;
                    if (res.resultado)
                    {
                        res.mensaje = "Datos del SubGrupo de productos grabados correctmente";
                    }
                    else
                    {
                        res.mensaje = "Los datos del SubGrupo de producto no pudieron ser grabados!, " + ((char)13) + "comuniquese con el administrador!";
                    }
                }

                conn.cerrar();
            }

            return res;
        }

        public DataTable buscar(String nombre, string grupo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT * FROM fnLstSgrp(@sgrpNbr, @grpCod) ORDER BY codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("sgrpNbr", '%' + nombre + '%'));
                parametros.Add(new SqlParameter("grpCod", grupo));

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
                String query = "SELECT * FROM tbIvtrSgrp WHERE sgrpCod = @sgrpCod";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("sgrpCod", this.sgrpCod));

                DataTable dt = null;

                dt = conn.ejecutaQuery(query, parametros);
                dt.TableName = "Listado";
                conn.cerrar();

                if (dt.Rows.Count > 0)
                {
                    this.sgrpCod = dt.Rows[0]["sgrpCod"].ToString();
                    this.sgrpNbr = dt.Rows[0]["sgrpNbr"].ToString();
                    this.grpCod = dt.Rows[0]["grpCod"].ToString();

                    res = true;
                }
            }

            return res;
        }

        public DataTable listarByGroup(String grupo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT sgrpCod AS codigo, sgrpNbr as descripcion FROM tbIvtrSgrp WHERE grpCod = @grupo ORDER BY sgrpNbr";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("grupo", grupo));

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
    }
}
