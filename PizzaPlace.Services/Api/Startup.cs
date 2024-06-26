﻿using System.Diagnostics;
using Asp.Versioning;
using FluentValidation;
using PizzaPlace.Services.Application;
using PizzaPlace.Services.Application.Infrastructure;
using PizzaPlace.Services.Application.Services;
using PizzaPlace.Services.Infrastructure;

namespace PizzaPlace.Services
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
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options => 
                options.CustomSchemaIds(type => type.ToString().Replace("+", ".")));
            services
                .AddSingleton<IDataStore, DataStore>()
                .AddScoped<IOrderService, OrderService>()
                .AddScoped<IOrderRepository, OrderRepository>()
                .AddScoped<IItemRepository, ItemRepository>()
                .AddScoped<IOrderPriceCalculatorService, OrderPriceCalculatorService>()
                .AddValidatorsFromAssembly(typeof(Program).Assembly);
            services
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1);
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'V";
                    options.SubstituteApiVersionInUrl = true;
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaPlace.Services v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}