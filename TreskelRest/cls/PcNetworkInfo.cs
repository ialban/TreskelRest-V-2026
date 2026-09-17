using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace TreskelRest.cls
{
    public class PcNetworkInfo
    {
        public string IpAddress { get; set; }
        public string PcName { get; set; }

        public PcNetworkInfo()
        {
            try
            {
                PcName = Dns.GetHostName();
                foreach (var ipEntity in Dns.GetHostEntry(PcName).AddressList)
                {
                    var ip = ipEntity.ToString();
                    var regex = new Regex(@"([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})");
                    if (regex.IsMatch(ip))
                    {
                        IpAddress = ip;
                    }
                }
            }
            catch
            {
                PcName = "Desconocido";
                IpAddress = "127.0.0.1";
            }
        }

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
  
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        public static bool InternetStatus()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool InternetConnectionStatus()
        {
            Uri url = new Uri("http://www.google.com/");
            WebRequest wr;
            wr = WebRequest.Create(url);
            WebResponse res;
            try
            {
                res = wr.GetResponse();

                return true;
            }catch(Exception e)
            {
                return false;
            }
        }
    }
}
