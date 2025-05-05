using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serverInstance = new ServerClass();
            ServiceHost serverHost = null;
            try
            {
                using (serverHost = new ServiceHost(serverInstance, new Uri("net.tcp://localhost:60001/Server")))
                {
                    serverHost.AddServiceEndpoint(typeof(IServerClass), new NetTcpBinding()
                    {
                        ReliableSession = new OptionalReliableSession()
                        {
                            Enabled = true,
                            InactivityTimeout = new TimeSpan(1, 0, 0),
                        },
                        ReceiveTimeout = TimeSpan.MaxValue, // Set to maximum possible TimeSpan value  
                    }, "");

                    serverHost.Open();
                    Console.WriteLine("Server started on port 60001");
                    Thread.Sleep(1000); // Wait for the client to start

                    var serverBinding = new NetTcpBinding()
                    {
                        ReliableSession = new OptionalReliableSession()
                        {
                            Enabled = true,
                            InactivityTimeout = new TimeSpan(1, 0, 0),
                        }
                    };
                    var endpoint = new EndpointAddress("net.tcp://localhost:60002/Client");

                    using (var channelFactory = new ChannelFactory<IClientClass>(serverBinding, endpoint))
                    {
                        var checkChannel = channelFactory.CreateChannel();
                        Console.WriteLine("Client callback connected on port 60002");
                        Console.WriteLine("Press 'C' to check the client or 'Enter' to exit.");
                        ConsoleKeyInfo input = default;
                        do
                        {
                            input = Console.ReadKey();
                            switch (input.Key)
                            {
                                case ConsoleKey.C:
                                    Console.WriteLine($"Client Check {checkChannel.CheckClient()}");
                                    break;
                            }
                        } while (input.Key != ConsoleKey.Enter);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server Error \n{ex.Message}");
            }
        }
    }
}
