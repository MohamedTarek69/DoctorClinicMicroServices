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
            builder.Property(ts => ts.IsAvailable).HasDefaultValue(true);

            // Optional: index for quick lookup
            builder.HasIndex(ts => new { ts.ClinicId, ts.StartTime, ts.EndTime });

            // Relationship TimeSlot (1) <-> (0..1) Appointment
            // The Appointment entity will hold the FK (TimeSlotId) and it must be unique.
            builder.HasOne(ts => ts.Appointment)
                   .WithOne(a => a.TimeSlot)
                   .HasForeignKey<Appointment>(a => a.TimeSlotId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

