// Simple test to verify pagination calculation
Console.WriteLine("Testing Pagination Calculation");
Console.WriteLine("===============================");

// Test the exact calculation used in the view
void TestPagination(int from, int size, int total, int expectedPage)
{
    var currentPage = (from / size) + 1;
    var totalPages = (int)Math.Ceiling((double)total / size);
    var startPage = Math.Max(1, currentPage - 2);
    var endPage = Math.Min(totalPages, currentPage + 2);

    Console.WriteLine($"From={from}, Size={size}, Total={total}");
    Console.WriteLine($"  => CurrentPage={currentPage} (Expected: {expectedPage})");
    Console.WriteLine($"  => TotalPages={totalPages}");
    Console.WriteLine($"  => StartPage={startPage}, EndPage={endPage}");
    Console.WriteLine($"  => Pages shown: {startPage} to {endPage}");
    Console.WriteLine();
}

// Test scenarios based on search results with 253 total results, 12 per page
int total = 253;
int size = 12;

Console.WriteLine($"Testing with Total={total}, Size={size}");
Console.WriteLine();

TestPagination(0, size, total, 1);   // Page 1
TestPagination(12, size, total, 2);  // Page 2  
TestPagination(24, size, total, 3);  // Page 3
TestPagination(36, size, total, 4);  // Page 4
TestPagination(48, size, total, 5);  // Page 5
