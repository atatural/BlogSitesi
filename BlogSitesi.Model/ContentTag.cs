using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.Model
{
    public class ContentTag : Core.ModelBase
    {
        public int TagId{ get; set; }
        public int ContentId { get; set; }
    }
}
