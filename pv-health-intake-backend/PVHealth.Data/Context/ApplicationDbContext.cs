using Microsoft.EntityFrameworkCore;
using PVHealth.Domain.Entities;

namespace PVHealth.Data.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Patient>Patients {get;set;}
    public DbSet<Survey>Surveys {get;set;}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("patients");
            entity.HasKey(e=>e.Id);
            entity.Property(e=>e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e=>e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e=>e.Phone).IsRequired().HasMaxLength(20);
            entity.HasIndex(e=>e.Email);
        });
        modelBuilder.Entity<Survey>(entity =>
        {
            entity.ToTable("surveys");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SurveyData).HasColumnType("jsonb");
            entity.HasOne(e => e.Patient)
                  .WithOne(p => p.Survey)
                  .HasForeignKey<Survey>(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}