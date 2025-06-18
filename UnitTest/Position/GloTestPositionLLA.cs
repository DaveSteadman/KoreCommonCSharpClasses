using System;
using System.Data.Entity.Hierarchy;

public static class GloTestPositionLLA
{
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestGloLLAPointCreation(testLog);
            TestGloLLAPointMovement(testLog);
            TestGloLLAPoint_RangeBearing(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("GloTestPositionLLA RunTests", false, ex.Message);
        }
    }

    private static void TestGloLLAPointCreation(GloTestLog testLog)
    {
        var p = new GloLLAPoint() { LatDegs = 10.0, LonDegs = 20.0, AltMslM = 1000.0 };

        bool okLat = GloValueUtils.EqualsWithinTolerance(p.LatDegs, 10.0);
        bool okLon = GloValueUtils.EqualsWithinTolerance(p.LonDegs, 20.0);
        bool okAlt = GloValueUtils.EqualsWithinTolerance(p.AltMslM, 1000.0);
        testLog.AddResult("GloLLAPoint Creation", okLat && okLon && okAlt);
    }

    private static void TestGloLLAPointMovement(GloTestLog testLog)
    {
        var start = new GloLLAPoint() { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 0.0 };
        var offset = new GloAzElRange() { RangeM = 1000.0 };
        offset.AzDegs = 90.0;
        offset.ElDegs = 0.0;

        var dest = start.PlusPolarOffset(offset);

        double expectedLon = 1000.0 / GloWorldConsts.EarthRadiusM * GloConsts.RadsToDegsMultiplier;

        testLog.AddResult("GloLLAPoint Movement Lat", GloValueUtils.EqualsWithinTolerance(dest.LatDegs, 0.0, 0.0001));
        testLog.AddResult("GloLLAPoint Movement Lon", GloValueUtils.EqualsWithinTolerance(dest.LonDegs, expectedLon, 0.0001));
        testLog.AddResult("GloLLAPoint Movement Alt", GloValueUtils.EqualsWithinTolerance(dest.AltMslM, 0.0, 0.0001));

        var measured = start.StraightLinePolarOffsetTo(dest);
        testLog.AddResult("GloLLAPoint PolarOffset Range", GloValueUtils.EqualsWithinTolerance(measured.RangeM, 1000.0, 0.01));
        testLog.AddResult("GloLLAPoint PolarOffset Az", GloValueUtils.EqualsWithinTolerance(measured.AzDegs, 90.0, 0.001));

        var roundTrip = start.PlusPolarOffset(measured);
        bool sameLat = GloValueUtils.EqualsWithinTolerance(roundTrip.LatDegs, dest.LatDegs, 0.01);
        bool sameLon = GloValueUtils.EqualsWithinTolerance(roundTrip.LonDegs, dest.LonDegs, 0.01);
        bool sameAlt = GloValueUtils.EqualsWithinTolerance(roundTrip.AltMslM, dest.AltMslM, 0.1);
        testLog.AddResult("GloLLAPoint PolarOffset RoundTrip", sameLat && sameLon && sameAlt);
    }

    private static void TestGloLLAPoint_RangeBearing(GloTestLog testLog)
    {
        // Setup two points with non-trivial lat/lon/alt values
        var first  = new GloLLAPoint() { LatDegs = 50.0, LonDegs = -1.0, AltMslM = 0.0 };
        var second = new GloLLAPoint() { LatDegs = 50.1, LonDegs = -0.9, AltMslM = 0.0 };

        // Determine the delta between the two points
        GloRangeBearing pointRangeBearing = first.RangeBearingTo(second);

        // Apply the range-bearing to the first point to get a new point, validating that the way we calculate it and the way we
        // apply it to a point are consistent.
        var moved = first.PlusRangeBearing(pointRangeBearing);

        bool sameLat = GloValueUtils.EqualsWithinTolerance(moved.LatDegs, second.LatDegs, 0.0005);
        bool sameLon = GloValueUtils.EqualsWithinTolerance(moved.LonDegs, second.LonDegs, 0.0005);
        bool sameAlt = GloValueUtils.EqualsWithinTolerance(moved.AltMslM, second.AltMslM, 1.0);

        // create strings of all the comparisons, to expose some details of the checks.
        string latCompareStr = $"Lat: {first.LatDegs:F5} -> {second.LatDegs:F5}";
        string lonCompareStr = $"Lon: {first.LonDegs:F5} -> {second.LonDegs:F5}";
        string altCompareStr = $"Alt: {first.AltMslM:F1} -> {second.AltMslM:F1}";
        string rbStr = $"Range: {pointRangeBearing.RangeM:F1}m, Bearing: {pointRangeBearing.BearingDegs:F1}°";

        testLog.AddResult("GloLLAPoint Delta Lat", sameLat, latCompareStr);
        testLog.AddResult("GloLLAPoint Delta Lon", sameLon, lonCompareStr);
        testLog.AddResult("GloLLAPoint Delta Alt", sameAlt, altCompareStr);
        testLog.AddComment($"GloLLAPoint Delta RangeBearing: {rbStr}");
    }
}


