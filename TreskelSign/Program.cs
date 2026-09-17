using LibElectSign;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TreskelSign
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                global::java.util.logging.LogManager.getLogManager().reset();
                global::java.util.logging.Logger globalLogger = global::java.util.logging.Logger.getLogger(global::java.util.logging.Logger.GLOBAL_LOGGER_NAME);

                // Establecer el nivel a OFF para que no muestre nada
                globalLogger.setLevel(global::java.util.logging.Level.OFF);

                // Opcional: Silenciar específicamente el logger raíz
                global::java.util.logging.Logger.getLogger("").setLevel(global::java.util.logging.Level.OFF);
            }
            catch (Exception e){
                Console.WriteLine("Error de Inicializacion : " + e.Message);
            }

            string env = "";
            string ruta = "";
            string firma = "";
            string clave = "";

            var parametros = new Dictionary<string, string>();

            if (args.Length > 0)
            {
                Console.WriteLine("Se va a procesar los parámetros");

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("-") && i + 1 < args.Length)
                    {
                        parametros[args[i]] = args[i + 1];
                    }
                }

                ruta = parametros.ContainsKey("-x") ? parametros["-x"] : "No definida";
                firma = parametros.ContainsKey("-f") ? parametros["-f"] : "No definida";
                clave = parametros.ContainsKey("-p") ? parametros["-p"] : "No definida";
                env = (parametros.ContainsKey("-e") ? parametros["-e"] : "P").ToUpper();

                // 3. VALIDACIÓN PREVIA DE ARCHIVOS (Evita errores en LibElectSign)
                if (!File.Exists(ruta))
                {
                    Console.WriteLine($"ERROR: No se encontró el XML en {ruta}");
                    Finalizar(env); 
                    return 2;
                }
                if (!File.Exists(firma))
                {
                    Console.WriteLine($"ERROR: No se encontró el certificado en {firma}");
                    Finalizar(env); 
                    return 3;
                }

                Console.WriteLine($"Procesando archivo: {ruta}");
                Console.WriteLine($"Firma electrónica: {firma}");
                Console.WriteLine($"Clave: {clave}");
                Console.WriteLine($"Ambiente: {env}");

                try
                {
                    FirmarXML firmar = new FirmarXML();
                    firmar.xmlPath = ruta;
                    firmar.firmaPath = firma;
                    firmar.CertificadoClave = clave;

                    Console.WriteLine("Firmando...");
                    var res = firmar.SignXML();

                    if (res)
                    {
                        Console.WriteLine("SUCESO: El archivo se firmó correctamente.");
                        return 1;
                    }
                    else
                    {
                        Console.WriteLine("FALLO: La librería devolvió false (Posible error de credenciales o estructura).");
                        return 0;
                    }
                }
                catch(Exception e)
                {
                    // CAPTURA DETALLADA DE ERRORES (Clave para debuggear IKVM)
                    Console.WriteLine("*********************************************");
                    Console.WriteLine("ERROR CRÍTICO DURANTE LA FIRMA:");
                    Console.WriteLine(e.Message);
                    if (e.InnerException != null)
                        Console.WriteLine("Detalle Interno: " + e.InnerException.Message);
                    Console.WriteLine("*********************************************");
                    return 4;
                }
                
            }
            else
            {
                MostrarAyuda();
            }

            Finalizar(env);
            return -1;
        }

        private static void MostrarAyuda()
        {
            Console.WriteLine("Uso: TreskelSign.exe -x <ruta_xml> -f <ruta_firma> -p <clave> -e <ambiente>");
            Console.WriteLine("Parámetros:");
            Console.WriteLine("  -x : Ruta absoluta del archivo XML a firmar.");
            Console.WriteLine("  -f : Ruta del certificado digital (.p12 o .pfx).");
            Console.WriteLine("  -p : Contraseña del certificado.");
            Console.WriteLine("  -e : Ambiente (P = Producción, T = Test).");
        }


        private static void Finalizar(string env)
        {
            if (env == "T" || env == "D")
            {
                Console.WriteLine("\nPresione cualquier tecla para salir...");
                Console.ReadKey();
            }
        }
    }
}
