using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Chatapp.Hubs
{
    public class ChatHub : Hub
    {
        public string GetConnectionId()=> Context.ConnectionId;

        public  Task JoinRoom(string roomId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId,roomId);
        }
        
        public Task LeavRoom(string roomId){
            
            return Groups.RemoveFromGroupAsync(Context.ConnectionId,roomId);
        }
    }
}