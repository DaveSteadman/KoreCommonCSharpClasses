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

        // Adhoc tests - a function designed to be reworked to consider issues-of-the-day
        //GloTestLog testres = GloTestCenter.RunAdHocTests();

        // Get the test reports
        // Add default statements if no tests passed or failed
        string fullReport = testres.FullReport();
        string failReport = testres.FailReport();


        Console.WriteLine("------------------------------------------------------------------------");
        if (testres.OverallPass())
            Console.WriteLine(fullReport);
        else
            Console.WriteLine(failReport);
        Console.WriteLine("------------------------------------------------------------------------");
        Console.WriteLine(testres.OneLineReport());
        Console.WriteLine("------------------------------------------------------------------------");
    }
}
