using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Chatapp.Models
{
    public class User:IdentityUser
     {
         public ICollection<ChatUser> UserChats { get; set; }
     }
}