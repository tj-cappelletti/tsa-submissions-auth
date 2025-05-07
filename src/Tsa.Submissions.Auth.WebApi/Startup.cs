using System.Linq;
using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tsa.Submissions.Auth.WebApi.Services;

namespace Tsa.Submissions.Auth.WebApi;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;
    // public object ConfigurationKeys { get; private set; }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

        // Add Pingable Services - Should match MongoDB Services
        var pingableServiceType = typeof(IPingableService);
        var pingableServices = assemblyTypes
            .Where(type => pingableServiceType.IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false })
            .ToList();

        foreach (var pingableService in pingableServices)
        {
            services.AddSingleton(typeof(IPingableService), pingableService);
        }

        // Setup Controllers
        services
            .AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        // Add Fluent Validation
        services.AddFluentValidationAutoValidation();

        // Add Swagger
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        });
    }
}
