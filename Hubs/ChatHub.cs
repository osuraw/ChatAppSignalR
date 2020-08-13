using Microsoft.AspNetCore.SignalR;

namespace Chatapp.Hubs
{
    public class ChatHub : Hub
    {
        public string GetConnectionId()=> Context.ConnectionId;
    }
}