using System;

// KoreXYArc: class representing a 2D arc, which can be clockwise or anti-clockwise in direction.
// Will use radians natively and the distance units are abstract.
// Will use KoreValueUtils.Angle to ensure angles are wrapped and differences are calculated correctly.

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

namespace KoreCommon;

public struct KoreXYArc
{
    public KoreXYPoint Center { get; }
    public double Radius { get; }
    public double StartAngleRads { get; }
    public double DeltaAngleRads { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double EndAngleRads { get { return StartAngleRads + DeltaAngleRads; } }

    public double Diameter { get { return 2 * Radius; } }
    public double AngleSpanRads { get { return KoreValueUtils.AngleDiffRads(StartAngleRads, EndAngleRads); } }
    public double LengthCurved { get { return Radius * AngleSpanRads; } }
    public double LengthStraightLine { get { return Math.Sqrt(Diameter * Diameter + LengthCurved * LengthCurved); } }

    public KoreXYPoint StartPoint { get { return KoreXYPointOps.OffsetPolar(Center, Radius, StartAngleRads); } }
    public KoreXYPoint EndPoint { get { return KoreXYPointOps.OffsetPolar(Center, Radius, EndAngleRads); } }

    public double StartAngleDegs { get { return KoreValueUtils.RadsToDegs(StartAngleRads); } }
    public double EndAngleDegs { get { return KoreValueUtils.RadsToDegs(EndAngleRads); } }
    public double AngleSpanDegs { get { return KoreValueUtils.RadsToDegs(AngleSpanRads); } }

    public KoreXYCircle Circle { get { return new KoreXYCircle(Center, Radius); } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public KoreXYArc(KoreXYPoint center, double radius, double startAngleRads, double deltaAngleRads)
    {
        Center = center;
        Radius = radius;
        StartAngleRads = startAngleRads;
        DeltaAngleRads = deltaAngleRads;
    }

    public KoreXYArc(KoreXYArc arc)
    {
        Center = arc.Center;
        Radius = arc.Radius;
        StartAngleRads = arc.StartAngleRads;
        DeltaAngleRads = arc.DeltaAngleRads;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public KoreXYArc offset(double x, double y)
    {
        return new KoreXYArc(Center.Offset(x, y), Radius, StartAngleRads, DeltaAngleRads);
    }

    // return if the angle is within the arc

    public bool ContainsAngle(double angleRads)
    {
        // return KoreValueUtils.IsAngleInRangeRads(angleRads, StartAngleDegs, EndAngleDegs);

        return KoreValueUtils.IsAngleInRangeRadsDelta(angleRads, StartAngleDegs, DeltaAngleRads);
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------


}