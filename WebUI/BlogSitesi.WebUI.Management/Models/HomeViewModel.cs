using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogSitesi.WebUI.Management.Models
{
    public class HomeViewModel : Controller
    {
        public HomeViewModel()
        {
            Tags = new List<Model.Tag>();
            Categories = new List<Model.Category>();
            Contents = new List<Model.Content>();
        }

        public List<Model.Tag> Tags { get; set; }
        public List<Model.Category> Categories { get; set; }
        public List<Model.Content> Contents { get; set; }
    }
}
