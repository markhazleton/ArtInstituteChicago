#Requires -Version 7.0
<#
.SYNOPSIS
    Database optimization and maintenance script for WebSpark.ArtSpark
.DESCRIPTION
    This script provides database optimization, indexing strategies, and maintenance
    operations to improve performance and ensure data integrity.
.NOTES
    Use this script to optimize database performance and maintain data health.
#>

param(
    [string]$Environment = "Development",
    [switch]$WhatIf,
    [switch]$CreateIndexes,
    [switch]$AnalyzePerformance,
    [switch]$OptimizeQueries
)

Write-Host "🗄️  WebSpark ArtSpark - Database Optimization" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# Database optimization SQL scripts
$OptimizationScripts = @{
    "CreateIndexes"       = @"
-- Performance Indexes for WebSpark.ArtSpark

-- User Collections - frequently queried by user
CREATE INDEX IF NOT EXISTS IX_UserCollections_UserId_IsPublic 
ON UserCollections (UserId, IsPublic) 
WHERE IsPublic = 1;

-- Collection Items - optimize for collection browsing
CREATE INDEX IF NOT EXISTS IX_UserCollectionItems_CollectionId_DisplayOrder 
ON UserCollectionItems (CollectionId, DisplayOrder)
INCLUDE (ArtworkId, IsHighlighted);

-- User Reviews - optimize for artwork review queries
CREATE INDEX IF NOT EXISTS IX_UserReviews_ArtworkId_CreatedAt 
ON UserReviews (ArtworkId, CreatedAt DESC)
INCLUDE (UserId, Rating, ReviewText);

-- User Favorites - optimize for user favorite lookups
CREATE INDEX IF NOT EXISTS IX_UserFavorites_UserId_ArtworkId 
ON UserFavorites (UserId, ArtworkId);

-- Composite index for collection search
CREATE INDEX IF NOT EXISTS IX_UserCollections_Search 
ON UserCollections (IsPublic, IsFeatured, CreatedAt DESC)
INCLUDE (Title, Description, Slug);

-- Text search optimization (if using FTS)
-- Note: SQLite FTS5 virtual table for text search
-- CREATE VIRTUAL TABLE IF NOT EXISTS Collections_FTS USING fts5(
--     Title, Description, Tags,
--     content='UserCollections',
--     content_rowid='Id'
-- );
"@

    "PerformanceAnalysis" = @"
-- Performance Analysis Queries

-- Top 10 most popular artworks (by favorites)
SELECT 
    ArtworkId,
    COUNT(*) as FavoriteCount,
    AVG(CAST(ur.Rating as REAL)) as AvgRating,
    COUNT(ur.Id) as ReviewCount
FROM UserFavorites uf
LEFT JOIN UserReviews ur ON uf.ArtworkId = ur.ArtworkId
GROUP BY ArtworkId
ORDER BY FavoriteCount DESC
LIMIT 10;

-- User engagement statistics
SELECT 
    COUNT(DISTINCT UserId) as TotalUsers,
    COUNT(DISTINCT CASE WHEN CreatedAt > datetime('now', '-30 days') THEN UserId END) as ActiveUsers30d,
    COUNT(*) as TotalCollections,
    AVG(ViewCount) as AvgViewCount
FROM UserCollections
WHERE IsPublic = 1;

-- Collection size distribution
SELECT 
    CASE 
        WHEN ItemCount = 0 THEN '0 items'
        WHEN ItemCount BETWEEN 1 AND 5 THEN '1-5 items'
        WHEN ItemCount BETWEEN 6 AND 10 THEN '6-10 items'
        WHEN ItemCount BETWEEN 11 AND 25 THEN '11-25 items'
        WHEN ItemCount > 25 THEN '25+ items'
    END as SizeRange,
    COUNT(*) as CollectionCount
FROM (
    SELECT 
        uc.Id,
        COUNT(uci.Id) as ItemCount
    FROM UserCollections uc
    LEFT JOIN UserCollectionItems uci ON uc.Id = uci.CollectionId
    GROUP BY uc.Id
) CollectionSizes
GROUP BY SizeRange
ORDER BY 
    CASE SizeRange
        WHEN '0 items' THEN 1
        WHEN '1-5 items' THEN 2
        WHEN '6-10 items' THEN 3
        WHEN '11-25 items' THEN 4
        WHEN '25+ items' THEN 5
    END;
"@

    "QueryOptimization"   = @"
-- Query Optimization Examples

-- Optimized collection search query
WITH SearchTerms AS (
    SELECT 'modern art' as SearchTerm -- Parameter placeholder
),
RankedResults AS (
    SELECT 
        uc.*,
        ROW_NUMBER() OVER (
            PARTITION BY uc.UserId 
            ORDER BY 
                CASE WHEN uc.IsFeatured = 1 THEN 0 ELSE 1 END,
                uc.ViewCount DESC,
                uc.CreatedAt DESC
        ) as UserRank,
        -- Simple relevance scoring
        (CASE WHEN uc.Title LIKE '%' || st.SearchTerm || '%' THEN 3 ELSE 0 END +
         CASE WHEN uc.Description LIKE '%' || st.SearchTerm || '%' THEN 2 ELSE 0 END +
         CASE WHEN uc.Tags LIKE '%' || st.SearchTerm || '%' THEN 1 ELSE 0 END) as RelevanceScore
    FROM UserCollections uc
    CROSS JOIN SearchTerms st
    WHERE uc.IsPublic = 1
    AND (
        uc.Title LIKE '%' || st.SearchTerm || '%' OR
        uc.Description LIKE '%' || st.SearchTerm || '%' OR
        uc.Tags LIKE '%' || st.SearchTerm || '%'
    )
)
SELECT 
    Id, Title, Description, Slug, ViewCount, CreatedAt, RelevanceScore
FROM RankedResults
WHERE UserRank <= 3  -- Limit to top 3 collections per user
ORDER BY RelevanceScore DESC, ViewCount DESC
LIMIT 20 OFFSET 0;  -- Pagination

-- Optimized artwork recommendations
WITH UserPreferences AS (
    SELECT 
        uf.UserId,
        GROUP_CONCAT(DISTINCT ur.ArtworkId) as FavoriteArtworks,
        AVG(ur.Rating) as AvgRating
    FROM UserFavorites uf
    LEFT JOIN UserReviews ur ON uf.UserId = ur.UserId
    WHERE uf.UserId = @UserId  -- Parameter
    GROUP BY uf.UserId
),
SimilarUsers AS (
    SELECT DISTINCT uf2.UserId
    FROM UserFavorites uf1
    JOIN UserFavorites uf2 ON uf1.ArtworkId = uf2.ArtworkId AND uf1.UserId != uf2.UserId
    WHERE uf1.UserId = @UserId
    AND uf2.UserId != @UserId
)
SELECT DISTINCT
    uf.ArtworkId,
    COUNT(*) as SimilarUserFavorites,
    AVG(ur.Rating) as AvgRating
FROM SimilarUsers su
JOIN UserFavorites uf ON su.UserId = uf.UserId
LEFT JOIN UserReviews ur ON uf.ArtworkId = ur.ArtworkId
WHERE uf.ArtworkId NOT IN (
    SELECT ArtworkId FROM UserFavorites WHERE UserId = @UserId
)
GROUP BY uf.ArtworkId
HAVING COUNT(*) >= 2  -- At least 2 similar users liked it
ORDER BY SimilarUserFavorites DESC, AvgRating DESC
LIMIT 10;
"@

    "MaintenanceChecks"   = @"
-- Database Maintenance and Health Checks

-- Check for orphaned collection items
SELECT COUNT(*) as OrphanedItems
FROM UserCollectionItems uci
LEFT JOIN UserCollections uc ON uci.CollectionId = uc.Id
WHERE uc.Id IS NULL;

-- Check for duplicate favorites
SELECT UserId, ArtworkId, COUNT(*) as DuplicateCount
FROM UserFavorites
GROUP BY UserId, ArtworkId
HAVING COUNT(*) > 1;

-- Data integrity checks
SELECT 
    'Collections without items' as CheckType,
    COUNT(*) as IssueCount
FROM UserCollections uc
LEFT JOIN UserCollectionItems uci ON uc.Id = uci.CollectionId
WHERE uci.Id IS NULL

UNION ALL

SELECT 
    'Reviews without rating' as CheckType,
    COUNT(*) as IssueCount
FROM UserReviews
WHERE Rating IS NULL OR Rating < 1 OR Rating > 5

UNION ALL

SELECT 
    'Collections with invalid display order' as CheckType,
    COUNT(*) as IssueCount
FROM UserCollectionItems
WHERE DisplayOrder < 0;

-- Database size and statistics
SELECT 
    name as TableName,
    sql as CreateStatement
FROM sqlite_master 
WHERE type = 'table' 
AND name NOT LIKE 'sqlite_%'
ORDER BY name;
"@
}

function Invoke-DatabaseScript {
    param(
        [string]$ScriptName,
        [string]$SqlScript,
        [string]$ConnectionString
    )
    
    Write-Host "`n📊 Executing $ScriptName..." -ForegroundColor Yellow
    
    if ($WhatIf) {
        Write-Host "Would execute SQL script: $ScriptName" -ForegroundColor Yellow
        Write-Host "Script preview:" -ForegroundColor Gray
        Write-Host $SqlScript.Substring(0, [Math]::Min(200, $SqlScript.Length)) -ForegroundColor Gray
        if ($SqlScript.Length -gt 200) {
            Write-Host "... (truncated)" -ForegroundColor Gray
        }
        return
    }

    try {
        # Save script to temp file for execution
        $tempFile = [System.IO.Path]::GetTempFileName() + ".sql"
        Set-Content -Path $tempFile -Value $SqlScript -Encoding UTF8
        
        Write-Host "✓ Script saved to: $tempFile" -ForegroundColor Green
        Write-Host "  Execute manually with: sqlite3 ArtSpark.db < $tempFile" -ForegroundColor Cyan
        
        # Note: In a real implementation, you would use a proper database connection
        # For demonstration, we're showing how the script would be organized
        
    }
    catch {
        Write-Error "❌ Failed to execute $ScriptName`: $_"
    }
}

function Get-DatabaseInsights {
    Write-Host "`n📈 Database Performance Insights" -ForegroundColor Yellow
    Write-Host "=================================" -ForegroundColor Yellow
    
    $insights = @(
        "🔍 Index Strategy Recommendations:",
        "   • Create composite indexes for frequently queried column combinations",
        "   • Use covering indexes to avoid key lookups",
        "   • Consider partial indexes for filtered queries (WHERE IsPublic = 1)",
        "",
        "⚡ Query Optimization Tips:",
        "   • Use EXPLAIN QUERY PLAN to analyze query execution",
        "   • Avoid SELECT * in production queries",
        "   • Use appropriate LIMIT and OFFSET for pagination",
        "   • Consider query result caching for expensive operations",
        "",
        "🛠️ Maintenance Best Practices:",
        "   • Regular VACUUM operations to optimize database file",
        "   • ANALYZE to update query planner statistics",
        "   • Monitor database file size growth",
        "   • Implement soft deletes for audit trails",
        "",
        "📊 Monitoring Recommendations:",
        "   • Track slow query performance",
        "   • Monitor database connection pool usage",
        "   • Set up alerts for database file size",
        "   • Log and analyze most frequent queries"
    )
    
    foreach ($insight in $insights) {
        Write-Host $insight -ForegroundColor White
    }
}

function New-MigrationScript {
    param([string]$Name)
    
    $timestamp = Get-Date -Format "yyyyMMddHHmmss"
    $fileName = "${timestamp}_${Name}.cs"
    $filePath = Join-Path $PSScriptRoot "Migrations" $fileName
    
    $migrationTemplate = @"
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSpark.ArtSpark.Demo.Migrations
{
    /// <inheritdoc />
    public partial class $Name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add your migration logic here
            // Example: Create indexes for performance
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS IX_UserCollections_Performance 
                ON UserCollections (IsPublic, IsFeatured, CreatedAt DESC)
                INCLUDE (Title, ViewCount);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add rollback logic here
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_UserCollections_Performance;");
        }
    }
}
"@

    if ($WhatIf) {
        Write-Host "Would create migration: $filePath" -ForegroundColor Yellow
    }
    else {
        if (-not (Test-Path (Split-Path $filePath))) {
            New-Item -ItemType Directory -Path (Split-Path $filePath) -Force | Out-Null
        }
        Set-Content -Path $filePath -Value $migrationTemplate -Encoding UTF8
        Write-Host "✓ Created migration: $fileName" -ForegroundColor Green
    }
}

# Main execution
try {
    Write-Host "`n🚀 Starting database optimization..." -ForegroundColor Green
    
    $connectionString = "Data Source=ArtSpark.db"
    
    if ($CreateIndexes -or $args.Count -eq 0) {
        Invoke-DatabaseScript -ScriptName "Performance Indexes" -SqlScript $OptimizationScripts["CreateIndexes"] -ConnectionString $connectionString
    }
    
    if ($AnalyzePerformance -or $args.Count -eq 0) {
        Invoke-DatabaseScript -ScriptName "Performance Analysis" -SqlScript $OptimizationScripts["PerformanceAnalysis"] -ConnectionString $connectionString
    }
    
    if ($OptimizeQueries -or $args.Count -eq 0) {
        Invoke-DatabaseScript -ScriptName "Query Optimization" -SqlScript $OptimizationScripts["QueryOptimization"] -ConnectionString $connectionString
    }
    
    # Always run maintenance checks
    Invoke-DatabaseScript -ScriptName "Maintenance Checks" -SqlScript $OptimizationScripts["MaintenanceChecks"] -ConnectionString $connectionString
    
    Get-DatabaseInsights
    
    Write-Host "`n✅ Database Optimization Complete!" -ForegroundColor Green
    Write-Host "`n📋 Next Steps:" -ForegroundColor Yellow
    Write-Host "   1. Review and execute the generated SQL scripts" -ForegroundColor White
    Write-Host "   2. Monitor query performance improvements" -ForegroundColor White
    Write-Host "   3. Set up regular maintenance schedule" -ForegroundColor White
    Write-Host "   4. Consider implementing query result caching" -ForegroundColor White
    Write-Host "   5. Create performance monitoring dashboard" -ForegroundColor White
    
}
catch {
    Write-Error "❌ Error during database optimization: $_"
    exit 1
}
