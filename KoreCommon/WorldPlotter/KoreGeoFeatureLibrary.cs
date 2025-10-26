// <fileheader>

using System;
using System.Collections.Generic;
using System.Linq;

namespace KoreCommon;

/// <summary>
/// Library for managing and querying geographic features
/// Provides storage, retrieval, and spatial filtering of geo features
/// </summary>
public partial class KoreGeoFeatureLibrary
{
    // Core storage - features indexed by name
    private Dictionary<string, KoreGeoFeature> features = new Dictionary<string, KoreGeoFeature>();

    // Type-specific indexes for faster querying
    private Dictionary<string, KoreGeoPoint> points = new Dictionary<string, KoreGeoPoint>();
    private Dictionary<string, KoreGeoMultiPoint> multiPoints = new Dictionary<string, KoreGeoMultiPoint>();
    private Dictionary<string, KoreGeoLine> lines = new Dictionary<string, KoreGeoLine>();
    private Dictionary<string, KoreGeoMultiLineString> multiLines = new Dictionary<string, KoreGeoMultiLineString>();
    private Dictionary<string, KoreGeoPolygon> polygons = new Dictionary<string, KoreGeoPolygon>();
    private Dictionary<string, KoreGeoCircle> circles = new Dictionary<string, KoreGeoCircle>();

    public string Name { get; set; } = string.Empty;

    // --------------------------------------------------------------------------------------------
    // MARK: Spatial Queries
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Get all points within a bounding box
    /// </summary>
    public IEnumerable<KoreGeoPoint> GetPointsInBox(KoreLLBox bounds)
    {
        return points.Values.Where(p => bounds.Contains(p.Position));
    }

    /// <summary>
    /// Get all features that intersect with a bounding box
    /// </summary>
    public IEnumerable<KoreGeoFeature> GetFeaturesInBox(KoreLLBox bounds)
    {
        var result = new List<KoreGeoFeature>();

        // Add points within bounds
        result.AddRange(GetPointsInBox(bounds));

        // Add lines with any point in bounds
        foreach (var line in lines.Values)
        {
            if (line.Points.Any(p => bounds.Contains(p)))
            {
                result.Add(line);
            }
        }

        foreach (var multiLine in multiLines.Values)
        {
            if (multiLine.LineStrings.Any(line => line.Any(p => bounds.Contains(p))))
            {
                result.Add(multiLine);
            }
        }

        // Add polygons with any point in bounds
        foreach (var polygon in polygons.Values)
        {
            if (polygon.OuterRing.Any(p => bounds.Contains(p)))
            {
                result.Add(polygon);
            }
        }

        // Add circles with center in bounds
        foreach (var circle in circles.Values)
        {
            if (bounds.Contains(circle.Center))
            {
                result.Add(circle);
            }
        }

        foreach (var multiPoint in multiPoints.Values)
        {
            if (multiPoint.Points.Any(bounds.Contains))
            {
                result.Add(multiPoint);
            }
        }

        return result;
    }

    /// <summary>
    /// Get all points within a radius (in meters) of a center point
    /// </summary>
    public IEnumerable<KoreGeoPoint> GetPointsNear(KoreLLPoint center, double radiusMeters)
    {
        // TODO: Implement proper geodetic distance calculation
        // For now, use a simple bounding box approximation
        double latDegsPerMeter = 1.0 / 111320.0;
        double radiusDegs = radiusMeters * latDegsPerMeter;

        var bounds = new KoreLLBox()
        {
            MinLatDegs = center.LatDegs - radiusDegs,
            MaxLatDegs = center.LatDegs + radiusDegs,
            MinLonDegs = center.LonDegs - radiusDegs,
            MaxLonDegs = center.LonDegs + radiusDegs
        };

        return GetPointsInBox(bounds);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Statistics
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Get the total number of features
    /// </summary>
    public int Count => features.Count;

    /// <summary>
    /// Get the number of features by type
    /// </summary>
    public (int Points, int MultiPoints, int Lines, int MultiLineStrings, int Polygons, int Circles) GetCountsByType()
    {
        return (points.Count, multiPoints.Count, lines.Count, multiLines.Count, polygons.Count, circles.Count);
    }

    /// <summary>
    /// Calculate bounding box that contains all features
    /// </summary>
    public KoreLLBox? CalculateBoundingBox()
    {
        if (features.Count == 0)
            return null;

        double minLat = double.MaxValue;
        double maxLat = double.MinValue;
        double minLon = double.MaxValue;
        double maxLon = double.MinValue;

        // Check all points
        foreach (var point in points.Values)
        {
            minLat = Math.Min(minLat, point.Position.LatDegs);
            maxLat = Math.Max(maxLat, point.Position.LatDegs);
            minLon = Math.Min(minLon, point.Position.LonDegs);
            maxLon = Math.Max(maxLon, point.Position.LonDegs);
        }

        // Check all point collections
        foreach (var multiPoint in multiPoints.Values)
        {
            foreach (var p in multiPoint.Points)
            {
                minLat = Math.Min(minLat, p.LatDegs);
                maxLat = Math.Max(maxLat, p.LatDegs);
                minLon = Math.Min(minLon, p.LonDegs);
                maxLon = Math.Max(maxLon, p.LonDegs);
            }
        }

        // Check all line points
        foreach (var line in lines.Values)
        {
            foreach (var p in line.Points)
            {
                minLat = Math.Min(minLat, p.LatDegs);
                maxLat = Math.Max(maxLat, p.LatDegs);
                minLon = Math.Min(minLon, p.LonDegs);
                maxLon = Math.Max(maxLon, p.LonDegs);
            }
        }

        foreach (var multiLine in multiLines.Values)
        {
            foreach (var line in multiLine.LineStrings)
            {
                foreach (var p in line)
                {
                    minLat = Math.Min(minLat, p.LatDegs);
                    maxLat = Math.Max(maxLat, p.LatDegs);
                    minLon = Math.Min(minLon, p.LonDegs);
                    maxLon = Math.Max(maxLon, p.LonDegs);
                }
            }
        }

        // Check all polygon points
        foreach (var polygon in polygons.Values)
        {
            foreach (var p in polygon.OuterRing)
            {
                minLat = Math.Min(minLat, p.LatDegs);
                maxLat = Math.Max(maxLat, p.LatDegs);
                minLon = Math.Min(minLon, p.LonDegs);
                maxLon = Math.Max(maxLon, p.LonDegs);
            }
        }

        // Check all circle centers
        foreach (var circle in circles.Values)
        {
            minLat = Math.Min(minLat, circle.Center.LatDegs);
            maxLat = Math.Max(maxLat, circle.Center.LatDegs);
            minLon = Math.Min(minLon, circle.Center.LonDegs);
            maxLon = Math.Max(maxLon, circle.Center.LonDegs);
        }

        return new KoreLLBox()
        {
            MinLatDegs = minLat,
            MaxLatDegs = maxLat,
            MinLonDegs = minLon,
            MaxLonDegs = maxLon
        };
    }

}
