using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MessageTableAddDateCreatedUnixColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DateCreatedUnix",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql("UPDATE messages SET DateCreatedUnix = UNIX_TIMESTAMP(DateCreated);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreatedUnix",
                table: "messages");
        }
    }
}
