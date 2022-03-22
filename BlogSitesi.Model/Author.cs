using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BlogSitesi.Model
{
    public class Author : Core.ModelBase
    {
        public int RoleId { get; set; }
        public string FullName { get; set; }
        public string Mail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
