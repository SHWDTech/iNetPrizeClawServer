using System;
using System.Linq;
using System.Text;

namespace NetWorkCore.IpcObjects
{
    public class CommandDispatcher : MarshalByRefObject
    {
        public static TcpSocketListener ServerListener { get; set; }

        public bool SendCommand(string clientCode, ControlCommand command)
        {
            ServerListener.ConnectedClients.FirstOrDefault(c => c.ClientCode == clientCode)?.Send(Encoding.UTF8.GetBytes(ParseCommand(command)));
            return true;
        }

        private static string ParseCommand(ControlCommand command)
        {
            return command.ToString();
        }
    }
}
