using BlogSitesi.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogSitesi.WebUI.Management.Controllers
{
    public class CategoryController : Controller
    {
        CategoryData _categoryData;
        public CategoryController(CategoryData categoryData)
        {
            _categoryData = categoryData;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var categories = _categoryData.GetBy(x => x.IsDeleted);
            return View(categories);
        }
        [HttpGet]
        public IActionResult Add()
        {
            var category = new Model.Category()
            {
                IsActive = false,
                IsDeleted = false,
                MetaDescription = "",
                MetaTitle = "",
                Name = "",
                Slug = "",
            };
            return View(category);
        }
        [HttpPost]
        public IActionResult Add(Model.Category category)
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(category.Name)) errors.Add("Kategori boş bırakılamaz.");
            if (string.IsNullOrEmpty(category.Slug)) errors.Add("Kategori slug boş bırakılamaz.");
            if(errors.Count > 0)
            {
                ViewBag.Result = "";
                return View(category);
            }
            var operationResult = _categoryData.Insert(category);
            if (operationResult.IsSucceed)
            {
                ViewBag.Result = "Eklendi";
                return View(new Model.Category());
            }
            ViewBag.Result = "";
            return View(category);

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _categoryData.GetByKey(id);
            if (category == null)
                return RedirectToAction("Index", "Home", new { q = "kategori-bulunamadi" });

            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Model.Category category)
        {
            var errors = new List<string>();

            var modelInDb = _categoryData.GetByKey(category.Id);

            if (string.IsNullOrEmpty(category.Name)) errors.Add("Kategori boş bırakılamaz.");
            if (string.IsNullOrEmpty(category.Slug)) errors.Add("Kategori slug boş bırakılamaz.");
            if (errors.Count > 0)
            {
                ViewBag.Result = "";
                return View(category);
            }


            //Sistemde kayıtlı olan kategoriye eşit mi girilen değerler
            var exist_category = _categoryData.GetBy(x => x.Slug == category.Slug && x.Id == category.Id).FirstOrDefault();
            if(exist_category != null)
            {
                ViewBag.Result = "";
                return View(category);
            }

            modelInDb.Name = category.Name;
            modelInDb.MetaTitle = category.MetaTitle;
            modelInDb.MetaDescription = category.MetaDescription;
            modelInDb.Slug = category.Slug;
            modelInDb.IsActive = category.IsActive;

            var operationResult = _categoryData.Update(modelInDb);
            if (operationResult.IsSucceed)
            {
                ViewBag.Result = "Güncellendi!";
                return View(category);
            }
            ViewBag.Result = "";
            return View(category);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _categoryData.GetByKey(id);
            if (category == null)
                return RedirectToAction("Index", "Category", new { q = "kategori-bulunamadi" });

            var operationResult = _categoryData.Update(category);
            if (operationResult.IsSucceed)
                return RedirectToAction("Index", "Category", new { q = "kategori-silindi" });
            else
                return RedirectToAction("Index", "Category", new { q = "kategori-silinemedi" }); //17:48
        }
    }
}
