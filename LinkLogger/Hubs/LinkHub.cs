using LinkLogger.Controllers.Api;
using Microsoft.AspNet.SignalR;

namespace LinkLogger.Hubs
{
    public class LinkHub : Hub
    {
        public void Send(string message)
        {
            Clients.All.addNewMessage(message);
        }
    }
}