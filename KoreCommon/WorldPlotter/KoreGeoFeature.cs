// <fileheader>

using System.Collections.Generic;

namespace KoreCommon;

/// <summary>
/// Base class for all geographic features that can be drawn on a world map
/// </summary>
public abstract class KoreGeoFeature
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}

// --------------------------------------------------------------------------------------------
// MARK: Point
// --------------------------------------------------------------------------------------------

/// <summary>
/// A single geographic point with optional label
/// </summary>
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
// MARK: Line
// --------------------------------------------------------------------------------------------

/// <summary>
/// A line or path through multiple geographic points
/// </summary>
public class KoreGeoLine : KoreGeoFeature
{
    public List<KoreLLPoint> Points { get; set; } = new List<KoreLLPoint>();
    public double LineWidth { get; set; } = 1.0;
    public KoreColorRGB Color { get; set; } = KoreColorRGB.Black;
    public bool IsGreatCircle { get; set; } = false; // Future: curved vs straight
}

// --------------------------------------------------------------------------------------------
// MARK: Polygon
// --------------------------------------------------------------------------------------------

/// <summary>
/// A filled polygon area with optional holes (like islands with lakes)
/// </summary>
public class KoreGeoPolygon : KoreGeoFeature
{
    public List<KoreLLPoint> OuterRing { get; set; } = new List<KoreLLPoint>();
    public List<List<KoreLLPoint>> InnerRings { get; set; } = new List<List<KoreLLPoint>>(); // Holes
    public KoreColorRGB? FillColor { get; set; }
    public KoreColorRGB? StrokeColor { get; set; }
    public double StrokeWidth { get; set; } = 1.0;
}

// --------------------------------------------------------------------------------------------
// MARK: Circle
// --------------------------------------------------------------------------------------------

/// <summary>
/// A circular coverage area (radius in meters)
/// </summary>
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

/// <summary>
/// A collection of geographic features with optional bounding box
/// </summary>
public class KoreGeoFeatureCollection
{
    public List<KoreGeoFeature> Features { get; set; } = new List<KoreGeoFeature>();
    public KoreLLBox? BoundingBox { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}
