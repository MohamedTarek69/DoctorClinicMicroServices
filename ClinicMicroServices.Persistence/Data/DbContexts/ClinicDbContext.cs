using ClinicMicroServices.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Persistence.Data.DbContexts
{
    public class ClinicDbContext : DbContext
    {
        #region Constructors
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) 
            : base(options) 
        { 
        
        }

        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply Fluent Configurations
            modelBuilder.ApplyConfiguration(new DoctorConfiguration());
            modelBuilder.ApplyConfiguration(new DoctorClinicConfiguration());
            modelBuilder.ApplyConfiguration(new TimeSlotConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
        }

        #endregion

        #region DbSets
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<DoctorClinic> DoctorClinics => Set<DoctorClinic>();
        public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        #endregion
    }
}
