using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OMSServiceMini.AppHelpers;
using OMSServiceMini.Data;
using System;
using System.Configuration;

namespace OMSServiceMini
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
            #region In-Memory Cache
            // настройки конфигураций 
            services.Configure<CacheConfiguration>(Configuration.GetSection("CacheConfiguration"));

            // для In-Memory Caching
            services.AddMemoryCache();
            services.AddTransient<MemoryCacheService>();

            // todo: добавить для Redis
            // для RedisCacheService

            services.AddTransient<Func<CacheTech, ICacheService>>(serviceProvider => key =>
            { // todo: по идее свич не нужен, так как используется только In-Memory Caching
                switch (key)
                {
                    case CacheTech.Memory:
                        return serviceProvider.GetService<MemoryCacheService>();
                    // место для case для Redis
                    default:
                        return serviceProvider.GetService<MemoryCacheService>();
                }
            });

            // строка подключения для хранения данных задания Hangfire
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("OMSDatabase")));
            services.AddHangfireServer();

            #endregion

            services.AddControllers();
            string connection = Configuration.GetConnectionString("OMSDatabase");
            services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(connection));

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "OMSServiceMini";
                    document.Info.Description = "A simple study project ASP.NET Core web API";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Boris Minin",
                        Email = "boris.minin@outlook.com",
                        Url = "https://www.facebook.com/borisminindeveloper"
                    };
                    document.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = "Look at my GitHub",
                        Url = "https://github.com/BorisMinin"
                    };
                };
            });

            // https://stackoverflow.com/questions/59199593/net-core-3-0-possible-object-cycle-was-detected-which-is-not-supported
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            //путь, по которому отслеживается задания Hangfire через панель мониторинга 
            app.UseHangfireDashboard("/jobs");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}