using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessCenter.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentStatusToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Appointments");
        }
    }
}

