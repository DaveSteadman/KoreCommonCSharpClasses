// <fileheader>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace KoreCommon;

/// <summary>
/// Library for managing and querying geographic features
/// Provides storage, retrieval, and spatial filtering of geo features
/// </summary>
public partial class KoreGeoFeatureLibrary
{
    // --------------------------------------------------------------------------------------------
    // MARK: GeoJSON I/O
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Load features from a GeoJSON file
    /// </summary>
    public void LoadFromGeoJSON(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("GeoJSON file path cannot be null or empty", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("GeoJSON file not found", filePath);

        var geoJsonText = File.ReadAllText(filePath);
        ImportGeoJSON(geoJsonText);
    }

    /// <summary>
    /// Save features to a GeoJSON file
    /// </summary>
    public void SaveToGeoJSON(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("GeoJSON file path cannot be null or empty", nameof(filePath));

        var geoJsonText = ExportToGeoJSON();
        File.WriteAllText(filePath, geoJsonText);
    }

    /// <summary>
    /// Import from a GeoJSON string
    /// </summary>
    public void ImportGeoJSON(string geoJsonString)
    {
        if (string.IsNullOrWhiteSpace(geoJsonString))
            throw new ArgumentException("GeoJSON string cannot be null or empty", nameof(geoJsonString));

        using var document = JsonDocument.Parse(geoJsonString);
        var root = document.RootElement;

        var type = GetStringCaseInsensitive(root, "type");
        if (string.Equals(type, "FeatureCollection", StringComparison.OrdinalIgnoreCase))
        {
            if (!root.TryGetProperty("features", out var featuresElement) || featuresElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException("GeoJSON FeatureCollection is missing a features array");

            foreach (var featureElement in featuresElement.EnumerateArray())
            {
                TryImportFeature(featureElement);
            }
        }
        else if (string.Equals(type, "Feature", StringComparison.OrdinalIgnoreCase))
        {
            TryImportFeature(root);
        }
        else
        {
            throw new NotSupportedException($"GeoJSON type '{type}' is not supported. Only FeatureCollection and Feature documents are supported.");
        }
    }

    /// <summary>
    /// Export to a GeoJSON string
    /// </summary>
    public string ExportToGeoJSON()
    {
        var allFeatures = new List<object>();

        // Export all points
        foreach (var point in GetAllPoints())
        {
            var properties = BuildPointProperties(point);

            allFeatures.Add(new
            {
                type = "Feature",
                properties,
                geometry = new
                {
                    type = "Point",
                    coordinates = new[] { point.Position.LonDegs, point.Position.LatDegs }
                }
            });
        }

        // Export all lines
        foreach (var line in GetAllLines())
        {
            var properties = BuildLineProperties(line);

            allFeatures.Add(new
            {
                type = "Feature",
                properties,
                geometry = new
                {
                    type = "LineString",
                    coordinates = line.Points.ConvertAll(p => new[] { p.LonDegs, p.LatDegs })
                }
            });
        }

        // Export all polygons
        foreach (var polygon in GetAllPolygons())
        {
            var properties = BuildPolygonProperties(polygon);

            // Build rings array: [outerRing, hole1, hole2, ...]
            var rings = new List<List<double[]>>();

            // Outer ring
            rings.Add(polygon.OuterRing.ConvertAll(p => new[] { p.LonDegs, p.LatDegs }));

            // Inner rings (holes)
            foreach (var innerRing in polygon.InnerRings)
            {
                rings.Add(innerRing.ConvertAll(p => new[] { p.LonDegs, p.LatDegs }));
            }

            allFeatures.Add(new
            {
                type = "Feature",
                properties,
                geometry = new
                {
                    type = "Polygon",
                    coordinates = rings
                }
            });
        }

        var featureCollection = new
        {
            type = "FeatureCollection",
            features = allFeatures
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        return JsonSerializer.Serialize(featureCollection, options);
    }

    // ----------------------------------------------------------------------------------------
    // MARK: Generic Helpers (Current support: Points, Lines)
    // ----------------------------------------------------------------------------------------

    private void TryImportFeature(JsonElement featureElement)
    {
        if (featureElement.ValueKind != JsonValueKind.Object)
            return;

        var featureType = GetStringCaseInsensitive(featureElement, "type");
        if (!string.Equals(featureType, "Feature", StringComparison.OrdinalIgnoreCase))
            return;

        if (!featureElement.TryGetProperty("geometry", out var geometryElement) || geometryElement.ValueKind != JsonValueKind.Object)
            return;

        var geometryType = GetStringCaseInsensitive(geometryElement, "type");
        switch (geometryType)
        {
            case "Point":
                ImportPointFeature(featureElement, geometryElement);
                break;
            case "LineString":
                ImportLineFeature(featureElement, geometryElement);
                break;
            case "Polygon":
                ImportPolygonFeature(featureElement, geometryElement);
                break;
            case "MultiPolygon":
                ImportMultiPolygonFeature(featureElement, geometryElement);
                break;
            default:
                // Other geometry types will be supported in future iterations
                break;
        }
    }


    private void PopulateFeatureProperties(KoreGeoFeature feature, JsonElement propertiesElement)
    {
        foreach (var property in propertiesElement.EnumerateObject())
        {
            object? value = property.Value.ValueKind switch
            {
                JsonValueKind.String => property.Value.GetString(),
                JsonValueKind.Number => property.Value.TryGetInt64(out var longValue)
                    ? longValue
                    : property.Value.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => property.Value.GetRawText()
            };

            if (value is not null)
            {
                feature.Properties[property.Name] = value;
            }
        }
    }

    private bool TryConvertPropertyValue(object? value, out object? converted)
    {
        switch (value)
        {
            case null:
                converted = null;
                return true;
            case string s:
                converted = s;
                return true;
            case bool b:
                converted = b;
                return true;
            case int i:
            case long l:
            case short sh:
            case byte by:
            case uint ui:
            case ulong ul:
            case ushort us:
                converted = Convert.ToInt64(value, CultureInfo.InvariantCulture);
                return true;
            case float f:
            case double d:
            case decimal m:
                converted = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return true;
            default:
                converted = value.ToString();
                return !string.IsNullOrEmpty(converted as string);
        }
    }

    private string GenerateUniqueName(string baseName)
    {
        var candidate = baseName.Trim();
        if (string.IsNullOrEmpty(candidate))
        {
            candidate = "Feature";
        }

        if (GetFeature(candidate) is null)
        {
            return candidate;
        }

        var suffix = 2;
        var uniqueCandidate = candidate;
        while (GetFeature(uniqueCandidate) is not null)
        {
            uniqueCandidate = $"{candidate}_{suffix}";
            suffix++;
        }

        return uniqueCandidate;
    }

    private static string? GetStringCaseInsensitive(JsonElement element, string propertyName)
    {
        if (element.ValueKind != JsonValueKind.Object)
            return null;

        if (element.TryGetProperty(propertyName, out var directProperty))
            return directProperty.ValueKind == JsonValueKind.String ? directProperty.GetString() : null;

        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase) && property.Value.ValueKind == JsonValueKind.String)
            {
                return property.Value.GetString();
            }
        }

        return null;
    }
}
