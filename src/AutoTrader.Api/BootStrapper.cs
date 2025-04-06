using System.Text.Json.Serialization;
using AutoTrader.Api.Filters;
using AutoTrader.Api.Converters;
using Microsoft.OpenApi.Models;
using Elastic.Apm.DiagnosticSource;
using Elastic.Apm.AspNetCore.DiagnosticListener;
using Serilog;
using Serilog.Exceptions;
using Elastic.Apm.SerilogEnricher;
using Serilog.Sinks.Elasticsearch;
using Elastic.CommonSchema.Serilog;


namespace AutoTrader.Api;

public static class BootStrapper
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, IConfiguration configuration, string applicationName)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithExceptionDetails()
            .Enrich.WithElasticApmCorrelationInfo()
            .Enrich.WithProperty("ApplicationName", $"{applicationName} - {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}")
            .Enrich.WithProperty("ApplicationVersion", Environment.GetEnvironmentVariable("APPLICATION_VERSION") ?? "0.1")
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearch:URI"] ?? "http://localhost:9200"))
            {
                AutoRegisterTemplate = true,
                CustomFormatter = new EcsTextFormatter(),
                IndexFormat = configuration["ElasticSearch:IndexFormat"] ?? "AutoTrade.Api-logs-{0:yyyy-MM-dd}",
                ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ElasticSearch:username"], configuration["ElasticSearch:password"])
            })
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog(Log.Logger, true);

        return builder;
    }

    public static IServiceCollection AddElasticApmConfiguration(
        this IServiceCollection services)
    {
        services.AddElasticApm(
            new HttpDiagnosticsSubscriber(),
            new AspNetCoreDiagnosticSubscriber()
            // new EfCoreDiagnosticsSubscriber()
            );

        return services;
    }

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

internal class EfCoreDiagnosticsSubscriber
{
    public EfCoreDiagnosticsSubscriber()
    {
    }
}