using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatapp.Database;
using Chatapp.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatapp.Infrastructure
{
    public class ChatRepostory : IChatRepostory
    {
        private AppDbContext _context;

        public ChatRepostory(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateChat(string roomName,string userId)
        {
            var chat = new Chat
            {
                Name = roomName,
                Type = ChatType.Room,
            };
            chat.ChatUsers.Add(new ChatUser
            {
                UserId = userId,
                Role = UserRole.Admin,
            });
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
        }

        public async Task<Message> CreateMessage(int chatId, string message,string userName)
        {
           var _message = new Message
            {
                ChatId = chatId,
                Text = message,
                Name = userName,
                TimeStamp = DateTime.Now
            };
           _context.Messages.Add(_message);
           await _context.SaveChangesAsync();
           return _message;
        }

        public async Task<int> CreatePrivateChat(string rootUserId,string targetUserId)
        {
            var chat = new Chat
            {
                Type = ChatType.Private,
            };
            chat.ChatUsers.Add(new ChatUser
            {
                UserId = rootUserId,
                Role = UserRole.Member,
            });
            chat.ChatUsers.Add(new ChatUser
            {
                UserId = targetUserId,
                Role = UserRole.Admin,
            });
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return chat.Id;
        }
        public Chat GetChat(int Id)
        {
            return _context.Chats
                .Include(chat => chat.Messages)
                .FirstOrDefault(chat => chat.Id == Id);
        }

        public IEnumerable<Chat> GetPrivateChats(string userId)
        {
            return _context.Chats
                        .Include(x=>x.ChatUsers)
                            .ThenInclude(y=>y.User)
                        .Where(x=>x.Type == ChatType.Private 
                            && x.ChatUsers.Any(y=>y.UserId == userId))
                        .ToList();
        }

        public IEnumerable<Chat> GetPublicChats(string userId)
        {
            return _context.Chats
                    .Include(x => x.ChatUsers)
                    .Where(x => !x.ChatUsers
                        .Any(y => y.UserId == userId))
                    .ToList();
        }

        public async Task JoinChat(int roomId,string userId)
        {
            var chatuser = new ChatUser
            {
                ChatId = roomId,
                UserId = userId,
                Role = UserRole.Member,
            };

            _context.ChatUsers.Add(chatuser);
            await _context.SaveChangesAsync();
        }
    }
}