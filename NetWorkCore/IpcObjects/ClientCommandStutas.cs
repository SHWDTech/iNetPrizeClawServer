namespace NetWorkCore.IpcObjects
{
    public class ClientCommandStutas
    {
        public ClientCommandStutas(string clientCode)
        {
            ClientCode = clientCode;
        }

        public string ClientCode { get; }

        private ControlCommand _lastSendControlCommand;

        public bool IsStopWait { get; private set; }

        public bool IsCommandCanSend(ControlCommand command)
        {
            if (_lastSendControlCommand == ControlCommand.Stop && command == ControlCommand.Stop)
            {
                IsStopWait = true;
                return false;
            }
            _lastSendControlCommand = command;
            return true;
        }

        public void UpdateLastCommand(ControlCommand command)
        {
            _lastSendControlCommand = command;
            if (command == ControlCommand.Stop)
            {
                IsStopWait = false;
            }
        }
    }
}
