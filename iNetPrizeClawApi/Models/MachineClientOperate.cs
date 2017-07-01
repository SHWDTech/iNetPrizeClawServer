using NetWorkCore.IpcObjects;

namespace iNetPrizeClawApi.Models
{
    public class MachineClientOperate
    {
        public string ClientCode { get; set; }

        public MachineOperate Operate { get; set; }
    }
}