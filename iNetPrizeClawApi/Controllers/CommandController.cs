using System;
using System.Web.Http;
using iNetPrizeClawApi.Models;
using NetWorkCore.IpcObjects;

namespace iNetPrizeClawApi.Controllers
{
    public class CommandController : ApiController
    {
        public string Post([FromBody]MachineClientCommand model)
        {
            try
            {
                var dispatcher = (CommandDispatcher) Activator.GetObject(typeof(CommandDispatcher),
                    "ipc://PrizeClawControlChannel/CommandDispatcher");
                dispatcher.SendCommand(model.ClientCode, model.Command);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
