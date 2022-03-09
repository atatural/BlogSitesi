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

        public CacheHelper(ICache cache, CategoryData categoryData)
        {
            this.cache = cache;
            _categoryData = categoryData;
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
