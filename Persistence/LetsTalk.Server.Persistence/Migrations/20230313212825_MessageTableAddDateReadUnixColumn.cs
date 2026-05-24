using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MessageTableAddDateReadUnixColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DateReadUnix",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql("UPDATE messages SET DateReadUnix = UNIX_TIMESTAMP(DateRead);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateReadUnix",
                table: "messages");
        }
    }
}
