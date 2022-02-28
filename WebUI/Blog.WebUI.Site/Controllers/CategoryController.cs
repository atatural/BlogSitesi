using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.WebUI.Site.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index(string slug)
        {
            return View();
        }
    }
}
