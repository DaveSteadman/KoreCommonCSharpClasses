using System;

public static class GloTestTriangle
{
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestInternalAnglesSum(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("GloTestTriangle RunTests", false, ex.Message);
        }
    }

    private static void TestInternalAnglesSum(GloTestLog testLog)
    {
        var tri = new GloXYTriangle(new GloXYPoint(0, 0), new GloXYPoint(1, 0), new GloXYPoint(0, 1));

        double angAB = tri.InternalAngleABRads();
        double angBC = tri.InternalAngleBCRads();
        double angCA = tri.InternalAngleCARads();
        double sum = angAB + angBC + angCA;

        bool withinRange = GloDoubleRange.ZeroToPiRadians.IsInRange(angAB) &&
                           GloDoubleRange.ZeroToPiRadians.IsInRange(angBC) &&
                           GloDoubleRange.ZeroToPiRadians.IsInRange(angCA);
        testLog.AddResult("GloXYTriangle Angles < π", withinRange);
        testLog.AddResult("GloXYTriangle Angles Sum", GloValueUtils.EqualsWithinTolerance(sum, Math.PI));
    }
}
