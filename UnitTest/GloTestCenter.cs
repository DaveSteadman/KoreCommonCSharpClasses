using System;

public static class GloTestCenter
{
    public static GloTestLog RunCoreTests()
    {
        GloTestLog testLog = new GloTestLog();

        try
        {
            GloTestMath.RunTests(testLog);
            GloTestPosition.RunTests(testLog);
            //GloTestPlotter.RunTests(testLog);
            GloTestList1D.RunTests(testLog);
            GloTestList2D.RunTests(testLog);
            GloTestMesh.RunTests(testLog);
        }
        catch (Exception)
        {
            testLog.Add("Test Centre Run", false, "Exception");
        }

        return testLog;
    }

    // --------------------------------------------------------------------------------------------

    // Usage: GloTestCenter.RunAdHocTests()
    public static void RunAdHocTests()
    {
        //RunCoreTests();

        GloCentralLog.AddEntry("=====> Test Centre - Running");

        GloTestLog testLog = new GloTestLog();

        try
        {
            GloTestStringDictionary.RunAll(testLog);
        }
        catch (Exception)
        {
            testLog.Add("Test Centre Run", false, "Exception");
        }

        // Print the test log
        GloCentralLog.AddEntry(testLog.FullReport());
    }
}

