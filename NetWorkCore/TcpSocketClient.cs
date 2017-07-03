using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetWorkCore.IpcObjects;

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
        public TcpSocketClient DisconnectedClient { get; set; }

        public string DisconnectedSocketRemoteEndPoint { get; set; }
    }

    public class TcpSocketClient
    {
        private readonly Socket _clientSocket;

        private readonly SocketAsyncEventArgs _asyncEventArgs;

        public event TcpDataReceived TcpDataReceived;

        public event Disconnected Disconnected;

        public bool IsAvaliable { get; private set; } = true;

        private bool _isDisposed;

        public string ClientCode => _machineStatus.ClientCode;

        private readonly MachineStutas _machineStatus;

        public TcpSocketClient(Socket client)
        {
            _clientSocket = client;
            _machineStatus = new MachineStutas(this);
            _asyncEventArgs = new SocketAsyncEventArgs();
            _asyncEventArgs.SetBuffer(new byte[4096], 0, 4096);
            _asyncEventArgs.Completed += (sender, args) =>
            {
                ProcessReceive();
            };
            var willRaiseEvent = _clientSocket.ReceiveAsync(_asyncEventArgs); //投递接收请求
            Authentication();
            if (willRaiseEvent) return;
            lock (_clientSocket)
            {
                ProcessReceive();
            }
        }

        private void Authentication()
        {
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(ClientCode))
                {
                    Send("WhoAreYou\r\n");
                    Thread.Sleep(500);
                    Authentication();
                }
            });
        }

        public void SendCommand(ControlCommand command)
        {
            Send(ParseCommand(command));
        }

        public void Send(string contentStr)
        {
            var sendBytes = Encoding.UTF8.GetBytes(contentStr);
            try
            {
                _clientSocket.Send(sendBytes);
                Console.WriteLine($"send content:{Encoding.UTF8.GetString(sendBytes)}, send datetime: {DateTime.Now:yyyy-MM-dd HH:mm:ss fff}");
                if (contentStr.Contains("CoinIn"))
                {
                    
                }
            }
            catch (Exception)
            {
                ClientDisconnected(new SocketClientDisconnectedArgs
                {
                    DisconnectedClient = this,
                    DisconnectedSocketRemoteEndPoint = _clientSocket.RemoteEndPoint.ToString()
                });
            }
        }

        public MachineOperateResult ExecuteOperate(MachineOperate operate) => _machineStatus.ExecuteOperate(operate);

        private static string ParseCommand(ControlCommand command)
        {
            return $"{command}\r\n";
        }

        public bool IsCoinReady()
        {
            return _machineStatus.IsCoinReady;
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
                        DisconnectedClient = this,
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
                    Console.WriteLine($"get data, dataContent:{protocolContent}, gettime:{DateTime.Now:yyyy-MM-dd HH:mm:ss fff}");
                    if (protocolContent.Contains("ClientCode"))
                    {
                        try
                        {
                            _machineStatus.ClientCode = protocolContent.Split(':')[1];
                            Send("OK\r\n");
                            Console.WriteLine($"client authenticated, clientCode:{ClientCode}");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    if (protocolContent.Contains("CoinGet"))
                    {
                        _machineStatus.CoinReady();
                    }
                    if (protocolContent.Contains("CatchSuccess"))
                    {
                        _machineStatus.CatchSuccessed();
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
                    DisconnectedClient = this,
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
            IsAvaliable = false;
            _isDisposed = true;
            _clientSocket.Dispose();
        }
    }
}
