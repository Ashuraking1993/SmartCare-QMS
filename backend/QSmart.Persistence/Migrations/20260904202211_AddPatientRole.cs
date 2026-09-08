using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QSmart.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), "Patient" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));
        }
    }
}
