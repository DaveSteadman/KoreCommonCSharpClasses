// <fileheader>

#nullable enable

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace KoreCommon;

/// <summary>
/// GeoJSON MultiPolygon feature import/export for KoreGeoFeatureLibrary
/// </summary>
public partial class KoreGeoFeatureLibrary
{
    // ----------------------------------------------------------------------------------------
    // MARK: MultiPolygon Feature Import/Export
    // ----------------------------------------------------------------------------------------

    private void ImportMultiPolygonFeature(JsonElement featureElement, JsonElement geometryElement)
    {
        if (!geometryElement.TryGetProperty("coordinates", out var coordinatesElement) || coordinatesElement.ValueKind != JsonValueKind.Array)
            return;

        var multiPolygon = new KoreGeoMultiPolygon();

        foreach (var polygonCoords in coordinatesElement.EnumerateArray())
        {
            if (polygonCoords.ValueKind != JsonValueKind.Array)
                continue;

            var polygon = new KoreGeoPolygon();
            var ringIndex = 0;

            foreach (var ringElement in polygonCoords.EnumerateArray())
            {
                if (ringElement.ValueKind != JsonValueKind.Array)
                    continue;

                var ring = new List<KoreLLPoint>();

                foreach (var coordElement in ringElement.EnumerateArray())
                {
                    if (coordElement.ValueKind != JsonValueKind.Array)
                        continue;

                    var coordEnumerator = coordElement.EnumerateArray();
                    if (!coordEnumerator.MoveNext())
                        continue;
                    var lon = coordEnumerator.Current.GetDouble();

                    if (!coordEnumerator.MoveNext())
                        continue;
                    var lat = coordEnumerator.Current.GetDouble();

                    ring.Add(new KoreLLPoint
                    {
                        LonDegs = lon,
                        LatDegs = lat
                    });
                }

                if (ring.Count < 3)
                    continue;

                if (ringIndex == 0)
                {
                    polygon.OuterRing = ring;
                }
                else
                {
                    polygon.InnerRings.Add(ring);
                }

                ringIndex++;
            }

            if (polygon.OuterRing.Count < 3)
                continue;

            multiPolygon.Polygons.Add(polygon);
        }

        if (multiPolygon.Polygons.Count == 0)
            return;

        // Load optional id field (RFC 7946 Section 3.2)
        PopulateFeatureId(multiPolygon, featureElement);

        if (featureElement.TryGetProperty("properties", out var propertiesElement) && propertiesElement.ValueKind == JsonValueKind.Object)
        {
            PopulateFeatureProperties(multiPolygon, propertiesElement);

            if (propertiesElement.TryGetProperty("fillColor", out var fillColorElement) && fillColorElement.ValueKind == JsonValueKind.String)
            {
                var fillColorStr = fillColorElement.GetString();
                if (!string.IsNullOrWhiteSpace(fillColorStr))
                {
                    multiPolygon.FillColor = KoreColorIO.HexStringToRGB(fillColorStr);
                }
            }

            if (propertiesElement.TryGetProperty("strokeColor", out var strokeColorElement) && strokeColorElement.ValueKind == JsonValueKind.String)
            {
                var strokeColorStr = strokeColorElement.GetString();
                if (!string.IsNullOrWhiteSpace(strokeColorStr))
                {
                    multiPolygon.StrokeColor = KoreColorIO.HexStringToRGB(strokeColorStr);
                }
            }

            if (propertiesElement.TryGetProperty("strokeWidth", out var strokeWidthElement) && strokeWidthElement.ValueKind == JsonValueKind.Number)
            {
                multiPolygon.StrokeWidth = strokeWidthElement.GetDouble();
            }
        }

        var rawName = multiPolygon.Properties.TryGetValue("name", out var storedNameObj) ? storedNameObj?.ToString() : null;
        multiPolygon.Name = GenerateUniqueName(string.IsNullOrWhiteSpace(rawName) ? "MultiPolygon" : rawName!);
        multiPolygon.Properties["name"] = multiPolygon.Name;

        AddFeature(multiPolygon);
    }

    // ----------------------------------------------------------------------------------------

    private Dictionary<string, object?> BuildMultiPolygonProperties(KoreGeoMultiPolygon multiPolygon)
    {
        var properties = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = multiPolygon.Name
        };

        if (multiPolygon.StrokeWidth != 1.0)
        {
            properties["strokeWidth"] = multiPolygon.StrokeWidth;
        }

        if (multiPolygon.FillColor.HasValue)
        {
            properties["fillColor"] = KoreColorIO.RBGtoHexString(multiPolygon.FillColor.Value);
        }

        if (multiPolygon.StrokeColor.HasValue)
        {
            properties["strokeColor"] = KoreColorIO.RBGtoHexString(multiPolygon.StrokeColor.Value);
        }

        foreach (var kvp in multiPolygon.Properties)
        {
            if (string.Equals(kvp.Key, "name", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kvp.Key, "strokeWidth", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kvp.Key, "fillColor", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kvp.Key, "strokeColor", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (TryConvertPropertyValue(kvp.Value, out var convertedValue))
            {
                properties[kvp.Key] = convertedValue;
            }
        }

        return properties;
    }
}
