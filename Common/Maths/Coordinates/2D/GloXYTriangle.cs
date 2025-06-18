using System;

// Represents a triangle in 2D space defined by three points (A, B, C).
// Provides geometric utilities such as area, centroid, containment, and edge access.

public struct GloXYTriangle
{
    public GloXYPoint A { get; set; }
    public GloXYPoint B { get; set; }
    public GloXYPoint C { get; set; }

    public GloXYLine LineAB => new GloXYLine(A, B);
    public GloXYLine LineBC => new GloXYLine(B, C);
    public GloXYLine LineCA => new GloXYLine(C, A);

    // -------------------------------------------------------------------------------
    // MARK: Angle Properties
    // -------------------------------------------------------------------------------

    private static double InternalAngle(GloXYPoint prev, GloXYPoint vertex, GloXYPoint next)
    {
        double angle = GloXYPointOperations.AngleBetweenRads(prev, vertex, next);
        if (angle > Math.PI)
            angle = (2 * Math.PI) - angle; // convert reflex angle to internal
        return GloDoubleRange.ZeroToPiRadians.Apply(angle);
    }

    // Internal angle at the corner formed by AB -> BC
    public double InternalAngleABRads() => InternalAngle(A, B, C);

    // Internal angle at the corner formed by BC -> CA
    public double InternalAngleBCRads() => InternalAngle(B, C, A);

    // Internal angle at the corner formed by CA -> AB
    public double InternalAngleCARads() => InternalAngle(C, A, B);

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Create a triangle from three points.
    public GloXYTriangle(GloXYPoint a, GloXYPoint b, GloXYPoint c)
    {
        A = a;
        B = b;
        C = c;
    }

    public static GloXYTriangle Zero { get => new GloXYTriangle(new GloXYPoint(0, 0), new GloXYPoint(0, 0), new GloXYPoint(0, 0)); }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Properties
    // --------------------------------------------------------------------------------------------

    // Returns the centroid (center point) of the triangle.
    public GloXYPoint CenterPoint() => new GloXYPoint((A.X + B.X + C.X) / 3.0, (A.Y + B.Y + C.Y) / 3.0);

    // Returns the area of the triangle.
    public double Area() => Math.Abs((A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y)) / 2.0);

    // Returns the perimeter (sum of edge lengths) of the triangle.
    public double Perimeter() => LineAB.Length + LineBC.Length + LineCA.Length;

    // Returns true if the triangle is degenerate (area is zero or nearly zero).
    public bool IsDegenerate() => Area() < 1e-10;

    public GloXYRect AABB()
    {
        GloXYPoint topLeft     = new GloXYPoint( GloNumericUtils.Min3(A.X, B.X, C.X), GloNumericUtils.Min3(A.Y, B.Y, C.Y) );
        GloXYPoint bottomRight = new GloXYPoint( GloNumericUtils.Min3(A.X, B.X, C.X), GloNumericUtils.Min3(A.Y, B.Y, C.Y) );
        return new GloXYRect(topLeft, bottomRight);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Utilities
    // --------------------------------------------------------------------------------------------

    // Returns true if the given point lies inside the triangle (or on its edge).
    // This is done by comparing the area of the triangle to the sum of the areas of three sub-triangles
    // formed by the test point and each pair of triangle vertices. If the sum of the sub-areas equals
    // the original area (within a small tolerance for floating-point precision), the point is inside or on the triangle.
    public bool Contains(GloXYPoint point)
    {
        double area = Area();
        double area1 = new GloXYTriangle(point, B, C).Area();
        double area2 = new GloXYTriangle(A, point, C).Area();
        double area3 = new GloXYTriangle(A, B, point).Area();
        return Math.Abs(area - (area1 + area2 + area3)) < 1e-10; // Allow for floating-point precision issues
    }

    public GloXYTriangle Inset(double inset)
    {
        // Construct lines for each edge
        var ab = new GloXYLine(A, B);
        var bc = new GloXYLine(B, C);
        var ca = new GloXYLine(C, A);

        // Inward-offset each line w.r.t. the opposite point
        var abIn = GloXYLineOperations.OffsetInward(ab, C, inset);
        var bcIn = GloXYLineOperations.OffsetInward(bc, A, inset);
        var caIn = GloXYLineOperations.OffsetInward(ca, B, inset);

        // Intersect adjacent pairs
        if (!GloXYLineOperations.TryIntersect(abIn, bcIn, out var i1)) return this;
        if (!GloXYLineOperations.TryIntersect(bcIn, caIn, out var i2)) return this;
        if (!GloXYLineOperations.TryIntersect(caIn, abIn, out var i3)) return this;

        return new GloXYTriangle(i1, i2, i3);
    }

    // Returns a new triangle translated by the given offset.
    public GloXYTriangle Translate(double dx, double dy) => new GloXYTriangle(A.Offset(dx, dy), B.Offset(dx, dy), C.Offset(dx, dy));

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Management
    // --------------------------------------------------------------------------------------------

    // Returns the triangle's vertices as an array.
    public GloXYPoint[] ToArray() => new[] { A, B, C };

    // Returns a string representation of the triangle.
    public override string ToString() => $"Triangle: A={A}, B={B}, C={C}, Area={Area():F2}, Perimeter={Perimeter():F2}, Centroid={CenterPoint()}";
}


