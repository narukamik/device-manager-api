using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace device_manager_api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Brand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValue: new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    RefreshToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "devices",
                columns: new[] { "Id", "Brand", "CreatedBy", "CreationTime", "ModifiedAt", "ModifiedBy", "Name", "State" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000001"), "Apple", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Laptop 001", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000002"), "Samsung", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Smartphone 002", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000003"), "Dell", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Tablet 003", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000004"), "HP", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Desktop 004", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000005"), "Lenovo", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Monitor 005", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000006"), "Sony", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Printer 006", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000007"), "LG", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Scanner 007", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000008"), "Microsoft", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Router 008", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000009"), "Google", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Server 009", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000010"), "Asus", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Smartwatch 010", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000011"), "Apple", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Laptop 011", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000012"), "Samsung", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Smartphone 012", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000013"), "Dell", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Tablet 013", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000014"), "HP", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Desktop 014", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000015"), "Lenovo", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Monitor 015", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000016"), "Sony", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Printer 016", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000017"), "LG", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Scanner 017", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000018"), "Microsoft", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Router 018", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000019"), "Google", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Server 019", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000020"), "Asus", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Smartwatch 020", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000021"), "Apple", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Laptop 021", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000022"), "Samsung", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Smartphone 022", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000023"), "Dell", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Tablet 023", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000024"), "HP", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Desktop 024", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000025"), "Lenovo", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Monitor 025", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000026"), "Sony", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Printer 026", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000027"), "LG", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Scanner 027", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000028"), "Microsoft", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Router 028", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000029"), "Google", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Server 029", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000030"), "Asus", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Smartwatch 030", 0 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000031"), "Apple", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Laptop 031", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000032"), "Samsung", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Smartphone 032", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000033"), "Dell", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Tablet 033", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000034"), "HP", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Desktop 034", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000035"), "Lenovo", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Monitor 035", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000036"), "Sony", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Printer 036", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000037"), "LG", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Scanner 037", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000038"), "Microsoft", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Router 038", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000039"), "Google", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Server 039", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000040"), "Asus", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Smartwatch 040", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000041"), "Apple", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Laptop 041", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000042"), "Samsung", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Smartphone 042", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000043"), "Dell", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Tablet 043", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000044"), "HP", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Desktop 044", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000045"), "Lenovo", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Monitor 045", 1 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000046"), "Sony", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Printer 046", 2 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000047"), "LG", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Scanner 047", 2 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000048"), "Microsoft", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Router 048", 2 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000049"), "Google", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Server 049", 2 },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-000000000050"), "Asus", "system", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Smartwatch 050", 2 }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "Role", "Username" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "$2a$12$yc7jeLROf1Gv/PpQbqQkgOnnOKWbHJZt17fcNZrbM5TkDFOiVtJta", null, null, 0, "admin1" },
                    { new Guid("11111111-1111-1111-1111-111111111112"), "$2a$12$IgdRp/rI37ALjB7qsKAu8Oo70B8ZM7GX1g.r031nJ96fJazRuLhPO", null, null, 0, "admin2" },
                    { new Guid("22222222-2222-2222-2222-222222222221"), "$2a$12$9KedhTiOP/MuS/9OVjTXmeV8.4WjfiGT4JO3qhFHNBlI/JaoqBbwa", null, null, 1, "manager1" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "$$2a$12$I1fLhtp9vplrLTNKEADFOO85IaQFg0nLyeoUUqPAGHTELD5/96Iii", null, null, 1, "manager2" },
                    { new Guid("33333333-3333-3333-3333-333333333331"), "$2a$12$iHTaa1AYo6r2hJVJY876JeUzGQTf8tjFf32jSybFbbf7S6vaayGui", null, null, 2, "viewer1" },
                    { new Guid("33333333-3333-3333-3333-333333333332"), "$2a$12$J.8.n1hf.KXKf8FcOGVSI.r5DFmD.ykbTSsqHMvTKGvGVEw.uI8Dy", null, null, 2, "viewer2" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_devices_Brand",
                table: "devices",
                column: "Brand");

            migrationBuilder.CreateIndex(
                name: "IX_devices_Brand_State",
                table: "devices",
                columns: new[] { "Brand", "State" });

            migrationBuilder.CreateIndex(
                name: "IX_devices_State",
                table: "devices",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                table: "users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
