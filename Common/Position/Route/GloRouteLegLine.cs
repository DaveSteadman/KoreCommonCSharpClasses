// GloRouteLegPointToPoint: A class that has a start and end point for straight line route leg, along with speed.

using System;

public class GloRouteLegLine : IGloRouteLeg
{
    private readonly double speedMps;

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // CReate a route leg from two points and a speed.
    public GloRouteLegLine(GloLLAPoint startPoint, GloLLAPoint endPoint, double speedMPS)
    {
        SetupRoute(startPoint, endPoint, speedMPS);
    }

    // Create a route leg from a start point, a range bearing offset and a speed.
    public GloRouteLegLine(GloLLAPoint startPoint, GloRangeBearing rbLeg, double speedMPS)
    {
        SetupRoute(startPoint, rbLeg, speedMPS);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Setup Methods
    // --------------------------------------------------------------------------------------------

    // From a start point, an end point and a speed, set up the remaining route leg properties.

    private void SetupRoute(GloLLAPoint startPoint, GloLLAPoint endPoint, double speedMPS)
    {
        // Calculate the course and distance between the two points
        StartPoint = startPoint;
        EndPoint   = endPoint;
        SpeedMps   = speedMPS;

        GloRangeBearing rb = startPoint.RangeBearingTo(endPoint);

        GloCourse course = new GloCourse()
        {
            SpeedMps     = speedMPS,
            HeadingDegs  = rb.BearingDegs,
            ClimbRateMps = 0
        };

        // Set the course and distance
        StartCourse = course;
        EndCourse = course;

        // Set the attitude and attitude delta
        StartAttitude = GloAttitude.Zero;
        EndAttitude = GloAttitude.Zero;
        StartAttitudeDelta = GloAttitudeDelta.Zero;
        EndAttitudeDelta = GloAttitudeDelta.Zero;
    }

    public void SetupRoute(GloLLAPoint startPoint, GloRangeBearing rbLeg, double speedMPS)
    {
        StartPoint = startPoint;
        EndPoint   = startPoint.PlusRangeBearing(rbLeg);
        SpeedMps   = speedMPS;

        // Calculate the course based on the range bearing
        GloCourse course = new GloCourse() {
            SpeedMps     = speedMPS,
            HeadingDegs  = rbLeg.BearingDegs,
            ClimbRateMps = 0
        };
        StartCourse = course;
        EndCourse   = course;

        StartAttitude      = GloAttitude.Zero;
        EndAttitude        = GloAttitude.Zero;
        StartAttitudeDelta = GloAttitudeDelta.Zero;
        EndAttitudeDelta   = GloAttitudeDelta.Zero;
    }

    // --------------------------------------------------------------------------------------------
    // Public Methods
    // --------------------------------------------------------------------------------------------

    public override double GetCalculatedDistanceM() => StartPoint.CurvedDistanceToM(EndPoint);

    public override double GetDurationS()
    {
        double dist = GetCalculatedDistanceM();
        return (SpeedMps < GloConsts.ArbitraryMinDouble) ? 0 : dist / SpeedMps;
    }

    public override GloLLAPoint PositionAtLegTime(double legtimeS)
    {
        double dist = SpeedMps * legtimeS;
        double frac = (GetCalculatedDistanceM() > 0) ? dist / GetCalculatedDistanceM() : 0;
        frac = GloDoubleRange.ZeroToOne.Apply(frac);
        return GloLLAPointOperations.RhumbLineInterpolation(StartPoint, EndPoint, frac);
    }
}



