using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace AccesoDatos
{
    public class DbSqlConnection
    {
        private SqlConnection _conn;

        public DbSqlConnection(string strConnection)
        {
            this._conn = new SqlConnection(strConnection);

        }

        public Boolean conectar()
        {
            try
            {
                this._conn.Open();
                return true;
            }catch(SqlException e)
            {
                return false;
            }
        }

        public void cerrar()
        {
            if(_conn.State == System.Data.ConnectionState.Open)
            {
                _conn.Close();
            }
        }

        public DataTable ejecutaQuery(string query)
        {
            DataTable dt = null;

            if(_conn.State == ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand(query, _conn);

                SqlDataReader dr = null;
                try
                {
                    dr= cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        dt = new DataTable();
                        dt.Load(dr);                        
                    }

                    dr.Close();
                }
                catch(SqlException e)
                {
                    if (dr!=null)
                    {
                        dr.Close();
                    }
                }
            }

            return dt;
        }

        public Boolean updateQuery(string query, List<SqlParameter> parametros)
        {
            Boolean resultado = false;

            if (_conn.State == System.Data.ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand(query, _conn);

                SqlCommand _cmd = new SqlCommand();

                foreach (SqlParameter parametro in parametros)
                {
                    cmd.Parameters.Add(new SqlParameter(parametro.ParameterName, parametro.Value));
                }

                try
                {
                    SqlTransaction tr = _conn.BeginTransaction();
                    int res = cmd.ExecuteNonQuery();

                    if (res > 0)
                    {
                        resultado = true;
                        tr.Commit();
                    }
                    else
                    {
                        tr.Rollback();
                    }
                }
                catch (SqlException e)
                {
                    resultado = false;
                }
            }

            return resultado;
        }

        public DataTable ejecutaQuery(string query, List<SqlParameter> parametros)
        {
            DataTable dt = null;

            if (_conn.State == System.Data.ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand(query, _conn);

                foreach (SqlParameter parametro in parametros)
                {
                    cmd.Parameters.Add(new SqlParameter(parametro.ParameterName, parametro.Value));
                }

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        dt = new DataTable();
                        dt.Load(dr);                        
                    }
                    dr.Close();

                    cmd.Dispose();
                }
                catch (SqlException e)
                {
                    Console.Write(e.Message);
                }
            }

            return dt;
        }
    }
}
