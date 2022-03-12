using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.Model
{
    public class ContentCategory : Core.ModelBase
    {
        public int CategoryId { get; set; }
        public int ContentId { get; set; }
    }
}
