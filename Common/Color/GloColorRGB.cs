// Custom colour class, protable between frameworks.

using System;

public struct GloColorRGB
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }

    public float Rf => R / 255f;
    public float Gf => G / 255f;
    public float Bf => B / 255f;
    public float Af => A / 255f;

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public GloColorRGB(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public GloColorRGB(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
        A = GloColorIO.MaxByte;
    }

    public GloColorRGB(float r, float g, float b, float a)
    {
        R = GloColorIO.FloatToByte(r);
        G = GloColorIO.FloatToByte(g);
        B = GloColorIO.FloatToByte(b);
        A = GloColorIO.FloatToByte(a);
    }

    public GloColorRGB(float r, float g, float b)
    {
        R = GloColorIO.FloatToByte(r);
        G = GloColorIO.FloatToByte(g);
        B = GloColorIO.FloatToByte(b);
        A = GloColorIO.MaxByte;
    }

    public static readonly GloColorRGB Zero  = new GloColorRGB(GloColorIO.MinByte, GloColorIO.MinByte, GloColorIO.MinByte, GloColorIO.MinByte);
    public static readonly GloColorRGB White = new GloColorRGB(GloColorIO.MaxByte, GloColorIO.MaxByte, GloColorIO.MaxByte, GloColorIO.MaxByte);

    // --------------------------------------------------------------------------------------------
    // MARK: Changes
    // --------------------------------------------------------------------------------------------

    // Usage: GloColorRGB.Lerp(col1, col2, t)
    // 0 = col1, 1 = col2, 0.5 = halfway between col1 and col2

    public static GloColorRGB Lerp(GloColorRGB col1, GloColorRGB col2, float col2fraction)
    {
        if (col2fraction < 0.0f) col2fraction = 0.0f;
        if (col2fraction > 1.0f) col2fraction = 1.0f;

        float newRf = col1.Rf + (col2.Rf - col1.Rf) * col2fraction;
        float newGf = col1.Gf + (col2.Gf - col1.Gf) * col2fraction;
        float newBf = col1.Bf + (col2.Bf - col1.Bf) * col2fraction;
        float newAf = col1.Af + (col2.Af - col1.Af) * col2fraction;

        return new GloColorRGB(newRf, newGf, newBf, newAf);
    }


}