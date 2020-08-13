using System;
using System.Threading.Tasks;
using Chatapp.Database;
using Chatapp.Hubs;
using Chatapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chatapp.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        private IHubContext<ChatHub> _chat;

        public ChatController(IHubContext<ChatHub> chat)
        {
            _chat =chat;
        }
        
        [HttpPost("[action]/{connectionId}/{roomId}")]
        public async Task<IActionResult> JoinRoom(string connectionId, string roomId){
            
            await _chat.Groups.AddToGroupAsync(connectionId,roomId);
            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> LeavRoom(string connectionId, string roomId){
            
            await _chat.Groups.RemoveFromGroupAsync(connectionId,roomId);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(int chatId,string message,[FromServices] AppDbContext context ){
            var msg =new Message
            {
                ChatId = chatId,
                Text = message,
                Name = User.Identity.Name,
                TimeStamp = DateTime.Now
            };
            context.Messages.Add(msg);
            await context.SaveChangesAsync();

            await _chat.Clients.Group(chatId.ToString()).SendAsync("RecieveMessage",new {
                msg.Name,
                msg.Text,
                timestamp = msg.TimeStamp.ToString("dd-mm-yyyy hh:MM:ss")
            });
            return Ok();
        }


    }
}