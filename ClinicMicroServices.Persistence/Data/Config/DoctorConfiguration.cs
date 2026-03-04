using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicMicroServices.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicMicroServices.Persistence
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doctors");

            // PK
            builder.HasKey(d => d.Id);

            // ✅ Guid auto-generated
            builder.Property(d => d.Id)
                   .ValueGeneratedOnAdd()
                   .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Properties
            builder.Property(d => d.IdentityUserId).IsRequired();
            builder.Property(d => d.DisplayName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Email).IsRequired().HasMaxLength(150);
            builder.Property(d => d.PhoneNumber).IsRequired().HasMaxLength(11);
            builder.Property(d => d.Specialty).IsRequired().HasMaxLength(100);

            // Useful indexes
            builder.HasIndex(d => d.IdentityUserId).IsUnique();
            builder.HasIndex(d => d.Email).IsUnique();

            // Relationship: Doctor 1 -> Many Clinics
            builder.HasMany(d => d.DoctorClinics)
                   .WithOne(c => c.Doctor)
                   .HasForeignKey(c => c.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

