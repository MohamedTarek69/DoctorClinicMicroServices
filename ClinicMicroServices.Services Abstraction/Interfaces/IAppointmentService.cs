using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.Appointment;
using ClinicMicroServices.Shared.DTOs.PatientDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services_Abstraction.Interfaces
{
    public interface IAppointmentService
    {
        Task<Result<AppointmentResponse>> BookAsync(string patientId, CreateAppointmentRequest request);
        Task<Result<bool>> CancelAsync(int appointmentId, string patientId);
        Task<Result<AppointmentResponse>> UpdateStatusAsync(UpdateAppointmentStatusRequest request);
        Task<Result<List<AppointmentResponse>>> ShowClinicAppointmentsAsync(int clinicId);
        Task<Result<List<AppointmentResponse>>> ShowPatientAppointmentsAsync(string patientId);
        Task<Result<List<AppointmentResponse>>> ShowPatientPendingAppointmentsAsync(string patientId);
        Task<Result<List<AppointmentResponse>>> ShowPatientConfirmedAppointmentsAsync(string patientId);
        Task<Result<List<AppointmentResponse>>> ShowPatientCancelledAppointmentsAsync(string patientId);
        Task<Result<List<AppointmentResponse>>> ShowClinicConfirmedAppointmentsAsync(int clinicId);
        Task<Result<List<AppointmentResponse>>> ShowClinicPendingAppointmentsAsync(int clinicId);
        Task<Result<List<AppointmentResponse>>> ShowClinicCancelledAppointmentsAsync(int clinicId);
        Task<Result<ReturnedPatientDetailsDto>> GetAppointmentPatientDetailsAsync(int appointmentId, string token);
        Task<Result<List<AppointmentResponse>>> GetAllAppointmentsAsync();
    }
}
