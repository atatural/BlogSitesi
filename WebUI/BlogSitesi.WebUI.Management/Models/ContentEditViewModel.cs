using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogSitesi.WebUI.Management.Models
{
    public class ContentEditViewModel : Controller
    {
        public ContentEditViewModel()
        {
            Categories = new List<Model.ContentCategory>();
            Tags = new List<Model.ContentTag>();
        }

        public Model.Content Content { get; set; }
        public List<Model.ContentCategory> Categories { get; set; }
        public List<Model.ContentTag> Tags { get; set; }
        public List<string> TagNames { get; set; }
    }
}
