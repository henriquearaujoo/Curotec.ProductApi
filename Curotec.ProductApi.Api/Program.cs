using Curotec.ProductApi.Api.Extensions;
using Curotec.ProductApi.Api.Middlewares;
using Curotec.ProductApi.Api.Options;
using Curotec.ProductApi.Application.Validators;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.ResponseCompression;

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

services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // 🔒 compress even if using HTTPS
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();

    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/json",
        "text/plain",
        "application/javascript",
        "text/css"
    });
});

services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

// Add API Explorer for Swagger
services.AddEndpointsApiExplorer();

services.Configure<RequestLoggingOptions>(
    builder.Configuration.GetSection("RequestLogging"));

services.AddFluentValidationAutoValidation();
services.AddValidatorsFromAssemblyContaining<ProductCreateDtoValidator>();


// Build app
var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
        options.RoutePrefix = "swagger"; 
    });
}

app.UseResponseCompression(); 
app.UseCorrelationId();            
app.UseExceptionHandling(); 
app.UseRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();