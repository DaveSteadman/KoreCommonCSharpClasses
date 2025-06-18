using System;

public static class GloTestLine
{
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestCreateSimpleLine(testLog);
            TestOffsetLine(testLog);
            TestInterpolateAndExtendLine(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("GloTestLine RunTests", false, ex.Message);
            return;
        }
    }

    private static void TestCreateSimpleLine(GloTestLog testLog)
    {
        var line = new GloXYLine(new GloXYPoint(0, 0), new GloXYPoint(3, 4));
        testLog.AddResult("GloXYLine Length", GloValueUtils.EqualsWithinTolerance(line.Length, 5.0));
        var mid = line.MidPoint();
        testLog.AddResult("GloXYLine MidPoint", GloXYPoint.EqualsWithinTolerance(mid, new GloXYPoint(1.5, 2.0)));
    }

    private static void TestOffsetLine(GloTestLog testLog)
    {
        var line = new GloXYLine(new GloXYPoint(0, 0), new GloXYPoint(1, 1));
        var offset = line.Offset(2, 3);
        bool p1 = GloXYPoint.EqualsWithinTolerance(offset.P1, new GloXYPoint(2, 3));
        bool p2 = GloXYPoint.EqualsWithinTolerance(offset.P2, new GloXYPoint(3, 4));
        testLog.AddResult("GloXYLine Offset", p1 && p2);
    }

    private static void TestInterpolateAndExtendLine(GloTestLog testLog)
    {
        var line = new GloXYLine(new GloXYPoint(0, 0), new GloXYPoint(2, 0));
        var half = line.Fraction(0.5);
        testLog.AddResult("GloXYLine Fraction 0.5", GloXYPoint.EqualsWithinTolerance(half, new GloXYPoint(1, 0)));

        var extr = line.ExtrapolateDistance(3);
        testLog.AddResult("GloXYLine ExtrapolateDistance 3", GloXYPoint.EqualsWithinTolerance(extr, new GloXYPoint(3, 0)));

        var extended = GloXYLineOperations.ExtendLine(line, 1, 1);
        bool p1 = GloXYPoint.EqualsWithinTolerance(extended.P1, new GloXYPoint(-1, 0));
        bool p2 = GloXYPoint.EqualsWithinTolerance(extended.P2, new GloXYPoint(3, 0));
        testLog.AddResult("GloXYLineOperations ExtendLine", p1 && p2 && GloValueUtils.EqualsWithinTolerance(extended.Length, 4.0));
    }
}
