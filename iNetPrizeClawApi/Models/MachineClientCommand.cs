using NetWorkCore.IpcObjects;

namespace iNetPrizeClawApi.Models
{
    public class MachineClientCommand
    {
        public string ClientCode { get; set; }

        public ControlCommand Command { get; set; }
    }
}