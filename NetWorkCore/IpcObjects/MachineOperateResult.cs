using System;

namespace NetWorkCore.IpcObjects
{
    public class MachineOperateResult : MarshalByRefObject
    {
        public string ClientCode { get; set; }

        public bool IsOperateSuccess { get; set; }

        public bool IsOperateResultOk { get; set; }

        public string OperateMessage { get; set; }
    }
}