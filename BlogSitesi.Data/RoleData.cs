using BlogSitesi.Data.Infrastructor;
using BlogSitesi.Data.Infrastructor.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.Data //54:03
{
    public class RoleData :EntityBaseData<Model.Role>
    {
        public RoleData(IOptions<DatabaseSetting> dbOptions) 
            : base(new DataContext(dbOptions.Value.ConnectionString))
        {

        }
    }
}
