using Curotec.ProductApi.Api.Extensions;
using Curotec.ProductApi.Api.Middlewares;
using Curotec.ProductApi.Api.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Add Application Services (DbContext, Repositories, Caching, etc.)
services.AddApplicationServices(config);

// Add Controllers
services.AddControllers();

// Add Swagger Generation
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product API",
        Version = "v1",
        Description = "Backend API with specification pattern, caching, and clean architecture"
    });

    // Optional: Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add API Explorer for Swagger
services.AddEndpointsApiExplorer();

services.Configure<RequestLoggingOptions>(
    builder.Configuration.GetSection("RequestLogging"));

// Optional: Add FluentValidation, API Versioning, etc.

// Build app
var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
        options.RoutePrefix = string.Empty; // Show Swagger at root
    });
}

app.UseRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();