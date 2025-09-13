using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Message = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResponseText = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatResponses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResponseMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    role = table.Column<string>(type: "TEXT", nullable: true),
                    content = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseMessage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    role = table.Column<string>(type: "TEXT", nullable: true),
                    content = table.Column<string>(type: "TEXT", nullable: true),
                    ChatRequestId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestMessage_ChatRequests_ChatRequestId",
                        column: x => x.ChatRequestId,
                        principalTable: "ChatRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Choice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    messageId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChatResponseId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Choice_ChatResponses_ChatResponseId",
                        column: x => x.ChatResponseId,
                        principalTable: "ChatResponses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Choice_ResponseMessage_messageId",
                        column: x => x.messageId,
                        principalTable: "ResponseMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Choice_ChatResponseId",
                table: "Choice",
                column: "ChatResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_Choice_messageId",
                table: "Choice",
                column: "messageId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestMessage_ChatRequestId",
                table: "RequestMessage",
                column: "ChatRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Choice");

            migrationBuilder.DropTable(
                name: "RequestMessage");

            migrationBuilder.DropTable(
                name: "ChatResponses");

            migrationBuilder.DropTable(
                name: "ResponseMessage");

            migrationBuilder.DropTable(
                name: "ChatRequests");
        }
    }
}
