

// Custom colour class, protable between frameworks.

using System;

public struct GloColorGreyscale
{
    public byte V { get; set; }

    public float Vf => V / 255f;

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public GloColorGreyscale(byte v)
    {
        V = v;
    }

    public GloColorGreyscale(float v)
    {
        V = GloColorIO.FloatToByte(v);
    }

    // --------------------------------------------------------------------------------------------

    public static readonly GloColorGreyscale Zero = new GloColorGreyscale(GloColorIO.MinByte);
    public static readonly GloColorGreyscale White = new GloColorGreyscale(GloColorIO.MaxByte);
    public static readonly GloColorGreyscale Black = new GloColorGreyscale(GloColorIO.MinByte);
}


