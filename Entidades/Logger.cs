using AccesoDatos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Logger : EntidadBase
    {

        public Logger(string strConnection) : base(strConnection)
        {

        }

        public Respuesta registraLog(string tipo, string descripcion, string usuario, string dispositivo)
        {
            Respuesta respuesta = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT * FROM fnLogger(@tip, @desc, @usr, @disp) ";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("tip", tipo));
                parametros.Add(new SqlParameter("desc", descripcion));
                parametros.Add(new SqlParameter("usr", usuario));
                parametros.Add(new SqlParameter("disp", dispositivo));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (int.Parse(dr["codigo"].ToString()) == 1)
                        {
                            respuesta.resultado = true;
                            respuesta.mensaje = "Configuración de backup realizada correctamente";
                        }
                        else
                        {
                            respuesta.resultado = false;
                            respuesta.mensaje = "Registro del backup no pudo ser registrado correctamente";
                        }

                    }
                }
                else
                {
                    respuesta.resultado = false;
                    respuesta.mensaje = "Registro del backup no pudo ser registrado correctamente";
                }
            }
            conn.cerrar();
            return respuesta;
        }
    }
}
