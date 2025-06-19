// Class to define a simple route turn, a circular arc defined by a start point, a turn point, and a delta angle (that can be plus or minus), but generally
// defined by the turn point being perpendicular to the start point and a distance away (that defines the radius of the turn).

public class GloRouteSimpleTurn
{
    // public GloLLAPoint StartPoint { get; set; }
    // public GloLLAPoint TurnPoint { get; set; }
    // public double DeltaAngleRads { get; set; } // Positive for right turn, negative for left turn

    // // Define values useful to traversing the turn
    // public double DistancePerSecondM { get; set; } // Speed through the turn

    // // ----------------------------------------------------------------------------------------------

    // public double DeltaAngleDegs
    // {
    //     get => GloValueUtils.RadsToDegs(DeltaAngleRads);
    //     set => DeltaAngleRads = GloValueUtils.DegsToRads(value);
    // }

    // // ----------------------------------------------------------------------------------------------

    // // Constructor
    // public GloRouteSimpleTurn(GloLLAPoint startPoint, GloLLAPoint turnPoint, double deltaAngleRads)
    // {
    //     StartPoint = startPoint;
    //     TurnPoint = turnPoint;
    //     DeltaAngleRads = deltaAngleRads;
    // }

    // // ----------------------------------------------------------------------------------------------

    // public double TurnRadiusM()
    // {
    //     // Calculate the radius of the turn based on the distance from the start point to the turn point
    //     double distance = StartPoint.StraightLineDistanceToM(TurnPoint);
    //     return distance / Math.Abs(Math.Sin(DeltaAngleRads / 2));
    // }

    // public double TurnLengthM()
    // {
    //     // Calculate the length of the turn arc
    //     double radius = TurnRadiusM();
    //     return Math.Abs(DeltaAngleRads) * radius;
    // }

    // public double TurnDurationSecs()
    // {
    //     // Calculate the duration of the turn based on the length of the turn and the speed through the turn
    //     double length = TurnLengthM();
    //     return length / DistancePerSecondM;
    // }

    // // Return the direction of travel and the end of the turn, to help inform the next leg of the route.
    // public GloCourse LegEndCourse()
    // {
    //     // Calculate the range and bearing from the start point to the turn point
    //     double rangeM = StartPoint.StraightLineDistanceToM(TurnPoint);
    //     double bearingRads = StartPoint.BearingToRads(TurnPoint);

    //     // Create a GloCourse object for the turn end point
    //     return new GloCourse()
    //     {
    //         SpeedMps = DistancePerSecondM,
    //         HeadingRads = bearingRads,
    //         ClimbRateMps = 0.0 // Assuming no vertical component for the turn
    //     };
    // }

    // // ----------------------------------------------------------------------------------------------

    // // Using a time for 0 at the start of the turn, calculate the position at a specific time during the turn.

    // public GloLLAPoint PositionAtTurnTime(double turnTimeS)
    // {
    //     // Calculate the fraction of the turn completed
    //     double fraction = turnTimeS / TurnDurationSecs();

    //     // Ensure the fraction is within the valid range
    //     fraction = GloNumericRange<double>.ZeroToOne.Apply(fraction);

    //     // Delegate to PositionAtTurnFraction for the actual position calculation
    //     return PositionAtTurnFraction(fraction);
    // }

    // // Find the position at a specific fraction of the turn duration, where 0 is the start of the turn and 1 is the end of the turn.
    // // Useful when drawing the route in X segments.

    // public GloLLAPoint PositionAtTurnFraction(double turnFraction)
    // {
    //     // Ensure the fraction is within the valid range
    //     turnFraction = GloNumericRange<double>.ZeroToOne.Apply(turnFraction);

    //     // Calculate the time at the given fraction of the turn duration
    //     double turnTimeS = TurnDurationSecs() * turnFraction;

    //     return PositionAtTurnTime(turnTimeS);
    // }

    // // ----------------------------------------------------------------------------------------------

    // // Static method to help define a new turn, based on a previous leg.
    // // - We use the leg direction, plus a perpendicular direction to the leg, to find the turn point.

    // public static GloLLAPoint FindTurnPoint(GloLLAPoint prevLegStartPoint, GloLLAPoint prevLegEndPoint, double turnRadiusM, double deltaAngleRads)
    // {
    //     // Calculate the leg direction
    //     double legDirectionRads = prevLegStartPoint.BearingToRads(prevLegEndPoint);

    //     double perpendicularAngleRads = 0;

    //     // Determine the turn direction based on the delta angle
    //     // If delta angle is positive, we turn right (clockwise), if negative, we turn left (counter-clockwise).
    //     // We add the delta angle to the leg direction to find the turn direction.
    //     if (deltaAngleRads > 0)
    //     {
    //         // Right turn
    //         perpendicularAngleRads = (Math.PI / 2);
    //     }
    //     else
    //     {
    //         // Left turn
    //         perpendicularAngleRads = -(Math.PI / 2);
    //     }

    //     // Find the angle from the previous leg end point to the turn point.
    //     // This is the leg direction plus the perpendicular angle (with the perpendicular angle being defined by the turn direction).
    //     double endPointBearingToTurnPoint = GloNumericRange<double>.ZeroToTwoPiRadians.Apply(legDirectionRads + perpendicularAngleRads);

    //     GloRangeBearing rbToTurnPoint = new GloRangeBearing(turnRadiusM, endPointBearingToTurnPoint);

    //     return prevLegEndPoint.PlusRangeBearing(rbToTurnPoint);
    // }

}

