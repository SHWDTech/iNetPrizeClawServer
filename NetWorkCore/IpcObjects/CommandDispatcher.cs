using System;
using System.Collections.Generic;
using System.Linq;

namespace NetWorkCore.IpcObjects
{
    public class CommandDispatcher : MarshalByRefObject
    {
        public static TcpSocketListener ServerListener { get; set; }

        public static Dictionary<string, ClientCommandStutas> LastSendCommands = new Dictionary<string, ClientCommandStutas>();

        public bool SendCommand(string clientCode, ControlCommand command)
        {
            if (!IsCommandCanSend(clientCode, command, out ClientCommandStutas status)) return false;
            ServerListener.ConnectedClients.Where(c => c.ClientCode == clientCode && c.IsAvaliable).ToList()
                .ForEach(cs => cs.SendCommand(command));
            status.UpdateLastCommand(command);
            TryMakeStop(clientCode, status);
            return true;
        }

        public MachineOperateResult MachineOperate(string clientCode, MachineOperate operate)
        {
            return ServerListener.ConnectedClients.FirstOrDefault(c => c.ClientCode == clientCode)
                ?.ExecuteOperate(operate);
        }

        private static bool IsCommandCanSend(string clientCode, ControlCommand command, out ClientCommandStutas status)
        {
            lock (LastSendCommands)
            {
                if (!LastSendCommands.ContainsKey(clientCode))
                {
                    LastSendCommands.Add(clientCode, new ClientCommandStutas(clientCode));
                }
                status = LastSendCommands[clientCode];
                return status.IsCommandCanSend(command);
            }
        }

        private static void TryMakeStop(string clientCode, ClientCommandStutas status)
        {
            if (status.IsStopWait)
            {
                ServerListener.ConnectedClients.FirstOrDefault(c => c.ClientCode == clientCode)?.SendCommand(ControlCommand.Stop);
                status.UpdateLastCommand(ControlCommand.Stop);
            }
        }
    }
}
