// GloRouteLegPointToPoint: A class that has a start and end point for straight line route leg, along with speed.

using System;

public class GloRouteLegLine : IGloRouteLeg
{
    private readonly double speedMps;

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public GloRouteLegLine(GloLLAPoint startPoint, GloLLAPoint endPoint, double speedMPS)
    {
        StartPoint = startPoint;
        EndPoint   = endPoint;
        speedMps   = speedMPS;
        SetupRoute(speedMPS);
    }

    // --------------------------------------------------------------------------------------------
    // Public Methods
    // --------------------------------------------------------------------------------------------

    private void SetupRoute(double speedMPS)
    {
        // Calculate the course and distance between the two points

        GloRangeBearing rb = StartPoint.RangeBearingTo(EndPoint);

        GloCourse course = new GloCourse() {
            SpeedMps     = speedMPS,
            HeadingDegs  = rb.BearingDegs,
            ClimbRateMps = 0
        };

        // Set the course and distance
        StartCourse = course;
        EndCourse   = course;

        // Set the attitude and attitude delta
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
        return (speedMps < GloConsts.ArbitraryMinDouble) ? 0 : dist / speedMps;
    }

    public override GloLLAPoint PositionAtLegTime(double legtimeS)
    {
        double dist = speedMps * legtimeS;
        double frac = (GetCalculatedDistanceM() > 0) ? dist / GetCalculatedDistanceM() : 0;
        frac = GloDoubleRange.ZeroToOne.Apply(frac);
        return GloLLAPointOperations.RhumbLineInterpolation(StartPoint, EndPoint, frac);
    }
}



