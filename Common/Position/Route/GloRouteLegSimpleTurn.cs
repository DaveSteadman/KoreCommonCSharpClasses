using System;

// Simple turn leg following a circular arc defined by a start point,
// a turn centre point and an angular change.
public class GloRouteLegSimpleTurn : IGloRouteLeg
{
    // The pre-determined centre point of the turn arc. The distance from the start point to this point defines the turn radius.
    public GloLLAPoint TurnPoint { get; set; }

    // Delta angle. Positive for right turns, negative for left turns, from the perspective of the platform.
    // Right turns are clockwise, left turns are counter-clockwise.
    public double DeltaAngleRads { get; set; }
    public double DeltaAngleDegs
    {
        get => DeltaAngleRads * GloConsts.RadsToDegsMultiplier;
        set => DeltaAngleRads = value * GloConsts.DegsToRadsMultiplier;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    public GloRouteLegSimpleTurn(GloLLAPoint startPoint, GloLLAPoint turnPoint, double deltaAngleRads, double speedMps)
    {
        StartPoint = startPoint;
        TurnPoint = turnPoint;
        DeltaAngleRads = deltaAngleRads;
        SpeedMps = speedMps;
        Setup();
    }

    private void Setup()
    {
        double radius = TurnRadiusM();
        double startBearing = TurnPoint.BearingToRads(StartPoint);
        double endBearing = startBearing + DeltaAngleRads;

        EndPoint = TurnPoint.PlusRangeBearing(new GloRangeBearing(radius, endBearing));

        double startHeading = startBearing + (DeltaAngleRads > 0 ? Math.PI / 2 : -Math.PI / 2);
        double endHeading = endBearing + (DeltaAngleRads > 0 ? Math.PI / 2 : -Math.PI / 2);

        StartCourse = new GloCourse() { SpeedMps = SpeedMps, HeadingRads = startHeading, ClimbRateMps = 0 };
        EndCourse = new GloCourse() { SpeedMps = SpeedMps, HeadingRads = endHeading, ClimbRateMps = 0 };

        StartAttitude = GloAttitude.Zero;
        EndAttitude = GloAttitude.Zero;
        StartAttitudeDelta = GloAttitudeDelta.Zero;
        EndAttitudeDelta = GloAttitudeDelta.Zero;
    }

    // --------------------------------------------------------------------------------------------

    // Radius from centre to start
    public double TurnRadiusM() => TurnPoint.CurvedDistanceToM(StartPoint);

    // Turn fraction of a whole circle (2PiRads)
    //public double TurnFraction() => Math.Abs(DeltaAngleRads) / (2 * Math.PI);
    public double TurnFraction() => Math.Abs(DeltaAngleRads) * TurnRadiusM();

    // Turn length, calculated as 2PiR * TurnFraction
    public double TurnLengthM() => (2 * Math.PI * TurnRadiusM()) * TurnFraction();

    public double TurnDurationSecs() => (SpeedMps < GloConsts.ArbitraryMinDouble) ? 0 : TurnLengthM() / SpeedMps;


    public override double GetDurationS() => TurnDurationSecs();

    // ---------------------------------------------------------------------
    // MARK: Distances
    // ---------------------------------------------------------------------


    public override double GetCalculatedDistanceM() => TurnLengthM();


    // ---------------------------------------------------------------------
    // MARK: Time
    // ---------------------------------------------------------------------

    //public override double GetDurationS() => GetCalculatedDistanceM() / SpeedMps;

    // ---------------------------------------------------------------------
    // MARK: Position and derivatives
    // ---------------------------------------------------------------------

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

    // --------------------------------------------------------------------------------------------
    // MARK: Static Helper Methods
    // --------------------------------------------------------------------------------------------

    // Given a previous leg values (position and course), a desired turn radius, and a delta angle (sign for left/right),
    // compute the center point of the turn arc (the turn point).
    // public static GloLLAPoint FindTurnPoint(GloLLAPoint prevLegStartPoint, GloLLAPoint prevLegEndPoint, double turnRadiusM, double deltaAngleRads)

    public static GloLLAPoint FindTurnPoint(GloLLAPoint startPoint, GloCourse startCourse, double turnRadiusM, double deltaAngleRads)
    {
        double legDirectionRads = startCourse.HeadingRads;

        // Determine the perpendicular direction for the turn center - +ve is pilot turning right X radians, -ve if left.
        double perpendicularAngleRads = (deltaAngleRads > 0) ? (Math.PI / 2) : -(Math.PI / 2);

        // Combined angle from the start point to the turn point
        double startPointBearingToTurnPoint = GloDoubleRange.ZeroToTwoPiRadians.Apply(legDirectionRads + perpendicularAngleRads);

        // The turn center is offset from the previous leg end point by the turn radius in the perpendicular direction
        GloRangeBearing rbToTurnPoint = new GloRangeBearing(turnRadiusM, startPointBearingToTurnPoint);
        return startPoint.PlusRangeBearing(rbToTurnPoint);
    }

}


