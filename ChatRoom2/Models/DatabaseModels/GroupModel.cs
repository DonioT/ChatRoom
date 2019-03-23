using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChatRoom2.Models
{
    [Table("Groups")]
    public class GroupModel
    {
        [Key]
        public Guid GroupId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModified { get; set; }
      
    }
}