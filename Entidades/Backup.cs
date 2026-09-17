using AccesoDatos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class BkpDemand
    {
        public int bkpNid { get; set; }
        public bool bkpAtv { get; set; }
        public string bkpCmt { get; set; }
        public int bkpCAc { get; set; } // Cantidad Procesos o Acciones a ejecutar
        public int bkpCPg { get; set; } // Cantidad de Procesos ejecutados (Progreso)
    }

    public class Backup: EntidadBase
    {
        public string horaBackup { get; set; } = "00:00";
        public string horaBackup2 { get; set; } = "";
        public string horaBackup3 { get; set; } = "";
        public string cntFiles { get; set; } = "4";
        public Boolean envMail { get; set; } = false;
        public Boolean UploadFile { get; set; } = false;
        public string server { get; set; } = "";
        public int puerto { get; set; } = 21;
        public string usuario { get; set; } = "";
        public string clave { get; set; } = "";
        public int controlSecuencia { get; set; } = 1;
        public string SeguridadElimarFacturas { get; set; } = "N";

        private int cntProgreso = 3;

        public Backup(string strConnection) : base(strConnection)
        {

        }

        public void load()
        {
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT  confNid AS codigo, desencryptar(confVal) AS valor " +
                               "FROM    tbConfParam " + 
                               "WHERE   confNid IN (29, 30, 31, 33, 34, 35, 36, 37, 38, 143)";
                
                DataTable dt = conn.ejecutaQuery(query);

                if (dt != null)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        if ((int)dr["codigo"] == 29)
                            this.horaBackup = dr["valor"].ToString();

                        if ((int)dr["codigo"] == 30)
                            this.cntFiles = dr["valor"].ToString();

                        if ((int)dr["codigo"] == 31)
                            this.envMail = dr["valor"].ToString() == "1" ? true : false;

                        if ((int)dr["codigo"] == 33)
                            this.UploadFile = dr["valor"].ToString() == "1" ? true : false;

                        if ((int)dr["codigo"] == 34)
                            this.server = dr["valor"].ToString();

                        if ((int)dr["codigo"] == 35)
                            this.usuario = dr["valor"].ToString();

                        if ((int)dr["codigo"] == 36)
                            this.clave = dr["valor"].ToString();

                        if ((int)dr["codigo"] == 37)
                            this.horaBackup2 = dr["valor"].ToString();

                        if ((int)dr["codigo"] == 38)
                            this.horaBackup3 = dr["valor"].ToString();

                        if ((int)dr["codigo"] == 114)
                            this.puerto = !string.IsNullOrEmpty(dr["valor"].ToString()) ? int.Parse(dr["valor"].ToString()) : 21;

                        if ((int)dr["codigo"] == 115)
                            this.controlSecuencia = int.Parse(dr["valor"].ToString());

                        if ((int)dr["codigo"] == 143)
                            this.SeguridadElimarFacturas = dr["valor"].ToString();
                    }
                }
            }
        }

        public Respuesta grabar()
        {
            Respuesta respuesta = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "UPDATE 	tbConfParam " +
                               "SET     confval = a.valor " +
                               "FROM " +
                               "( " +
                               "    SELECT @cod29 AS codigo, encryptar(@hora) AS valor " +
                               "    UNION " +
                               "    SELECT @cod30 AS codigo, encryptar(@cantidad) AS valor " +
                               "    UNION " +
                               "    SELECT @cod31 AS codigo, encryptar(@mail) AS valor " +
                               "    UNION " +
                               "    SELECT @cod33 AS codigo, encryptar(@upload) AS valor " +
                               "    UNION " +
                               "    SELECT @cod34 AS codigo, encryptar(@server) AS valor " +
                               "    UNION " +
                               "    SELECT @cod35 AS codigo, encryptar(@usuario) AS valor " +
                               "    UNION " +
                               "    SELECT @cod36 AS codigo, encryptar(@clave) AS valor " +
                               "    UNION " +
                               "    SELECT @cod37 AS codigo, encryptar(@hora2) AS valor " +
                               "    UNION " +
                               "    SELECT @cod38 AS codigo, encryptar(@hora3) AS valor " +
                               ") a " +
                               "WHERE   confnid = a.codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("cod29", 29));
                parametros.Add(new SqlParameter("hora", this.horaBackup));
                parametros.Add(new SqlParameter("cod30", 30));
                parametros.Add(new SqlParameter("cantidad", this.cntFiles));
                parametros.Add(new SqlParameter("cod31", 31));
                parametros.Add(new SqlParameter("mail", this.envMail == true? "1": "0"));
                parametros.Add(new SqlParameter("cod33", 33));
                parametros.Add(new SqlParameter("upload", this.UploadFile == true ? "1" : "0"));
                parametros.Add(new SqlParameter("cod34", 34));
                parametros.Add(new SqlParameter("server", this.server));
                parametros.Add(new SqlParameter("cod35", 35));
                parametros.Add(new SqlParameter("usuario", this.usuario));
                parametros.Add(new SqlParameter("cod36", 36));
                parametros.Add(new SqlParameter("clave", this.clave));
                parametros.Add(new SqlParameter("cod37", 37));
                parametros.Add(new SqlParameter("hora2", this.horaBackup2));
                parametros.Add(new SqlParameter("cod38", 38));
                parametros.Add(new SqlParameter("hora3", this.horaBackup3));

                var result= conn.updateQuery(query, parametros);

                if (result.resultado)
                {
                    respuesta.resultado = true;
                    respuesta.mensaje = "Configuración de backup realizada correctamente";
                }
                else
                {
                    respuesta.resultado = false;
                    respuesta.mensaje = "Configuración de backup no pudo ser realizada";
                }
            }
            conn.cerrar();
            return respuesta;
        }

        public Respuesta registraLog(string descripcion, string usuario, string dispositivo)
        {
            Respuesta respuesta = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT * FROM fnLogger(@tip, @desc, @usr, @disp) ";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("tip", "BKP"));
                parametros.Add(new SqlParameter("desc", descripcion));
                parametros.Add(new SqlParameter("usr", usuario));
                parametros.Add(new SqlParameter("disp", dispositivo));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach(DataRow dr in dt.Rows)
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

        public int StartBackup(string origen)
        {
            int Id = -1;
            Respuesta respuesta = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT COALESCE(MAX(bkpNid) + 1, 1) AS Id FROM tbBkpRqsts ";

                DataTable dt = conn.ejecutaQuery(query);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Id = (int)dr["Id"];
                    }
                }
                
                if (Id > 0)
                {
                    query = "INSERT INTO tbBkpRqsts (bkpNid, cajacod, bkpAtv, bkpCAc) VALUES(@Id, @origen, false, @cantidad)";

                    List<SqlParameter> parametros = new List<SqlParameter>();

                    parametros.Add(new SqlParameter("Id", Id));
                    parametros.Add(new SqlParameter("origen", origen));
                    parametros.Add(new SqlParameter("cantidad", cntProgreso));

                    var result = conn.updateQuery(query, parametros);

                    if (result.resultado)
                        return Id;
                    else
                        return -1;
                }
            }
            conn.cerrar();
            return Id;
        }

        public void SetStartOnDemand()
        {
            Parametros ObjParam = new Parametros(strConnection);

            DataTable dt = ObjParam.ParametrosBackup();
            bool opcion = false;

            foreach (DataRow dr in dt.Rows)
            {
                switch ((int)dr["codigo"])
                {
                    case 31:
                        opcion = dr["valor"].ToString() == "1" ? true : false;
                        if (opcion) cntProgreso++;
                        break;
                    case 33:
                        opcion = dr["valor"].ToString() == "1" ? true : false;
                        if (opcion) cntProgreso++;
                        break;
                }
            }

            var iniciado = ObjParam.SetEjecutaBackup(true);
        }

        public void setStopOnDemand()
        {
            Parametros ObjParam = new Parametros(strConnection);

            ObjParam.SetEjecutaBackup(false);
        }

        public BkpDemand GetBkpDemand()
        {
            BkpDemand bkp = new BkpDemand();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                //String query = "SELECT bkpNid, bkpAtv, bkpCmt, bkpCAc, bkpCPg FROM tbBkpRqsts ORDER BY bkpNid DESC limit 1";
                string query = "SELECT bkpNid, bkpAtv, bkpCmt, 0 AS bkpCAc, bkpCPg FROM tbBkpRqsts ORDER BY bkpNid DESC limit 1";

                DataTable dt = conn.ejecutaQuery(query);

                if (dt != null)
                {

                    foreach (DataRow dr in dt.Rows)
                    {
                        bkp.bkpNid = int.Parse(dr["bkpNid"].ToString());
                        bkp.bkpAtv = (bool)dr["bkpAtv"];
                        bkp.bkpCmt = dr["bkpCmt"].ToString();
                        bkp.bkpCAc = int.Parse(dr["bkpCAc"].ToString());
                        bkp.bkpCPg = int.Parse(dr["bkpCPg"].ToString());
                    }
                }
            }

            conn.cerrar();
            return bkp;
        }

        public BkpDemand UpdateBkpDemand(BkpDemand value)
        {
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "UPDATE tbBkpRqsts SET bkpAtv = @estado, bkpCmt = @comment, bkpCPg = @progreso WHERE bkpNid = @Id";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("Id", value.bkpNid));
                parametros.Add(new SqlParameter("estado", value.bkpAtv));
                parametros.Add(new SqlParameter("comment", value.bkpCmt));
                parametros.Add(new SqlParameter("progreso", value.bkpCPg));

                var res = conn.updateQuery(query, new List<SqlParameter>());

                if (res.resultado)
                {
                    conn.cerrar();
                    return value;
                }
            }

            conn.cerrar();
            return null;
        }

        public void UpdateControlSecuencia(int numero)
        {
            DbConnection dbConnection = new DbConnection(this.strConnection);
            if (dbConnection.conectar())
            {
                string query = "UPDATE tbConfParam SET confval = encryptar(@numero) WHERE confnid = 115";
                dbConnection.updateQuery(query, new List<SqlParameter>()
                {
                    new SqlParameter(nameof (numero), (object) numero.ToString())
                });
            }
            dbConnection.cerrar();
        }

        public void CheckCaducidadWhatsappCoords()
        {
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT * FROM fnCheckCaducidadMsgsCoords();";

                DataTable dt = conn.ejecutaQuery(query);
            }

            conn.cerrar();
        }
    }
}
