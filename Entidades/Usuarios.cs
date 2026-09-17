using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Usuarios : EntidadBase
    {
        public string usuaCod { get; set; }
        public string usuaUsr { get; set; }
        public string usuaNbr { get; set; }
        public string usuaClv { get; set; }
        public string usuaCad { get; set; }
        public string usuaTip { get; set; }
        public string empdNid { get; set; }
        public string usuaAtv { get; set; }
        public string usuacoord { get; set; }

        public List<mPermiso> permisos { get; set; }

        public Usuarios(string strConnection) : base(strConnection)
        {
        }

        //public bool login(String usuario, String clave, Boolean claveNumerica)
        //{
        //    bool res = false;
        //    DbConnection conn = new DbConnection(strConnection);

        //    if (conn.conectar())
        //    {
        //        string columnaClave = (usuario == "admin" || !claveNumerica) ? "usuaclv" : "usuacnm";

        //        string query = $"SELECT 1 FROM tbadsiusua WHERE usuausr = @usuario AND desencryptar({columnaClave}) = @clave LIMIT 1";

        //        List<SqlParameter> parametros = new List<SqlParameter>
        //        {
        //            new SqlParameter("usuario", usuario),
        //            new SqlParameter("clave", clave)
        //        };

        //        DataTable dt = null;

        //        try
        //        {
        //            dt = conn.ejecutaQuery(query, parametros);
        //        }
        //        finally
        //        {
        //            conn.cerrar();
        //        }

        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            res = true;
        //        }
        //    }

        //    return res;
        //}

        public lUsuario autenticar(string usuario, string clave)
        {
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "SELECT codigo, username FROM fnlogin(@usuario, @clave) LIMIT 1";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("usuario", usuario),
                    new SqlParameter("clave", clave)
                };

                DataTable dt = null;

                try
                {
                    dt = conn.ejecutaQuery(query, parametros);
                }
                finally
                {
                    conn.cerrar();
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new lUsuario
                    {
                        codigo = row["codigo"]?.ToString(),
                        usuario = row["username"]?.ToString()
                    };
                }
            }

            return null;
        }


        public String getCodMesero(string clave, Boolean claveNumerica)
        {
            String codigo = "";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "";
                if (claveNumerica)
                {
                    //query = "SELECT usuaCod FROM tbadsiusua WHERE desencryptar(usuaCnm)=@clave AND usuaTip IN ('M','S','A','C') AND usuaatv = true";
                    query = "SELECT u.usuaCod, e.empdcel, " +
                            "       empdnbr || " +
                            "       (CASE " +
                            "           WHEN TRIM(COALESCE(empdape,'')) != '' THEN ' ' || empdape " +
                            "           ELSE '' " +
                            "       END) AS empdnbr " +
                            "FROM   tbadsiusua u " +
                            "       INNER JOIN tbAdsiEmpd e ON e.empdnid = u.empdnid " +
                            "WHERE  desencryptar(u.usuaCnm)= @clave " +
                            "       AND u.usuaTip IN('M','S','A','C') " +
                            "       AND u.usuaatv = true";
                }
                else
                {
                    //query = "SELECT usuaCod FROM tbadsiusua WHERE desencryptar(usuaclv)=@clave AND usuaTip IN ('M','S','A','C') AND usuaatv = true";
                    query = "SELECT u.usuaCod, e.empdcel, " +
                            "       empdnbr || " +
                            "       (CASE " +
                            "           WHEN TRIM(COALESCE(empdape,'')) != '' THEN ' ' || empdape " +
                            "           ELSE '' " +
                            "       END) AS empdnbr " +
                            "FROM   tbadsiusua u " +
                            "       INNER JOIN tbAdsiEmpd e ON e.empdnid = u.empdnid " +
                            "WHERE  desencryptar(u.usuaclv)= @clave " +
                            "       AND u.usuaTip IN('M','S','A','C') " +
                            "       AND u.usuaatv = true";
                }                    

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("clave", clave));

                DataTable dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    codigo = dt.Rows[0]["usuaCod"].ToString() + ";" + 
                             dt.Rows[0]["empdcel"].ToString() + ";" +
                             dt.Rows[0]["empdnbr"].ToString();
                }
            }

            return codigo;
        }

        public UsrEmpleado getEmpleadoMesero(string codigo)
        {
            UsrEmpleado clsUsr = new UsrEmpleado();
            
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT 	u.usuaUsr, TRIM(e.empdnbr || ' ' ||e.empdape) AS empdNbr " +
                               "FROM    tbadsiusua u " +
                               "        INNER JOIN " +
                               "        tbAdsiEmpd e ON e.empdNid = u.empdNid " +
                               "WHERE usuaCod = @codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codigo", codigo));

                DataTable dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    clsUsr.usrusr = dt.Rows[0]["usuaUsr"].ToString();
                    clsUsr.usrnbr = dt.Rows[0]["empdNbr"].ToString();
                }
            }

            return clsUsr;
        }

        public Boolean getById()
        {
            Boolean encontrado = false;

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT  usuaCod, usuaUsr, usuaNbr,  " +
                               "        CASE (SELECT desencryptar(confVal) FROM tbConfParam where confNid=20) " +
                               "            WHEN '1' THEN " +
                               "                desencryptar(usuaCnm) " + 
                               "            ELSE " +
                               "                desencryptar(usuaClv) " +
                               "        END AS usuaClv, " +
                               "        usuaTip, empdNid, usuaAtv, usuacoord " + 
                               "FROM    tbAdsiUsua " +
                               "WHERE   usuaCod = @codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codigo", this.usuaCod));

                DataTable dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    this.usuaNbr = dt.Rows[0]["usuaNbr"].ToString();
                    this.usuaUsr = dt.Rows[0]["usuaUsr"].ToString();
                    this.usuaClv = dt.Rows[0]["usuaClv"].ToString();
                    this.usuaNbr = dt.Rows[0]["usuaNbr"].ToString();
                    this.usuaTip = dt.Rows[0]["usuaTip"].ToString();
                    this.empdNid = dt.Rows[0]["empdNid"].ToString();
                    this.usuaAtv = dt.Rows[0]["usuaAtv"].ToString();
                    this.usuacoord = dt.Rows[0]["usuacoord"].ToString();

                    encontrado = true;
                }
            }

            return encontrado;
        }

        public List<mUsuario> getUsuarios(String empleado, String estado)
        {
            List<mUsuario> data = new List<mUsuario>();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "SELECT * FROM fnLstUsuarios(@empleado, @estado) ORDER BY codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("@empleado", empleado));
                parametros.Add(new SqlParameter("@estado", estado));

                DataTable dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null) { 
                    foreach(DataRow dr in dt.Rows)
                    {
                        data.Add(new mUsuario
                        {
                            codigo = dr["codigo"].ToString(),
                            usuario = dr["usuario"].ToString(),
                            empleado = dr["empleado"].ToString(),
                            tipo = dr["tipo"].ToString(),
                            estado = dr["estado"].ToString()
                        });                                                            
                    }
                }
            }

            return data;
        }

        public Respuesta grabar(string accion)
        {
            Respuesta respuesta = new Respuesta();

            respuesta.resultado = false;
            respuesta.mensaje = accion.Equals("N") ? "El usuario no pudo ser creado correctamente": 
                                                     "El usuario no pudo ser modificado";

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "SELECT * FROM fnUsuarios(@accion, @secuencial, @creador, @usuario, @clave, @detalle, @tipo, @empleado, @estado, @coords, @permisos)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("@accion", accion));
                parametros.Add(new SqlParameter("@secuencial", this.usuaCod));
                parametros.Add(new SqlParameter("@creador", this.usuaCad));                
                parametros.Add(new SqlParameter("@usuario", this.usuaUsr));
                parametros.Add(new SqlParameter("@clave", this.usuaClv));
                parametros.Add(new SqlParameter("@detalle", this.usuaNbr));
                parametros.Add(new SqlParameter("@tipo", this.usuaTip));
                parametros.Add(new SqlParameter("@empleado", this.empdNid));
                parametros.Add(new SqlParameter("@estado", this.usuaAtv));
                parametros.Add(new SqlParameter("@permisos", JsonConvert.SerializeObject(this.permisos)));
                parametros.Add(new SqlParameter("@coords", this.usuacoord == null ? "": this.usuacoord));

                DataTable dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    respuesta.resultado = dt.Rows[0]["codigo"].ToString().Equals("1")==true? true : false;
                    respuesta.mensaje = dt.Rows[0]["descripcion"].ToString();
                }
            }

            return respuesta;
        }

        public List<mPermiso> getPermisos()
        {
            List<mPermiso> data = new List<mPermiso>();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "SELECT codigo, tag, descripcion FROM fnLstPermisos()";

                DataTable dt = conn.ejecutaQuery(query);
                conn.cerrar();

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        data.Add(new mPermiso
                        {
                            codigo = dr["codigo"].ToString(),
                            tag = dr["tag"].ToString(),
                            descripcion = dr["descripcion"].ToString(),
                            estado = false
                        });
                    }
                }
            }

            return data;
        }

        public List<mPermiso> getPermisoUsuario(string usuario)
        {
            List<mPermiso> permiso = new List<mPermiso>();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT 	p.pmsocod AS codigo, " +
                               "        desencryptar(pmsoTag) AS tag, " +
                               "        p.pmsoDsc AS descripcion,  " +
                               "        COALESCE(" +
                               "             (SELECT DISTINCT uperatv  " +
                               "              FROM tbAdsiUsPer u  " +
                               "              WHERE u.usuaCod = @codigo  " +
                               "                    AND u.pmsocod = p.pmsocod)  " +
                               "        , false)	AS estado " +
                               "FROM    tbAdsiPmso p " +
                               "WHERE   p.pmsoAtv = true";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codigo", usuario));

                DataTable dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        permiso.Add(new mPermiso
                        {
                            codigo = dr["codigo"].ToString(),
                            tag = dr["tag"].ToString(),
                            descripcion = dr["descripcion"].ToString(),
                            estado = (Boolean) dr["estado"]
                        });
                    }
                    
                }
            }

            return permiso;
        }
    }

    public class UsrEmpleado
    {
        public string usrnbr { get; set; }
        public string usrusr { get; set; }
    }
}
