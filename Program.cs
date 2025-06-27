using System;

// Environment Creation:
// dotnet new console -n UnitTest
// dotnet add package System.Data.SQLite
// dotnet add package SkiaSharp
// dotnet new console


using KoreCommon;
using KoreCommon.UnitTest;


class Program
{

    static void Main()
    {
        // Uncomment to run the bulk rename (use with caution!)
        // RenameKoreToKoreInFiles(".");
        KoreTestLog testres = KoreTestCenter.RunCoreTests();

        // Adhoc tests - a function designed to be reworked to consider issues-of-the-day
        //KoreTestLog testres = KoreTestCenter.RunAdHocTests();

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
