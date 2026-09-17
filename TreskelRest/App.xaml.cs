using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TreskelRest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Crear una copia de la cultura actual pero modificando los separadores
            CultureInfo culturaPunto = new CultureInfo(CultureInfo.CurrentCulture.Name);
            culturaPunto.NumberFormat.NumberDecimalSeparator = ".";
            culturaPunto.NumberFormat.NumberGroupSeparator = ",";

            // Asignar al hilo principal de la aplicación
            CultureInfo.DefaultThreadCurrentCulture = culturaPunto;
            CultureInfo.DefaultThreadCurrentUICulture = culturaPunto;

            // Forzar a los controles visuales de WPF a usar esta cultura
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(culturaPunto.IetfLanguageTag)
                )
            );

            base.OnStartup(e);
        }
    }
}
