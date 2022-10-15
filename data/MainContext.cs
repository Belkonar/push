using data.ORM;
using Microsoft.EntityFrameworkCore;

namespace data;

public class MainContext : DbContext
{
    public DbSet<ConfigDto> Configs { get; set; }
    public DbSet<OrganizationDto> Organizations { get; set; }
    public DbSet<ThingDto> Things { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: Actually this lol
    }

    // The purpose of this is to ensure dates are tracked correctly
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new ())
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseDto && e.State is EntityState.Added or EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            var dto = (BaseDto)entityEntry.Entity;
            
            dto.Updated = DateTime.Now;

            if (entityEntry.State == EntityState.Added)
            {
                dto.Created = DateTime.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}