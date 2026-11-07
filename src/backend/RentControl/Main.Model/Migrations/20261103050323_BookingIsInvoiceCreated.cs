using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Model.Migrations
{
    /// <inheritdoc />
    public partial class BookingIsInvoiceCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_InvoiceId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Bookings");

            migrationBuilder.AddColumn<bool>(
                name: "IsInvoiceCreated",
                table: "Bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInvoiceCreated",
                table: "Bookings");

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceId",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_InvoiceId",
                table: "Bookings",
                column: "InvoiceId",
                unique: true);
        }
    }
}
