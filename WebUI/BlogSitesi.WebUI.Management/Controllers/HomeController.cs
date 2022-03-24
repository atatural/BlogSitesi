using BlogSitesi.Data;
using BlogSitesi.WebUI.Infrastructure.Cache;
using BlogSitesi.WebUI.Management.Authorize;
using BlogSitesi.WebUI.Management.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogSitesi.WebUI.Management.Controllers
{
    public class HomeController : Controller
    {
        AuthorData _authorData;

        public HomeController(AuthorData _authorData)
        {
            this._authorData = _authorData;
        }
        [Authorize]
        public IActionResult Index()
        {         
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            var model = new LoginModel()
            {
                Username = "",
                Password = "",
            };

            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var errors = new List<string>();
            var return_model = new LoginModel();

            if (string.IsNullOrEmpty(username)) errors.Add("Kullanıcı Adı Boş Bırakılamaz");
            if (string.IsNullOrEmpty(password)) errors.Add("Şifre Boş Bırakılamaz");
            if (errors.Count() > 0)
            {
                ViewBag.Result = new ViewModelResult(false, "Hata Oluştu", errors);
                return View(return_model);
            }

            var author = _authorData.GetBy(x => x.Username == username && x.Password == password && x.IsActive && !x.IsDeleted).FirstOrDefault();
            if (author == null)
            {
                ViewBag.Result = new ViewModelResult(false, "Böyle bir kullanıcı bulunamadı.");
                return_model.Username = username;
                return View(return_model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, author.FullName),
                new Claim("AuthorId", author.Id.ToString()),
                new Claim(ClaimTypes.Role, author.RoleId.ToString())
            };

            var claims_identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var auth_properties = new AuthenticationProperties
            {
                ExpiresUtc = System.DateTimeOffset.UtcNow.AddMinutes(60)
            };

            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claims_identity),
                auth_properties
            );
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return RedirectToAction("Login", "Home");
        }
        [HttpGet]
        public IActionResult _403()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return RedirectToAction("Login", "Home");
        }

    }
    
}