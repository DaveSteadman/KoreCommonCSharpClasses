using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

// Functions to serialize and deserialize GloMeshData to/from JSON format.
// Note that some elements are stored in custom string formats, prioritizing human-readability over strict JSON compliance.
// - If a user is after a higher performance serialization, they should use the binary format instead of text.

public static partial class GloMeshDataIO
{
    // --------------------------------------------------------------------------------------------
    // MARK: ToJson
    // --------------------------------------------------------------------------------------------

    // Save GloMeshData to JSON (triangles as 3 points, lines as native structure)
    public static string ToJson(GloMeshData mesh)
    {
        var obj = new
        {
            name           = GloStringOperations.WhitelistString(mesh.Name),
            vertices       = mesh.Vertices,
            lines          = mesh.Lines,
            triangles      = mesh.Triangles,
            normals        = mesh.Normals,
            uvs            = mesh.UVs,
            vertexColors   = mesh.VertexColors,
            lineColors     = mesh.LineColors,
            triangleColors = mesh.TriangleColors,
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = {
                new Vector3Converter(),
                new Vector2Converter(),
                new ColorConverter(),
                new TriangleConverter(),
                new LineConverter(),
            }
        };
        return JsonSerializer.Serialize(obj, options);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: FromJson
    // --------------------------------------------------------------------------------------------

    // Load GloMeshData from JSON (optimistic: ignore unknowns, default missing)
    public static GloMeshData FromJson(string json)
    {
        var mesh = new GloMeshData();
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        mesh.Name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";

        // --- Vertices ---
        var vertexList = new List<GloXYZVector>();
        if (root.TryGetProperty("vertices", out var vertsProp) && vertsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var v in vertsProp.EnumerateArray())
                vertexList.Add(Vector3Converter.ReadVector3(v));
        }
        mesh.Vertices = vertexList;

        // --- Lines ---
        mesh.Lines = new List<(int, int)>();
        if (root.TryGetProperty("lines", out var linesProp) && linesProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var l in linesProp.EnumerateArray())
                mesh.Lines.Add(LineConverter.ReadLineTuple(l));
        }

        // --- Triangles ---
        mesh.Triangles = new List<(int, int, int)>();
        if (root.TryGetProperty("triangles", out var trisProp) && trisProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var tri in trisProp.EnumerateArray())
            {
                int a = tri[0].GetInt32();
                int b = tri[1].GetInt32();
                int c = tri[2].GetInt32();
                mesh.Triangles.Add((a, b, c));
            }
        }

        // --- Normals ---
        mesh.Normals = new List<GloXYZVector>();
        if (root.TryGetProperty("normals", out var normalsProp) && normalsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var n in normalsProp.EnumerateArray())
                mesh.Normals.Add(Vector3Converter.ReadVector3(n));
        }

        // --- UVs ---
        mesh.UVs = new List<GloXYVector>();
        if (root.TryGetProperty("uvs", out var uvsProp) && uvsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var uv in uvsProp.EnumerateArray())
                mesh.UVs.Add(Vector2Converter.ReadVector2(uv));
        }

        // --- VertexColors ---
        mesh.VertexColors = new List<GloColorRGB>();
        if (root.TryGetProperty("vertexColors", out var colorsProp) && colorsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var c in colorsProp.EnumerateArray())
                mesh.VertexColors.Add(ColorConverter.ReadColor(c));
        }

        // --- LineColors ---
        mesh.LineColors = new List<GloColorRGB>();
        if (root.TryGetProperty("lineColors", out var lineColorsProp) && lineColorsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var c in lineColorsProp.EnumerateArray())
                mesh.LineColors.Add(ColorConverter.ReadColor(c));
        }

        // --- TriangleColors ---
        mesh.TriangleColors = new List<GloColorRGB>();
        if (root.TryGetProperty("triangleColors", out var triangleColorsProp) && triangleColorsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var c in triangleColorsProp.EnumerateArray())
                mesh.TriangleColors.Add(ColorConverter.ReadColor(c));
        }

        return mesh;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Converters
    // --------------------------------------------------------------------------------------------

    private class Vector3Converter : JsonConverter<GloXYZVector>
    {
        public override GloXYZVector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadVector3(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, GloXYZVector value, JsonSerializerOptions options)
        {
            string str = GloXYZVectorIO.ToString(value);
            writer.WriteStringValue(str);
        }

        public static GloXYZVector ReadVector3(JsonElement el)
        {
            // if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() == 3)
            //     return new GloXYZVector(el[0].GetSingle(), el[1].GetSingle(), el[2].GetSingle());
            // return GloXYZVector.Zero;

            string str = el.GetString() ?? "";
            if (!string.IsNullOrEmpty(str))
            {
                return GloXYZVectorIO.FromString(str);
            }

            return GloXYZVector.Zero;
        }
    }

    // --------------------------------------------------------------------------------------------

    private class Vector2Converter : JsonConverter<GloXYVector>
    {
        public override GloXYVector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadVector2(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, GloXYVector value, JsonSerializerOptions options)
        {
            string str = GloXYVectorIO.ToString(value);
            writer.WriteStringValue(str);
        }
        public static GloXYVector ReadVector2(JsonElement el)
        {
            string str = el.GetString() ?? "";
            if (!string.IsNullOrEmpty(str))
            {
                return GloXYVectorIO.FromString(str);
            }

            return GloXYVector.Zero;
        }
    }

    // --------------------------------------------------------------------------------------------

    private class ColorConverter : JsonConverter<GloColorRGB>
    {
        public override GloColorRGB Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadColor(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, GloColorRGB value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(GloColorIO.RBGtoHexString(value));
        }
        public static GloColorRGB ReadColor(JsonElement el)
        {
            string? hex = el.GetString();
            if (hex != null)
                return GloColorIO.HexStringToRGB(hex);
            return GloColorRGB.Zero;
        }
    }


    // --------------------------------------------------------------------------------------------
    // MARK: LineConverter
    // --------------------------------------------------------------------------------------------

    private class LineConverter : JsonConverter<(int, int)>
    {
        public override (int, int) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadLineTuple(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, (int, int) value, JsonSerializerOptions options)
        {

            string str = $"{value.Item1}, {value.Item2}";
            writer.WriteStringValue(str);
        }
        public static (int, int, GloColorRGB, GloColorRGB) ReadLineTuple(JsonElement el)
        {
            // read the string representation
            string? str = el.GetString() ?? "";

            // split by comma
            if (!string.IsNullOrEmpty(str))
            {
                var parts = str.Split(',');
                if (parts.Length != 3) throw new FormatException("Invalid GloXYZVector string format.");

                int pnt1Id = int.Parse(parts[0].Split(':')[1]);
                int pnt2Id = int.Parse(parts[1].Split(':')[1]);

                string col1Str = parts[2].Split(':')[1].Trim();
                string col2Str = parts[3].Split(':')[1].Trim();

                GloColorRGB col1 = GloColorIO.HexStringToRGB(col1Str);
                GloColorRGB col2 = GloColorIO.HexStringToRGB(col2Str);

                return (pnt1Id, pnt2Id, col1, col2);

            }
            return (0, 0, GloColorRGB.Zero, GloColorRGB.Zero);

            // if (!string.IsNullOrEmpty(str))

        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: TriangleConverter
    // --------------------------------------------------------------------------------------------

    private class TriangleConverter : JsonConverter<(int, int, int)>
    {
        public override (int, int, int) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() == 3)
            {
                // read a string representation
                string? str = doc.RootElement.GetString();

                if (!string.IsNullOrEmpty(str))
                {
                    var parts = str.Split(',');
                    if (parts.Length != 3) throw new FormatException("Invalid GloXYZVector string format.");

                    int a = int.Parse(parts[0].Split(':')[1]);
                    int b = int.Parse(parts[1].Split(':')[1]);
                    int c = int.Parse(parts[2].Split(':')[1]);

                    return (a, b, c);
                }
            }
            return (0, 0, 0);
        }

        public override void Write(Utf8JsonWriter writer, (int, int, int) value, JsonSerializerOptions options)
        {
            string str = $"{value.Item1}, {value.Item2}, {value.Item3}";
            writer.WriteStringValue(str);
        }
    }


    // --------------------------------------------------------------------------------------------
    // MARK: LineColourConverter
    // --------------------------------------------------------------------------------------------

    // --------------------------------------------------------------------------------------------
    // MARK: TriangleColourConverter
    // --------------------------------------------------------------------------------------------



}

