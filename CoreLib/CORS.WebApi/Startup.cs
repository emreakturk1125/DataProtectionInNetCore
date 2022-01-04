using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CORS.WebApi
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
            services.AddCors(opt =>
            {
                #region Kural 1 => Uygulama bazında CORS ayarları bütün domainlerden gelen isteklere izin verdik

                //opt.AddDefaultPolicy(builder =>
                //      {
                //          builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                //      }); 

                #endregion

                #region Kural 2 => Uygulama bazında CORS ayarları yaptık kurak koyduk, belirlediğimiz domainlerden istek gelsin diye

                opt.AddPolicy("AllowSites", builder =>
                 {
                     builder.WithOrigins("https://localhost:44325", "https://www.mysite.com").AllowAnyHeader().AllowAnyMethod();
                 });

                opt.AddPolicy("AllowSites2", builder =>
                {
                    builder.WithOrigins("https://www.mysite2.com").WithHeaders(HeaderNames.ContentType,"x-custom-header"); // Header ında cOntentType'ı => "x-custom-header" olanlara izin ver demektir.
                });

                opt.AddPolicy("AllowSites3", builder =>
                {
                    builder.WithOrigins("https://*.example.com").SetIsOriginAllowedToAllowWildcardSubdomains(); //  .example.com ile biten subdomainlere izin ver ör : mysite.example.com
                });



                #endregion


                #region Kural 3 => Method,COntroller bazında CORS ayarları bütün domainlerden gelen isteklere izin verdik

                opt.AddPolicy("AllowSites4", builder =>
                {
                    builder.WithOrigins("https://localhost:44325").WithMethods("POST","GET").AllowAnyHeader();  
                });


                #endregion

            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CORS.WebApi", Version = "v1" });
            });
        }
         
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CORS.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseCors();  // Kural 1 için CORS Ayarı

           //  app.UseCors("AllowSites");  // Kural 2 için CORS Ayarı eklediğimiz policy kural adını middleware e eklememiz gerekiyor
            
            app.UseCors();  // Kural 3 için CORS Ayarı  Method ve Controller bazında olacağı için boş olmalıdır, COntroller da  ayarlama yaptık

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
