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
            var binding = new NetTcpBinding()
            {
                ReliableSession = new OptionalReliableSession()
                {
                    Enabled = true,
                    InactivityTimeout = new TimeSpan(1, 0, 0),
                }
            };
            var endpoint = new EndpointAddress("net.tcp://localhost:60001/Server");

            Thread.Sleep(1000); // Wait for the server to start

            using (var channelFactory = new ChannelFactory<IServerClass>(binding, endpoint))
            {
                // Create a channel to the service
                IServerClass proxy = null;
                try
                {
                    proxy = channelFactory.CreateChannel();

                    // Call the Update method on the server
                    proxy.Update();
                    Console.WriteLine("Client started on port 60001");
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
                    } while (input.Key == ConsoleKey.Enter);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }
            }
        }
    }
}
