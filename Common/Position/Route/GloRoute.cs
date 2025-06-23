using System;
using System.Collections.Generic;

// A common class that represents a route. Each leg on the route could be from a separate
// class, so we will use common interface for each, but their implementation will be different.

// Design Decisions:
// - We will use a list of legs to represent the route.

public class GloRoute
{
    public List<IGloRouteLeg> Legs;

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    public GloRoute()
    {
        Legs = new List<IGloRouteLeg>();
    }

    public GloRoute(List<IGloRouteLeg> legs)
    {
        Legs = legs;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Basic Leg Management
    // --------------------------------------------------------------------------------------------

    public int NumLegs() => Legs.Count;
    public void AppendLeg(IGloRouteLeg leg) => Legs.Add(leg);
    public void InsertLeg(int index, IGloRouteLeg leg) => Legs.Insert(index, leg);
    public void RemoveLeg(int index) => Legs.RemoveAt(index);
    public void ClearLegs() => Legs.Clear();

    public IGloRouteLeg GetLeg(int index) => Legs[index];
    public IGloRouteLeg LastLeg() => Legs[Legs.Count - 1];

    // --------------------------------------------------------------------------------------------
    // MARK: Route traversal
    // --------------------------------------------------------------------------------------------

    public double TotalStraightLineDistanceM() => Legs.Sum(leg => leg.GetStraightLineDistanceM());
    public double TotalCalculatedDistanceM() => Legs.Sum(leg => leg.GetCalculatedDistanceM());
    public double TotalDurationSeconds() => Legs.Sum(leg => leg.GetDurationS());

    // ------------------------------------------------------------------------------------------------

    public IGloRouteLeg LegAtRouteTime(double timeS)
    {
        double timeRemainingS = timeS;
        foreach (IGloRouteLeg leg in Legs)
        {
            if (timeRemainingS < leg.GetDurationS())
                return leg;

            timeRemainingS -= leg.GetDurationS();
        }
        return Legs[Legs.Count - 1]; // Return the last leg if time exceeds total duration
    }

    public double RouteTimeForFraction(double fraction)
    {
        if ((fraction < 0) || (NumLegs() == 0))
            return 0;

        double totalDurationS = TotalDurationSeconds();

        if (fraction > 1)
            return totalDurationS;

        return totalDurationS * fraction;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Complex Methods
    // --------------------------------------------------------------------------------------------

    public double GetStraightLineDistanceM()
    {
        double distanceM = 0;
        foreach (IGloRouteLeg leg in Legs)
        {
            distanceM += leg.GetStraightLineDistanceM();
        }
        return distanceM;
    }

    // ------------------------------------------------------------------------------------------------

    public double GetDurationSeconds()
    {
        double durationS = 0;
        foreach (IGloRouteLeg leg in Legs)
        {
            durationS += leg.GetDurationS();
        }
        return durationS;
    }

    // ------------------------------------------------------------------------------------------------

    public GloLLAPoint PositionAtRouteTime(double timeS)
    {
        // Setup the start time for the first leg
        double currLegStartSecs = 0;

        foreach (IGloRouteLeg leg in Legs)
        {
            // Find the times for the current leg duration, the current leg end time, and the current leg time
            double currLegDurationSecs = leg.GetDurationS();
            double currLegEndSecs = currLegStartSecs + currLegDurationSecs;
            double currLegTime = timeS - currLegStartSecs;

            // If the current leg time fits in the current leg's duration
            if ((currLegTime > 0) && (currLegTime < currLegDurationSecs))
            {
                // If the current time is within the current leg's duration, return the position
                return leg.PositionAtLegTime(currLegTime);
            }

            // Set the start time for the next leg
            currLegStartSecs = currLegEndSecs;
        }

        // If we get to the end of the logic without finding a leg, return a default position
        return GloLLAPoint.Zero;
    }

    // ------------------------------------------------------------------------------------------------

    // Get the route, in a list of points.
    // - Throw if there are no legs, or less than two points (the start and end points are required).

    public List<GloLLAPoint> RoutePositions(int numPoints)
    {
        if (NumLegs() == 0 || numPoints < 2)
            throw new InvalidOperationException("Route must have at least two points.");

        // Determine the fractions and number of seconds between points
        double totalDurationS = TotalDurationSeconds();
        double secondsPerPoint = totalDurationS / (numPoints - 1);

        List<GloLLAPoint> points = new List<GloLLAPoint>();

        for (int i = 0; i < numPoints; i++)
        {
            double currTime = i * secondsPerPoint;
            points.Add(PositionAtRouteTime(currTime));
        }

        return points;
    }
}





