using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace Final_Proj_Network_Client
{
    public static class IP_Info
    {
        //not working
        //  public string[] CB = new string[2];

        public static IPAddress MyLocalIpC = IPAddress.Parse(GetLocalIPAddress());
       public static IPEndPoint myEPC= new IPEndPoint(MyLocalIpC, 922);
       public static IPEndPoint ServerEpC;
        public static Socket ServerC = new Socket(MyLocalIpC.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

         private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {if (ip.AddressFamily == AddressFamily.InterNetwork)
                {return ip.ToString();}
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
