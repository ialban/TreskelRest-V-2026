using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TreskelRest.cls
{
    public class ObjComportamiento
    {
        public enum tiposEntradas
        {
            soloNumeros = 0,
            numerosPunto = 1,
            numerosLetras = 2,
            numerosLetrasConPunto=3,
            numerosLetrasPuntoSpace=4,
            correoElectronico=5,
            direccionIp=6,
            nombreServidor=7,
            nombrePersona=8,
            direccion=9,
            nombreBaseDatos=10,
            contrasenia=11,
            usuario=12,
        }

        public static tiposEntradas tipos;        

        public static Boolean PreviewTextInput(tiposEntradas tipos, TextCompositionEventArgs e)
        {
            String validos ="";
            switch (tipos)
            {
                case ObjComportamiento.tiposEntradas.soloNumeros:
                    validos = "0123456789";
                    break;
                case ObjComportamiento.tiposEntradas.numerosPunto:
                    validos = "0123456789.";
                    break;
                case ObjComportamiento.tiposEntradas.numerosLetras:
                    validos = "0123456789abcdfghijklmnopqrstuvwxyzñ";
                    break;
                case ObjComportamiento.tiposEntradas.numerosLetrasConPunto:
                    validos = "0123456789abcdfghijklmnopqrstuvwxyzñ.";
                    break;
                case ObjComportamiento.tiposEntradas.numerosLetrasPuntoSpace:
                    validos = "0123456789abcdfghijklmnopqrstuvwxyzñ. ";
                    break;
                case ObjComportamiento.tiposEntradas.correoElectronico:
                    validos = "0123456789abcdfghijklmnopqrstuvwxyz._@";
                    break;
                case ObjComportamiento.tiposEntradas.nombreServidor:
                    validos = "abcdefghijklmnopqrstuvwxyz0123456789/._";                
                    break;
                case ObjComportamiento.tiposEntradas.nombrePersona:
                    validos = "abcdefghijklmnopqrstuvwxyzñ ";
                    break;
                case ObjComportamiento.tiposEntradas.direccion:
                    validos = "abcdefghijklmnopqrstuvwxyz0123456789.,()#&/áéíóúñ ";
                    break;
                case ObjComportamiento.tiposEntradas.nombreBaseDatos:
                    validos = "abcdefghijklmnopqrstuvwxyz0123456789_";
                    break;
                case ObjComportamiento.tiposEntradas.contrasenia:
                    validos = "abcdefghijklmnopqrstuvwxyz0123456789_.-#;:&$%!?+*";
                    break;
                case ObjComportamiento.tiposEntradas.usuario:
                    validos = "abcdefghijklmnopqrstuvwxyz0123456789._";
                    break;
            }
            return validos.Contains(e.Text) ? false : validos.ToUpper().Contains(e.Text)? false: true;
        }

        public static void showMsgInfo(String mensaje)
        {
            MessageBox.Show(mensaje, "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void showMsgError(String mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult showMsgYesNo(String mensaje)
        {
            return MessageBox.Show(mensaje, "Información", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No);
        }

        public static void showMuestraMensaje(Respuesta respuesta)
        {
            if (respuesta.resultado)
            {
                ObjComportamiento.showMsgInfo(respuesta.mensaje);
            }
            else
            {
                ObjComportamiento.showMsgError(respuesta.mensaje);
            }
        }
    }    
}
