using BlogSitesi.Data;
using BlogSitesi.WebUI.Management.Helpers;
using BlogSitesi.WebUI.Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogSitesi.WebUI.Management.Controllers
{
    [Authorize]
    public class ContentController : Controller
    {
        CategoryData _categoryData;
        ContentCategoryData _contentCategoryData;
        ContentData _contentData;
        TagData _tagData;
        ContentTagData _contentTagData;
        MediaData _mediaData;

        public ContentController(CategoryData categoryData, ContentCategoryData contentCategoryData, ContentData contentData,
            TagData tagData, ContentTagData contentTagData, MediaData mediaData)
        {
            _categoryData = categoryData;
            _contentCategoryData = contentCategoryData;
            _contentData = contentData;
            _tagData = tagData;
            _contentTagData = contentTagData;
            _mediaData = mediaData;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var contents = _contentData.GetBy(x => !x.IsDeleted);
            return View(contents);
        }
        [HttpGet]
        public IActionResult Add()
        {
            var content = new Model.Content();
            return View(content);
        }

        [HttpPost]
        public IActionResult Add(Model.Content content, List<int> CategoryIds, string Tags)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(content.Title)) errors.Add("Makale Başlığı Boş Bırakılamaz");
            if (string.IsNullOrEmpty(content.Slug)) errors.Add("Makale Slug Boş Bırakılamaz");
            if (errors.Count() > 0)
            {
                ViewBag.Result = new ViewModelResult(false, "Hata Oluştu", errors);
                return View(content);
            }

            content.Slug = content.Slug.ToSlug();
            content.AuthorId = 1;

            var operationResult = _contentData.Insert(content);
            if (operationResult.IsSucceed)
            {
                var content_category_list = new List<Model.ContentCategory>();
                foreach (var cat in CategoryIds)
                {
                    content_category_list.Add(new Model.ContentCategory()
                    {
                        CategoryId = cat,
                        ContentId = content.Id,
                    });
                }

                var insert_categories = _contentCategoryData.InsertBulk(content_category_list);

                if (!string.IsNullOrEmpty(Tags))
                {
                    var tag_split = Tags.Split(',');
                    foreach (var tag in tag_split)
                    {
                        var name = tag.Trim();
                        var check_inDb_tag = _tagData.GetBy(x => x.Name == name && !x.IsDeleted).FirstOrDefault();
                        if (check_inDb_tag == null)
                        {
                            var tag_model = new Model.Tag()
                            {
                                Name = name,
                                IsActive = true,
                                IsDeleted = false,
                                MetaDescription = name,
                                MetaTitle = name,
                                Slug = name.ToSlug(),
                            };

                            var insert_tag = _tagData.Insert(tag_model);

                            var tag_content_model = new Model.ContentTag()
                            {
                                ContentId = content.Id,
                                TagId = tag_model.Id,
                            };

                            var insert_tag_content = _contentTagData.Insert(tag_content_model);
                        }
                        else
                        {
                            var tag_content_model = new Model.ContentTag()
                            {
                                ContentId = content.Id,
                                TagId = check_inDb_tag.Id,
                            };

                            var insert_tag_content = _contentTagData.Insert(tag_content_model);
                        }
                    }
                }
                ViewBag.Result = new ViewModelResult(true, "Yeni Makale eklendi");
                return View(new Model.Content());
            }

            ViewBag.Result = new ViewModelResult(false, operationResult.Message);
            return View(content);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var content = _contentData.GetByKey(id);
            if (content == null)
                return RedirectToAction("Index", "Content", new { q = "makale-bulunamadi" });

            var categories = _contentCategoryData.GetBy(x => x.ContentId == id);
            var tags = _contentTagData.GetBy(x => x.ContentId == id);

            var tag_ids = tags.Select(x => x.TagId).ToList();
            var tag_names = _tagData.GetBy(x => tag_ids.Contains(x.Id)).Select(x => x.Name).ToList();

            var model = new ContentEditViewModel()
            {
                Categories = categories,
                Content = content,
                Tags = tags,
                TagNames = tag_names,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Model.Content content, List<int> CategoryIds, string Tags)
        {
            var viewModel = new ContentEditViewModel();
            var modelInDb = _contentData.GetByKey(content.Id);
            viewModel.Categories = _contentCategoryData.GetBy(x => x.ContentId == content.Id);
            viewModel.Tags = _contentTagData.GetBy(x => x.ContentId == content.Id);

            var errors = new List<string>();
            if (string.IsNullOrEmpty(content.Title)) errors.Add("Makale Başlığı Boş Bırakılamaz");
            if (string.IsNullOrEmpty(content.Slug)) errors.Add("Makale Slug Boş Bırakılamaz");
            if (errors.Count() > 0)
            {
                var tag_ids = viewModel.Tags.Select(x => x.TagId).ToList();
                var tag_names = _tagData.GetBy(x => tag_ids.Contains(x.Id)).Select(x => x.Name).ToList();
                viewModel.TagNames = tag_names;
                viewModel.Content = content;
                ViewBag.Result = new ViewModelResult(false, "Hata Oluştu", errors);
                return View(viewModel);
            }

            var exist_content = _contentData.GetBy(x => x.Slug == content.Slug && !x.IsDeleted && x.Id != content.Id).FirstOrDefault();
            if (exist_content != null)
            {
                viewModel.Content = content;
                ViewBag.Result = new ViewModelResult(false, "Bu slug ile kayıt var");
                return View(viewModel);
            }

            modelInDb.Title = content.Title;
            modelInDb.Description = content.Description;
            modelInDb.MetaDescription = content.MetaDescription;
            modelInDb.IsActive = content.IsActive;
            modelInDb.MediaId = content.MediaId;
            modelInDb.Slug = content.Slug.ToSlug();
            modelInDb.UpdateDate = DateTime.Now;
            modelInDb.PublishDate = content.PublishDate;

            var operationResult = _contentData.Update(modelInDb);
            if (operationResult.IsSucceed)
            {
                if (!SetwiseEquivalentTo<int>(viewModel.Categories.Select(x => x.CategoryId).ToList(), CategoryIds))
                {
                    _contentCategoryData.DeleteBulk(x => x.ContentId == modelInDb.Id);
                    foreach (var cat in CategoryIds)
                    {
                        var cat_model = new Model.ContentCategory()
                        {
                            CategoryId = cat,
                            ContentId = content.Id,
                        };
                        _contentCategoryData.Insert(cat_model);
                    }
                }

                var tag_Ids = viewModel.Tags.Select(x => x.TagId);
                var tags = _tagData.GetBy(x => tag_Ids.Contains(x.Id));

                if (!string.IsNullOrEmpty(Tags) && !SetwiseEquivalentTo<string>(tags.Select(x => x.Name).ToList(), Tags.Split(',').Select(x => x.Trim()).ToList()))
                {
                    _contentTagData.DeleteBulk(x => x.ContentId == modelInDb.Id);
                    var tag_split = Tags.Split(',');

                    var content_tag_list = new List<Model.ContentTag>();

                    foreach (var tag in tag_split)
                    {
                        var name = tag.Trim();
                        var check_inDb_tag = _tagData.GetBy(x => x.Name == name && !x.IsDeleted).FirstOrDefault();
                        if (check_inDb_tag == null)
                        {
                            var tag_model = new Model.Tag()
                            {
                                Name = name,
                                IsActive = true,
                                IsDeleted = false,
                                MetaDescription = name,
                                MetaTitle = name,
                                Slug = name.ToSlug(),
                            };

                            var insert_tag = _tagData.Insert(tag_model);

                            content_tag_list.Add(new Model.ContentTag()
                            {
                                ContentId = content.Id,
                                TagId = tag_model.Id,
                            });
                        }
                        else
                        {
                            content_tag_list.Add(new Model.ContentTag()
                            {
                                ContentId = content.Id,
                                TagId = check_inDb_tag.Id,
                            });
                        }
                    }
                    _contentTagData.InsertBulk(content_tag_list);
                }

                ViewBag.Result = new ViewModelResult(true, "Makale Güncellendi.");

                viewModel.Content = modelInDb;
                viewModel.Categories = _contentCategoryData.GetBy(x => x.ContentId == content.Id);
                viewModel.Tags = _contentTagData.GetBy(x => x.ContentId == content.Id);
                var tag_ids = viewModel.Tags.Select(x => x.TagId).ToList();
                var tag_names = _tagData.GetBy(x => tag_ids.Contains(x.Id)).Select(x => x.Name).ToList();
                viewModel.TagNames = tag_names;

                return View(viewModel);
            }

            ViewBag.Result = new ViewModelResult(false, operationResult.Message);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var content = _contentData.GetByKey(id);
            if (content == null)
                return RedirectToAction("Index", "Content", new { q = "makale-bulunamadi" });

            var content_categories = _contentCategoryData.DeleteBulk(x => x.ContentId == content.Id);
            var content_tags = _contentTagData.DeleteBulk(x => x.ContentId == content.Id);

            content.IsDeleted = true;
            var operationResult = _contentData.Update(content);
            if (operationResult.IsSucceed)
                return RedirectToAction("Index", "Content", new { q = "makale-silindi" });
            else
                return RedirectToAction("Index", "Content", new { q = "makale-silemedim" });
        }

        public bool SetwiseEquivalentTo<T>(List<T> list, List<T> other)
            where T : IEquatable<T>
        {
            if (list.Except(other).Any())
                return false;
            if (other.Except(list).Any())
                return false;
            return true;
        }
    }
}
