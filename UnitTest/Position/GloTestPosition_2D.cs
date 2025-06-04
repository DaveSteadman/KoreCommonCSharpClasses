using System;

public static partial class GloTestPosition
{
    private static void TestGloXYPoint(GloTestLog testLog)
    {
        // Example: Test creation of GloXYPoint points and basic operations
        var pointA = new GloXYPoint(1, 2);
        var pointB = new GloXYPoint(4, 5);

        testLog.Add("GloXYPoint Creation", pointA.X == 1 && pointA.Y == 2);

        double calcDistance = Math.Sqrt((4 - 1) * (4 - 1) + (5 - 2) * (5 - 2));
        testLog.Add("GloXYPoint Distance", GloValueUtils.EqualsWithinTolerance(pointA.DistanceTo(pointB), calcDistance, 0.001));


        // Add more tests for GloXYZPoint
    }

    private static void TestGloXYLine(GloTestLog testLog)
    {

    }



}
