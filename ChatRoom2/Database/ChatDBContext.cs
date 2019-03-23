using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ChatRoom2.Database
{
    public class ChatDBContext : DbContext
    {
        public DbSet<Models.UserModel> Users { get; set; }
        public DbSet<Models.ChatMessageModel> ChatMessages { get; set; }
        public DbSet<Models.GroupModel> Groups { get; set; }
        public DbSet<Models.GroupMemberModel> GroupMembers { get; set; }
        public DbSet<Models.GroupMessageModel> GroupMessages { get; set; }
        public DbSet<Models.DatabaseModels.MessageFlagModel> MessageFlags { get; set; }
    }
}