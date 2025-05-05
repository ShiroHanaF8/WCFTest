using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Tcp;
using System.ServiceModel;
using Server;
using System.Threading;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create a channel factory for the service
            var clientBinding = new NetTcpBinding()
            {
                ReliableSession = new OptionalReliableSession()
                {
                    Enabled = true,
                    InactivityTimeout = new TimeSpan(1, 0, 0),
                },
                ReceiveTimeout = TimeSpan.MaxValue, // Set to maximum possible TimeSpan value  
            };
            var serverEndpoint = new EndpointAddress("net.tcp://localhost:60001/Server");

            Thread.Sleep(1000); // Wait for the server to start

            using (var channelFactory = new ChannelFactory<IServerClass>(clientBinding, serverEndpoint))
            {
                // Create a channel to the service
                IServerClass proxy = null;
                try
                {
                    proxy = channelFactory.CreateChannel();
                    Console.WriteLine("Client connected on port 60001");
                    IClientClass clientClass = new ClientClass();
                    using (var clientHost = new ServiceHost(clientClass, new Uri("net.tcp://localhost:60002/Client")))
                    {
                        clientHost.AddServiceEndpoint(typeof(IClientClass), new NetTcpBinding()
                        {
                            ReliableSession = new OptionalReliableSession()
                            {
                                Enabled = true,
                                InactivityTimeout = new TimeSpan(1, 0, 0),
                            }
                        }, "");
                        clientHost.Open();

                        Console.WriteLine("Client callback started on port 60002");
                        Console.WriteLine("Press 'U' to update the server or 'Enter' to exit.");
                        ConsoleKeyInfo input = default;
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Client Error \n{ex.Message}");
                }
            }
        }
    }
}
