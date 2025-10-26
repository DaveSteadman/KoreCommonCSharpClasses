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
    // --------------------------------------------------------------------------------------------
    // MARK: Get Features
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Get any feature by name
    /// </summary>
    public KoreGeoFeature? GetFeature(string name)
    {
        features.TryGetValue(name, out var feature);
        return feature;
    }

    /// <summary>
    /// Get a point feature by name
    /// </summary>
    public KoreGeoPoint? GetPoint(string name)
    {
        points.TryGetValue(name, out var point);
        return point;
    }

    /// <summary>
    /// Get a line feature by name
    /// </summary>
    public KoreGeoLine? GetLine(string name)
    {
        lines.TryGetValue(name, out var line);
        return line;
    }

    /// <summary>
    /// Get a polygon feature by name
    /// </summary>
    public KoreGeoPolygon? GetPolygon(string name)
    {
        polygons.TryGetValue(name, out var polygon);
        return polygon;
    }

    /// <summary>
    /// Get a circle feature by name
    /// </summary>
    public KoreGeoCircle? GetCircle(string name)
    {
        circles.TryGetValue(name, out var circle);
        return circle;
    }

    /// <summary>
    /// Get all features
    /// </summary>
    public IEnumerable<KoreGeoFeature> GetAllFeatures()
    {
        return features.Values;
    }

    /// <summary>
    /// Get all points
    /// </summary>
    public IEnumerable<KoreGeoPoint> GetAllPoints()
    {
        return points.Values;
    }

    /// <summary>
    /// Get all lines
    /// </summary>
    public IEnumerable<KoreGeoLine> GetAllLines()
    {
        return lines.Values;
    }

    /// <summary>
    /// Get all polygons
    /// </summary>
    public IEnumerable<KoreGeoPolygon> GetAllPolygons()
    {
        return polygons.Values;
    }

    /// <summary>
    /// Get all circles
    /// </summary>
    public IEnumerable<KoreGeoCircle> GetAllCircles()
    {
        return circles.Values;
    }

    /// <summary>
    /// Get all features that have a specific property defined (property exists and is not null/empty)
    /// </summary>
    public IEnumerable<KoreGeoFeature> GetFeaturesWithProperty(string propertyName)
    {
        return features.Values.Where(f =>
            f.Properties.ContainsKey(propertyName) &&
            f.Properties[propertyName] != null &&
            f.Properties[propertyName].ToString() != string.Empty);
    }

    /// <summary>
    /// Get all features where a property matches a specific value
    /// </summary>
    public IEnumerable<KoreGeoFeature> GetFeaturesWithPropertyValue(string propertyName, object value)
    {
        return features.Values.Where(f =>
            f.Properties.ContainsKey(propertyName) &&
            f.Properties[propertyName] != null &&
            f.Properties[propertyName].Equals(value));
    }

}
