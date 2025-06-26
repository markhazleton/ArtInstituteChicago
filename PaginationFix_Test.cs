using System;

namespace WebSpark.ArtSpark.Tests
{
    /// <summary>
    /// Test to demonstrate the pagination fix
    /// </summary>
    public class PaginationFixTest
    {
        public static void TestPaginationCalculation()
        {
            Console.WriteLine("Testing Pagination Calculation Fix");
            Console.WriteLine("=================================");

            // Test cases for 12 items per page
            int size = 12;

            // Test Page 1 (From = 0)
            int from1 = 0;
            int currentPage1 = (from1 / size) + 1;
            Console.WriteLine($"Page 1: From={from1}, Size={size} => CurrentPage={currentPage1} (Expected: 1)");

            // Test Page 2 (From = 12)
            int from2 = 12;
            int currentPage2 = (from2 / size) + 1;
            Console.WriteLine($"Page 2: From={from2}, Size={size} => CurrentPage={currentPage2} (Expected: 2)");

            // Test Page 3 (From = 24)
            int from3 = 24;
            int currentPage3 = (from3 / size) + 1;
            Console.WriteLine($"Page 3: From={from3}, Size={size} => CurrentPage={currentPage3} (Expected: 3)");

            Console.WriteLine();
            Console.WriteLine("BEFORE THE FIX:");
            Console.WriteLine("The code was incorrectly setting From = 1 when From < 1,");
            Console.WriteLine("which broke the calculation for page 1 and subsequent pages.");
            Console.WriteLine();
            Console.WriteLine("AFTER THE FIX:");
            Console.WriteLine("From is now correctly handled as 0-based (Elasticsearch style)");
            Console.WriteLine("and the validation only sets From = 0 when From < 0.");
        }
    }
}
