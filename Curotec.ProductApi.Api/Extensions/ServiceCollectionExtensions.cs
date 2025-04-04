using Curotec.ProductApi.Application.Interfaces;
using Curotec.ProductApi.Infrastructure.Caching;
using Curotec.ProductApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using static Curotec.ProductApi.Infrastructure.Data.AppDbContext_;

namespace Curotec.ProductApi.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddMemoryCache();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<CachedProductService>();

        return services;
    }
}