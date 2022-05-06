using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace OxfordParser.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WordCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PicturePath = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WordType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameEng = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Word",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    SoundPathUK = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    SoundPathUS = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    WordLevel = table.Column<int>(type: "integer", nullable: false),
                    PicturePath = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    WordTypeId = table.Column<int>(type: "integer", nullable: false),
                    Processed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Word", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Word_WordType_WordTypeId",
                        column: x => x.WordTypeId,
                        principalTable: "WordType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WordAndWordCategory",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "integer", nullable: false),
                    WordsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordAndWordCategory", x => new { x.CategoriesId, x.WordsId });
                    table.ForeignKey(
                        name: "FK_WordAndWordCategory_Word_WordsId",
                        column: x => x.WordsId,
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordAndWordCategory_WordCategory_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "WordCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WordUsage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Comment = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    WordId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordUsage_Word_WordId",
                        column: x => x.WordId,
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Word_WordTypeId",
                table: "Word",
                column: "WordTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WordAndWordCategory_WordsId",
                table: "WordAndWordCategory",
                column: "WordsId");

            migrationBuilder.CreateIndex(
                name: "IX_WordUsage_WordId",
                table: "WordUsage",
                column: "WordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordAndWordCategory");

            migrationBuilder.DropTable(
                name: "WordUsage");

            migrationBuilder.DropTable(
                name: "WordCategory");

            migrationBuilder.DropTable(
                name: "Word");

            migrationBuilder.DropTable(
                name: "WordType");
        }
    }
}
