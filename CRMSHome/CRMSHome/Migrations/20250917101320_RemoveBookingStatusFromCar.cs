using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMSHome.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBookingStatusFromCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingStatus",
                table: "Cars");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookingStatus",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
