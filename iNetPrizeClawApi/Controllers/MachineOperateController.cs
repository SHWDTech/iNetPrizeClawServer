using System;
using System.Web.Http;
using iNetPrizeClawApi.Models;
using NetWorkCore.IpcObjects;

namespace iNetPrizeClawApi.Controllers
{
    public class MachineOperateController : ApiController
    {
        public MachineOperateResult Post([FromBody] MachineClientOperate model)
        {

            try
            {
                var dispatcher = (CommandDispatcher)Activator.GetObject(typeof(CommandDispatcher),
                    "ipc://PrizeClawControlChannel/CommandDispatcher");
                return dispatcher.MachineOperate(model.ClientCode, model.Operate);
            }
            catch (Exception ex)
            {
                return new MachineOperateResult
                {
                    ClientCode = model.ClientCode,
                    IsOperateResultOk = false,
                    OperateMessage = ex.Message
                };
            }
        }
    }
}
