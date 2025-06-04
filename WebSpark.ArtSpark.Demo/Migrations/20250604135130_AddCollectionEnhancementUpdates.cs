using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSpark.ArtSpark.Demo.Migrations
{
    /// <inheritdoc />
    public partial class AddCollectionEnhancementUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MediaUrl",
                table: "CollectionMedia",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CollectionMedia",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "CollectionLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CollectionLinks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "CollectionContentSections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaUrl",
                table: "CollectionMedia");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CollectionMedia");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "CollectionLinks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CollectionLinks");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "CollectionContentSections");
        }
    }
}
