// <fileheader>

#nullable enable

using System.Collections.Generic;

namespace KoreCommon;

// --------------------------------------------------------------------------------------------
// MARK: KoreGeoFeature
// --------------------------------------------------------------------------------------------

public abstract class KoreGeoFeature
{
    public string Name { get; set; } = string.Empty;
    public string? Id { get; set; } // Optional GeoJSON Feature id (RFC 7946 Section 3.2)
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}

// --------------------------------------------------------------------------------------------
// MARK: Point
// --------------------------------------------------------------------------------------------

public class KoreGeoPoint : KoreGeoFeature
{
    public KoreLLPoint Position { get; set; }
    public string? Label { get; set; }
    public double Size { get; set; } = 5.0;
    public KoreColorRGB Color { get; set; } = KoreColorRGB.Black;
    public KoreXYRectPosition LabelPosition { get; set; } = KoreXYRectPosition.TopRight;
    public int LabelFontSize { get; set; } = 12;
}

// --------------------------------------------------------------------------------------------
// MARK: MultiPoint
// --------------------------------------------------------------------------------------------

public class KoreGeoMultiPoint : KoreGeoFeature
{
    public List<KoreLLPoint> Points { get; set; } = new List<KoreLLPoint>();
    public double Size { get; set; } = 5.0;
    public KoreColorRGB Color { get; set; } = KoreColorRGB.Black;
    public KoreLLBox? BoundingBox { get; private set; }

    public void CalcBoundingBox()
    {
        BoundingBox = Points.Count > 0 ? KoreLLBox.FromList(Points) : null;
    }
}

// --------------------------------------------------------------------------------------------
// MARK: LineString
// --------------------------------------------------------------------------------------------

public class KoreGeoLineString : KoreGeoFeature
{
    public List<KoreLLPoint> Points { get; set; } = new List<KoreLLPoint>();
    public double LineWidth { get; set; } = 1.0;
    public KoreColorRGB Color { get; set; } = KoreColorRGB.Black;
    public bool IsGreatCircle { get; set; } = false; // Future: curved vs straight
    public KoreLLBox? BoundingBox { get; private set; }

    public void CalcBoundingBox()
    {
        BoundingBox = Points.Count > 0 ? KoreLLBox.FromList(Points) : null;
    }
}

// --------------------------------------------------------------------------------------------
// MARK: MultiLineString
// --------------------------------------------------------------------------------------------

public class KoreGeoMultiLineString : KoreGeoFeature
{
    public List<List<KoreLLPoint>> LineStrings { get; set; } = new List<List<KoreLLPoint>>();
    public double LineWidth { get; set; } = 1.0;
    public KoreColorRGB Color { get; set; } = KoreColorRGB.Black;
    public bool IsGreatCircle { get; set; } = false;
    public KoreLLBox? BoundingBox { get; private set; }

    public void CalcBoundingBox()
    {
        var allPoints = new List<KoreLLPoint>();
        foreach (var line in LineStrings)
        {
            allPoints.AddRange(line);
        }
        BoundingBox = allPoints.Count > 0 ? KoreLLBox.FromList(allPoints) : null;
    }
}

// --------------------------------------------------------------------------------------------
// MARK: Polygon
// --------------------------------------------------------------------------------------------

public class KoreGeoPolygon : KoreGeoFeature
{
    public List<KoreLLPoint> OuterRing { get; set; } = new List<KoreLLPoint>();
    public List<List<KoreLLPoint>> InnerRings { get; set; } = new List<List<KoreLLPoint>>(); // Holes
    public KoreColorRGB? FillColor { get; set; }
    public KoreColorRGB? StrokeColor { get; set; }
    public double StrokeWidth { get; set; } = 1.0;
    public KoreLLBox? BoundingBox { get; private set; }

    public void CalcBoundingBox()
    {
        var allPoints = new List<KoreLLPoint>();
        allPoints.AddRange(OuterRing);
        foreach (var innerRing in InnerRings)
        {
            allPoints.AddRange(innerRing);
        }
        BoundingBox = allPoints.Count > 0 ? KoreLLBox.FromList(allPoints) : null;
    }
}

// --------------------------------------------------------------------------------------------
// MARK: MultiPolygon
// --------------------------------------------------------------------------------------------

public class KoreGeoMultiPolygon : KoreGeoFeature
{
    public List<KoreGeoPolygon> Polygons { get; set; } = new List<KoreGeoPolygon>();
    public KoreColorRGB? FillColor { get; set; }
    public KoreColorRGB? StrokeColor { get; set; }
    public double StrokeWidth { get; set; } = 1.0;
    public KoreLLBox? BoundingBox { get; private set; }

    public void CalcBoundingBox()
    {
        var allPoints = new List<KoreLLPoint>();
        foreach (var polygon in Polygons)
        {
            allPoints.AddRange(polygon.OuterRing);
            foreach (var innerRing in polygon.InnerRings)
            {
                allPoints.AddRange(innerRing);
            }
        }
        BoundingBox = allPoints.Count > 0 ? KoreLLBox.FromList(allPoints) : null;
    }
}

// --------------------------------------------------------------------------------------------
// MARK: Circle
// --------------------------------------------------------------------------------------------

public class KoreGeoCircle : KoreGeoFeature
{
    public KoreLLPoint Center { get; set; }
    public double RadiusMeters { get; set; }
    public KoreColorRGB? FillColor { get; set; }
    public KoreColorRGB? StrokeColor { get; set; }
    public double StrokeWidth { get; set; } = 1.0;
}

// --------------------------------------------------------------------------------------------
// MARK: Feature Collection
// --------------------------------------------------------------------------------------------

public class KoreGeoFeatureCollection
{
    public List<KoreGeoFeature> Features { get; set; } = new List<KoreGeoFeature>();
    public KoreLLBox? BoundingBox { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}
