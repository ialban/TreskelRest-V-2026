// Decompiled with JetBrains decompiler
// Type: TreskelService.TreskelService
// Assembly: TreskelService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F136DB3E-5B19-4A97-A334-8AEBAFB3A9F6
// Assembly location: C:\Desarrollo\TreskelSoftServ\TreskelService.exe

using Entidades;
using LibConfig;
using LibConfig.cls;
using LibFtpClient;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TreskelRestServ
{
    public partial class TrsklServ : BackgroundService
    {
        private bool procesar = true;
        private Logger logger;
        private string ErrorMessage = "";
        private Thread VerificaWhatsCoords = null;

        public TrsklServ()
        {
            InitializeComponent();
            string str1 = "Treskel";
            string str2 = "TreskelLog";
            EventLog eventLog = new EventLog()
            {
                Source = str1,
                Log = str2
            };
            Console.WriteLine("Servicio Treskel ejecutado");
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Servicio Treskel iniciado");
            Procesar();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Servicio Treskel detenido");
            logger.registraLog("END", "Servicio TreskelRest finalizado", "Admin", "DBSERVER");
            procesar = false;
        }

        private void UpdateBkpDemand(Backup bkp, BkpDemand bkpD, string comment, bool Estado, int progreso)
        {
            if (!General.EJECUTA_BKP)
                return;

            bkpD.bkpCmt = comment;
            bkpD.bkpAtv = Estado;
            bkpD.bkpCPg = progreso;
            bkp.UpdateBkpDemand(bkpD);
        }

        private string remueveCaracteres(string texto)
        {
            texto = texto.Replace(" ", "");
            texto = texto.Replace("á", "a");
            texto = texto.Replace("é", "e");
            texto = texto.Replace("í", "i");
            texto = texto.Replace("ó", "o");
            texto = texto.Replace("ú", "u");
            texto = texto.Replace("ñ", "n");
            texto = texto.Replace("Á", "A");
            texto = texto.Replace("É", "E");
            texto = texto.Replace("Í", "I");
            texto = texto.Replace("Ó", "O");
            texto = texto.Replace("Ú", "U");
            texto = texto.Replace("Ñ", "N");
            return texto;
        }

        public void Procesar()
        {
            Console.WriteLine("Iniciando Servicio");
            Configuracion configuracion = new Configuracion();
            BkpDemand bkpDemand1 = new BkpDemand();
            if (!configuracion.ExistConfig())
            {
                Config nuevo = new Config();
                nuevo.database.Server = "localhost";
                nuevo.database.DbName = "Trsklrest";
                nuevo.database.Puerto = 5432L;
                nuevo.database.Usuario = "trsklusr";
                nuevo.database.Clave = "Tr3sk3lS0ft";
                configuracion.creaConfig(nuevo);
                nuevo.Dispose();
            }
            string path = "C:/TrklBkp";
            string descripcion = "";
            int num1 = 0;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            num1 = Directory.GetFiles(path).Length;
            Config config = configuracion.readConfig();
            General.generaConnectionString(config);
            this.logger = new Logger(General.connectionString);
            Backup bkp = new Backup(General.connectionString);
            General.loadParametros();
            bkp.load();
            this.logger.registraLog("STR", "Servicio TreskelRest iniciado", "Admin", "DBSERVER");
            int numero = bkp.controlSecuencia;
            int progreso = 0;
            while (this.procesar)
            {
                DateTime now1 = DateTime.Now;
                string str1 = now1.ToString("HH:mm:ss");
                General.loadParametrosBackup();
                bkp.load();
                if (str1 == bkp.horaBackup + ":00" || str1 == bkp.horaBackup2 + ":00" || str1 == bkp.horaBackup3 + ":00" || General.EJECUTA_BKP)
                {
                    BkpDemand bkpDemand2 = bkp.GetBkpDemand();
                    Respuesta respuesta1 = new Respuesta();
                    DateTime now2 = DateTime.Now;
                    string str2 = now2.Year.ToString();
                    int num2 = now2.Month;
                    string str3 = num2.ToString().PadLeft(2, '0');
                    num2 = now2.Day;
                    string str4 = num2.ToString().PadLeft(2, '0');
                    string str5 = str1.Substring(0, 2);
                    string str6 = str1.Substring(3, 2);
                    string str7 = $"{str2}-{str3}-{str4}-{str5}-{str6}";
                    string str8;
                    if (General.EJECUTA_BKP)
                        str8 = $"{path}/trklrest_{this.remueveCaracteres(General.EMPRESA.RemoveWhitespace())}_{str7}.backup";
                    else
                        str8 = $"{path}/trklrest_{this.remueveCaracteres(General.EMPRESA.RemoveWhitespace())}_{numero.ToString()}.backup";
                    string str9 = str1;
                    string str10 = "";
                    string str11 = $"{General.PG_DUMP_LOC}/pg_dump -v -Fc \"host={config.database.Server} port={config.database.Puerto.ToString()} dbname={config.database.DbName} user={config.database.Usuario} password={config.database.Clave}\" > {str8}";
                    ++progreso;
                    FileInfo fileInfo = new FileInfo(str8);
                    Respuesta respuesta2 = new Respuesta();
                    this.UpdateBkpDemand(bkp, bkpDemand2, "Iniciando BKP", false, progreso);
                    Respuesta respuesta3 = this.logger.registraLog("BKP", "Iniciando Backup", config.database.Usuario, "DBSERVER");
                    try
                    {
                        Process process = Process.Start(new ProcessStartInfo("cmd.exe", "/C " + str11)
                        {
                            CreateNoWindow = false,
                            UseShellExecute = false
                        });
                        process.EnableRaisingEvents = true;
                        process.WaitForExit();
                        int exitCode = process.ExitCode;
                        process.Close();
                        ++progreso;
                        this.UpdateBkpDemand(bkp, bkpDemand2, "BKP terminado", false, progreso);
                        descripcion = "[INFO] : Bakup Generado :: " + str8;
                        now1 = DateTime.Now;
                        str10 = now1.ToString("HH:mm:ss");
                        respuesta3 = this.logger.registraLog("BKP", descripcion, config.database.Usuario, "DBSERVER");
                        ++progreso;
                        this.UpdateBkpDemand(bkp, bkpDemand2, "LOG BKP terminado", false, progreso);
                        if (General.UPLOAD == "1")
                        {
                            respuesta3 = this.logger.registraLog("BKP", "[INFO] : FTP Cargando archivo " + str8, config.database.Usuario, "DBSERVER");
                            respuesta2 = new ftpClient(General.SERVER, General.USER, General.PASSWORD, General.PUERTO).Upload(str8, this.remueveCaracteres(General.EMPRESA));
                            ++progreso;
                            respuesta3 = this.logger.registraLog("BKP", "[INFO] : FTP Upload BKP terminado", config.database.Usuario, "DBSERVER");
                            this.UpdateBkpDemand(bkp, bkpDemand2, "FTP BKP terminado", false, progreso);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!respuesta2.resultado && respuesta2.mensaje == "")
                        {
                            descripcion = "[ERROR] : Error en proceso de Backup :: " + str8;
                            this.UpdateBkpDemand(bkp, bkpDemand2, "BKP NO PUDO SER REALIZADO", false, progreso);
                            respuesta3 = this.logger.registraLog("BKP", descripcion, config.database.Usuario, "DBSERVER");
                        }
                        if (!respuesta2.resultado && respuesta2.mensaje != "")
                        {
                            descripcion = "[ERROR] : Error al subir backup al cloud :: " + str8;
                            this.UpdateBkpDemand(bkp, bkpDemand2, "UPLOAD FALLÓ", false, progreso);
                            respuesta3 = this.logger.registraLog("BKP", descripcion, config.database.Usuario, "DBSERVER");
                        }
                        progreso = 4;
                    }
                    if (General.ENVIAMAIL == "1")
                    {
                        new Thread(new ThreadStart(new Email(General.CORREO_HOST, General.CORREO_PORT, General.CORREO_USER, General.CORREO_PASSWORD, General.CORREO_AUTH, General.CORREO_UTLS, "")
                        {
                            subject = "Realización Backup : " + General.EMPRESA,
                            body = $"<h1>REALIZACIÓN DEL BACKUP</h1><p>El proceso de inicio a {str9}</p><p>{descripcion}</p><p>Finalizó a {str10}</p><br><br><p><strong>Resultado del registro del logg: </strong></p>{respuesta3.mensaje}"
                        }.enviar)).Start();
                        ++progreso;
                        this.UpdateBkpDemand(bkp, bkpDemand2, "EMAIL Msg terminado", false, progreso);
                    }
                    if (!General.EJECUTA_BKP)
                    {
                        if (numero >= int.Parse(bkp.cntFiles))
                            numero = 1;
                        else
                            ++numero;
                        bkp.UpdateControlSecuencia(numero);
                    }
                    this.FinalizaBackup(bkp, bkpDemand2, ref progreso);
                    respuesta1 = this.logger.registraLog("BKP", "[INFO] : Backup Finalizado", config.database.Usuario, "DBSERVER");
                }
                Thread.Sleep(1000);
            }
        }

        private void procesoExited(object sender, EventArgs e)
        {
            if (!File.Exists("LogFile.log"))
                File.Create("LogFile.log");
            if (!(this.ErrorMessage != ""))
                return;
            File.AppendAllText("LogFile.log", this.ErrorMessage);
        }

        private void FinalizaBackup(Backup bkp, BkpDemand bkpDemand, ref int progreso)
        {
            General.FinEjecutaBackup();
            ++progreso;
            this.UpdateBkpDemand(bkp, bkpDemand, "FIN BKP", true, progreso);
            General.EJECUTA_BKP = false;
        }

        private enum FasesBackup
        {
            Inicio,
            InicioBackup,
            Uploading,
            Notificacion,
        }

        private void EmpiezaVerificaWhatsCoords(Backup bkp)
        {
            while (this.procesar)
            {
                General.loadParametrosBackup();

                if (General.MODO_VALIDACION_ANULA_ELIMINA != "N")
                {
                    bkp.CheckCaducidadWhatsappCoords();
                }

                Thread.Sleep(5000);
            }
        }
    }
}
