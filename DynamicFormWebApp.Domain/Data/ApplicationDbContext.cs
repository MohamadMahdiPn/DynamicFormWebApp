using DynamicFormWebApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DynamicFormWebApp.Domain.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    :IdentityDbContext<ApplicationUser,ApplicationRole,Guid>(options)
{
    public DbSet<BaseForm> Forms => Set<BaseForm>();
    public DbSet<FormField> FormFields => Set<FormField>();
    public DbSet<FormSubmission> FormSubmissions => Set<FormSubmission>();
    public DbSet<FormFieldValidation> FormFieldValidations => Set<FormFieldValidation>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FormField>()
            .Property(f => f.OptionsJson)
            .HasColumnType("nvarchar(max)");

        modelBuilder.Entity<FormSubmission>()
            .Property(s => s.ValuesJson)
            .HasColumnType("nvarchar(max)");

        base.OnModelCreating(modelBuilder);
    }
}