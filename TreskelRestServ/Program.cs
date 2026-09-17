using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TreskelRestServ;

var builder = Host.CreateApplicationBuilder(args);

// ESTA ES LA CLAVE: Reemplaza al antiguo Installer y ServiceBase
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "TrsklServ";
});

builder.Services.AddHostedService<TrsklServ>();

var host = builder.Build();
host.Run();

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TreskelRestServ
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new TrsklServ()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
*/