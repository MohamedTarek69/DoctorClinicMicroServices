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
    public class DoctorClinicConfiguration : IEntityTypeConfiguration<DoctorClinic>
    {
        public void Configure(EntityTypeBuilder<DoctorClinic> builder)
        {
            builder.ToTable("DoctorClinics");

            // PK
            builder.HasKey(c => c.Id);

            // ✅ int auto-increment (Identity)
            builder.Property(c => c.Id)
                   .ValueGeneratedOnAdd();

            // Properties
            builder.Property(c => c.ClinicName).IsRequired().HasMaxLength(120);
            builder.Property(c => c.ClinicAddress).IsRequired().HasMaxLength(250);
            builder.Property(c => c.Description).HasMaxLength(1000);

            // Relationship: Clinic 1 -> Many TimeSlots
            builder.HasMany(c => c.TimeSlots)
                   .WithOne(ts => ts.Clinic)
                   .HasForeignKey(ts => ts.ClinicId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Clinic 1 -> Many Appointments
            builder.HasMany(c => c.Appointments)
                   .WithOne(a => a.Clinic)
                   .HasForeignKey(a => a.ClinicId)
                   .OnDelete(DeleteBehavior.Restrict);
            // Restrict عشان لو عندك حجوزات، متتمسحش العيادة بالغلط
        }
    }
}
