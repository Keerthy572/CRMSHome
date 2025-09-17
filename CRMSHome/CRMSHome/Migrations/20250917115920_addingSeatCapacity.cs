using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMSHome.Migrations
{
    /// <inheritdoc />
    public partial class addingSeatCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeatCapacity",
                table: "Filters",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatCapacity",
                table: "Filters");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
