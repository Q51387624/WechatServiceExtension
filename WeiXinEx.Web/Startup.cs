using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Generic;
using WeiXinEx.Web.Controllers;

namespace WeiXinEx.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //注册连接字符串
            var connection = Configuration.GetConnectionString("aliyun");
            NetRube.Data.DbFactory.SetConnectionString(connection);
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            //授权
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Deadline",
                          policy => policy.Requirements.Add(new DeadlineRequirement(new System.DateTime(2017, 10, 1))));
            })//验证
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/Login");
                options.AccessDeniedPath = new PathString("/Denied");
            });
            
            //注入授权Handler
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=index}/{id?}");

                routes.MapRoute(
                    name: "Login",
                    template: "Login",
                    defaults: new { controller = "Home", action = "Login" });
                routes.MapRoute(
                    name: "Denied",
                    template: "Denied",
                    defaults: new { controller = "Home", action = "Denied" });
            });
        }
    }
}
