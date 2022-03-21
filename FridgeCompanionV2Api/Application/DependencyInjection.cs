using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FridgeCompanionV2Api.Application.Common.CommonServices;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Options;
using Microsoft.Extensions.Configuration;
using FridgeCompanionV2Api.Application.Common.HttpClients;
using System;

namespace FridgeCompanionV2Api.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.Configure<AwsOptions>(configuration.GetSection(nameof(AwsOptions)));
            var barcodeOptions = configuration.GetSection(nameof(BarcodeOptions)).Get<BarcodeOptions>();
            services.AddHttpClient<IBarcodeClient, BarcodeClient>("BarcodeApi",
                client =>
                {
                    client.BaseAddress = new Uri(barcodeOptions.BaseUrl);
                    client.DefaultRequestHeaders.Add("User-Agent", "FridgeCompanion");
                });
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddScoped<IRecipeService, RecipeService>();


            return services;
        }
    }
}
