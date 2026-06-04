using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicMicroServices.Persistence.Data.ClinicMigrations
{
    /// <inheritdoc />
    public partial class Fix_Appointment_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_TimeSlotId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "TimeSlots");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "TimeSlots",
                type: "int",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TimeSlotId",
                table: "Appointments",
                column: "TimeSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TimeSlotId_PatientId",
                table: "Appointments",
                columns: new[] { "TimeSlotId", "PatientId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_TimeSlotId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_TimeSlotId_PatientId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "TimeSlots");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "TimeSlots",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TimeSlotId",
                table: "Appointments",
                column: "TimeSlotId",
                unique: true);
        }
    }
}
