using BlogSitesi.Data.Infrastructor;
using BlogSitesi.Data.Infrastructor.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogSitesi.Data //54:03
{
    public class ContentData :EntityBaseData<Model.Content>
    {
        public ContentData(IOptions<DatabaseSetting> dbOptions) 
            : base(new DataContext(dbOptions.Value.ConnectionString))
        {

        }
        public List<Model.Content> GetBlogNewContents(int count = 25)
        {
            var dtNow = DateTime.Now;

            return _context.Set<Model.Content>()
                .OrderByDescending("PublishDate")
                .Include(x => x.Media)
                .Include(x => x.Author)
                .Where(x => x.PublishDate <= dtNow && !x.IsDeleted && x.IsActive)
                .Include(x => x.ContentCategories)
                .Take(count).ToList();
        }
    }
}
