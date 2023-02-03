using data.ORM;
using Microsoft.EntityFrameworkCore;

namespace data;

public class MainContext : DbContext
{
    // KVs
    public DbSet<ConfigDto> Configs { get; set; }
    public DbSet<PolicyDto> Policies { get; set; }

    // Org Structure
    public DbSet<OrganizationDto> Organizations { get; set; }
    public DbSet<ThingDto> Things { get; set; }
    
    public DbSet<PipelineDTO> Pipelines { get; set; }
    public DbSet<PipelineVersionDTO> PipelineVersions { get; set; }

    // Resource types
    public DbSet<DeployableDto> Deployables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost:5430;Database=devops;Username=postgres;Password=testpwd");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrganizationDto>()
            .HasMany<ThingDto>()
            .WithOne()
            .HasForeignKey(x => x.OrganizationId);
        
        modelBuilder.Entity<OrganizationDto>()
            .HasMany<PipelineDTO>()
            .WithOne()
            .HasForeignKey(x => x.OrganizationId);

        modelBuilder.Entity<ThingDto>()
            .HasOne<DeployableDto>()
            .WithOne()
            .HasForeignKey<DeployableDto>(x => x.ThingId);
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