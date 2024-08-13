using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Services;
using RecipeRandomizer.Business.Utils.Settings;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Web.Utils;

namespace RecipeRandomizer.Web
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private readonly string _version;
        private readonly string _appName;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _version = Configuration.GetValue<string>("AppSettings:Version");
            _appName = Configuration.GetValue<string>("AppSettings:AppName");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RRContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(60))
            );

            // configure strongly typed settings
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            InjectServices(services);

            // configure CORS
            services.AddCors(options =>
            {
                options.AddPolicy("RRCorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed(origin => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // configure API
            services.AddControllers()
                // uses old newtonsoft json serializer as the new json serializer does not support ignoring reference loops yet
                .AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });

            // Register the Swagger API documentation generator
            services.AddSwaggerGen(gen =>
            {
                gen.SwaggerDoc(_version, new OpenApiInfo {Title = $"{_appName} API", Version = _version});
                gen.CustomSchemaIds(selector => selector.FullName);
                gen.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseCors("RRCorsPolicy");
            app.UseHttpsRedirection();

            // set default API route
            app.Map("/api", builder =>
            {
                builder.UseRouting();
                builder.UseMiddleware<ErrorHandlerMiddleware>(); // global error handler
                builder.UseMiddleware<JwtMiddleware>(); // custom jwt auth middleware
                builder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            });

            // url for serving static files
            app.Map("/static", builder => { builder.UseFileServer(); });

            // serve SPA files on root route
            if (!env.IsDevelopment())
                app.UseSpaStaticFiles();

            // enable swagger
            app.UseSwagger();
            app.UseSwaggerUI(opt => { opt.SwaggerEndpoint($"/swagger/{_version}/swagger.json", $"{_appName} {_version}"); });

            // enable SPA frontend
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4242");
            });
        }

        private static void InjectServices(IServiceCollection services)
        {
            services.AddTransient<IRecipeService, RecipeService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<IQuantityService, QuantityService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserService, UserService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IFileService, FileService>();
        }
    }
}
