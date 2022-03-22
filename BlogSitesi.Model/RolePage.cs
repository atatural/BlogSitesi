using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.Model
{
    public class RolePage : Core.ModelBase
    {
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        public string Route { get; set; }
    }
}
