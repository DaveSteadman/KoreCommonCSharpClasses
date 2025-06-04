using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            name         = GloStringOperations.WhitelistString(mesh.Name),
            vertices     = mesh.Vertices,
            lines        = mesh.Lines, // (int, int, Color, Color)
            triangles    = mesh.Triangles,
            normals      = mesh.Normals,
            uvs          = mesh.UVs,
            vertexColors = mesh.VertexColors
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters    = { new Vector3Converter(), new Vector2Converter(), new ColorConverter(), new LineTupleConverter() }
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

        // Vertices
        var vertexList = new List<GloXYZVector>();
        if (root.TryGetProperty("vertices", out var vertsProp) && vertsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var v in vertsProp.EnumerateArray())
                vertexList.Add(Vector3Converter.ReadVector3(v));
        }
        mesh.Vertices = vertexList;

        // Lines
        mesh.Lines = new List<(int, int, GloColorRGB, GloColorRGB)>();
        if (root.TryGetProperty("lines", out var linesProp) && linesProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var l in linesProp.EnumerateArray())
                mesh.Lines.Add(LineTupleConverter.ReadLineTuple(l));
        }

        // Triangles (as sets of 3 points)
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

        // Normals
        mesh.Normals = new List<GloXYZVector>();
        if (root.TryGetProperty("normals", out var normalsProp) && normalsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var n in normalsProp.EnumerateArray())
                mesh.Normals.Add(Vector3Converter.ReadVector3(n));
        }

        // UVs
        mesh.UVs = new List<GloXYVector>();
        if (root.TryGetProperty("uvs", out var uvsProp) && uvsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var uv in uvsProp.EnumerateArray())
                mesh.UVs.Add(Vector2Converter.ReadVector2(uv));
        }

        // VertexColors
        mesh.VertexColors = new List<GloColorRGB>();
        if (root.TryGetProperty("vertexColors", out var colorsProp) && colorsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var c in colorsProp.EnumerateArray())
                mesh.VertexColors.Add(ColorConverter.ReadColor(c));
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

            // writer.WriteStartArray();
            // writer.WriteNumberValue(value.X);
            // writer.WriteNumberValue(value.Y);
            // writer.WriteNumberValue(value.Z);
            // writer.WriteEndArray();
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

    private class LineTupleConverter : JsonConverter<(int, int, GloColorRGB, GloColorRGB)>
    {
        public override (int, int, GloColorRGB, GloColorRGB) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadLineTuple(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, (int, int, GloColorRGB, GloColorRGB) value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Item1);
            writer.WriteNumberValue(value.Item2);
            writer.WriteStringValue(GloColorIO.RBGtoHexString(value.Item3));
            writer.WriteStringValue(GloColorIO.RBGtoHexString(value.Item4));
            writer.WriteEndArray();
        }
        public static (int, int, GloColorRGB, GloColorRGB) ReadLineTuple(JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() == 4)
            {
                int a = el[0].GetInt32();
                int b = el[1].GetInt32();

                string? Col1Str = el[2].GetString();
                string? Col2Str = el[3].GetString();
                GloColorRGB cA = GloColorRGB.Zero;
                GloColorRGB cB = GloColorRGB.Zero;
                if (Col1Str != null) cA = GloColorIO.HexStringToRGB(Col1Str);
                if (Col2Str != null) cB = GloColorIO.HexStringToRGB(Col2Str);

                return (a, b, cA, cB);
            }
            return (0, 0, new GloColorRGB(1, 1, 1, 1), new GloColorRGB(1, 1, 1, 1));
        }
    }
}

