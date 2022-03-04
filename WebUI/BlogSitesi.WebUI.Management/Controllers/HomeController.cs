using BlogSitesi.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogSitesi.WebUI.Management.Controllers
{
    public class HomeController : Controller
    {
        CategoryData _categoryData;
        public HomeController(CategoryData categoryData)
        {
            _categoryData = categoryData;
        }

        public IActionResult Index()
        {
            var categories = _categoryData.GetAll();
            return View();
        }
    }
}
