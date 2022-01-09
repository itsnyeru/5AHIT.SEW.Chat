using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class initcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CHATS_BT",
                columns: table => new
                {
                    CHAT_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LAST_ENTRY = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHATS_BT", x => x.CHAT_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    USER_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USERNAME = table.Column<string>(type: "varchar(16)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EMAIL = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IMAGE_CONTENT = table.Column<byte[]>(type: "longblob", nullable: true),
                    IMAGE_TYPE = table.Column<string>(type: "varchar(32)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CHAT_MODE = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DARK_MODE = table.Column<ulong>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.USER_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GROUPCHATS",
                columns: table => new
                {
                    CHAT_ID = table.Column<long>(type: "bigint", nullable: false),
                    NAME = table.Column<string>(type: "varchar(32)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GROUPCHATS", x => x.CHAT_ID);
                    table.ForeignKey(
                        name: "FK_GROUPCHATS_CHATS_BT_CHAT_ID",
                        column: x => x.CHAT_ID,
                        principalTable: "CHATS_BT",
                        principalColumn: "CHAT_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SINGLECHATS",
                columns: table => new
                {
                    CHAT_ID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SINGLECHATS", x => x.CHAT_ID);
                    table.ForeignKey(
                        name: "FK_SINGLECHATS_CHATS_BT_CHAT_ID",
                        column: x => x.CHAT_ID,
                        principalTable: "CHATS_BT",
                        principalColumn: "CHAT_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FRIEND_REQUESTS_JT",
                columns: table => new
                {
                    SENDER_ID = table.Column<long>(type: "bigint", nullable: false),
                    RECEIVER_ID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FRIEND_REQUESTS_JT", x => new { x.SENDER_ID, x.RECEIVER_ID });
                    table.ForeignKey(
                        name: "FK_FRIEND_REQUESTS_JT_USERS_RECEIVER_ID",
                        column: x => x.RECEIVER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID");
                    table.ForeignKey(
                        name: "FK_FRIEND_REQUESTS_JT_USERS_SENDER_ID",
                        column: x => x.SENDER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FRIENDS_JT",
                columns: table => new
                {
                    USER_ID = table.Column<long>(type: "bigint", nullable: false),
                    FRIEND_ID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FRIENDS_JT", x => new { x.USER_ID, x.FRIEND_ID });
                    table.ForeignKey(
                        name: "FK_FRIENDS_JT_USERS_FRIEND_ID",
                        column: x => x.FRIEND_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FRIENDS_JT_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MESSAGES",
                columns: table => new
                {
                    MESSAGE_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<long>(type: "bigint", nullable: true),
                    CHAT_ID = table.Column<long>(type: "bigint", nullable: true),
                    SEND_AT = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Content = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MESSAGES", x => x.MESSAGE_ID);
                    table.ForeignKey(
                        name: "FK_MESSAGES_CHATS_BT_CHAT_ID",
                        column: x => x.CHAT_ID,
                        principalTable: "CHATS_BT",
                        principalColumn: "CHAT_ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MESSAGES_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "USER_CODES",
                columns: table => new
                {
                    TYPE = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    USER_ID = table.Column<long>(type: "bigint", nullable: false),
                    CODE = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_CODES", x => new { x.USER_ID, x.TYPE });
                    table.ForeignKey(
                        name: "FK_USER_CODES_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CHAT_HAS_USERS_JT",
                columns: table => new
                {
                    CHAT_ID = table.Column<long>(type: "bigint", nullable: false),
                    USER_ID = table.Column<long>(type: "bigint", nullable: false),
                    MESSAGE_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHAT_HAS_USERS_JT", x => new { x.CHAT_ID, x.USER_ID });
                    table.ForeignKey(
                        name: "FK_CHAT_HAS_USERS_JT_CHATS_BT_CHAT_ID",
                        column: x => x.CHAT_ID,
                        principalTable: "CHATS_BT",
                        principalColumn: "CHAT_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CHAT_HAS_USERS_JT_MESSAGES_MESSAGE_ID",
                        column: x => x.MESSAGE_ID,
                        principalTable: "MESSAGES",
                        principalColumn: "MESSAGE_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CHAT_HAS_USERS_JT_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MESSAGE_HAS_ATTACHMENTS",
                columns: table => new
                {
                    ATTACHMENT_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MESSAGE_ID = table.Column<long>(type: "bigint", nullable: false),
                    NAME = table.Column<string>(type: "varchar(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ATTACHMENT_CONTENT = table.Column<byte[]>(type: "longblob", nullable: false),
                    ATTACHMENT_TYPE = table.Column<string>(type: "varchar(32)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MESSAGE_HAS_ATTACHMENTS", x => x.ATTACHMENT_ID);
                    table.ForeignKey(
                        name: "FK_MESSAGE_HAS_ATTACHMENTS_MESSAGES_MESSAGE_ID",
                        column: x => x.MESSAGE_ID,
                        principalTable: "MESSAGES",
                        principalColumn: "MESSAGE_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CHAT_HAS_USERS_JT_MESSAGE_ID",
                table: "CHAT_HAS_USERS_JT",
                column: "MESSAGE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CHAT_HAS_USERS_JT_USER_ID",
                table: "CHAT_HAS_USERS_JT",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FRIEND_REQUESTS_JT_RECEIVER_ID",
                table: "FRIEND_REQUESTS_JT",
                column: "RECEIVER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FRIENDS_JT_FRIEND_ID",
                table: "FRIENDS_JT",
                column: "FRIEND_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGE_HAS_ATTACHMENTS_MESSAGE_ID",
                table: "MESSAGE_HAS_ATTACHMENTS",
                column: "MESSAGE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGES_CHAT_ID",
                table: "MESSAGES",
                column: "CHAT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGES_USER_ID",
                table: "MESSAGES",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_EMAIL",
                table: "USERS",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USERS_USERNAME",
                table: "USERS",
                column: "USERNAME",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHAT_HAS_USERS_JT");

            migrationBuilder.DropTable(
                name: "FRIEND_REQUESTS_JT");

            migrationBuilder.DropTable(
                name: "FRIENDS_JT");

            migrationBuilder.DropTable(
                name: "GROUPCHATS");

            migrationBuilder.DropTable(
                name: "MESSAGE_HAS_ATTACHMENTS");

            migrationBuilder.DropTable(
                name: "SINGLECHATS");

            migrationBuilder.DropTable(
                name: "USER_CODES");

            migrationBuilder.DropTable(
                name: "MESSAGES");

            migrationBuilder.DropTable(
                name: "CHATS_BT");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
