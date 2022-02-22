using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.Model
{
    public class ContentTag : Core.ModelBase
    {
        public ContentTag(int contentId, int tagId)
        {
            TagId = tagId;
            ContentId = contentId;
        }

        public int TagId{ get; set; }
        public int ContentId { get; set; }
    }
}
