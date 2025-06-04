using System;

// GloXYVectorIO: Converts the GloXYVector struct to and from various formats, such as JSON or binary.

public static class GloXYVectorIO
{
    // Usage: string str = GloXYVectorIO.ToString(new GloXYVector(1.0, 2.0));
    public static string ToString(GloXYVector vector)
    {
        return $"X:{vector.X:F3}, Y:{vector.Y:F3}";
    }

    // Usage: GloXYVector vector = GloXYVectorIO.FromString("X:1.0, Y:2.0");
    public static GloXYVector FromString(string str)
    {
        var parts = str.Split(',');
        if (parts.Length != 2) throw new FormatException("Invalid GloXYVector string format.");

        double x = double.Parse(parts[0].Split(':')[1]);
        double y = double.Parse(parts[1].Split(':')[1]);

        return new GloXYVector(x, y);
    }
}