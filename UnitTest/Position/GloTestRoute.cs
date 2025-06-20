using System;
using System.Collections.Generic;

public static class GloTestRoute
{
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestSimpleRoute(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("GloTestRoute RunTests", false, ex.Message);
        }
    }

    private static void TestSimpleRoute(GloTestLog testLog)
    {
        // Build two legs: a straight line followed by a 90 degree right turn
        GloLLAPoint start = new GloLLAPoint() { LatDegs = 0, LonDegs = 0, AltMslM = 0 };
        GloLLAPoint mid = start.PlusRangeBearing(new GloRangeBearing(1000, 90 * GloConsts.DegsToRadsMultiplier));

        double speed = 100; // m/s
        var leg1 = new GloRouteLegLine(start, mid, speed);

        // Turn centre 500 m to the right of mid point
        double centreBearing = GloDoubleRange.ZeroToTwoPiRadians.Apply(leg1.EndCourse.HeadingRads + Math.PI / 2);
        GloLLAPoint centre = mid.PlusRangeBearing(new GloRangeBearing(500, centreBearing));

        var turnLeg = new GloRouteSimpleTurn(mid, centre, Math.PI / 2, speed);

        var route = new GloRoute(new List<IGloRouteLeg>() { leg1, turnLeg });

        testLog.AddResult("Route NumLegs", route.NumLegs() == 2);

        double expectedDuration = leg1.GetDurationS() + turnLeg.GetDurationS();
        bool durOk = GloValueUtils.EqualsWithinTolerance(route.GetDurationSeconds(), expectedDuration, 0.001);
        testLog.AddResult("Route Duration", durOk);

        GloLLAPoint midOfLine = route.CurrentPosition(leg1.GetDurationS() / 2);
        GloLLAPoint expectedMid = GloLLAPointOperations.RhumbLineInterpolation(start, mid, 0.5);
        bool midLat = GloValueUtils.EqualsWithinTolerance(midOfLine.LatDegs, expectedMid.LatDegs, 0.0001);
        bool midLon = GloValueUtils.EqualsWithinTolerance(midOfLine.LonDegs, expectedMid.LonDegs, 0.0001);
        testLog.AddResult("Route Line MidPos", midLat && midLon);

        GloLLAPoint endPos = route.CurrentPosition(expectedDuration);
        bool endLat = GloValueUtils.EqualsWithinTolerance(endPos.LatDegs, turnLeg.EndPoint.LatDegs, 0.0001);
        bool endLon = GloValueUtils.EqualsWithinTolerance(endPos.LonDegs, turnLeg.EndPoint.LonDegs, 0.0001);
        testLog.AddResult("Route EndPos", endLat && endLon);

        GloLLAPoint midTurnPos = route.CurrentPosition(leg1.GetDurationS() + turnLeg.GetDurationS() / 2);
        GloLLAPoint expectedMidTurn = turnLeg.PositionAtLegFraction(0.5);
        bool mtLat = GloValueUtils.EqualsWithinTolerance(midTurnPos.LatDegs, expectedMidTurn.LatDegs, 0.0001);
        bool mtLon = GloValueUtils.EqualsWithinTolerance(midTurnPos.LonDegs, expectedMidTurn.LonDegs, 0.0001);
        testLog.AddResult("Route Turn MidPos", mtLat && mtLon);
    }
}
