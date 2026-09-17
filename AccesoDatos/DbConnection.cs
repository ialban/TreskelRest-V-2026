using Microsoft.Data.SqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace AccesoDatos
{
    public class Resultado
    {
        public bool resultado { get; set; }
        public string detalle { get; set; }
    }

    public class DbConnection: IDisposable
    {
        private NpgsqlConnection _conn;
        private bool disposedValue;

        public DbConnection(String strConnection)
        {            
            this._conn = new NpgsqlConnection(strConnection);            
        }

        public Boolean conectar()
        {
            try
            {
                this._conn.Open();
                return true;
            }
            catch(NpgsqlException e)
            {
                return false;
            }catch(Exception e)
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

            if (_conn.State == System.Data.ConnectionState.Open)
            {
                NpgsqlCommand cmd = new NpgsqlCommand(query, _conn);

                try
                {
                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        dt = new DataTable();
                        dt.Load(dr);
                    }
                    dr.Close();
                }
                catch (NpgsqlException e)
                {
                    Console.Write(e.Message);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                        _conn.Close();
                }
            }

            return dt;
        }

        public Resultado updateQuery(string query, List<SqlParameter> parametros)
        {
            Resultado resultado = new Resultado { 
                resultado = false,
                detalle  = ""
            };

            if (_conn.State == System.Data.ConnectionState.Open)
            {
                NpgsqlCommand cmd = new NpgsqlCommand(query, _conn);
                
                foreach (SqlParameter parametro in parametros)
                {
                    cmd.Parameters.Add(new NpgsqlParameter(parametro.ParameterName, parametro.Value));
                }

                NpgsqlTransaction tr = null;
                try
                {
                    tr = _conn.BeginTransaction();
                    int res=cmd.ExecuteNonQuery();
                    
                    if (res>0)
                    {
                        resultado.resultado = true;
                        tr.Commit();
                    }else
                    {
                        tr.Rollback();
                    }
                }
                catch (NpgsqlException e)
                {
                    if (tr != null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }

                    resultado.detalle = e.Message;
                }
                catch (Exception e)
                {
                    if (tr != null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }

                    resultado.detalle = e.Message;
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                        _conn.Close();
                }
            }

            return resultado;
        }

        public DataTable ejecutaQuery(string query,List<SqlParameter> parametros)
        {
            DataTable dt = null;
            
            if (_conn.State == System.Data.ConnectionState.Open)
            {
                NpgsqlCommand cmd = new NpgsqlCommand(query, _conn);

                foreach(SqlParameter parametro in parametros)
                {
                    cmd.Parameters.Add(new NpgsqlParameter(parametro.ParameterName,parametro.Value));
                }

                try
                {
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    
                    if (dr.HasRows)
                    {
                        dt = new DataTable();
                        dt.Load(dr);                        
                    }
                    dr.Close();

                    cmd.Dispose();
                }
                catch (NpgsqlException e)
                {
                    Console.Write(e.Message);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                        _conn.Close();
                }
            }

            return dt;
        }

        public Object getParametroValue(int codigo)
        {
            Object valor = null;

            if (_conn.State == System.Data.ConnectionState.Open)
            {
                String query = "select desencryptar(confval) as valor from tbconfparam where confnid=@valor";
                NpgsqlCommand cmd = new NpgsqlCommand(query, _conn);

                cmd.Parameters.Add(new NpgsqlParameter("valor", codigo));

                try
                {
                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.NextResult())
                        {
                            valor = dr["valor"];
                        }

                        dr.Close();
                    }

                    cmd.Dispose();
                }
                catch (NpgsqlException e)
                {
                    Console.Write(e.Message);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
                finally
                {
                    if (_conn.State == ConnectionState.Open)
                        _conn.Close();
                }
            }

            return valor;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminar el estado administrado (objetos administrados)
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~DbConnection()
        // {
        //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
