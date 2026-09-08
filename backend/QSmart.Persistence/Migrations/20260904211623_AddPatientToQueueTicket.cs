using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QSmart.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientToQueueTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "QueueTickets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QueueTickets_UserId",
                table: "QueueTickets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTickets_Users_UserId",
                table: "QueueTickets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueTickets_Users_UserId",
                table: "QueueTickets");

            migrationBuilder.DropIndex(
                name: "IX_QueueTickets_UserId",
                table: "QueueTickets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QueueTickets");
        }
    }
}
