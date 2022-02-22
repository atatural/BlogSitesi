using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.Model
{
    public class Comment:Core.ModelBase
    {
        public int ContentId { get; set; }
        public string FullName { get; set; }
        public string Mail { get; set; }
        public string Text { get; set; }
        public bool IsApprovied { get; set; }
    }
}
