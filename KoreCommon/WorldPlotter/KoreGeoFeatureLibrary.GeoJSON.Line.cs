// <fileheader>

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace KoreCommon;

/// <summary>
/// GeoJSON Line feature import/export for KoreGeoFeatureLibrary
/// </summary>
public partial class KoreGeoFeatureLibrary
{
    // ----------------------------------------------------------------------------------------
    // MARK: Line Feature Import/Export
    // ----------------------------------------------------------------------------------------

    private void ImportLineFeature(JsonElement featureElement, JsonElement geometryElement)
    {
        if (!geometryElement.TryGetProperty("coordinates", out var coordinatesElement) || coordinatesElement.ValueKind != JsonValueKind.Array)
            return;

        var line = new KoreGeoLine();

        // Parse coordinate array [[lon, lat], [lon, lat], ...]
        foreach (var coordElement in coordinatesElement.EnumerateArray())
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

            line.Points.Add(new KoreLLPoint
            {
                LonDegs = lon,
                LatDegs = lat
            });
        }

        // Need at least 2 points for a line
        if (line.Points.Count < 2)
            return;

        // Load properties (name, lineWidth, color, etc.)
        if (featureElement.TryGetProperty("properties", out var propertiesElement) && propertiesElement.ValueKind == JsonValueKind.Object)
        {
            PopulateFeatureProperties(line, propertiesElement);
        }

        var rawName = line.Properties.TryGetValue("name", out var storedNameObj) ? storedNameObj?.ToString() : null;
        line.Name = GenerateUniqueName(string.IsNullOrWhiteSpace(rawName) ? "Line" : rawName!);

        // Ensure the feature dictionary reflects the final name
        line.Properties["name"] = line.Name;

        AddFeature(line);
    }

    private Dictionary<string, object?> BuildLineProperties(KoreGeoLine line)
    {
        var properties = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = line.Name
        };

        // Add line-specific properties if they differ from defaults
        if (line.LineWidth != 1.0)
        {
            properties["lineWidth"] = line.LineWidth;
        }

        if (line.IsGreatCircle)
        {
            properties["isGreatCircle"] = line.IsGreatCircle;
        }

        // Include other custom properties
        foreach (var kvp in line.Properties)
        {
            if (string.Equals(kvp.Key, "name", StringComparison.OrdinalIgnoreCase))
                continue;

            if (TryConvertPropertyValue(kvp.Value, out var convertedValue))
            {
                properties[kvp.Key] = convertedValue;
            }
        }

        return properties;
    }
}
