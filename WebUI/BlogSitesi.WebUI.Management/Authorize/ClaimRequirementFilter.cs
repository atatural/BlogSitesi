using BlogSitesi.WebUI.Infrastructure.Cache;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogSitesi.WebUI.Management.Authorize
{
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        CacheHelper cacheHelper;

        public ClaimRequirementFilter(CacheHelper cacheHelper)
        {
            this.cacheHelper = cacheHelper;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var role = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Role);
            if(role == null)
            {
                context.Result = new RedirectResult("/Home/Login");
                return;
            }
            //Bu role idli kullanıcı adminse devam et sorgulamaya
            if (role.Value == "q")
                return;
        }
    }
}