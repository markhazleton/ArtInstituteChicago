using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSpark.ArtSpark.Demo.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceCollectionSEOAndContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CuratorNotes",
                table: "Collections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FeaturedUntil",
                table: "Collections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Collections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LongDescription",
                table: "Collections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Collections",
                type: "TEXT",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeywords",
                table: "Collections",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Collections",
                type: "TEXT",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Collections",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SocialImageUrl",
                table: "Collections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Collections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Collections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Collections",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CuratorNotes",
                table: "CollectionArtworks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomDescription",
                table: "CollectionArtworks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomTitle",
                table: "CollectionArtworks",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "CollectionArtworks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsHighlighted",
                table: "CollectionArtworks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "CollectionArtworks",
                type: "TEXT",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "CollectionArtworks",
                type: "TEXT",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "CollectionArtworks",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CollectionContentSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CollectionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    SectionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionContentSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionContentSections_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CollectionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LinkType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionLinks_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionMedia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CollectionId = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    MediaType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    AltText = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionMedia_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collections_Slug",
                table: "Collections",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionArtworks_CollectionId_Slug",
                table: "CollectionArtworks",
                columns: new[] { "CollectionId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionContentSections_CollectionId",
                table: "CollectionContentSections",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionLinks_CollectionId",
                table: "CollectionLinks",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionMedia_CollectionId",
                table: "CollectionMedia",
                column: "CollectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionContentSections");

            migrationBuilder.DropTable(
                name: "CollectionLinks");

            migrationBuilder.DropTable(
                name: "CollectionMedia");

            migrationBuilder.DropIndex(
                name: "IX_Collections_Slug",
                table: "Collections");

            migrationBuilder.DropIndex(
                name: "IX_CollectionArtworks_CollectionId_Slug",
                table: "CollectionArtworks");

            migrationBuilder.DropColumn(
                name: "CuratorNotes",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "FeaturedUntil",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "LongDescription",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "MetaKeywords",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "SocialImageUrl",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "CuratorNotes",
                table: "CollectionArtworks");

            migrationBuilder.DropColumn(
                name: "CustomDescription",
                table: "CollectionArtworks");

            migrationBuilder.DropColumn(
                name: "CustomTitle",
                table: "CollectionArtworks");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "CollectionArtworks");

            migrationBuilder.DropColumn(
                name: "IsHighlighted",
                table: "CollectionArtworks");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "CollectionArtworks");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "CollectionArtworks");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "CollectionArtworks");
        }
    }
}
