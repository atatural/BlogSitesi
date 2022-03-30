using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.WebUI.Site.Controllers
{
    public class ContentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
