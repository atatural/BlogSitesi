using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.Model
{
    public class Media:Core.ModelBase
    {
        public string FileName   { get; set; }
        public string FileSlug { get; set; }
        public string Alt { get; set; }
        public string Title { get; set; }
        public string MediaUrl { get; set; }
    }
}
