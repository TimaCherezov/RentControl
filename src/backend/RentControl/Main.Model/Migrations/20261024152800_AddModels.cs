using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "users",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "users",
                newName: "ix_users_email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "refresh_tokens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "refresh_tokens",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "refresh_tokens",
                newName: "token_hash");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "refresh_tokens",
                newName: "revoked_at");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "refresh_tokens",
                newName: "expires_at");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_TokenHash",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_token_hash");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_refresh_tokens",
                table: "refresh_tokens",
                column: "id");

            migrationBuilder.CreateTable(
                name: "booking_statuses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    market_value = table.Column<decimal>(type: "numeric", nullable: false),
                    deposit_override = table.Column<decimal>(type: "numeric", nullable: true),
                    daily_rate = table.Column<decimal>(type: "numeric", nullable: false),
                    hourly_rate = table.Column<decimal>(type: "numeric", nullable: false),
                    weekly_rate = table.Column<decimal>(type: "numeric", nullable: true),
                    monthly_rate = table.Column<decimal>(type: "numeric", nullable: true),
                    short_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "states",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    storage_key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_main = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    alt_text = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_images_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_units",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    state_id = table.Column<Guid>(type: "uuid", nullable: false),
                    serial_number = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    purchase_date = table.Column<DateOnly>(type: "date", nullable: false),
                    purchase_price = table.Column<decimal>(type: "numeric", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_units_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_units_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_units_states_state_id",
                        column: x => x.state_id,
                        principalTable: "states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    actual_return_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    total_rental_cost = table.Column<decimal>(type: "numeric", nullable: true),
                    deposit_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    status_changed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    booking_status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bookings", x => x.id);
                    table.ForeignKey(
                        name: "fk_bookings_booking_statuses_booking_status_id",
                        column: x => x.booking_status_id,
                        principalTable: "booking_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bookings_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bookings_product_units_product_unit_id",
                        column: x => x.product_unit_id,
                        principalTable: "product_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bookings_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_booking_statuses_code",
                table: "booking_statuses",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bookings_booking_status_id",
                table: "bookings",
                column: "booking_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_location_id",
                table: "bookings",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_product_unit_id",
                table: "bookings",
                column: "product_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_user_id",
                table: "bookings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_images_product_id",
                table: "product_images",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_units_location_id",
                table: "product_units",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_units_product_id_serial_number",
                table: "product_units",
                columns: new[] { "product_id", "serial_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_units_state_id",
                table: "product_units",
                column: "state_id");

            migrationBuilder.CreateIndex(
                name: "ix_states_code",
                table: "states",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                table: "refresh_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                table: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "product_images");

            migrationBuilder.DropTable(
                name: "booking_statuses");

            migrationBuilder.DropTable(
                name: "product_units");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "states");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "users",
                newName: "PasswordHash");

            migrationBuilder.RenameIndex(
                name: "ix_users_email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "refresh_tokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "refresh_tokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "token_hash",
                table: "refresh_tokens",
                newName: "TokenHash");

            migrationBuilder.RenameColumn(
                name: "revoked_at",
                table: "refresh_tokens",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "refresh_tokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_token_hash",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_TokenHash");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
