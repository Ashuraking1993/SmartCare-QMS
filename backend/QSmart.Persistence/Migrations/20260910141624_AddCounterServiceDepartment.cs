using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QSmart.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterServiceDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "Counters",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Counters",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "Name", "ServiceId" },
                values: new object[] { "General Consultation", new Guid("10000000-0000-0000-0000-000000000001") });

            migrationBuilder.UpdateData(
            table: "Counters",
            keyColumn: "Id",
            keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"),
            columns: new[] { "Name", "ServiceId" },
            values: new object[]
            {
                "Emergency",
                new Guid("10000000-0000-0000-0000-000000000002")
            });
            migrationBuilder.InsertData(
                table: "Counters",
                columns: new[] { "Id", "BranchId", "IsActive", "Name", "ServiceId" },
                values: new object[,]
                {
                   
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), true, "Specialist", new Guid("10000000-0000-0000-0000-000000000003") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), true, "Laboratory", new Guid("10000000-0000-0000-0000-000000000004") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), true, "Dental Care", new Guid("10000000-0000-0000-0000-000000000005") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), true, "Other Services", new Guid("10000000-0000-0000-0000-000000000006") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Counters_ServiceId",
                table: "Counters",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Counters_QueueServices_ServiceId",
                table: "Counters",
                column: "ServiceId",
                principalTable: "QueueServices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Counters_QueueServices_ServiceId",
                table: "Counters");

            migrationBuilder.DropIndex(
                name: "IX_Counters_ServiceId",
                table: "Counters");

           
            migrationBuilder.DeleteData(
                table: "Counters",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"));

            migrationBuilder.DeleteData(
                table: "Counters",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"));

            migrationBuilder.DeleteData(
                table: "Counters",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"));

            migrationBuilder.DeleteData(
                table: "Counters",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6"));

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Counters");

            migrationBuilder.UpdateData(
                table: "Counters",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Name",
                value: "Counter 1");
        }
    }
}
