using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChatRoom2.Models
{
    [Table("Group_Messages")]
    public class GroupMessageModel
    {
        [Key]
        public Guid GroupMessageId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid GroupId { get; set; }
        public Guid ChatMessageId { get; set; }

        [ForeignKey("ChatMessageId")]
        public virtual ChatMessageModel ChatMessage { get; set; }
        [ForeignKey("GroupId")]
        public virtual GroupModel Group { get; set; }
    }
}