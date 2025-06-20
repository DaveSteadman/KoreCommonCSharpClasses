using System;

// Simple turn leg following a circular arc defined by a start point,
// a turn centre point and an angular change.
public class GloRouteSimpleTurn : IGloRouteLeg
{
    public GloLLAPoint TurnPoint { get; set; }
    public double      DeltaAngleRads { get; set; }
    public double      SpeedMps { get; set; }

    // ------------------------------------------------------------------
    public double DeltaAngleDegs
    {
        get => DeltaAngleRads * GloConsts.RadsToDegsMultiplier;
        set => DeltaAngleRads = value * GloConsts.DegsToRadsMultiplier;
    }

    public GloRouteSimpleTurn(GloLLAPoint startPoint, GloLLAPoint turnPoint, double deltaAngleRads, double speedMps)
    {
        StartPoint     = startPoint;
        TurnPoint      = turnPoint;
        DeltaAngleRads = deltaAngleRads;
        SpeedMps       = speedMps;
        Setup();
    }

    private void Setup()
    {
        double radius = TurnRadiusM();
        double startBearing = TurnPoint.BearingToRads(StartPoint);
        double endBearing   = startBearing + DeltaAngleRads;

        EndPoint = TurnPoint.PlusRangeBearing(new GloRangeBearing(radius, endBearing));

        double startHeading = startBearing + (DeltaAngleRads > 0 ? Math.PI / 2 : -Math.PI / 2);
        double endHeading   = endBearing   + (DeltaAngleRads > 0 ? Math.PI / 2 : -Math.PI / 2);

        StartCourse = new GloCourse() { SpeedMps = SpeedMps, HeadingRads = startHeading, ClimbRateMps = 0 };
        EndCourse   = new GloCourse() { SpeedMps = SpeedMps, HeadingRads = endHeading, ClimbRateMps = 0 };

        StartAttitude      = GloAttitude.Zero;
        EndAttitude        = GloAttitude.Zero;
        StartAttitudeDelta = GloAttitudeDelta.Zero;
        EndAttitudeDelta   = GloAttitudeDelta.Zero;
    }

    // Radius from centre to start
    public double TurnRadiusM() => TurnPoint.CurvedDistanceToM(StartPoint);

    public double TurnLengthM() => Math.Abs(DeltaAngleRads) * TurnRadiusM();

    public double TurnDurationSecs() => (SpeedMps < GloConsts.ArbitraryMinDouble) ? 0 : TurnLengthM() / SpeedMps;

    public override double GetCalculatedDistanceM() => TurnLengthM();

    public override double GetDurationS() => TurnDurationSecs();

    private GloLLAPoint PositionAtTurnFraction(double fraction)
    {
        fraction = GloDoubleRange.ZeroToOne.Apply(fraction);
        double startBearing = TurnPoint.BearingToRads(StartPoint);
        double angle = startBearing + DeltaAngleRads * fraction;
        return TurnPoint.PlusRangeBearing(new GloRangeBearing(TurnRadiusM(), angle));
    }

    public override GloLLAPoint PositionAtLegTime(double legtimeS)
    {
        double frac = (GetDurationS() > 0) ? legtimeS / GetDurationS() : 0;
        return PositionAtTurnFraction(frac);
    }

    public override GloCourse CourseAtLegTime(double legtimeS)
    {
        double frac = (GetDurationS() > 0) ? legtimeS / GetDurationS() : 0;
        frac = GloDoubleRange.ZeroToOne.Apply(frac);
        double startBearing = TurnPoint.BearingToRads(StartPoint);
        double angle = startBearing + DeltaAngleRads * frac;
        double heading = angle + (DeltaAngleRads > 0 ? Math.PI / 2 : -Math.PI / 2);
        return new GloCourse() { SpeedMps = SpeedMps, HeadingRads = GloDoubleRange.ZeroToTwoPiRadians.Apply(heading), ClimbRateMps = 0 };
    }
}
