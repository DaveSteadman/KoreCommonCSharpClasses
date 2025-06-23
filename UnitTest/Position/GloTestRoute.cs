using System;
using System.Collections.Generic;

public static class GloTestRoute
{
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestRouteLegLine(testLog);
            TestRouteSimpleTurn(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("GloTestRoute RunTests", false, ex.Message);
        }
    }

    // --------------------------------------------------------------------------------------------------
    // MARK: Route Elements
    // --------------------------------------------------------------------------------------------------


    // Create a route element and query various attributes.

    private static void TestRouteLegLine(GloTestLog testLog)
    {
        // Create a simple route leg with a straight line
        GloLLAPoint start = new GloLLAPoint() { LatDegs = 0, LonDegs = 0, AltMslM = 0 };
        GloLLAPoint end = start.PlusRangeBearing(new GloRangeBearing(1000, 90 * GloConsts.DegsToRadsMultiplier));
        double speed = 100; // m/s
        var leg = new GloRouteLegLine(start, end, speed);

        // testLog.AddResult("Route Leg Line Start", GloValueUtils.EqualsWithinTolerance(leg.StartPoint.LatDegs, start.LatDegs, 0.0001) &&
        //                                         GloValueUtils.EqualsWithinTolerance(leg.StartPoint.LonDegs, start.LonDegs, 0.0001));
        // testLog.AddResult("Route Leg Line End", GloValueUtils.EqualsWithinTolerance(leg.EndPoint.LatDegs, end.LatDegs, 0.0001) &&
        //                                       GloValueUtils.EqualsWithinTolerance(leg.EndPoint.LonDegs, end.LonDegs, 0.0001));
        // testLog.AddResult("Route Leg Line Speed", GloValueUtils.EqualsWithinTolerance(leg.SpeedMps, speed, 0.001));
        // testLog.AddResult("Route Leg Line Duration", GloValueUtils.EqualsWithinTolerance(leg.GetDurationS(), 10.0, 0.001));
    }

    // --------------------------------------------------------------------------------------------------

    private static void TestRouteSimpleTurn(GloTestLog testLog)
    {
        // Create a simple turn leg with a 90 degree right turn
        GloLLAPoint startPoint = new GloLLAPoint() { LatDegs = 0, LonDegs = 0, AltMslM = 0 };

        GloCourse startCourse = new GloCourse()
        {
            SpeedMps = 100, // m/s
            HeadingRads = 90 * GloConsts.DegsToRadsMultiplier
        };
        double turnRadiusM = 5000; // radius of the turn arc
        double turnAngleRads = 90 * GloConsts.DegsToRadsMultiplier; // +ve (right) 90 degrees

        GloLLAPoint turnCentre = GloRouteLegSimpleTurn.FindTurnPoint(startPoint, startCourse, turnRadiusM, turnAngleRads);

        var turnLeg = new GloRouteLegSimpleTurn(startPoint, turnCentre, turnRadiusM, turnAngleRads);

        // Now we can test various properties of the turn leg
        double testRouteLen = (2 * Math.PI * 5000) / 4; // quarter circle length at 5000m
        string testComment = $"Turn Leg: Start={startPoint}, Centre={turnCentre}, Radius={turnRadiusM:F1}, Angle={turnAngleRads:F3}, calcLen={turnLeg.GetCalculatedDistanceM():F1} checklen={testRouteLen:F1}";
        testLog.AddResult("Route Length", GloValueUtils.EqualsWithinTolerance(turnLeg.GetCalculatedDistanceM(), testRouteLen, 0.001), testComment);

    }


    // private static void TestSimpleRoute(GloTestLog testLog)
    // {
    //     // Build two legs: a straight line followed by a 90 degree right turn
    //     GloLLAPoint start = new GloLLAPoint() { LatDegs = 0, LonDegs = 0, AltMslM = 0 };
    //     GloLLAPoint mid = start.PlusRangeBearing(new GloRangeBearing(1000, 90 * GloConsts.DegsToRadsMultiplier));

    //     double speed = 100; // m/s
    //     var leg1 = new GloRouteLegLine(start, mid, speed);

    //     // Turn centre 500 m to the right of mid point
    //     double centreBearing = GloDoubleRange.ZeroToTwoPiRadians.Apply(leg1.EndCourse.HeadingRads + Math.PI / 2);
    //     GloLLAPoint centre = mid.PlusRangeBearing(new GloRangeBearing(500, centreBearing));

    //     var turnLeg = new GloRouteLegSimpleTurn(mid, centre, Math.PI / 2, speed);

    //     var route = new GloRoute(new List<IGloRouteLeg>() { leg1, turnLeg });

    //     testLog.AddResult("Route NumLegs", route.NumLegs() == 2);

    //     double expectedDuration = leg1.GetDurationS() + turnLeg.GetDurationS();
    //     bool durOk = GloValueUtils.EqualsWithinTolerance(route.GetDurationSeconds(), expectedDuration, 0.001);
    //     testLog.AddResult("Route Duration", durOk);

    //     GloLLAPoint midOfLine = route.CurrentPosition(leg1.GetDurationS() / 2);
    //     GloLLAPoint expectedMid = GloLLAPointOperations.RhumbLineInterpolation(start, mid, 0.5);
    //     bool midLat = GloValueUtils.EqualsWithinTolerance(midOfLine.LatDegs, expectedMid.LatDegs, 0.0001);
    //     bool midLon = GloValueUtils.EqualsWithinTolerance(midOfLine.LonDegs, expectedMid.LonDegs, 0.0001);
    //     testLog.AddResult("Route Line MidPos", midLat && midLon);

    //     GloLLAPoint endPos = route.CurrentPosition(expectedDuration);
    //     bool endLat = GloValueUtils.EqualsWithinTolerance(endPos.LatDegs, turnLeg.EndPoint.LatDegs, 0.0001);
    //     bool endLon = GloValueUtils.EqualsWithinTolerance(endPos.LonDegs, turnLeg.EndPoint.LonDegs, 0.0001);
    //     testLog.AddResult("Route EndPos", endLat && endLon);

    //     GloLLAPoint midTurnPos = route.CurrentPosition(leg1.GetDurationS() + turnLeg.GetDurationS() / 2);
    //     GloLLAPoint expectedMidTurn = turnLeg.PositionAtLegFraction(0.5);
    //     bool mtLat = GloValueUtils.EqualsWithinTolerance(midTurnPos.LatDegs, expectedMidTurn.LatDegs, 0.0001);
    //     bool mtLon = GloValueUtils.EqualsWithinTolerance(midTurnPos.LonDegs, expectedMidTurn.LonDegs, 0.0001);
    //     testLog.AddResult("Route Turn MidPos", mtLat && mtLon);
    // }
}
