using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Web;
using System.Web.Http;

namespace iNetPrizeClawApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var channel = new IpcClientChannel();
            //Register the channel with ChannelServices.
            ChannelServices.RegisterChannel(channel, false);
        }
    }
}
