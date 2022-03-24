using BlogSitesi.WebUI.Infrastructure.Cache;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using BlogSitesi.Data;

namespace BlogSitesi.WebUI.Management.Authorize
{
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        CacheHelper cacheHelper;
        RolePageData _rolePageData;

        public ClaimRequirementFilter(CacheHelper cacheHelper, RolePageData _rolePageData)
        {
            this.cacheHelper = cacheHelper;
            this._rolePageData = _rolePageData;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var role = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Role);
            if(role == null || role.Value == "") 
            {
                context.Result = new RedirectResult("/Home/Login");
                return;
            }
            //Bu role idli kullanıcı adminse devam et sorgulamaya
            if (role.Value == "1")
                return;

            var role_id = int.Parse(role.Value);
            var rolePages = cacheHelper.RolePages(role_id);

            //admin değilse kontrol edelim
            var route = (context.RouteData.Values["controller"] + "/" + context.RouteData.Values["action"]).ToLowerInvariant();
            if(!rolePages.Any(x=>x.Route == route))
            {
                var role_page = _rolePageData.GetBy(x => x.Route == route && x.RoleId == role_id).FirstOrDefault();
                if (role_page == null)
                {
                    if (context.HttpContext.Request.Headers.ContainsKey("x-requested-with"))
                    {
                        var xRequestedWithACheck = context.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "x-requested-with");
                        var value = xRequestedWithACheck.Value;
                        if (value == "XMLHttpRequest")
                        {
                            context.Result = new JsonResult("özel mesaj") { StatusCode = 403};
                            return;
                        }
                    }
                    context.Result = new RedirectResult("/Home/_403url=" + route); //15:35
                }
            }

        }
    }
}