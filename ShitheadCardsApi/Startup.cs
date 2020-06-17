using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShitheadCardsApi.DataContext;
using ShitheadCardsApi.Interfaces;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;
using System;

namespace ShitheadCardsApi
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
            services.AddDbContext<ShitheadDBContext>(options => options.UseInMemoryDatabase(databaseName: "ShitheadGames"));

            services.AddControllers();

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "Shithead API", Description = "Quick description of all methods available on the API.", });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);
            });

            services.AddScoped<IGameService, GameService>();
            services.AddSingleton<IShitheadService, ShitheadService>();
            services.AddScoped<IBotPlayerService, BotPlayerService>();
            services.AddHostedService<ConsumeBotPlayersService>();


            services.AddCors(o => o.AddPolicy("CorsDisabled", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shithead API");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors("CorsDisabled");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
