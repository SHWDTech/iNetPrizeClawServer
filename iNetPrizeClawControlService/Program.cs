using System;
using System.Net;
using System.Threading;
using NetWorkCore;
using NetWorkCore.IpcObjects;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

namespace iNetPrizeClawControlService
{
    class Program
    {
        private static IPAddress _address;

        private static bool _isInProcess;

        private static ushort _port;

        private static void Main()
        {
            Console.Write("input server ip address:");
            var readLine = Console.ReadLine();
            while (readLine == null || !IPAddress.TryParse(readLine, out _address))
            {
                Console.WriteLine("wrong ip address");
                Console.Write("input server ip address:");
                readLine = Console.ReadLine();
            }
            Console.Write("input server port:");
            while (!ushort.TryParse(Console.ReadLine(), out _port))
            {
                Console.WriteLine("wrong port number");
                Console.Write("input server port:");
            }
            var server = new TcpSocketListener();
            CommandDispatcher.ServerListener = server;
            server.SocketAcceptd += args =>
            {
                args.AcceptClient.TcpDataReceived += eventArgs =>
                {

                };
            };
            Console.WriteLine(server.Start(new IPEndPoint(_address, _port)) ? "server started" : "server start failed");

            var channel = new IpcServerChannel("PrizeClawControlChannel");
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(CommandDispatcher), nameof(CommandDispatcher), WellKnownObjectMode.SingleCall);
            while (!_isInProcess)
            {
                Thread.Sleep(10000);
            }
        }

        public static void Stop()
        {
            _isInProcess = true;
        }
    }
}