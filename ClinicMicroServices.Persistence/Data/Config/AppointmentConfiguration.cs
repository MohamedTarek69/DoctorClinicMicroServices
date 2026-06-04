using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicMicroServices.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicMicroServices.Persistence.Data.Config
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(a => a.PatientId).IsRequired();

            builder.Property(a => a.Status)
                   .IsRequired()
                   .HasConversion<int>()
                   .HasDefaultValue(AppointmentStatus.Pending);

            // ✅ NOT unique anymore
            builder.HasIndex(a => a.TimeSlotId);

            // ✅ prevent duplicate booking by same patient
            builder.HasIndex(a => new { a.TimeSlotId, a.PatientId })
                   .IsUnique();

            builder.HasIndex(a => new { a.ClinicId, a.PatientId });

            // ✅ relation
            builder.HasOne(a => a.TimeSlot)
                   .WithMany(ts => ts.Appointments)
                   .HasForeignKey(a => a.TimeSlotId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

