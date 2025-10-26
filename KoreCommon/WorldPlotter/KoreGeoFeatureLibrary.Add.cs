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
    // MARK: Add/Remove Features
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Add a feature to the library
    /// </summary>
    public void AddFeature(KoreGeoFeature feature)
    {
        if (string.IsNullOrEmpty(feature.Name))
        {
            throw new ArgumentException("Feature must have a name");
        }

        // Add to main collection
        features[feature.Name] = feature;

        // Add to type-specific indexes
        switch (feature)
        {
            case KoreGeoPoint point:
                points[feature.Name] = point;
                break;
            case KoreGeoMultiPoint multiPoint:
                multiPoints[feature.Name] = multiPoint;
                break;
            case KoreGeoLineString lineString:
                lineStrings[feature.Name] = lineString;
                break;
            case KoreGeoMultiLineString multiLine:
                multiLines[feature.Name] = multiLine;
                break;
            case KoreGeoPolygon polygon:
                polygons[feature.Name] = polygon;
                break;
            case KoreGeoMultiPolygon multiPolygon:
                multiPolygons[feature.Name] = multiPolygon;
                break;
            case KoreGeoCircle circle:
                circles[feature.Name] = circle;
                break;
        }
    }

    /// <summary>
    /// Add multiple features at once
    /// </summary>
    public void AddFeatures(IEnumerable<KoreGeoFeature> featuresToAdd)
    {
        foreach (var feature in featuresToAdd)
        {
            AddFeature(feature);
        }
    }

    /// <summary>
    /// Remove a feature by name
    /// </summary>
    public bool RemoveFeature(string name)
    {
        if (!features.TryGetValue(name, out var feature))
            return false;

        features.Remove(name);
        points.Remove(name);
        multiPoints.Remove(name);
        lineStrings.Remove(name);
        multiLines.Remove(name);
        polygons.Remove(name);
        multiPolygons.Remove(name);
        circles.Remove(name);

        return true;
    }

    /// <summary>
    /// Clear all features
    /// </summary>
    public void Clear()
    {
        features.Clear();
        points.Clear();
        multiPoints.Clear();
        lineStrings.Clear();
        multiLines.Clear();
        polygons.Clear();
        multiPolygons.Clear();
        circles.Clear();
    }


}
