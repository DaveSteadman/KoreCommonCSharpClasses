using System;

public static class GloTestPositionLLA
{
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestGloLLAPointCreation(testLog);
            TestGloLLAPointMovement(testLog);
            TestGloLLAPointDelta(testLog);
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

    private static void TestGloLLAPointDelta(GloTestLog testLog)
    {
        var first = new GloLLAPoint() { LatDegs = 50.0, LonDegs = -1.0, AltMslM = 0.0 };
        var second = new GloLLAPoint() { LatDegs = 50.1, LonDegs = -0.9, AltMslM = 0.0 };

        var delta = first.StraightLinePolarOffsetTo(second);
        var moved = first.PlusPolarOffset(delta);

        bool sameLat = GloValueUtils.EqualsWithinTolerance(moved.LatDegs, second.LatDegs, 0.0005);
        bool sameLon = GloValueUtils.EqualsWithinTolerance(moved.LonDegs, second.LonDegs, 0.0005);
        bool sameAlt = GloValueUtils.EqualsWithinTolerance(moved.AltMslM, second.AltMslM, 15.0);

        testLog.AddResult("GloLLAPoint Delta Lat", sameLat);
        testLog.AddResult("GloLLAPoint Delta Lon", sameLon);
        testLog.AddResult("GloLLAPoint Delta Alt", sameAlt);
    }
}
