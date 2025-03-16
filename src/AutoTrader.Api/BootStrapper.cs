using System;
using System.Text.Json.Serialization;
using AutoTrader.Api.Filters;
using AutoTrader.Api.Converters;
using Microsoft.OpenApi.Models;


namespace AutoTrader.Api;

public static class BootStrapper
{
    public static IServiceCollection AddControllerAndFilters(
        this IServiceCollection services,
        Action<IMvcBuilder> action = default!)
    {
        IMvcBuilder mvcBuilder = services.AddControllers(options =>
        {
            options.Filters.Add(new ResponseActionFilter());
            // options.OutputFormatters.Add(new XmlSerializerOutputFormatterNamespace());
            // XmlWriterSettings xmlWriterSettings = options.OutputFormatters.OfType<XmlSerializerOutputFormatterNamespace>().Single().WriterSettings;
            // xmlWriterSettings.OmitXmlDeclaration = false;
        })
        .ConfigureApiBehaviorOptions(setup =>
        {
            setup.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.Handle;
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new CustomJsonConverterDateTime());
            options.JsonSerializerOptions.Converters.Add(new CustomJsonConverterDateTimeNullable());
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        // .AddXmlSerializerFormatters();
        action?.Invoke(mvcBuilder);
        return services;
    }
    
    public static IServiceCollection AddSwagger(
        this IServiceCollection services)
    {
        _ = services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "AutoTrader API",
                    Version = "0.1",
                    Description = "Setup AutoTrader API",
                    Contact = new OpenApiContact
                    {
                        Name = "Agnaldo Póvoa",
                        Url = new Uri(Environment.GetEnvironmentVariable("APOVOA_URI") ?? "https://github.com/agnaldopovoa")
                    }
                });
            string applicationBasePath = AppContext.BaseDirectory;
            string applicationName = AppDomain.CurrentDomain.FriendlyName;
            string xmlDocumentPath = Path.Combine(applicationBasePath, $"{applicationName}.xml");
            if (File.Exists(xmlDocumentPath))
            {
                options.IncludeXmlComments(xmlDocumentPath);
            }

            // options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            // {
            //     Description = "Insira o token JWT desta maneira: Bearer SEU_TOKEN",
            //     Name = "Authorization",
            //     Scheme = "Bearer",
            //     BearerFormat = "JWT",
            //     In = ParameterLocation.Header,
            //     Type = SecuritySchemeType.ApiKey
            // });

            // options.AddSecurityRequirement(new OpenApiSecurityRequirement
            // {
            //     {
            //         new OpenApiSecurityScheme
            //         {
            //             Reference = new OpenApiReference
            //             {
            //                 Type = ReferenceType.SecurityScheme,
            //                 Id = "Bearer"
            //             }
            //         },
            //         Array.Empty<string>()
            //     }
            // });
        });
        return services;
    }
    
    public static WebApplication UseEndpointsConfiguration(
        this WebApplication app)
    {
        app.MapControllers();
        app.MapGet("/", () => "Ok").WithName("Probe");
        return app;
    }
}
