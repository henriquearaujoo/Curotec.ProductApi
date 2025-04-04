using Curotec.ProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Curotec.ProductApi.Infrastructure.Data;

public class AppDbContext_
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Product> Products => Set<Product>();
    }
}