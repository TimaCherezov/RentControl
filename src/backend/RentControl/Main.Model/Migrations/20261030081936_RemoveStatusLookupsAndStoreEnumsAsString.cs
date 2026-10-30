using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Main.Model.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStatusLookupsAndStoreEnumsAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_BookingStatuses_BookingStatusId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductUnits_States_StateId",
                table: "ProductUnits");

            migrationBuilder.DropTable(
                name: "BookingStatuses");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropIndex(
                name: "IX_ProductUnits_StateId",
                table: "ProductUnits");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BookingStatusId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "ProductUnits");

            migrationBuilder.DropColumn(
                name: "BookingStatusId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "ProductUnits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BookingStatus",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "ProductUnits");

            migrationBuilder.DropColumn(
                name: "BookingStatus",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "ProductUnits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BookingStatusId",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BookingStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BookingStatuses",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "PENDING", "Pending" },
                    { 2, "CANCELLED", "Cancelled" },
                    { 3, "CONFIRMED", "Confirmed" },
                    { 4, "READY_FOR_PICKUP", "Ready for pickup" },
                    { 5, "ACTIVE", "Active" },
                    { 6, "OVERDUE", "Overdue" },
                    { 7, "RETURNED", "Returned" },
                    { 8, "COMPLETED", "Completed" }
                });

            migrationBuilder.InsertData(
                table: "States",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "NEW", "New" },
                    { 2, "EXCELLENT", "Excellent" },
                    { 3, "GOOD", "Good" },
                    { 4, "USED", "Used" },
                    { 5, "NEEDS_MAINTENANCE", "Needs maintenance" },
                    { 6, "NEEDS_REPAIR", "Needs repair" },
                    { 7, "BROKEN", "Broken" },
                    { 8, "WRITE_OFF", "Write off" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductUnits_StateId",
                table: "ProductUnits",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingStatusId",
                table: "Bookings",
                column: "BookingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingStatuses_Code",
                table: "BookingStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_Code",
                table: "States",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_BookingStatuses_BookingStatusId",
                table: "Bookings",
                column: "BookingStatusId",
                principalTable: "BookingStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUnits_States_StateId",
                table: "ProductUnits",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
