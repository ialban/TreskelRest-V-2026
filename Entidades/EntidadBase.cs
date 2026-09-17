using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Entidades
{
    public class EntidadBase: IDisposable
    {
        private bool disposedValue;

        protected String strConnection { get; set; }

        public EntidadBase(String strConnection)
        {
            this.strConnection = strConnection;
        }

        public Respuesta clearDatabase(bool caja, bool cierre, bool inventario, bool items, bool pedidos, bool usuarios, bool ventas, bool empresa)
        {
            Respuesta respuesta = new Respuesta();

            respuesta.resultado = false;
            respuesta.mensaje = "La Data no pudo ser eliminada";

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT * FROM fnresetdb(@caja, @cierre, @inventario, @items, @pedidos, @usuarios, @ventas, @empresa)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("caja", caja));
                parametros.Add(new SqlParameter("cierre", cierre));
                parametros.Add(new SqlParameter("inventario", inventario));
                parametros.Add(new SqlParameter("items", items));
                parametros.Add(new SqlParameter("pedidos", pedidos));
                parametros.Add(new SqlParameter("usuarios", usuarios));
                parametros.Add(new SqlParameter("ventas", ventas));
                parametros.Add(new SqlParameter("empresa", empresa));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    respuesta.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1 ? true : false;
                    respuesta.mensaje = dt.Rows[0]["descripcion"].ToString();
                }

                conn.cerrar();
            }

            return respuesta;
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
        // ~EntidadBase()
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
