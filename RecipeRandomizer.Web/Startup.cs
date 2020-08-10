using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace RecipeRandomizer.Web
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private readonly string _version;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _version = Configuration.GetValue<string>("Version");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*services.AddDbContext<RRContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(60))
            );*/

            // configure CORS
            services.AddCors(options =>
            {
                options.AddPolicy("RRCorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
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
                gen.SwaggerDoc(_version, new OpenApiInfo {Title = "Amiet API", Version = _version});
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
                builder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            });

            // use SPA files as frontend
            if (!env.IsDevelopment())
                app.UseSpaStaticFiles();

            // enable swagger
            app.UseSwagger();
            app.UseSwaggerUI(opt => { opt.SwaggerEndpoint("/swagger/" + _version + "/swagger.json", "Amiet API " + _version); });

            // enable SPA frontend
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
            });
        }
    }
}
