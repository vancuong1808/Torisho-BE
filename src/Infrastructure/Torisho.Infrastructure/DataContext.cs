using Microsoft.EntityFrameworkCore;
using Torisho.Application;

namespace Torisho.Infrastructure;

public class DataContext : DbContext, IDataContext
{
    public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions) {}
    DbSet<T> IDataContext.Set<T>() => Set<T>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // auto scan all assembly of IEntityTypeConfiguration<T> ()
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
