using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RestaurantListing.Data;
using RestaurantListing.DTOs;
using RestaurantListing.Repositories;
using RestaurantListing.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantListing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddDbContext<DatabaseContext>(
                options => options.UseSqlServer(
                    Configuration.GetConnectionString("sqlConnection")
                    )
                );

            services.AddMemoryCache();
            //services.AddResponseCaching();

            services.AddAuthentication();
            services.AddIdentityConfiguration();

            //authorization policies
            services.AddAutorizationPolicies();

            services.AddJWTAuthentication(Configuration);

            services.AddCors(
                  op => op.AddPolicy("AllowAll", builder =>
                  {
                      builder.AllowAnyOrigin();
                      builder.AllowAnyMethod();
                      builder.AllowAnyHeader();
                  })
                ); ;

            services.AddAutoMapper(typeof(MapperInitializer));

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAuthManager, AuthManager>();

            services.AddSingleton<CachingProperties>();

            services.AddControllers().AddNewtonsoftJson(
                op => op.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore
                ); 
           
            services.AddVersioning();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RestaurantListing", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSeedMiddleware();
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestaurantListing v1"));
                
            }

            app.UseCustomeExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();


            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors();

            //app.UseResponseCaching();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
