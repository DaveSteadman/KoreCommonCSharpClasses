using System;

// Environment Creation:
// dotnet new console -n UnitTest
// dotnet add package System.Data.SQLite
// dotnet new console


class Program
{
    static void Main()
    {
        Console.WriteLine("CSharpCommonClasses");

        GloTestLog testres = GloTestCenter.RunCoreTests();

        // Get the test reports
        // Add default statements if no tests passed or failed
        string passReport = testres.FullReport(true, false);
        string failReport = testres.FullReport(false, true);
        if (string.IsNullOrEmpty(passReport)) passReport = "No tests passed.";
        if (string.IsNullOrEmpty(failReport)) failReport = "No tests failed.";

        // Trim leading and trailing whitespace
        passReport = passReport.Trim();
        failReport = failReport.Trim();

        Console.WriteLine("------------------------------------------------------------------------");
        Console.WriteLine(passReport);
        Console.WriteLine("------------------------------------------------------------------------");
        Console.WriteLine(failReport);
        Console.WriteLine("------------------------------------------------------------------------");
        Console.WriteLine(testres.OneLineReport());
        Console.WriteLine("------------------------------------------------------------------------");
    }
}
