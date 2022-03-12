using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.Model.Core
{
    //Veri tabanında olmayıp bizim içerde ignore ettiğimiz bir sistem. *
    public class IgnoredAttribute:System.Attribute
    {
        public string SomeProperty { get; set; }
    }
}
