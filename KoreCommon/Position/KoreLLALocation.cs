using System;
using System.Collections.Generic;

namespace KoreCommon;


// KoreLLALocation: A class that encapsulates the idea of a location with latitude, longitude, and altitude, but without
// a fixed radius or altitude type.

public struct KoreLLALocation
{
    // SI Units and a pure radius value so the trig functions are simple.
    // Accomodate units and MSL during accessor functions

    public double LatRads { get; set; }
    public double LonRads { get; set; }
    public double HeightM { get; set; } // Alt above EarthCentre

    public enum HeightType { MSL, AGL, AGLMSL, AGLMSLAVG };
    public HeightType HeightTypeValue { get; set; } = HeightType.MSL;


    // --------------------------------------------------------------------------------------------
    // Additional simple accessors - adding units
    // --------------------------------------------------------------------------------------------

    public double LatDegs
    {
        get { return LatRads * KoreConsts.RadsToDegsMultiplier; }
        set { LatRads = value * KoreConsts.DegsToRadsMultiplier; }
    }
    public double LonDegs
    {
        get { return LonRads * KoreConsts.RadsToDegsMultiplier; }
        set { LonRads = value * KoreConsts.DegsToRadsMultiplier; }
    }
    // public double AltMslKm // Alt above MSL
    // {
    //     get { return (RadiusM - KoreWorldConsts.EarthRadiusM) * KoreWorldConsts.MetresToKmMultiplier; }
    //     set { RadiusM = (value + KoreWorldConsts.EarthRadiusKm) * KoreWorldConsts.KmToMetresMultiplier; }
    // }
    // public double AltMslM // Alt above M
    // {
    //     get { return (RadiusM - KoreWorldConsts.EarthRadiusM); }
    //     set { RadiusM = value + KoreWorldConsts.EarthRadiusM; }
    // }
    // public double RadiusKm // Alt above EarthRadius
    // {
    //     get { return (RadiusM * KoreWorldConsts.MetresToKmMultiplier); }
    //     set { RadiusM = (value * KoreWorldConsts.KmToMetresMultiplier); }
    // }

    public override string ToString()
    {
        return string.Format($"({LatDegs:F2}, {LonDegs:F2}, {HeightM:F2})");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors - different options and units
    // --------------------------------------------------------------------------------------------

    // Note that fields can be set:
    //   KoreLLAPoint pos = new KoreLLAPoint() { latDegs = X, LonDegs = Y, AltMslM = Z };

    public KoreLLALocation(double laRads, double loRads, double altM)
    {
        this.LatRads = laRads;
        this.LonRads = loRads;
        this.HeightM = altM;
    }

    public KoreLLALocation(double laRads, double loRads)
    {
        this.LatRads = laRads;
        this.LonRads = loRads;
        this.HeightM = 0;
    }

    public static KoreLLALocation Zero
    {
        get { return new KoreLLALocation { LatRads = 0.0, LonRads = 0.0, HeightM = 0.0 }; }
    }

    // Function to return an LLA position on the ground, either using the default MSL value, or an optional elevation above MSL in metres.

    public KoreLLAPoint ToLLA()
    {
        double newRadiusM = 0;

        switch(HeightTypeValue)
        {
            case HeightType.MSL:
                newRadiusM = KoreWorldConsts.EarthRadiusM + HeightM;
                break;
            case HeightType.AGL:
                newRadiusM = KoreWorldConsts.EarthRadiusM + HeightM;
                break;
            case HeightType.AGLMSL:
                newRadiusM = KoreWorldConsts.EarthRadiusM + HeightM;
                break;
            default:
                newRadiusM = KoreWorldConsts.EarthRadiusM + HeightM;
                break;
        }

        return new KoreLLAPoint(LatRads, LonRads) { RadiusM = newRadiusM };
    }

}
