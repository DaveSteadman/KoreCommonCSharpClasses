using System;
using System.Collections.Generic;

public static class GloTestRoute
{
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestRouteLegLine(testLog);
            //TestRouteSimpleTurn(testLog);
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
        // Setup some standard values
        GloLLAPoint p1 = new GloLLAPoint() { LatDegs = 45, LonDegs = 10, AltMslM = 1000 };
        GloLLAPoint p2 = new GloLLAPoint() { LatDegs = 48, LonDegs = 11, AltMslM = 1000 };
        double legSpeed = 100; // m/s

        // Determine some values for the leg to check against
        double checkDistance = p1.CurvedDistanceToM(p2); // distance in meters - curved across average altitude.
        double checkDuration = checkDistance / legSpeed; // duration in seconds
        double checkBearingRads = GloNumericRange<double>.ZeroToTwoPiRadians.Apply(p1.BearingToRads(p2));
        double checkBearingDegs = checkBearingRads * GloConsts.RadsToDegsMultiplier;
        GloRangeBearing rbLeg = p1.RangeBearingTo(p2);

        // - - - - - -

        // Create a simple route leg with a straight line
        var leg1 = new GloRouteLegLine(p1, p2, legSpeed);

        // Validate the leg properties
        testLog.AddResult("Route Leg Length", GloValueUtils.EqualsWithinTolerance(
            leg1.GetCalculatedDistanceM(), checkDistance, 0.001),
            $"Expected: {checkDistance:F1} m, Actual: {leg1.GetCalculatedDistanceM():F1} m");

        testLog.AddResult("Route Leg Duration", GloValueUtils.EqualsWithinTolerance(
            leg1.GetDurationS(), checkDuration, 0.001),
            $"Expected: {checkDuration:F1} s, Actual: {leg1.GetDurationS():F1} s");

        testLog.AddResult("Route Leg Bearing",
            GloValueUtils.EqualsWithinTolerance(leg1.StartCourse.HeadingRads, checkBearingRads, 0.001),
            $"Expected: {checkBearingRads:F3} rad {checkBearingDegs:F3} deg, Actual: {leg1.StartCourse.HeadingRads:F3} rad {leg1.StartCourse.HeadingRads * GloConsts.RadsToDegsMultiplier:F3} deg");

        // - - - - - -

        // Create a second leg using other constructor params
        var leg2 = new GloRouteLegLine(p1, rbLeg, legSpeed);

        // Validate the second leg properties
        testLog.AddResult("Route Leg2 Length", GloValueUtils.EqualsWithinTolerance(
            leg2.GetCalculatedDistanceM(), checkDistance, 0.001),
            $"Expected: {checkDistance:F1} m, Actual: {leg2.GetCalculatedDistanceM():F1} m");

        testLog.AddResult("Route Leg2 Duration", GloValueUtils.EqualsWithinTolerance(
            leg2.GetDurationS(), checkDuration, 0.001),
            $"Expected: {checkDuration:F1} s, Actual: {leg2.GetDurationS():F1} s");

        testLog.AddResult("Route Leg2 Bearing", GloValueUtils.EqualsWithinTolerance(
            leg2.StartCourse.HeadingRads, checkBearingRads, 0.001),
            $"Expected: {checkBearingRads:F3} rad {checkBearingDegs:F3} deg, Actual: {leg2.StartCourse.HeadingRads:F3} rad {leg2.StartCourse.HeadingRads * GloConsts.RadsToDegsMultiplier:F3} deg");
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
