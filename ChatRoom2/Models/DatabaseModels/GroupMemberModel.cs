using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChatRoom2.Models
{
    [Table("Group_Members")]
    public class GroupMemberModel
    {
        [Key]
        public Guid GroupMemberId { get; set; }

        public DateTime CreatedOn { get; set; }
        public Guid UserId { get; set; }

        public Guid GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual GroupModel Group { get; set; }

        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}