using System;

// Abstract base class for route legs. Provides common properties and helper
// functions that can be used by all leg types.
public abstract class IGloRouteLeg
{
    // End points
    public GloLLAPoint StartPoint { get; set; }
    public GloLLAPoint EndPoint   { get; set; }

    // Course information
    public GloCourse StartCourse { get; protected set; } = GloCourse.Zero;
    public GloCourse EndCourse   { get; protected set; } = GloCourse.Zero;

    // Attitude information
    public GloAttitude StartAttitude { get; protected set; } = GloAttitude.Zero;
    public GloAttitude EndAttitude   { get; protected set; } = GloAttitude.Zero;

    // Attitude delta information
    public GloAttitudeDelta StartAttitudeDelta { get; protected set; } = GloAttitudeDelta.Zero;
    public GloAttitudeDelta EndAttitudeDelta   { get; protected set; } = GloAttitudeDelta.Zero;

    // Speed in m/s
    public double SpeedMps { get; set; } = GloConsts.ArbitraryMinDouble;

    // ---------------------------------------------------------------------
    // MARK: Distances
    // ---------------------------------------------------------------------

    // Straight line distance between the start and end points
    public virtual double GetStraightLineDistanceM() => StartPoint.CurvedDistanceToM(EndPoint);

    // Calculated distance along the leg path. Default is straight line distance.
    public virtual double GetCalculatedDistanceM() => GetStraightLineDistanceM();

    // ---------------------------------------------------------------------
    // MARK: Time
    // ---------------------------------------------------------------------

    public virtual double GetDurationS() => GetCalculatedDistanceM() / SpeedMps;

    // ---------------------------------------------------------------------
    // MARK: Position and derivatives
    // ---------------------------------------------------------------------

    public double LegTimeForFraction(double fraction) => GloDoubleRange.ZeroToOne.Apply(fraction) * GetDurationS();

    public abstract GloLLAPoint PositionAtLegTime(double legtimeS);

    public GloLLAPoint PositionAtLegFraction(double fraction) => PositionAtLegTime(LegTimeForFraction(fraction));

    public virtual GloCourse CourseAtLegTime(double legtimeS) => StartCourse;
    public virtual GloCourse CourseAtLegFraction(double fraction) => CourseAtLegTime(LegTimeForFraction(fraction));

    public virtual GloAttitude AttitudeAtLegTime(double legtimeS) => StartAttitude;
    public virtual GloAttitude AttitudeAtLegFraction(double fraction) => AttitudeAtLegTime(LegTimeForFraction(fraction));

    public virtual GloAttitudeDelta AttitudeDeltaAtLegTime(double legtimeS) => StartAttitudeDelta;
    public virtual GloAttitudeDelta AttitudeDeltaAtLegFraction(double fraction) => AttitudeDeltaAtLegTime(LegTimeForFraction(fraction));
}

