using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicMicroServices.Persistence.data.ClinicMigrations
{
    /// <inheritdoc />
    public partial class AddPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "TimeSlots",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAtBooking",
                table: "Appointments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "PriceAtBooking",
                table: "Appointments");
        }
    }
}
