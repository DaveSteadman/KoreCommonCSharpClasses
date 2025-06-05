using System.Collections.Generic;

#nullable enable

public record struct GloMeshLine(int A, int B);
public record struct GloMeshTriangle(int A, int B, int C);


public partial class GloMeshData
{
    public string Name;

    // List of vertices
    public List<GloXYZVector> Vertices;

    // List of lines, the colours for each point - for a wireframe mesh
    public List<GloMeshLine> Lines;

    // List of triangles
    public List<GloMeshTriangle> Triangles;

    // List of normal per vertex for a solid mesh
    // Only used if it matches the number of vertices
    public List<GloXYZVector> Normals;

    // List of UV coordinates per vertex - for any texture wrapping
    // Only used if it matches the number of vertices
    public List<GloXYVector> UVs;

    // List of vertex colors - for a solid/coloured mesh
    // Only used if it matches the number of vertices
    public List<GloColorRGB> VertexColors;

    // List of line colors - for a wireframe mesh
    public List<(int, GloColorRGB, GloColorRGB)> LineColors;

    // List of triangle colors - for a solid mesh
    public List<(int, GloColorRGB)> TriangleColors;

    // --------------------------------------------------------------------------------------------
    // MARK: Validation Properties
    // --------------------------------------------------------------------------------------------

    public bool HasValidUVCount => UVs.Count == Vertices.Count;
    public bool HasValidNormalCount => Normals.Count == Vertices.Count;

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Empty constructor
    public GloMeshData()
    {
        this.Name = string.Empty;
        this.Vertices = new List<GloXYZVector>();
        this.Lines = new List<GloMeshLine>();
        this.Triangles = new List<GloMeshTriangle>();
        this.Normals = new List<GloXYZVector>();
        this.UVs = new List<GloXYVector>();
        this.VertexColors = new List<GloColorRGB>();
        this.LineColors = new List<(int, GloColorRGB, GloColorRGB)>();
        this.TriangleColors = new List<(int, GloColorRGB)>();
    }

    // Copy constructor
    public GloMeshData(
        string inName,
        List<GloXYZVector> vertices,
        List<GloMeshLine> lines,
        List<GloMeshTriangle> triangles,
        List<GloXYZVector> normals,
        List<GloXYVector> uvs,
        List<GloColorRGB> vertexColors,
        List<(int, GloColorRGB, GloColorRGB)> lineColors,
        List<(int, GloColorRGB)> triangleColors)
    {
        this.Name = inName;
        this.Vertices = vertices;
        this.Lines = lines;
        this.Triangles = triangles;
        this.Normals = normals;
        this.UVs = uvs;
        this.VertexColors = vertexColors;
        this.LineColors = lineColors;
        this.TriangleColors = triangleColors;
    }

    // Copy constructor
    public GloMeshData(GloMeshData mesh)
    {
        this.Name = mesh.Name;
        this.Vertices = new List<GloXYZVector>(mesh.Vertices);
        this.Lines = new List<GloMeshLine>(mesh.Lines);
        this.Triangles = new List<GloMeshTriangle>(mesh.Triangles);
        this.Normals = new List<GloXYZVector>(mesh.Normals);
        this.UVs = new List<GloXYVector>(mesh.UVs);
        this.VertexColors = new List<GloColorRGB>(mesh.VertexColors);
        this.LineColors = new List<(int, GloColorRGB, GloColorRGB)>(mesh.LineColors);
        this.TriangleColors = new List<(int, GloColorRGB)>(mesh.TriangleColors);
    }

    // Initialises the mesh data with empty lists
    public void ClearAllData()
    {
        Vertices.Clear();
        Lines.Clear();
        Triangles.Clear();
        Normals.Clear();
        UVs.Clear();
        VertexColors.Clear();
        LineColors.Clear();
        TriangleColors.Clear();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Validity Operations
    // --------------------------------------------------------------------------------------------

    public void MakeValid()
    {
        CreateMatchingNormals();
        CreateMatchingUVs();
        CreateMatchingVertexColors();
    }

    public void CreateMatchingNormals(GloXYZVector? defaultNormal = null)
    {
        GloXYZVector fallback = defaultNormal ?? new GloXYZVector(0, 1, 0);
        while (Normals.Count < Vertices.Count)
            Normals.Add(fallback);
    }

    public void CreateMatchingUVs(GloXYVector? defaultUV = null)
    {
        GloXYVector fallback = defaultUV ?? new GloXYVector(0, 0);
        while (UVs.Count < Vertices.Count)
            UVs.Add(fallback);
    }

    public void CreateMatchingVertexColors(GloColorRGB? defaultColor = null)
    {
        GloColorRGB fallback = defaultColor ?? new GloColorRGB(1, 1, 1);
        while (VertexColors.Count < Vertices.Count)
            VertexColors.Add(fallback);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Points
    // --------------------------------------------------------------------------------------------

    public int AddPoint(GloXYZVector vertex)
    {
        int index = Vertices.Count;
        Vertices.Add(vertex);
        return index;
    }

    public int AddPoint(GloXYZVector vertex, GloXYZVector? normal = null, GloColorRGB? color = null)
    {
        int index = AddPoint(vertex);

        // Add a default "up" normal if one is not provided.
        if (normal.HasValue)
            Normals.Add(normal.Value);
        else
            Normals.Add(new GloXYZVector(0, 1, 0));

        // Use provided color or default to White.
        VertexColors.Add(color ?? new GloColorRGB(1, 1, 1));

        return index;
    }

    public void SetPoint(int index, GloXYZVector vertex)
    {
        if (index < 0 || index >= Vertices.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        Vertices[index] = vertex;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Lines
    // --------------------------------------------------------------------------------------------

    public int AddLine(int idxA, int idxB)
    {
        int newLineIndex = Lines.Count;
        Lines.Add(new GloMeshLine(idxA, idxB));
        return newLineIndex;
    }

    public int AddLine(GloMeshLine line)
    {
        Lines.Add(line);
        return Lines.Count - 1; // Return the index of the newly added line
    }

    public int AddLine(int idxA, int idxB, GloColorRGB colStart, GloColorRGB colEnd)
    {
        Lines.Add(new GloMeshLine(idxA, idxB));

        int newLineIndex = LineColors.Count;
        LineColors.Add((newLineIndex, colStart, colEnd));
        return newLineIndex;
    }

    public void AddLine(GloXYZVector start, GloXYZVector end, GloColorRGB colStart, GloColorRGB colEnd)
    {
        int idxA = AddPoint(start, null, colStart);
        int idxB = AddPoint(end, null, colEnd);

        AddLine(idxA, idxB, colStart, colEnd);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangles
    // --------------------------------------------------------------------------------------------

    public int AddTriangle(int idxA, int idxB, int idxC)
    {
        Triangles.Add(new GloMeshTriangle(idxA, idxB, idxC));
        return Triangles.Count - 1;
    }

    public int AddTriangle(GloMeshTriangle triangle)
    {
        Triangles.Add(triangle);
        return Triangles.Count - 1; // Return the index of the newly added triangle
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
        Triangles.Add(new GloMeshTriangle(idxA, idxB, idxC));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Normals
    // --------------------------------------------------------------------------------------------

    public void AddNormal(GloXYZVector normal)
    {
        Normals.Add(normal);
    }

    public void SetNormal(int vertexIndex, GloXYZVector normal)
    {
        if (vertexIndex < 0 || vertexIndex >= Vertices.Count)
            throw new ArgumentOutOfRangeException(nameof(vertexIndex), "Vertex index is out of range.");

        Normals[vertexIndex] = normal;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UVs
    // --------------------------------------------------------------------------------------------

    public void AddUV(GloXYVector uv)
    {
        UVs.Add(uv);
    }

    public void SetUV(int vertexIndex, GloXYVector uv)
    {
        if (vertexIndex < 0 || vertexIndex >= Vertices.Count)
            throw new ArgumentOutOfRangeException(nameof(vertexIndex), "Vertex index is out of range.");

        UVs[vertexIndex] = uv;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Vertex Colors
    // --------------------------------------------------------------------------------------------

    public void AddVertexColor(GloColorRGB color)
    {
        VertexColors.Add(color);
    }

    public void SetVertexColor(int vertexIndex, GloColorRGB color)
    {
        if (vertexIndex < 0 || vertexIndex >= Vertices.Count)
            throw new ArgumentOutOfRangeException(nameof(vertexIndex), "Vertex index is out of range.");

        VertexColors[vertexIndex] = color;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Line Colors
    // --------------------------------------------------------------------------------------------

    public void AddLineColor(int lineIndex, GloColorRGB startColor, GloColorRGB endColor)
    {
        if (lineIndex < 0 || lineIndex >= Lines.Count)
            throw new ArgumentOutOfRangeException(nameof(lineIndex), "Line index is out of range.");

        LineColors.Add((lineIndex, startColor, endColor));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Colors
    // --------------------------------------------------------------------------------------------

    public void AddTriangleColor(int triangleIndex, GloColorRGB color)
    {
        if (triangleIndex < 0 || triangleIndex >= Triangles.Count)
            throw new ArgumentOutOfRangeException(nameof(triangleIndex), "Triangle index is out of range.");

        TriangleColors.Add((triangleIndex, color));
    }

}
