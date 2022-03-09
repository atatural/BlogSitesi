using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.WebUI.Infrastructure.Cache
{
    public  interface ICache
    {
        bool TryGetValue(string key, out object value);
        void Set(string key, object value, int minutesToCache);
        void Remove(string key);
    }
}
