using System;
using System.Collections.Generic;

// A common class that represents a route. Each leg on the route could be from a separate
// class, so we will use common interface for each, but their implementation will be different.

// Design Decisions:
// - We will use a list of legs to represent the route.

public class GloRoute_ToPointAndTurn : GloRoute
{

    // // --------------------------------------------------------------------------------------------
    // // MARK: Constructors
    // // --------------------------------------------------------------------------------------------

    // public GloRoute_ToPointAndTurn()
    // {
    //     Legs = new List<IGloRouteLeg>();
    // }

    // public GloRoute_ToPointAndTurn(List<IGloRouteLeg> legs)
    // {
    //     Legs = legs;
    // }

    // // --------------------------------------------------------------------------------------------
    // // MARK: Basic Leg Management
    // // --------------------------------------------------------------------------------------------


    // // --------------------------------------------------------------------------------------------
    // // MARK: Complex Methods
    // // --------------------------------------------------------------------------------------------

    // public double GetStraightLineDistanceM()
    // {
    //     double distanceM = 0;
    //     foreach (IGloRouteLeg leg in Legs)
    //     {
    //         distanceM += leg.GetStraightLineDistanceM();
    //     }
    //     return distanceM;
    // }

    // public double GetDurationSeconds()
    // {
    //     double durationS = 0;
    //     foreach (IGloRouteLeg leg in Legs)
    //     {
    //         durationS += leg.GetDurationS();
    //     }
    //     return durationS;
    // }

    // public GloLLAPoint CurrentPosition(double timeS)
    // {
    //     double timeRemainingS = timeS;
    //     foreach (IGloRouteLeg leg in Legs)
    //     {
    //         if (timeRemainingS < leg.GetDurationS())
    //         {
    //             return leg.PositionAtLegTime(timeRemainingS);
    //         }
    //         timeRemainingS -= leg.GetDurationS();
    //     }
    //     return Legs[Legs.Count - 1].EndPoint;
    // }

    // public GloAttitude CurrentAttitude(double timeS)
    // {
    //     // This is a placeholder for now. We will implement this later.
    //     return new GloAttitude(0, 0, 0);
    // }
}