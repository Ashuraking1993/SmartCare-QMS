using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QSmart.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "QueueTickets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "QueueTickets");
        }
    }
}
