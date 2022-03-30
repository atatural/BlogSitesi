using BlogSitesi.Data;
using BlogSitesi.Data.Infrastructor.Entities;
using BlogSitesi.WebUI.Infrastructure.Cache;
using BlogSitesi.WebUI.Infrastructure.Rules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.WebUI.Site
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IConfigurationRoot ConfigurationRoot { get; }
        public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; }


        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env )
        {
            Configuration = configuration;
            Environment = env;

            ConfigurationRoot = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();


        }
        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.IsDevelopment())
            {
                services.AddControllersWithViews().AddRazorRuntimeCompilation();
            }
            //Config
            services.Configure<DatabaseSetting>(Configuration.GetSection("DatabaseSetting"));
            services.AddOptions();

            //Cache
            services.AddMemoryCache();
            services.AddTransient<ICache, BlogSitesi.Infrastructure.Caching.Memory.Cache>();
            services.AddTransient<CacheHelper>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //Data
            services.AddTransient<CategoryData>();
            services.AddTransient<ContentData>();
            services.AddTransient<ContentCategoryData>();
            services.AddTransient<ContentTagData>();
            services.AddTransient<TagData>();
            services.AddTransient<MediaData>();
            services.AddTransient<AuthorData>();
            services.AddTransient<SettingData>();
            services.AddTransient<CommentData>();
            services.AddTransient<RolePageData>();
            services.AddTransient<RoleData>();

            services.AddMvc(x =>
            {
                x.EnableEndpointRouting = false;
            });

            services.Configure<RouteOptions>(routeOptions => routeOptions.AppendTrailingSlash = true);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseStatusCodePages();
            }
            app.UseDeveloperExceptionPage();

            RedirectToHttpsWwwNonWwwRule rule = new RedirectToHttpsWwwNonWwwRule
            {
                status_code = 301,
                redirect_to_https = true,
                redirect_to_www = false,
                redirect_to_non_www = true,
                append_slash = true

            };
            RewriteOptions options = new RewriteOptions();
            options.Rules.Add(rule);
            app.UseRewriter(options);

            app.UseRouting();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "category", template: "kategori/{slug}", defaults: new { controller = "Category", action = "Index", page = 1 });
                routes.MapRoute(name: "categoryWithPage", template: "kategori/{slug}/sayfa/{page}", defaults: new { controller = "Category", action = "Index", page = 1 });

                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
