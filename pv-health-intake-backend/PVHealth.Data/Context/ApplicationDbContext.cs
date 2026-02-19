using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using PVHealth.Domain.Entities;

namespace PVHealth.Data.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Patient>Patients {get;set;}
    public DbSet<Survey>Surveys {get;set;}
    public DbSet<User>Users {get;set;}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.OAuthProvider).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OAuthId).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.MfaEnabled).HasDefaultValue(false);
            entity.Property(e => e.MfaSecret).HasMaxLength(500);
            entity.Property(e => e.BackupCodes).HasMaxLength(2000);
        });
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("patients");
            entity.HasKey(e=>e.Id);
            entity.Property(e=>e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e=>e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e=>e.Phone).IsRequired().HasMaxLength(20);
            entity.HasIndex(e=>e.Email);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Patients)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
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