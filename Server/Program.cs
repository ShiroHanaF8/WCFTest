using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serverInstance = new ServerClass();
            var host = new ServiceHost(serverInstance, new Uri("net.tcp://localhost:60001/Server"));
            host.AddServiceEndpoint(typeof(IServerClass), new NetTcpBinding()
            {
                ReliableSession = new OptionalReliableSession()
                {
                    Enabled = true,
                    InactivityTimeout = new TimeSpan(1, 0, 0),
                }
            }, "");

            host.Open();
            Console.WriteLine("Server started on port 60001");
            Console.ReadLine();

            host.Close();
        }
    }
}
