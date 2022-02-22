using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.Data.Infrastructor.Entities
{
    /// CRUD işlemlerin sonucunda ne olduğunu görmek için


    public class DataResult
    {
        public DataResult(bool isSucceed, string message)
        {
            IsSucceed = isSucceed;
            Message = message;

        }
        public bool IsSucceed { get; set; }
        public string Message{ get; set; }
    }
}
