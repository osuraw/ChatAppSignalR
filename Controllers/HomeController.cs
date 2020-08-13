using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chatapp.Database;
using Chatapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chatapp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(){
            var chats = _context.Chats
                        .Include(x=>x.ChatUsers)
                        .Where(x=>!x.ChatUsers
                            .Any(y=>y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
                        .ToList();
            return View(chats);
        }
       
        [HttpGet]
        public IActionResult Users()
        {
            var users = _context.Users
                .Where(x=>x.Id!= User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .ToList();

            return View(users);
        }
        [HttpGet]
         public IActionResult Private(){
            var chats = _context.Chats
                        .Include(x=>x.ChatUsers)
                            .ThenInclude(y=>y.User)
                        .Where(x=>x.Type == ChatType.Private 
                            && x.ChatUsers.Any(y=>y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
                        .ToList();
            return View(chats);
        }


        [HttpGet("{Id}")]
        public IActionResult GoToChat(int Id)
        {
            var chat = _context.Chats
                .Include(chat => chat.Messages)
                .FirstOrDefault(chat => chat.Id == Id);
            return View("Chat", chat);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId, string message)
        {
            _context.Messages.Add(new Message
            {
                ChatId = chatId,
                Text = message,
                Name = User.Identity.Name,
                TimeStamp = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("GoToChat", new { Id = chatId });
        }

        [HttpGet]
        public async Task<IActionResult> CreatePrivateRoom(string userId)
        {
            var chat = new Chat
            {
                Type = ChatType.Private,
            };
            chat.ChatUsers.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Member,
            });
            chat.ChatUsers.Add(new ChatUser
            {
                UserId = userId,
                Role = UserRole.Member,
            });
            
            _context.Chats.Add(chat);
             
            await _context.SaveChangesAsync();

            return RedirectToAction("GoToChat","Home",new {Id = chat.Id});
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            var chat = new Chat
            {
                Name = name,
                Type = ChatType.Room,
            };
            chat.ChatUsers.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Admin,
            });
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> JoinChat(int Id)
        {
            var chatuser = new ChatUser
            {
                ChatId = Id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Member,
            };

            _context.ChatUsers.Add(chatuser);
            await _context.SaveChangesAsync();
            return RedirectToAction("GoToChat","Home",new{Id});
        }
    
    }
}