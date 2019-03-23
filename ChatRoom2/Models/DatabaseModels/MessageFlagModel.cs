using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChatRoom2.Models.DatabaseModels
{
    [Table("Message_Flags")]
    public class MessageFlagModel
    {
        [Key]
        public Guid MessageFlagId { get; set; }
        public bool MessageSeen { get; set; }
        public Guid UserId { get; set; }
        public Guid ChatMessageId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        [ForeignKey("ChatMessageId")]
        public virtual ChatMessageModel ChatMessage { get; set; }

    }
}