using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSpark.ArtSpark.Demo.Migrations
{
    /// <inheritdoc />
    public partial class PopulateCollectionSlugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing collections that have empty or null slugs
            // Generate slugs from collection names
            migrationBuilder.Sql(@"
                UPDATE Collections 
                SET Slug = CASE 
                    WHEN Name IS NOT NULL AND Name != '' THEN 
                        lower(
                            replace(
                                replace(
                                    replace(
                                        replace(
                                            replace(
                                                replace(
                                                    replace(
                                                        replace(
                                                            replace(
                                                                replace(trim(Name), ' ', '-'),
                                                                '&', 'and'
                                                            ),
                                                            '/', '-'
                                                        ),
                                                        '\', '-'
                                                    ),
                                                    '(', ''
                                                ),
                                                ')', ''
                                            ),
                                            '[', ''
                                        ),
                                        ']', ''
                                    ),
                                    ':', ''
                                ),
                                ';', ''
                            )
                        )
                    ELSE 'collection-' || Id
                END 
                WHERE Slug IS NULL OR Slug = '';
            ");

            // Handle potential duplicate slugs by appending collection ID
            migrationBuilder.Sql(@"
                UPDATE Collections 
                SET Slug = Slug || '-' || Id
                WHERE Id IN (
                    SELECT c1.Id 
                    FROM Collections c1
                    INNER JOIN Collections c2 ON c1.Slug = c2.Slug AND c1.Id > c2.Id
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert all slugs to empty string
            migrationBuilder.Sql("UPDATE Collections SET Slug = '' WHERE Slug IS NOT NULL;");
        }
    }
}
