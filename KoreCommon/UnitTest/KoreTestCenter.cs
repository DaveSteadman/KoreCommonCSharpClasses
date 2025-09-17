using System;
using System.IO;
using KoreCommon;
namespace KoreCommon.UnitTest;


// Usage: KoreTestLog testLog = KoreTestCenter.RunCoreTests();

public static class KoreTestCenter
{
    public static KoreTestLog RunCoreTests()
    {
        KoreTestLog testLog = new KoreTestLog();

        try
        {
            if (!EnsureTestDirectory(testLog))
                return testLog;

            KoreTestMath.RunTests(testLog);
            KoreTestXYZVector.RunTests(testLog);
            KoreTestLine.RunTests(testLog);
            KoreTestTriangle.RunTests(testLog);

            KoreTestPosition.RunTests(testLog);
            KoreTestPositionLLA.RunTests(testLog);
            KoreTestRoute.RunTests(testLog);
            //KoreTestPlotter.RunTests(testLog);
            KoreTestList1D.RunTests(testLog);
            KoreTestList2D.RunTests(testLog);
            KoreTestMesh.RunTests(testLog);

            KoreTestColor.RunTests(testLog);

            KoreTestStringDictionary.RunTests(testLog);

            // Run tests that depend on external libraries: DB & SkiaSharp
            KoreTestDatabase.RunTests(testLog);
            KoreTestSkiaSharp.RunTests(testLog);
            KoreTestMeshUvOps.RunTests(testLog);

            KoreTestMiniMesh.RunTests(testLog);

        }
        catch (Exception)
        {
            testLog.AddResult("Test Centre Run", false, "Exception");
        }

        return testLog;
    }

    // --------------------------------------------------------------------------------------------

    // Usage: KoreTestCenter.RunAdHocTests()
    public static KoreTestLog RunAdHocTests(KoreTestLog testLog)
    {

        try
        {
            KoreTestXYZVector.TestArbitraryPerpendicular(testLog);
        }
        catch (Exception)
        {
            testLog.AddResult("Test Centre Run", false, "Exception");
        }

        return testLog;
    }

    // --------------------------------------------------------------------------------------------

    private static bool EnsureTestDirectory(KoreTestLog testLog)
    {
        bool retval = false;

        // Use a proper path - either absolute or relative with proper separators
        // Option 1: Relative to current directory
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "UnitTestArtefacts");
        testLog.AddComment("Attempting to create directory at: " + testDir);

        KoreFileOps.CreateDirectory(testDir);

        // Verify the directory was actually created
        if (Directory.Exists(testDir))
        {
            testLog.AddComment("? Test directory successfully created at: " + testDir);
            retval = true;
        }
        else
        {
            testLog.AddResult("Test Directory Creation", false, $"? Test directory was NOT created at: {testDir}");
        }

        return retval;
    }

}

