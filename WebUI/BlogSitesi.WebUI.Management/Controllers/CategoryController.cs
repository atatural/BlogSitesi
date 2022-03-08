using BlogSitesi.Data;
using BlogSitesi.WebUI.Management.Helpers;
using BlogSitesi.WebUI.Management.Models;
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
            /*Booolean değerlerde başında "!" varsa pasif olmayan eğer yoksa aktif anlamına geliyor */
            //var categories = _categoryData.GetBy(x => !x.IsDeleted); => isdeleted false olanları getir
            var categories = _categoryData.GetBy(x => x.IsDeleted == false && x.IsActive);
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

            category.Slug = category.Slug.ToSlug();

            var operationResult = _categoryData.Insert(category);
            if (operationResult.IsSucceed)
            {
                ViewBag.Result = new ViewModelResult(true, "Yeni Kategori Eklendi!");
                return View(new Model.Category());
            }
            ViewBag.Result = new ViewModelResult(true, operationResult.Message);
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
                ViewBag.Result = new ViewModelResult(false, "Hata oluştu!",errors);
                return View(category);
            }

            //Sistemde kayıtlı olan kategoriye eşit mi girilen değerler
            var exist_category = _categoryData.GetBy(x => x.Slug == category.Slug && x.Id == category.Id).FirstOrDefault();
            if(exist_category != null)
            {
                ViewBag.Result = new ViewModelResult(false, "Bu zaten kayıtlı.");
                return View(category);
            }

            modelInDb.Name = category.Name;
            modelInDb.MetaTitle = category.MetaTitle;
            modelInDb.MetaDescription = category.MetaDescription;
            modelInDb.Slug = category.Slug.ToSlug();
            modelInDb.IsActive = category.IsActive;

            var operationResult = _categoryData.Update(modelInDb);
            if (operationResult.IsSucceed)
            {
                ViewBag.Result = new ViewModelResult(true, "Kategori güncellendi!");
                return View(category);
            }
            ViewBag.Result = new ViewModelResult(false, operationResult.Message);
            return View(category);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _categoryData.GetByKey(id);
            if (category == null)
                return RedirectToAction("Index", "Category", new { q = "kategori-bulunamadi" });

            category.IsDeleted = true;
            var operationResult = _categoryData.Update(category);

            if (operationResult.IsSucceed)
                return RedirectToAction("Index", "Category", new { q = "kategori-silindi" });       
          
            else
                return RedirectToAction("Index", "Category", new { q = "kategori-silinemedi" }); //17:48
        }
    }
}
