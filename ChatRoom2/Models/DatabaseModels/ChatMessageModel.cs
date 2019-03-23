using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChatRoom2.Models
{
    [Table("Chat_Messages")]
    public class ChatMessageModel
    {
        [Key]
        public Guid ChatMessageId { get; set; }

        public virtual Guid UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
        public string ChatMessage { get; set; }

        public DateTime DateSent { get; set; }

        public bool Private { get; set; }
    }
}