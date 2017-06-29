﻿using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace NetWorkCore
{
    public delegate void TcpDataReceived(SocketClientReceiveEventArgs args);

    public delegate void Disconnected(SocketClientDisconnectedArgs args);

    public class SocketClientEventArgs
    {
        public string Message { get; set; }

        public Exception Exception { get; set; }
    }

    public class SocketClientReceiveEventArgs : SocketClientEventArgs
    {
        public int BytesTransferred { get; set; }

        public byte[] Buffer { get; set; }
    }

    public class SocketClientDisconnectedArgs : SocketClientEventArgs
    {
        public string DisconnectedSocketRemoteEndPoint { get; set; }
    }

    public class TcpSocketClient
    {
        private readonly Socket _clientSocket;

        private readonly SocketAsyncEventArgs _asyncEventArgs;

        public event TcpDataReceived TcpDataReceived;

        public event Disconnected Disconnected;

        private bool _isDisposed;

        public string ClientCode { get; private set; }

        public TcpSocketClient(Socket client)
        {
            _clientSocket = client;
            _asyncEventArgs = new SocketAsyncEventArgs();
            _asyncEventArgs.SetBuffer(new byte[4096], 0, 4096);
            _asyncEventArgs.Completed += (sender, args) =>
            {
                ProcessReceive();
            };
            var willRaiseEvent = _clientSocket.ReceiveAsync(_asyncEventArgs); //投递接收请求
            if (willRaiseEvent) return;
            lock (_clientSocket)
            {
                ProcessReceive();
            }
        }

        public void Send(byte[] sendBytes)
        {
            try
            {
                _clientSocket.Send(sendBytes);
            }
            catch (Exception)
            {
                ClientDisconnected(new SocketClientDisconnectedArgs
                {
                    DisconnectedSocketRemoteEndPoint = _clientSocket.RemoteEndPoint.ToString()
                });
            }
        }

        private void ProcessReceive()
        {
            if (_isDisposed) return;
            try
            {
                if (_asyncEventArgs.BytesTransferred == 0)
                {
                    ClientDisconnected(new SocketClientDisconnectedArgs
                    {
                        DisconnectedSocketRemoteEndPoint = _clientSocket.RemoteEndPoint.ToString()
                    });
                }
                if (_asyncEventArgs.BytesTransferred > 0 && _asyncEventArgs.SocketError == SocketError.Success)
                {
                    DataReceived(new SocketClientReceiveEventArgs
                    {
                        BytesTransferred = _asyncEventArgs.BytesTransferred,
                        Buffer = _asyncEventArgs.Buffer
                    });
                    var protocolContent = Encoding.ASCII.GetString(_asyncEventArgs.Buffer, 0, _asyncEventArgs.BytesTransferred);
                    if (protocolContent.Contains("ClientCode"))
                    {
                        try
                        {
                            ClientCode = protocolContent.Split(':')[1];
                            Send(Encoding.ASCII.GetBytes("OK"));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    var willRaiseEvent = _clientSocket.ReceiveAsync(_asyncEventArgs); //投递接收请求
                    if (willRaiseEvent) return;
                    ProcessReceive();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ClientDisconnected(new SocketClientDisconnectedArgs
                {
                    DisconnectedSocketRemoteEndPoint = _clientSocket.RemoteEndPoint.ToString()
                });
            }

        }

        private void DataReceived(SocketClientReceiveEventArgs args)
        {
            TcpDataReceived?.Invoke(args);
        }

        private void ClientDisconnected(SocketClientDisconnectedArgs args)
        {
            Dispoose();
            Disconnected?.Invoke(args);
        }

        public void Dispoose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _clientSocket.Dispose();
        }
    }
}
