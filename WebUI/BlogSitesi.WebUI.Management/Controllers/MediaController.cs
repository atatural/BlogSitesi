using BlogSitesi.Data;
using BlogSitesi.WebUI.Management.Helpers;
using BlogSitesi.WebUI.Management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlogSitesi.WebUI.Management.Controllers
{
    public class MediaController : Controller
    {
        MediaData _mediaData;
        ContentData _contentData;

        public MediaController(MediaData _mediaData, ContentData _contentData)
        {
            this._mediaData = _mediaData;
            this._contentData = _contentData;
        }

        [HttpGet]
        public IActionResult Index()
        {

            var medias = _mediaData.GetAll();
            return View(medias);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var media = new Model.Media();
            return View(media);
        }

        [HttpPost]
        public IActionResult Add(Model.Media media, IFormFile file)
        {
            var errors = new List<string>();

            if (file == null || file.Length == 0)
            {
                ViewBag.Reuslt = new ViewModelResult(false, "Dosya yok");
                return View(media);
            }

            var extension = Path.GetExtension(file.FileName).Trim('.').ToLower();
            if (!(new[] {"jpg","png", "jpeg" }.Contains(extension)))
                {
                ViewBag.Result = new ViewModelResult(false, "Dosya uzantısı hatalı!");
                }

            var local_image_dir = $"wwwroot/_uploads/images";
            var local_image_path = $"{local_image_dir}/{file.FileName}";

            if (!Directory.Exists(Path.Combine(local_image_dir)))
                Directory.CreateDirectory(Path.Combine(local_image_dir));

            using (Stream fileStream = new FileStream(local_image_path, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            media.MediaUrl = $"{local_image_path}";
            media.FileSlug = Path.GetFileNameWithoutExtension(file.FileName).ToSlug();
            media.Alt = media.Alt ?? "";
            media.Title = media.Title ?? "";


            var operationResult = _mediaData.Insert(media);
            if (operationResult.IsSucceed)
            {
                ViewBag.Result = new ViewModelResult(true, "Yeni Medya Eklendi!");
                return View(new Model.Media());
            }
            ViewBag.Result = new ViewModelResult(true, operationResult.Message);
            return View(media);
        }

        //[HttpGet]
        //public IActionResult Edit(int id)
        //{
        //    var category = _categoryData.GetByKey(id);
        //    if (category == null)
        //        return RedirectToAction("Index", "Home", new { q = "kategori-bulunamadi" });

        //    return View(category);
        //}

        //[HttpPost]
        //public IActionResult Edit(Model.Category category)
        //{
        //    var errors = new List<string>();

        //    var modelInDb = _categoryData.GetByKey(category.Id);

        //    if (string.IsNullOrEmpty(category.Name)) errors.Add("Kategori boş bırakılamaz.");
        //    if (string.IsNullOrEmpty(category.Slug)) errors.Add("Kategori slug boş bırakılamaz.");
        //    if (errors.Count > 0)
        //    {
        //        ViewBag.Result = new ViewModelResult(false, "Hata oluştu!",errors);
        //        return View(category);
        //    }

        //    //Sistemde kayıtlı olan kategoriye eşit mi girilen değerler
        //    var exist_category = _categoryData.GetBy(x => x.Slug == category.Slug && x.Id == category.Id).FirstOrDefault();
        //    if(exist_category != null)
        //    {
        //        ViewBag.Result = new ViewModelResult(false, "Bu zaten kayıtlı.");
        //        return View(category);
        //    }

        //    modelInDb.Name = category.Name;
        //    modelInDb.MetaTitle = category.MetaTitle;
        //    modelInDb.MetaDescription = category.MetaDescription;
        //    modelInDb.Slug = category.Slug.ToSlug();
        //    modelInDb.IsActive = category.IsActive;

        //    var operationResult = _categoryData.Update(modelInDb);
        //    if (operationResult.IsSucceed)
        //    {
        //        ViewBag.Result = new ViewModelResult(true, "Kategori güncellendi!");
        //        return View(category);
        //    }
        //    ViewBag.Result = new ViewModelResult(false, operationResult.Message);
        //    return View(category);
        //}

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var result = true;

            var media = _mediaData.GetByKey(id);
            if (media == null)
                return RedirectToAction("Index", "Media", new { q = "Medya-Bulunamadi" });

            var media_url = media.MediaUrl;

            if (System.IO.File.Exists(media_url))
                System.IO.File.Delete(media_url);

            result = _mediaData.DeleteByKey(id).IsSucceed;
            if (result)
            {
                var contents = _contentData.GetBy(x => x.Id == media.Id);
                foreach (var item in contents)
                {
                    item.MediaId = -1;
                    _contentData.Update(item);
                }
            }
            return RedirectToAction("Index", "Media", new { q = "medya-silindi" }); //13:42
        }
    }
}
