using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Tcp;
using System.ServiceModel;
using Server;
using System.Threading;
using System.ServiceModel.Channels;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Thread.Sleep(1000); // Wait for the server to start
                IServerClass proxy = CreateClientEndpoint();

                Console.WriteLine("Client connected on port 60001");
                CreateCheckClientEndpoint();

                Console.WriteLine("Client callback started on port 60002");
                Console.WriteLine("Press 'U' to update the server or 'Enter' to exit.");
                ConsoleKeyInfo input = default;

                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(1000);
                        Console.WriteLine($"Polling {DateTime.Now} Client Check {proxy.Update()}");
                    }
                });

                do
                {
                    input = Console.ReadKey();
                    switch (input.Key)
                    {
                        case ConsoleKey.U:
                            Console.WriteLine($"Server Update {proxy.Update()}");
                            break;
                    }
                } while (input.Key != ConsoleKey.Enter);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client Error \n{ex.Message}");
            }
        }

        private static void CreateCheckClientEndpoint()
        {
            IClientClass clientClass = new ClientClass();
            var binding = new NetTcpBinding()
            {
                ReliableSession = new OptionalReliableSession
                {
                    InactivityTimeout = new TimeSpan(1, 0, 0),
                },
                ReceiveTimeout = new TimeSpan(1, 0, 0),
            };

            var clientHost = new ServiceHost(clientClass, new Uri("net.tcp://localhost:60002/Client"));
            clientHost.AddServiceEndpoint(typeof(IClientClass), binding, "");
            clientHost.Open();
        }

        private static IServerClass CreateClientEndpoint()
        {
            // Create a channel factory for the service
            var binding = new NetTcpBinding()
            {
                ReliableSession = new OptionalReliableSession
                {
                    InactivityTimeout = new TimeSpan(1, 0, 0),
                },
                ReceiveTimeout = new TimeSpan(1, 0, 0),
            };

            var serverEndpoint = new EndpointAddress("net.tcp://localhost:60001/Server");

            var channelFactory = new ChannelFactory<IServerClass>(binding, serverEndpoint);
            IServerClass proxy = channelFactory.CreateChannel();
            return proxy;
        }
    }
}
