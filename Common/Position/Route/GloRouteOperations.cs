

// GloRouteOperations: A class of static methods that perform operations on routes, such as merging, splitting, and transforming them.

public static class GloRouteOperations
{
    // --------------------------------------------------------------------------------------------
    // MARK Append
    // --------------------------------------------------------------------------------------------

    public static bool AppendLegLine(GloRoute route, GloLLAPoint newEndPoint)
    {
        //if (route == null || newEndPoint == null)
        //    return false;

        // Get the last leg of the route
        if (route.NumLegs() == 0)
            return false;

        IGloRouteLeg lastLeg = route.LastLeg();

        // If the last leg is a line leg, extend it
        if (lastLeg is GloRouteLegLine lineLeg)
        {
            lineLeg.EndPoint = newEndPoint;
            return true;
        }

        // Otherwise, create a new line leg and append it
        double speedMpsNew = 10; // Assuming a default speed for the new leg
        GloRouteLegLine newLineLeg = new GloRouteLegLine(lastLeg.EndPoint, newEndPoint, speedMpsNew);
        route.AppendLeg(newLineLeg);
        return true;
    }

    // --------------------------------------------------------------------------------------------

    public static GloRoute StraightLineRouteFromPoints(List<GloLLAPoint> points, double speedMps)
    {
        if (points == null || points.Count < 2)
            throw new ArgumentException("At least two points are required to create a route.");

        GloRoute route = new GloRoute();

        for (int i = 0; i < points.Count - 1; i++)
        {
            GloLLAPoint startPoint = points[i];
            GloLLAPoint endPoint = points[i + 1];
            GloRouteLegLine leg = new GloRouteLegLine(startPoint, endPoint, speedMps);
            route.AppendLeg(leg);
        }

        return route;
    }

    // --------------------------------------------------------------------------------------------

}