<h1 align="center">🏥 Doctor & Clinic Management MicroService</h1>

<p align="center">
  <b>ASP.NET Core Web API | Entity Framework Core | SQL Server | Microservices Architecture</b>
</p>

<p align="center">
  A dedicated microservice responsible for managing doctors, clinics, appointments, time slots, and healthcare dashboards within the MedGuide Healthcare Platform.
</p>

---

## 🏗️ Project Overview

The Doctor & Clinic Management MicroService handles healthcare operations related to doctors and clinics.

The service enables:

- Doctor Profile Management
- Clinic Management
- Appointment Booking
- Appointment Tracking
- Time Slot Scheduling
- Healthcare Dashboards
- Patient Appointment Monitoring

---

## 🎯 Goals

- Manage doctors and clinics independently.
- Simplify appointment scheduling.
- Improve clinic resource utilization.
- Support scalable healthcare operations.
- Enable real-time appointment management.

---

## ✨ Main Features

| Feature | Description |
|----------|-------------|
| 👨‍⚕️ Doctor Management | Create, update, activate, and deactivate doctors |
| 🏥 Clinic Management | Create and manage doctor clinics |
| 📅 Appointment Booking | Schedule and manage appointments |
| ⏰ Time Slot Scheduling | Manage clinic availability |
| 📊 Dashboards | Doctor, Clinic, and Admin analytics |
| 🔍 Specialty Search | Search doctors by specialty |
| 📈 Appointment Tracking | Monitor appointment statuses |

---

## 🧱 Architecture

The service follows a microservices-based architecture:

```text
Patient Service
       │
       ▼
Doctor & Clinic MicroService
       │
 ┌─────┼─────┐
 ▼     ▼     ▼
Doctors Clinics Appointments
       │
       ▼
Time Slots
```

### Benefits

- Independent Deployment
- Scalability
- Separation of Concerns
- Fault Isolation
- Easy Maintenance

---

## 🧰 Tech Stack

| Category | Technology |
|-----------|-------------|
| Backend | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Architecture | Microservices |
| Documentation | Swagger |
| Mapping | AutoMapper |
| Design Patterns | Repository Pattern, Dependency Injection |
| Version Control | Git & GitHub |

---

# 👨‍⚕️ Doctor Management

### Create Doctor

```http
POST /doctors/CreateDoctor
```

### Get All Doctors

```http
GET /doctors/GetAllDoctors
```

### Get Doctor Details

```http
GET /doctors/GetAllDoctorDetatils/{id}
```

### Update Doctor

```http
PUT /doctors/UpdateDoctor/{id}
```

### Update Password

```http
PUT /doctors/UpdateDoctorPassword/{id}
```

### Activate Doctor

```http
PATCH /doctors/activate/{id}
```

### Deactivate Doctor

```http
PATCH /doctors/deactivate/{id}
```

### Search By Specialty

```http
GET /doctors/BySpecialty/{specialty}
```

---

# 🏥 Clinic Management

### Create Clinic

```http
POST /doctorclinics/CreateClinics
```

### Get My Clinics

```http
GET /doctorclinics/MyClinics
```

### Update Clinic

```http
PUT /doctorclinics/UpdateClinics/{id}
```

### Delete Clinic

```http
DELETE /doctorclinics/DeleteClinics/{id}
```

### Get All Clinics

```http
GET /doctorclinics/GetAllClinics
```

### Get Clinic By Id

```http
GET /doctorclinics/GetClinicsById/{id}
```

---

# 📅 Appointment Management

### Book Appointment

```http
POST /appointments/book
```

### Cancel Appointment

```http
DELETE /appointments/{appointmentId}
```

### Update Appointment Status

```http
PUT /appointments/updatestatus
```

### Get All Appointments

```http
GET /appointments/all
```

### Show Clinic Appointments

```http
GET /appointments/ShowClinicAppointments
```

### Show Patient Appointments

```http
GET /appointments/ShowPatientAppointments
```

---

## Appointment Statuses

### Confirmed

```http
GET /appointments/clinic/{clinicId}/confirmed
```

### Pending

```http
GET /appointments/clinic/{clinicId}/pending
```

### Cancelled

```http
GET /appointments/clinic/{clinicId}/cancelled
```

---

# ⏰ Time Slot Management

### Create Time Slot

```http
POST /timeslots/createtimeslots
```

### Get Clinic Time Slots

```http
GET /timeslots/GetTimeSlotsByClinic/{clinicId}
```

### Get Available Time Slots

```http
GET /timeslots/getavailabletimeslots/{clinicId}
```

### Update Time Slot

```http
PUT /timeslots/updatetimeslots/{id}
```

### Delete Time Slot

```http
DELETE /timeslots/deletetimeslots/{id}
```

---

# 📊 Dashboard Analytics

### Clinic Dashboard

```http
GET /api/Dashboard/DoctorClinic/Dashboard/{clinicId}
```

### Doctor Dashboard

```http
GET /api/Dashboard/Doctor/Dashboard
```

### Admin Dashboard

```http
GET /api/Dashboard/admin/Dashboard
```

---

## 📦 Core Entities

### Doctor

- Profile Information
- Specialty
- Status
- Clinics

### Clinic

- Name
- Address
- Doctor
- Time Slots

### Appointment

- Patient
- Doctor
- Clinic
- Status
- Appointment Date

### TimeSlot

- Start Time
- End Time
- Availability

---

## 🧠 Key Concepts

- 🏥 Healthcare Management
- 📅 Appointment Scheduling
- 👨‍⚕️ Doctor Administration
- ⏰ Time Slot Management
- 📊 Dashboard Analytics
- 📦 Repository Pattern
- 💉 Dependency Injection
- 🗃️ Entity Framework Core
- 🚀 RESTful API Design

---

## 🚀 Getting Started

### Clone Repository

```bash
git clone <repository-url>
```

### Configure Database

Update:

```json
appsettings.json
```

### Apply Migrations

```powershell
Update-Database
```

### Run Application

```bash
dotnet run
```

---

## 📌 Future Enhancements

- Docker Support
- Kubernetes Deployment
- Notification Service Integration
- Email Reminders
- SMS Appointment Alerts
- Advanced Reporting

---

## 👨‍💻 Author

**Mohamed Tarek**

- GitHub: https://github.com/MohamedTarek69

---

<p align="center">
🏥 Healthcare Appointment & Clinic Management powered by Microservices.
</p>
