using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.WebUI.Infrastructure.Cache
{
    public  interface ICache
    {
        bool TryGetValue(string key, out object value);
        bool Set(string key, object value, int minutesToCache);
        bool Remove(string key);
    }
}
