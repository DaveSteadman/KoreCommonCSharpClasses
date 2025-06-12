using System;

public static class GloTestCenter
{
    public static GloTestLog RunCoreTests()
    {
        GloTestLog testLog = new GloTestLog();

        try
        {
            GloTestMath.RunTests(testLog);
            GloTestXYZVector.RunTests(testLog);

            GloTestPosition.RunTests(testLog);
            //GloTestPlotter.RunTests(testLog);
            GloTestList1D.RunTests(testLog);
            GloTestList2D.RunTests(testLog);
            GloTestMesh.RunTests(testLog);

            GloTestColor.RunTests(testLog);
            GloTestLangStrings.RunTests(testLog);

            GloTestDatabase.RunTests(testLog);

            GloTestStringDictionary.RunTests(testLog);
        }
        catch (Exception)
        {
            testLog.AddResult("Test Centre Run", false, "Exception");
        }

        return testLog;
    }

    // --------------------------------------------------------------------------------------------

    // Usage: GloTestCenter.RunAdHocTests()
    public static GloTestLog RunAdHocTests()
    {
        GloTestLog testLog = new GloTestLog();

        try
        {
            GloTestXYZVector.TestArbitraryPerpendicular(testLog);
        }
        catch (Exception)
        {
            testLog.AddResult("Test Centre Run", false, "Exception");
        }

        return testLog;
    }
}

