using BlogSitesi.Data;
using BlogSitesi.WebUI.Infrastructure.Cache;
using BlogSitesi.WebUI.Management.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogSitesi.WebUI.Management.Controllers
{
    public class HomeController : Controller
    {
        ContentData _contentData;
        TagData _tagData;
        public HomeController(ContentData _contentData, TagData _tagData)
        {
            this._contentData = _contentData;
            this._tagData = _tagData;
        }

        public IActionResult Index()
        {
            var content = _contentData.GetByPage(x => !x.IsDeleted, 1, 10, "PublishDate", true);
            var tag = _tagData.GetByPage(x => !x.IsDeleted, 1, 10, "PublishDate", true);

            var model = new HomeViewModel()
            {
                Contents = content,
                Tags = tag,
            };

            return View(model);
        }
    }
}
