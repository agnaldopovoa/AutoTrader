using AutoTrader.Api;
using AutoTrader.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddElasticApmConfiguration(); // Add Elastic APM
builder.Services.AddHttpContextAccessor();     // Add access to IHttpContextAccessor
builder.Services.AddEndpointsApiExplorer();    // Required for Minimal APIs
builder.Services.AddControllerAndFilters();    // Add the filters
builder.Services.AddSwagger();                 // Configure Swagger
builder.Services.AddInfrastructureRepositories();

var app = builder.Build();

app.UseEndpointsConfiguration(); // Map the Contorllers endpoints
app.UseSwagger();                // Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwaggerUI();              // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.;

app.Run();
