using System.Collections.Generic;

public partial class GloMeshData
{
    public string Name;

    // List of vertices
    public List<GloXYZVector> Vertices;

    // List of lines, the colours for each point - for a wireframe mesh
    public List<(int, int, GloColorRGB, GloColorRGB)> Lines;

    // List of triangles
    public List<(int, int, int)> Triangles;

    // List of normal per vertex for a solid mesh
    public List<GloXYZVector> Normals;

    // List of UV coordinates per vertex - for any texture wrapping
    public List<GloXYVector> UVs;

    // List of vertex colors - for a solid/coloured mesh
    public List<GloColorRGB> VertexColors;

    // public List<(int, Color, Color)>      LineColors;   // List of line ids, start and end colors
    // public List<(int, Color)>             TriangleColors; // List of triangle ids and colors

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Empty constructor
    public GloMeshData()
    {
        this.Name         = string.Empty;
        this.Vertices     = new List<GloXYZVector>();
        this.Lines        = new List<(int, int, GloColorRGB, GloColorRGB)>();
        this.Triangles    = new List<(int, int, int)>();
        this.Normals      = new List<GloXYZVector>();
        this.UVs          = new List<GloXYVector>();
        this.VertexColors = new List<GloColorRGB>();
    }

    // Copy constructor
    public GloMeshData(
        string                    inName,
        List<GloXYZVector>        vertices,
        List<(int, int, GloColorRGB, GloColorRGB)> lines,
        List<(int, int, int)>     triangles,
        List<GloXYZVector>        normals,
        List<GloXYVector>         uvs,
        List<GloColorRGB>         vertexColors)
    {
        this.Name         = inName;
        this.Vertices     = vertices;
        this.Lines        = lines;
        this.Triangles    = triangles;
        this.Normals      = normals;
        this.UVs          = uvs;
        this.VertexColors = vertexColors;
    }

    // Copy constructor
    public GloMeshData(GloMeshData mesh)
    {
        this.Name         = mesh.Name;
        this.Vertices     = new List<GloXYZVector>(mesh.Vertices);
        this.Lines        = new List<(int, int, GloColorRGB, GloColorRGB)>(mesh.Lines);
        this.Triangles    = new List<(int, int, int)>(mesh.Triangles);
        this.Normals      = new List<GloXYZVector>(mesh.Normals);
        this.UVs          = new List<GloXYVector>(mesh.UVs);
        this.VertexColors = new List<GloColorRGB>(mesh.VertexColors);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Basic Data Entry Functions
    // --------------------------------------------------------------------------------------------

    // Not the exclusive way to add data, but helper routines.

    public int AddPoint(GloXYZVector vertex, GloXYZVector? normal = null, GloColorRGB? color = null)
    {
        int index = Vertices.Count;
        Vertices.Add(vertex);

        // Add a default "up" normal if one is not provided.
        if (normal.HasValue)
            Normals.Add(normal.Value);
        else
            Normals.Add(new GloXYZVector(0, 1, 0));

        // Use provided color or default to White.
        VertexColors.Add(color ?? new GloColorRGB(1, 1, 1));

        return index;
    }

    // --------------------------------------------------------------------------------------------

    public void AddLine(int idxA, int idxB, GloColorRGB colStart, GloColorRGB colEnd)
    {
        Lines.Add((idxA, idxB, colStart, colEnd));
    }

    public void AddLine(GloXYZVector start, GloXYZVector end, GloColorRGB colStart, GloColorRGB colEnd)
    {
        int idxA = AddPoint(start, null, colStart);
        int idxB = AddPoint(end, null, colEnd);

        AddLine(idxA, idxB, colStart, colEnd);
    }

    // --------------------------------------------------------------------------------------------

    public void AddDottedLine(
        GloXYZVector start, GloXYZVector end,
        GloColorRGB colStart, GloColorRGB colEnd,
        int numDots)
    {
        GloXYZVector direction = (end - start).Normalize();
        double length = (end - start).Magnitude;
        double step   = length / (numDots + 1);

        for (int i = 0; i < numDots; i++)
        {
            GloXYZVector point = start + direction * step * (i + 1);
            AddPoint(point, null, colStart);
        }

        AddLine(start, end, colStart, colEnd);
    }

    // --------------------------------------------------------------------------------------------

    public void AddTriangle(int idxA, int idxB, int idxC)
    {
        Triangles.Add((idxA, idxB, idxC));
    }

    public void AddTriangle(GloXYZVector a, GloXYZVector b, GloXYZVector c, GloColorRGB? color = null)
    {
        int idxA = AddPoint(a);
        int idxB = AddPoint(b);
        int idxC = AddPoint(c);

        if (color.HasValue)
        {
            VertexColors[idxA] = color.Value;
            VertexColors[idxB] = color.Value;
            VertexColors[idxC] = color.Value;
        }

        GloColorRGB lineColor = color ?? new GloColorRGB(1, 1, 1);
        AddLine(idxA, idxB, lineColor, lineColor);
        AddLine(idxB, idxC, lineColor, lineColor);
        AddLine(idxC, idxA, lineColor, lineColor);

        // Add the triangle to the list
        Triangles.Add((idxA, idxB, idxC));
    }

    // --------------------------------------------------------------------------------------------

    public void AddNormal(GloXYZVector normal)
    {
        Normals.Add(normal);
    }
    public void AddUV(GloXYVector uv)
    {
        UVs.Add(uv);
    }
    public void AddColor(GloColorRGB color)
    {
        VertexColors.Add(color);
    }

}
