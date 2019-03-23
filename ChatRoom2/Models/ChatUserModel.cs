using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatRoom2.Models
{
    public class ChatUserModel
    {
        public Models.UserModel User { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}