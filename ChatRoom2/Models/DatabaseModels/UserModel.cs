using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChatRoom2.Models
{
    [Table("Chat_Users")]
    public class UserModel
    {
        [Key]
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedOn { get; set; }
        public string AvatarUrl { get; set; }
      
    }
}