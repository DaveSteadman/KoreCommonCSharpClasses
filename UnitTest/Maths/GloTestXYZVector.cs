using System;

public static class GloTestXYZVector
{
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestConstruction(testLog);
            TestNormalization(testLog);
            TestMagnitude(testLog);
            TestAddition(testLog);
            TestSubtraction(testLog);
            TestScaling(testLog);
            TestDotProduct(testLog);
            TestCrossProduct(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("GloXYZVector Tests", false, ex.Message);
        }
    }

    private static void TestConstruction(GloTestLog testLog)
    {
        var v = new GloXYZVector(1, 2, 3);
        testLog.AddResult("GloXYZVector Construction", v.X == 1 && v.Y == 2 && v.Z == 3);
    }

    private static void TestNormalization(GloTestLog testLog)
    {
        var v = new GloXYZVector(3, 0, 4);
        var norm = v.Normalize();
        double mag = Math.Sqrt(norm.X * norm.X + norm.Y * norm.Y + norm.Z * norm.Z);
        testLog.AddResult("GloXYZVector Normalize", GloValueUtils.EqualsWithinTolerance(mag, 1.0));
    }

    private static void TestMagnitude(GloTestLog testLog)
    {
        // Magnitude using a quick 3, 4, 5 triangle example:
        var v = new GloXYZVector(3, 4, 0);
        double mag = v.Magnitude;
        testLog.AddResult("GloXYZVector Magnitude", GloValueUtils.EqualsWithinTolerance(mag, 5.0));
    }

    private static void TestAddition(GloTestLog testLog)
    {
        var v1 = new GloXYZVector(1, 2, 3);
        var v2 = new GloXYZVector(4, 5, 6);
        var sum = v1 + v2;
        testLog.AddResult("GloXYZVector Addition", sum.X == 5 && sum.Y == 7 && sum.Z == 9);
    }

    private static void TestSubtraction(GloTestLog testLog)
    {
        var v1 = new GloXYZVector(4, 5, 6);
        var v2 = new GloXYZVector(1, 2, 3);
        var diff = v1 - v2;
        testLog.AddResult("GloXYZVector Subtraction", diff.X == 3 && diff.Y == 3 && diff.Z == 3);
    }

    private static void TestScaling(GloTestLog testLog)
    {
        var v = new GloXYZVector(1, -2, 3);
        var scaled = v * 2;
        testLog.AddResult("GloXYZVector Scaling", scaled.X == 2 && scaled.Y == -4 && scaled.Z == 6);
    }

    private static void TestDotProduct(GloTestLog testLog)
    {
        // Quick explanation of dot product and the answer here:
        // - The dot product of two vectors gives a scalar value that represents the cosine of
        //   the angle between them multiplied by their magnitudes.
        // - The dot product of two vectors (a, b, c) and (d, e, f) is calculated as:
        // -     a*d + b*e + c*f
        // - For vectors (1, 2, 3) and (4, -5, 6), the dot product is:
        // -     1*4 + 2*(-5) + 3*6 = 4 - 10 + 18 = 12
        // - So the expected result is 12.
        var v1 = new GloXYZVector(1, 2, 3);
        var v2 = new GloXYZVector(4, -5, 6);
        double dot = GloXYZVector.DotProduct(v1, v2);
        double expectedDot = 1 * 4 + 2 * (-5) + 3 * 6; // This should equal 12
        testLog.AddResult("GloXYZVector DotProduct", GloValueUtils.EqualsWithinTolerance(dot, expectedDot));
    }

    private static void TestCrossProduct(GloTestLog testLog)
    {
        // Quick explanation of cross product and the answer here:
        // - The cross product of two vectors results in a vector that is perpendicular to both.
        // - The formula for the cross product of vectors (a, b, c) and (d, e, f) is:
        // -     (bf - ce, cd - af, ae - bd)
        // - For vectors (1, 2, 3) and (4, 5, 6), the cross product is:
        // -     (2*6 - 3*5, 3*4 - 1*6, 1*5 - 2*4) = (-3, 6, -3)
        var v1 = new GloXYZVector(1, 2, 3);
        var v2 = new GloXYZVector(4, 5, 6);
        var cross = GloXYZVector.CrossProduct(v1, v2);
        testLog.AddResult("GloXYZVector CrossProduct", cross.X == -3 && cross.Y == 6 && cross.Z == -3);
    }
}


