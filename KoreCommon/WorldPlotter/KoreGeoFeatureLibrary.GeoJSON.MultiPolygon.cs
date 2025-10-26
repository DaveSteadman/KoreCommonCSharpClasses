using System.Text.Json;
using KoreCommon;

namespace KoreCommon
{
    public partial class KoreGeoFeatureLibrary
    {
        // MARK: MultiPolygon Feature Import/Export
        // ----------------------------------------------------------------------------------------

        private void ImportMultiPolygonFeature(JsonElement featureElement, JsonElement geometryElement)
        {
            if (!geometryElement.TryGetProperty("coordinates", out var coordinatesElement) || coordinatesElement.ValueKind != JsonValueKind.Array)
                return;

            // MultiPolygon coordinates: [ [[[outer], [hole1], ...]], [[[outer2], ...]], ... ]
            // Each element is a complete Polygon (outer ring + optional holes)

            foreach (var polygonCoords in coordinatesElement.EnumerateArray())
            {
                if (polygonCoords.ValueKind != JsonValueKind.Array)
                    continue;

                var polygon = new KoreGeoPolygon();

                int ringIndex = 0;
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
                        // First ring is the outer boundary
                        polygon.OuterRing = ring;
                    }
                    else
                    {
                        // Subsequent rings are holes
                        polygon.InnerRings.Add(ring);
                    }

                    ringIndex++;
                }

                if (polygon.OuterRing.Count < 3)
                    continue;

                // Load properties (shared across all polygons in MultiPolygon for now)
                if (featureElement.TryGetProperty("properties", out var propertiesElement) && propertiesElement.ValueKind == JsonValueKind.Object)
                {
                    PopulateFeatureProperties(polygon, propertiesElement);
                }

                var rawName = polygon.Properties.TryGetValue("name", out var storedNameObj) ? storedNameObj?.ToString() : null;
                polygon.Name = GenerateUniqueName(string.IsNullOrWhiteSpace(rawName) ? "Polygon" : rawName!);

                // Ensure the feature dictionary reflects the final name
                polygon.Properties["name"] = polygon.Name;

                AddFeature(polygon);
            }
        }

        // Note: For exporting, we treat each polygon separately for now
        // In future, we could group polygons with same properties into a single MultiPolygon feature
    }
}
