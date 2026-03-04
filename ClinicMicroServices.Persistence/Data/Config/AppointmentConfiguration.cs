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
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.Id);

            // ✅ int auto-increment
            builder.Property(a => a.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(a => a.PatientId).IsRequired();

            builder.Property(a => a.Status)
                   .IsRequired()
                   .HasConversion<int>() // store enum as int
                   .HasDefaultValue(AppointmentStatus.Pending);

            // ✅ Unique TimeSlotId to guarantee 0..1 booking
            builder.HasIndex(a => a.TimeSlotId).IsUnique();

            // Redundant-but-useful index
            builder.HasIndex(a => new { a.ClinicId, a.PatientId });
        }
    }
}

