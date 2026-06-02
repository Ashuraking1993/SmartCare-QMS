using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QSmart.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQueueServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "QueueTickets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "QueueServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPriority = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueServices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QueueTickets_ServiceId",
                table: "QueueTickets",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTickets_QueueServices_ServiceId",
                table: "QueueTickets",
                column: "ServiceId",
                principalTable: "QueueServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueTickets_QueueServices_ServiceId",
                table: "QueueTickets");

            migrationBuilder.DropTable(
                name: "QueueServices");

            migrationBuilder.DropIndex(
                name: "IX_QueueTickets_ServiceId",
                table: "QueueTickets");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "QueueTickets");
        }
    }
}
