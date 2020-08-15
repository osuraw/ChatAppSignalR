using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chatapp.Database;
using Chatapp.Hubs;
using Chatapp.Infrastructure;
using Chatapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Chatapp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private IChatRepostory _repo;

        public HomeController(IChatRepostory repo)
        {
            _repo = repo;
        }
        public IActionResult Index(){
            var chats = _repo.GetPublicChats(GetUserId());            
            return View(chats);
        }
       
        [HttpGet]
        public IActionResult Users([FromServices] AppDbContext _context)
        {
            var users = _context.Users
                .Where(x=>x.Id!= User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .ToList();

            return View(users);
        }
        [HttpGet]
         public IActionResult Private(){
            var chats = _repo.GetPrivateChats(GetUserId());
            return View(chats);
        }


        [HttpGet("{Id}")]
        public IActionResult GoToChat(int Id)
        {
            var chat = _repo.GetChat(Id);
            return View("Chat", chat);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId, string message)
        {
            await _repo.CreateMessage(chatId,message,User.Identity.Name);
            return RedirectToAction("GoToChat", new { Id = chatId });
        }

        [HttpGet]
        public async Task<IActionResult> CreatePrivateRoom(string userId)
        {
            var Id = await _repo.CreatePrivateChat(GetUserId(),userId);
            return RedirectToAction("GoToChat","Home",new {Id});
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            await _repo.CreateChat(name,GetUserId());
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> JoinChat(int Id)
        {
            await _repo.JoinChat(Id,GetUserId());
            return RedirectToAction("GoToChat","Home",new{Id});
        }
    
        public async Task<IActionResult> SendMessage(int chatId,string message,[FromServices] IHubContext<ChatHub> _chat){
            
            var _message = await _repo.CreateMessage(chatId,message,User.Identity.Name);
            await _chat.Clients.Group(chatId.ToString()).SendAsync("RecieveMessage",new {
                _message.Name,
                _message.Text,
                timestamp = _message.TimeStamp.ToString("dd-mm-yyyy hh:MM:ss")
            });
            return Ok();
        }
    }
}