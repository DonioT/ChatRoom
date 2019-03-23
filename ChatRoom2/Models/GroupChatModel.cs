using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatRoom2.Models
{
    public class GroupChatModel
    {
        public Guid GroupId { get; set; }
        public List<Models.GroupMemberModel> GroupMembers { get; set; }

        public List<Models.GroupMessageModel> GroupMessages { get; set; }
        
        public int UnreadCount { get; set; }
    }
}