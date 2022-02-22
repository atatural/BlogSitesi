using BlogSitesi.Data.Infrastructor;
using BlogSitesi.Data.Infrastructor.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.Data //54:03
{
    public class CommentData :EntityBaseData<Model.Comment>
    {
        public CommentData(IOptions<DatabaseSetting> dbOptions) 
            : base(new DataContext(dbOptions.Value.ConnectionString))
        {

        }
    }
}
