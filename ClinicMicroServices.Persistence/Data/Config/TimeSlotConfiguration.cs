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
    public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
    {
        public void Configure(EntityTypeBuilder<TimeSlot> builder)
        {
            builder.ToTable("TimeSlots");

            builder.HasKey(ts => ts.Id);

            // ✅ int auto-increment
            builder.Property(ts => ts.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(ts => ts.StartTime).IsRequired();
            builder.Property(ts => ts.EndTime).IsRequired();
            builder.Property(ts => ts.Capacity).HasDefaultValue(10);

            // Optional: index for quick lookup
            builder.HasIndex(ts => new { ts.ClinicId, ts.StartTime, ts.EndTime });

            // Relationship TimeSlot (1) <-> (0..M) Appointment
            // The Appointment entity will hold the FK (TimeSlotId) and it must be unique.
            builder.HasMany(ts => ts.Appointments)
                   .WithOne(a => a.TimeSlot)
                   .HasForeignKey(a => a.TimeSlotId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

