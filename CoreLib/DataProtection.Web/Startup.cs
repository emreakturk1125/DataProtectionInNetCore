using DataProtection.Web.Filters;
using DataProtection.Web.Middleware;
using DataProtection.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataProtection.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
         
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DenemeContext>(opt =>
            {
                opt.UseSqlServer("Server=TKARGCLNMS28;Database=Deneme;user id=sa;password=Emre123+;");
            });

            services.AddDataProtection(); // Veri koruması ile ilgili 

            services.AddScoped<CheckWhiteListAttribute>();

            services.Configure<IPList>(Configuration.GetSection("IPList"));  // appsettig.json dosyasındaki IPList'i class'ına atamak için

            services.AddControllersWithViews();
        }
         
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error"); 
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // metod bazlı filterımızı kontrol etmek için, uygulama bazlı middleware i yorum satırı yaptık
            // app.UseMiddleware<IPSafeMiddleware>();   // Uygulama bazında çalışması için Oluşturduğumuz middleware i burada tanımladık

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
