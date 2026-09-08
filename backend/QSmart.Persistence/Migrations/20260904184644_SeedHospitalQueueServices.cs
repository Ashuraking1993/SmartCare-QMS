using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QSmart.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedHospitalQueueServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "QueueServices",
                columns: new[] { "Id", "IsActive", "IsPriority", "Name", "Prefix" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), true, false, "General Consultation", "G" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), true, true, "Emergency", "E" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), true, false, "Specialist", "S" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), true, false, "Laboratory", "L" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), true, false, "Dental Care", "D" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), true, false, "Other Services", "O" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "QueueServices",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "QueueServices",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "QueueServices",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "QueueServices",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "QueueServices",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "QueueServices",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));
        }
    }
}
