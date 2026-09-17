using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Empleados : EntidadBase
    {
        public String empdNid { get; set; }
        public String empdTip { get; set; }
        public String empdNom { get; set; }
        public String empdApe { get; set; }
        public String empdDir { get; set; } = "";
        public String empdTlf { get; set; } = "";
        public String empdCel { get; set; } = "";
        public String empdFna { get; set; } = null;
        public int empdChj { get; set; } = 0;
        public double empdSal { get; set; } = 0;
        public Boolean empdAtv { get; set; } = true;

        public Empleados(string strConnection) : base(strConnection)
        {

        }

        public Respuesta grabar(String accion)
        {
            Respuesta res = new Respuesta
            { 
                resultado = false,
                mensaje = ""
            };

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("dni", this.empdNid));
                parametros.Add(new SqlParameter("tipo", this.empdTip));
                parametros.Add(new SqlParameter("nombre", this.empdNom));
                parametros.Add(new SqlParameter("apellido", this.empdApe));
                parametros.Add(new SqlParameter("direccion", this.empdDir));
                parametros.Add(new SqlParameter("telefono", this.empdTlf));
                parametros.Add(new SqlParameter("celular", this.empdCel));
                SqlParameter pDate= new SqlParameter("fecha", SqlDbType.Date);
                pDate.Value = DateTime.Parse(this.empdFna);
                parametros.Add(pDate);
                parametros.Add(new SqlParameter("hijo", this.empdChj));
                parametros.Add(new SqlParameter("salario", this.empdSal));
                parametros.Add(new SqlParameter("activo", this.empdAtv));

                //parametros.Add(new SqlParameter("empdNid", this.empdNid));
                //parametros.Add(new SqlParameter("empdTip", this.empdTip));
                //parametros.Add(new SqlParameter("empdNom", this.empdNom));
                //parametros.Add(new SqlParameter("empdApe", this.empdApe));
                //parametros.Add(new SqlParameter("empdDir", this.empdDir));
                //parametros.Add(new SqlParameter("empdTlf", this.empdTlf));
                //parametros.Add(new SqlParameter("empdCel", this.empdCel));
                //SqlParameter pDate = new SqlParameter("empdFna", SqlDbType.Date);
                //pDate.Value = DateTime.Parse(this.empdFna);
                //parametros.Add(pDate);
                //parametros.Add(new SqlParameter("empdChj", this.empdChj));
                //parametros.Add(new SqlParameter("empdSal", this.empdSal));
                //parametros.Add(new SqlParameter("empdAtv", this.empdAtv));

                string query = "SELECT * FROM fnEmpleados(@accion, @dni, @tipo, @nombre, @apellido, @direccion, @telefono, @celular, @fecha::character varying, @hijo, @salario::numeric, @activo::boolean)";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    res.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1 ? true : false;
                    res.mensaje = dt.Rows[0]["descripcion"].ToString();
                }
                //if (accion.Equals("N"))
                //{
                //    String query = "INSERT INTO tbAdsiEmpd VALUES (@empdNid, @empdTip, @empdNom, @empdApe, @empdDir, @empdTlf, @empdCel, @empdFna, @empdChj, @empdAtv)";

                //    var result = conn.updateQuery(query, parametros);

                //    res.resultado = result.resultado;

                //    if (result.detalle.Contains("llave duplicada"))
                //    {
                //        result.detalle = "El número de identificación del empleado ya existe, verifique";
                //    }
                //    res.mensaje = result.detalle;
                //} else{
                //    String query = "UPDATE tbAdsiEmpd SET empdTid=@empdTip, empdNbr=@empdNom, empdApe=@empdApe, empdDir=@empdDir, empdTlf=@empdTlf, empdCel=@empdCel, empdFna=@empdFna, empdChj=@empdChj, empdAtv=@empdAtv WHERE empdNid=@empdNid";

                //    var result = conn.updateQuery(query, parametros);
                //    res.resultado = result.resultado;
                //    res.mensaje = result.detalle;
                //}

                conn.cerrar();
            }

            return res;
        }

        public Respuesta pagarSalario(string codigo, string motivo, int anio, int mes, int dia, double valor, 
            string usuario, string codapro, int origen, string caja)
        {
            Respuesta res = new Respuesta
            {
                resultado = false,
                mensaje = ""
            };

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codemp", codigo));
                parametros.Add(new SqlParameter("caja", caja));
                parametros.Add(new SqlParameter("motivo", motivo));
                parametros.Add(new SqlParameter("anio", anio));
                parametros.Add(new SqlParameter("mes", mes));
                parametros.Add(new SqlParameter("dia", dia));
                parametros.Add(new SqlParameter("valor", valor));
                parametros.Add(new SqlParameter("usuario", usuario));
                parametros.Add(new SqlParameter("codapro", codapro));
                parametros.Add(new SqlParameter("origen", origen));
                parametros.Add(new SqlParameter("caja", caja));

                string query = "SELECT * FROM fnPagoSalario(@codemp::CHARACTER VARYING, @motivo::CHARACTER VARYING, " +
                               "@anio::INT, @mes::INT, @dia::INT, @valor::NUMERIC, @usuario::CHARACTER VARYING, @codapro::CHARACTER VARYING, " +
                               "@origen::INT, @caja::CHARACTER VARYING)";

                var dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    dt.TableName = "Listado";

                    res.resultado = dt.Rows[0]["codigo"].ToString() == "1" ? true : false;
                    res.mensaje = dt.Rows[0]["descripcion"].ToString();
                }
                else
                {
                    res.mensaje = "Hubo un error al momento de registrar el pago";
                }

                conn.cerrar();
            }

            return res;
        }

        public DataTable listar()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT * FROM tbAdsiEmpd WHERE empdAtv = true";
                
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

        public DataTable lista()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT empdNid AS codigo, (empdApe || ' ' || empdNbr) AS descripcion FROM tbAdsiEmpd WHERE empdAtv = true";

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

        public DataTable buscar(String apellido, string estado)
        {            
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT * FROM fnLstEmpd(@apellido, @estado) ORDER BY empleado";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("apellido", '%'+ apellido + '%'));
                parametros.Add(new SqlParameter("estado", estado));

                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    dt.TableName = "Listado";
                }else{
                    dt = new DataTable("Listado");
                }
            }

            return dt;
        }

        public List<msEmpleado> buscarEmpleado(String apellido, string estado)
        {
            DbConnection conn = new DbConnection(strConnection);
            List<msEmpleado> respuesta = new List<msEmpleado>();

            if (conn.conectar())
            {
                string query = "SELECT * FROM fnLstEmpd(@apellido, @estado) ORDER BY empleado";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("apellido", '%' + apellido + '%'));
                parametros.Add(new SqlParameter("estado", estado));

                DataTable dt = null;
                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    dt.TableName = "Listado";

                    foreach(DataRow dr in dt.Rows)
                    {
                        respuesta.Add(new msEmpleado {
                            codigo = dr["codigo"].ToString(),
                            empleado = dr["empleado"].ToString(),
                            telefono = dr["telefono"].ToString(),
                            tipo = dr["tipo"].ToString(),
                            estado = dr["estado"].ToString()
                        });
                    }
                }
            }

            return respuesta;
        }

        public mEmpleadoSalario buscarEmpleadoSalario(string codigo, int anio, int mes)
        {
            DbConnection conn = new DbConnection(strConnection);
            mEmpleadoSalario respuesta = new mEmpleadoSalario();

            if (conn.conectar())
            {
                string query = "SELECT * FROM fnInfoPagaSalario(@codigo, @anio, @mes)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codigo", codigo));
                parametros.Add(new SqlParameter("anio", anio));
                parametros.Add(new SqlParameter("mes", mes));

                DataTable dt = null;
                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    dt.TableName = "Listado";

                    foreach (DataRow dr in dt.Rows)
                    {
                        respuesta= new mEmpleadoSalario
                        {
                            codigo = dr["empdnid"].ToString(),
                            salario = double.Parse(dr["empdsal"].ToString()),
                            descuento = double.Parse(dr["dctoval"].ToString()),
                            valor = double.Parse(dr["pgosval"].ToString()),
                            saldo = double.Parse(dr["pgossal"].ToString()),
                            saldoPrevio = double.Parse(dr["pgosspr"].ToString())
                        };
                    }
                }
            }

            return respuesta;
        }

        public Boolean getById()
        {
            Boolean res = false;
            DbConnection conn = new DbConnection(strConnection);            ;
            if (conn.conectar())
            {
                String query = "SELECT * FROM tbAdsiEmpd WHERE empdNid = @empdNid";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("empdNid", this.empdNid));

                DataTable dt = null;

                dt = conn.ejecutaQuery(query, parametros);
                dt.TableName = "Listado";
                conn.cerrar();

                if (dt.Rows.Count > 0)
                {
                    this.empdNid = dt.Rows[0]["empdNid"].ToString();
                    this.empdTip = dt.Rows[0]["empdTid"].ToString();
                    this.empdNom = dt.Rows[0]["empdNbr"].ToString();
                    this.empdApe = dt.Rows[0]["empdApe"].ToString();
                    this.empdDir = dt.Rows[0]["empdDir"].ToString();
                    this.empdTlf = dt.Rows[0]["empdTlf"].ToString();
                    this.empdCel = dt.Rows[0]["empdCel"].ToString();
                    this.empdFna = dt.Rows[0]["empdFna"].ToString();
                    this.empdChj = int.Parse(dt.Rows[0]["empdChj"].ToString());
                    this.empdSal = double.Parse(dt.Rows[0]["empdSal"].ToString());
                    this.empdAtv = ((Boolean) dt.Rows[0]["empdAtv"]);

                    res = true;
                }                
            }

            return res;
        }
    }
}
