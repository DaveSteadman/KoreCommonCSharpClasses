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
    static string idName         = "id";
    static string startColorName = "start";
    static string endColorName   = "end";
    static string colorName      = "color";

    // --------------------------------------------------------------------------------------------
    // MARK: ToJson
    // --------------------------------------------------------------------------------------------

    // Save GloMeshData to JSON (triangles as 3 points, lines as native structure)
    public static string ToJson(GloMeshData mesh, bool dense = false)
    {
        var obj = new
        {
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
            WriteIndented = !dense,
            AllowTrailingCommas = true,
            Converters = {
                new Vector3Converter(),
                new Vector2Converter(),
                new ColorConverter(),
                new TriangleConverter(),
                new LineConverter(),
                new GloMeshTriangleColourConverter(),
                new GloMeshLineColourConverter()
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

        // --- Vertices ---
        if (root.TryGetProperty("vertices", out var vertsProp) && vertsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var v in vertsProp.EnumerateArray())
                mesh.AddPoint(Vector3Converter.ReadVector3(v));
        }

        // --- Lines ---
        if (root.TryGetProperty("lines", out var linesProp) && linesProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var l in linesProp.EnumerateArray())
                mesh.AddLine(LineConverter.ReadLine(l));
        }

        // --- Triangles ---
        if (root.TryGetProperty("triangles", out var trisProp) && trisProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var tri in trisProp.EnumerateArray())
                mesh.AddTriangle(TriangleConverter.ReadTriangle(tri));
        }

        // --- Normals ---
        if (root.TryGetProperty("normals", out var normalsProp) && normalsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var n in normalsProp.EnumerateArray())
                mesh.AddNormal(Vector3Converter.ReadVector3(n));
        }

        // --- UVs ---
        if (root.TryGetProperty("uvs", out var uvsProp) && uvsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var uv in uvsProp.EnumerateArray())
                mesh.AddUV(Vector2Converter.ReadVector2(uv));
        }

        // --- VertexColors ---
        if (root.TryGetProperty("vertexColors", out var colorsProp) && colorsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var c in colorsProp.EnumerateArray())
                mesh.AddVertexColor(ColorConverter.ReadColor(c));
        }

        // --- LineColors ---
        if (root.TryGetProperty("lineColors", out var lineColorsProp) && lineColorsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var c in lineColorsProp.EnumerateArray())
                mesh.SetLineColor(GloMeshLineColourConverter.ReadLineColour(c));
        }

        // --- TriangleColors ---
        if (root.TryGetProperty("triangleColors", out var triangleColorsProp) && triangleColorsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var c in triangleColorsProp.EnumerateArray())
                mesh.SetTriangleColor(GloMeshTriangleColourConverter.ReadTriangleColour(c));
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
            writer.WriteStringValue(GloColorIO.RBGtoHexStringShort(value));
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

    private class LineConverter : JsonConverter<GloMeshLine>
    {
        public override GloMeshLine Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadLine(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, GloMeshLine value, JsonSerializerOptions options)
        {

            string str = $"{value.A}, {value.B}";
            writer.WriteStringValue(str);
        }
        public static GloMeshLine ReadLine(JsonElement el)
        {
            // read the string representation
            string? str = el.GetString() ?? "";

            // split by comma
            if (!string.IsNullOrEmpty(str))
            {
                var parts = str.Split(',');
                if (parts.Length < 2) throw new FormatException("Invalid GloMeshLine string format.");

                int pnt1Id = int.Parse(parts[0].Trim());
                int pnt2Id = int.Parse(parts[1].Trim());

                // If GloMeshLine has color fields, parse them here as needed.
                // For now, just use the two indices.
                return new GloMeshLine(pnt1Id, pnt2Id);
            }
            return new GloMeshLine(0, 0);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: TriangleConverter
    // --------------------------------------------------------------------------------------------

    private class TriangleConverter : JsonConverter<GloMeshTriangle>
    {
        public override GloMeshTriangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadTriangle(doc.RootElement);
        }

        public override void Write(Utf8JsonWriter writer, GloMeshTriangle value, JsonSerializerOptions options)
        {
            string str = $"{value.A}, {value.B}, {value.C}";
            writer.WriteStringValue(str);
        }

        public static GloMeshTriangle ReadTriangle(JsonElement el)
        {
            // read the string representation
            string? str = el.GetString() ?? "";

            // split by comma
            if (!string.IsNullOrEmpty(str))
            {
                var parts = str.Split(',');
                if (parts.Length != 3) throw new FormatException("Invalid GloMeshTriangle string format.");

                int a = int.Parse(parts[0]);
                int b = int.Parse(parts[1]);
                int c = int.Parse(parts[2]);

                return new GloMeshTriangle(a, b, c);
            }
            return new GloMeshTriangle(0, 0, 0);
        }
    }


    // --------------------------------------------------------------------------------------------
    // MARK: LineColourConverter
    // --------------------------------------------------------------------------------------------

    private class GloMeshLineColourConverter : JsonConverter<GloMeshLineColour>
    {
        public override GloMeshLineColour Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadLineColour(doc.RootElement);
        }

        public override void Write(Utf8JsonWriter writer, GloMeshLineColour value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{idName}: {value.Index}, {startColorName}: {GloColorIO.RBGtoHexStringShort(value.StartColor)}, {endColorName}: {GloColorIO.RBGtoHexStringShort(value.EndColor)}");
        }

        public static GloMeshLineColour ReadLineColour(JsonElement el)
        {
            // read the string representation
            string? str = el.GetString() ?? "";

            // split by comma
            if (!string.IsNullOrEmpty(str))
            {
                var parts = str.Split(',');
                if (parts.Length != 3) throw new FormatException($"Invalid GloMeshLineColour string format. {parts}.");

                int lineIndex        = int.Parse(parts[0].Split(':')[1].Trim());
                string startColorStr = parts[1].Split(':')[1].Trim();
                string endColorStr   = parts[2].Split(':')[1].Trim();

                //Console.WriteLine($"GloMeshLineColourConverter: ReadLineColour: lineIndex: {lineIndex}, startColorStr: {startColorStr}, endColorStr: {endColorStr}");

                GloColorRGB startColor = GloColorIO.HexStringToRGB(startColorStr);
                GloColorRGB endColor = GloColorIO.HexStringToRGB(endColorStr);

                return new GloMeshLineColour(lineIndex, startColor, endColor);
            }
            return new GloMeshLineColour(0, GloColorRGB.White, GloColorRGB.White);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: TriangleColourConverter
    // --------------------------------------------------------------------------------------------

    private class GloMeshTriangleColourConverter : JsonConverter<GloMeshTriangleColour>
    {
        public override GloMeshTriangleColour Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadTriangleColour(doc.RootElement);
        }

        public override void Write(Utf8JsonWriter writer, GloMeshTriangleColour value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{idName}: {value.Index}, {colorName}: {GloColorIO.RBGtoHexStringShort(value.Color)}");
        }

        public static GloMeshTriangleColour ReadTriangleColour(JsonElement el)
        {
            // read the string representation
            string? str = el.GetString() ?? "";

            // split by comma
            if (!string.IsNullOrEmpty(str))
            {
                var parts = str.Split(',');
                if (parts.Length != 2) throw new FormatException("Invalid GloMeshTriangle string format.");

                int triIndex        = int.Parse(parts[0].Split(':')[1].Trim());
                string triColorStr = parts[1].Split(':')[1].Trim();

                GloColorRGB triColor = GloColorIO.HexStringToRGB(triColorStr);

                return new GloMeshTriangleColour(triIndex, triColor);
            }
            return new GloMeshTriangleColour(0, GloColorRGB.White);
        }
    }
}

