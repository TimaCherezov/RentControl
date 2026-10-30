using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Model.Migrations
{
    /// <inheritdoc />
    public partial class FixBookingStatusSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BookingStatuses",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Ready for pickup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BookingStatuses",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Ready for pickip");
        }
    }
}
