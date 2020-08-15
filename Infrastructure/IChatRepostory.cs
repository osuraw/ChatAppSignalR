using System.Collections.Generic;
using System.Threading.Tasks;
using Chatapp.Models;

namespace Chatapp.Infrastructure
{
    public interface IChatRepostory
    {
        Chat GetChat(int Id);
        IEnumerable<Chat> GetPublicChats(string userId);
        IEnumerable<Chat> GetPrivateChats(string userId);
        Task CreateChat(string roomName,string userId);
        Task<int> CreatePrivateChat(string rootUserId,string targetUserId);
        Task JoinChat(int roomId,string userId);
        
        Task<Message> CreateMessage(int chatId, string message,string userName);
    }
}