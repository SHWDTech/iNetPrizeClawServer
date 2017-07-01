using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetWorkCore
{
    public delegate void SocketAcceptHandler(SocketAcceptEventArgs args);

    public class SocketEventArgs
    {
        public string Message { get; set; }

        public Exception Exception { get; set; }

        public TcpSocketClient AcceptClient { get; set; }
    }

    public class SocketAcceptEventArgs : SocketEventArgs
    {
    }

    public class SocketDisconnectEventArgs : SocketEventArgs
    {

    }

    public class TcpSocketListener
    {
        private Socket _listenSocekt;

        private bool _isServerDisposed;

        public List<TcpSocketClient> ConnectedClients { get; } = new List<TcpSocketClient>();

        public event SocketAcceptHandler SocketAcceptd;

        public event Disconnected ClientDisconnected;

        public static Exception LastException { get; private set; }

        public bool Start(IPEndPoint endPoint)
        {
            try
            {
                _listenSocekt?.Dispose();
                _listenSocekt = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _listenSocekt.Bind(endPoint);
                _listenSocekt.Listen(4096);
                StartAccept(null);
            }
            catch (Exception ex)
            {
                LastException = ex;
                return false;
            }
            return true;
        }

        public bool Stop()
        {
            try
            {
                if (_isServerDisposed) return true;
                _isServerDisposed = true;
                _listenSocekt.Shutdown(SocketShutdown.Both);
                _listenSocekt.Dispose();
            }
            catch (Exception ex)
            {
                LastException = ex;
                return false;
            }
            return true;
        }

        private void StartAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (_isServerDisposed) return;
            if (acceptEventArgs == null)
            {
                acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += AcceptEventCompleted;
            }
            else
            {
                acceptEventArgs.AcceptSocket = null; //释放上次绑定的Socket，等待下一个Socket连接
            }

            var willRaiseEvent = _listenSocekt.AcceptAsync(acceptEventArgs);//同步才是false，大多数的情况下都是异步的
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArgs);
            }
        }

        private void AcceptEventCompleted(object sender, SocketAsyncEventArgs acceptEventArgs)
        {
            ProcessAccept(acceptEventArgs);
        }

        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            try
            {
                var client = new TcpSocketClient(acceptEventArgs.AcceptSocket);
                client.Disconnected += OnClientDisconnected;
                ConnectedClients.Add(client);
                SocketAcceptd?.Invoke(new SocketAcceptEventArgs
                {
                    AcceptClient = client
                });
                Console.WriteLine($"socket accepted, remote address:{acceptEventArgs.AcceptSocket.RemoteEndPoint}");
            }
            catch (Exception ex)
            {
                LastException = ex;
            }

            StartAccept(acceptEventArgs); //把当前异步事件释放，等待下次连接
        }

        private void OnClientDisconnected(SocketClientDisconnectedArgs args)
        {
            Console.WriteLine($"Client Disconnected, clientRemoteEndPoint : {args.DisconnectedSocketRemoteEndPoint}");
            lock (ConnectedClients)
            {
                if (ConnectedClients.Contains(args.DisconnectedClient))
                {
                    ConnectedClients.Remove(args.DisconnectedClient);
                }
            }
            ClientDisconnected?.Invoke(args);
        }
    }
}
