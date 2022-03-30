using BlogSitesi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogSitesi.WebUI.Infrastructure.Cache
{

    public class CacheHelper
    {
        ICache cache;
        CategoryData _categoryData;
        AuthorData _authorData;
        RolePageData _rolePageData;
        SettingData _settingData;
        ContentData _contentData;

        public CacheHelper(ICache cache, CategoryData categoryData, RolePageData rolePageData , ContentData contentData)
        {
            this.cache = cache;
            _categoryData = categoryData;
            _rolePageData = rolePageData;
            _contentData = contentData;
        }

        private string Categories_CacheKey = "Categories_CacheKey";
        public bool CategoriesClear() { return Clear(Categories_CacheKey); }
        public List<Model.Category> Categories
        {
            get
            {
                var fromCache = Get<List<Model.Category>>(Categories_CacheKey);
                if(fromCache == null)
                {
                    var datas = _categoryData.GetBy(x => !x.IsDeleted);
                    if(datas != null && datas.Count() > 0)
                    {
                        Set(Categories_CacheKey, datas);
                        fromCache = datas;
                    }
                }
                return fromCache;
            }
        }

        private string RolePage_CacheKey= "RolePage_CacheKey";
        public bool RolePage() { return Clear(RolePage_CacheKey); }
        public List<Model.RolePage> RolePages(int roleId)
        {
           
                var fromCache = Get<List<Model.RolePage>>(RolePage_CacheKey);
                if (fromCache == null)
                {
                    var datas = _rolePageData.GetBy(x => x.RoleId == roleId);
                    if (datas != null && datas.Count() > 0)
                    {
                        Set(Categories_CacheKey, datas);
                        fromCache = datas;
                    }
                }
            if (fromCache != null)
            {
                return fromCache.Where(x => x.RoleId == roleId).ToList();
            }
                return new List<Model.RolePage>();
            
        }

        private string NewContents_CacheKey = "NewContents_CacheKey";
        public bool NewContentsClear() { return Clear(NewContents_CacheKey); }
        public List<Model.Content> NewContents
        {
            get
            {
                var fromCache = Get<List<Model.Content>>(NewContents_CacheKey);
                if (fromCache == null)
                {
                    var datas = _contentData.GetBlogNewContents(25);
                    if (datas != null && datas.Count() > 0)
                    {
                        Set(NewContents_CacheKey, datas);
                        fromCache = datas;
                    }
                }
                return new List<Model.Content>();
            }         
        }

        public bool Clear(string name)
        {
            cache.Remove(name);
            return true;
        }       
        public T Get<T>(string cacheKey) where T : class
        {
            object cookies;

            if (!cache.TryGetValue(cacheKey, out cookies))
                return null;

            return cookies as T;
        }

        public void Set(string cacheKey, object value)
        {
            cache.Set(cacheKey, value, 100);
        }
    }
}
