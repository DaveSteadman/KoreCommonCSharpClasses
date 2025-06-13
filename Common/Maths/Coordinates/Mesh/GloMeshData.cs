using System.Collections.Generic;

#nullable enable

public record struct GloMeshLine(int A, int B);
public record struct GloMeshTriangle(int A, int B, int C);
public record struct GloMeshLineColour(int Index, GloColorRGB StartColor, GloColorRGB EndColor);
public record struct GloMeshTriangleColour(int Index, GloColorRGB Color);

// GloMeshData: A class to hold mesh data for 3D geometry.
// - points, lines, triangles, normals, UVs, vertex colors, line colors, and triangle colors.
// - Information about the larger context, such as the object's name, position, rotation, and scale is handled by a higher level class.

public partial class GloMeshData
{
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
    public List<GloMeshLineColour> LineColors;

    // List of triangle colors - for a solid mesh
    public List<GloMeshTriangleColour> TriangleColors;

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
        this.Vertices       = new List<GloXYZVector>();
        this.Lines          = new List<GloMeshLine>();
        this.Triangles      = new List<GloMeshTriangle>();
        this.Normals        = new List<GloXYZVector>();
        this.UVs            = new List<GloXYVector>();
        this.VertexColors   = new List<GloColorRGB>();
        this.LineColors     = new List<GloMeshLineColour>();
        this.TriangleColors = new List<GloMeshTriangleColour>();
    }

    // Copy constructor
    public GloMeshData(
        List<GloXYZVector>          vertices,
        List<GloMeshLine>           lines,
        List<GloMeshTriangle>       triangles,
        List<GloXYZVector>          normals,
        List<GloXYVector>           uvs,
        List<GloColorRGB>           vertexColors,
        List<GloMeshLineColour>     lineColors,
        List<GloMeshTriangleColour> triangleColors)
    {
        this.Vertices       = vertices;
        this.Lines          = lines;
        this.Triangles      = triangles;
        this.Normals        = normals;
        this.UVs            = uvs;
        this.VertexColors   = vertexColors;
        this.LineColors     = lineColors;
        this.TriangleColors = triangleColors;
    }

    // Copy constructor
    public GloMeshData(GloMeshData mesh)
    {
        this.Vertices       = new List<GloXYZVector>(mesh.Vertices);
        this.Lines          = new List<GloMeshLine>(mesh.Lines);
        this.Triangles      = new List<GloMeshTriangle>(mesh.Triangles);
        this.Normals        = new List<GloXYZVector>(mesh.Normals);
        this.UVs            = new List<GloXYVector>(mesh.UVs);
        this.VertexColors   = new List<GloColorRGB>(mesh.VertexColors);
        this.LineColors     = new List<GloMeshLineColour>(mesh.LineColors);
        this.TriangleColors = new List<GloMeshTriangleColour>(mesh.TriangleColors);
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

    // Function to fully populate the mesh data with matching normals, UVs, vertex colors, line colors, and triangle colors.
    public void FullyPopulate()
    {
        CreateMatchingNormals();
        CreateMatchingUVs();
        CreateMatchingVertexColors();
        CreateMatchingLineColors();
        CreateMatchingTriangleColors();
    }

    // Function to examine the vertex list, and remove any orphaned or duplicate lines, triangles, and colors.
    public void MakeValid()
    {
        RemoveOrphanedLines();
        RemoveDuplicateLines();

        RemoveOrphanedTriangles();
        RemoveDuplicateTriangles();

        RemoveOrphanedLineColors();
        RemoveDuplicateLineColors();

        RemoveOrphanedTriangleColors();
        RemoveDuplicateTriangleColors();
    }

    // --------------------------------------------------------------------------------------------

    public void CreateMatchingNormals(GloXYZVector? defaultNormal = null)
    {
        // Define the default normal to pad the normals list if it doesn't match the vertices count.
        GloXYZVector fallback = defaultNormal ?? new GloXYZVector(0, 1, 0);

        //Add missing normals
        while (Normals.Count < Vertices.Count)
            Normals.Add(fallback);

        // Remove excess normals if they exceed the vertices count
        if (Normals.Count > Vertices.Count)
            Normals.RemoveRange(Vertices.Count, Normals.Count - Vertices.Count);
    }

    public void CreateMatchingUVs(GloXYVector? defaultUV = null)
    {
        // Define the default UV to pad the UVs list if it doesn't match the vertices count.
        GloXYVector fallback = defaultUV ?? new GloXYVector(0, 0);

        // Add missing UVs
        while (UVs.Count < Vertices.Count)
            UVs.Add(fallback);

        // Remove excess UVs if they exceed the vertices count
        if (UVs.Count > Vertices.Count)
            UVs.RemoveRange(Vertices.Count, UVs.Count - Vertices.Count);
    }

    public void CreateMatchingVertexColors(GloColorRGB? defaultColor = null)
    {
        // Define the default color to pad the VertexColors list if it doesn't match the vertices count.
        GloColorRGB fallback = defaultColor ?? new GloColorRGB(1, 1, 1);

        // Add missing vertex colors
        while (VertexColors.Count < Vertices.Count)
            VertexColors.Add(fallback);

        // Remove excess vertex colors if they exceed the vertices count
        if (VertexColors.Count > Vertices.Count)
            VertexColors.RemoveRange(Vertices.Count, VertexColors.Count - Vertices.Count);
    }

    public void CreateMatchingLineColors(GloColorRGB? defaultColor = null)
    {
        // Define the default color to pad the LineColors list if it doesn't match the lines count.
        GloColorRGB fallback = defaultColor ?? new GloColorRGB(1, 1, 1);

        // Add missing line colors
        while (LineColors.Count < Lines.Count)
            LineColors.Add(new GloMeshLineColour(LineColors.Count, fallback, fallback));

        // Remove excess line colors if they exceed the lines count
        if (LineColors.Count > Lines.Count)
            LineColors.RemoveRange(Lines.Count, LineColors.Count - Lines.Count);
    }

    public void CreateMatchingTriangleColors(GloColorRGB? defaultColor = null)
    {
        // Define the default color to pad the TriangleColors list if it doesn't match the triangles count.
        GloColorRGB fallback = defaultColor ?? new GloColorRGB(1, 1, 1);

        // Add missing triangle colors
        while (TriangleColors.Count < Triangles.Count)
            TriangleColors.Add(new GloMeshTriangleColour(TriangleColors.Count, fallback));

        // Remove excess triangle colors if they exceed the triangles count
        if (TriangleColors.Count > Triangles.Count)
            TriangleColors.RemoveRange(Triangles.Count, TriangleColors.Count - Triangles.Count);
    }

    // --------------------------------------------------------------------------------------------

    public void RemoveOrphanedLines()
    {
        // Remove lines that reference vertices that no longer exist
        Lines.RemoveAll(line => line.A < 0 || line.A >= Vertices.Count || line.B < 0 || line.B >= Vertices.Count);
    }

    public void RemoveDuplicateLines()
    {
        // Remove duplicate lines based on their vertex indices
        var uniqueLines = new HashSet<(int, int)>();
        Lines.RemoveAll(line => !uniqueLines.Add((Math.Min(line.A, line.B), Math.Max(line.A, line.B))));
    }

    public void RemoveOrphanedTriangles()
    {
        // Remove triangles that reference vertices that no longer exist
        Triangles.RemoveAll(triangle => triangle.A < 0 || triangle.A >= Vertices.Count ||
                                          triangle.B < 0 || triangle.B >= Vertices.Count ||
                                          triangle.C < 0 || triangle.C >= Vertices.Count);
    }

    public void RemoveDuplicateTriangles()
    {
        // Remove duplicate triangles based on their vertex indices
        var uniqueTriangles = new HashSet<(int, int, int)>();
        Triangles.RemoveAll(triangle => !uniqueTriangles.Add((Math.Min(triangle.A, Math.Min(triangle.B, triangle.C)),
                                                              Math.Max(Math.Min(triangle.A, triangle.B), Math.Min(triangle.B, triangle.C)),
                                                              Math.Max(triangle.A, Math.Max(triangle.B, triangle.C)))));
    }

    public void RemoveOrphanedLineColors()
    {
        // Remove line colors that reference lines that no longer exist
        LineColors.RemoveAll(lineColor => lineColor.Index < 0 || lineColor.Index >= Lines.Count);
    }

    public void RemoveDuplicateLineColors()
    {
        // Remove duplicate line colors based on their index
        var uniqueLineColors = new HashSet<int>();
        LineColors.RemoveAll(lineColor => !uniqueLineColors.Add(lineColor.Index));
    }

    public void RemoveOrphanedTriangleColors()
    {
        // Remove triangle colors that reference triangles that no longer exist
        TriangleColors.RemoveAll(triangleColor => triangleColor.Index < 0 || triangleColor.Index >= Triangles.Count);
    }

    public void RemoveDuplicateTriangleColors()
    {
        // Remove duplicate triangle colors based on their index
        var uniqueTriangleColors = new HashSet<int>();
        TriangleColors.RemoveAll(triangleColor => !uniqueTriangleColors.Add(triangleColor.Index));
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
        LineColors.Add(new GloMeshLineColour(newLineIndex, colStart, colEnd));
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

    // Add a completely independent triangle with vertices and optional color.
    public void AddTriangle(GloXYZVector a, GloXYZVector b, GloXYZVector c, GloColorRGB? linecolor = null, GloColorRGB? fillColor = null)
    {
        int idxA = AddPoint(a);
        int idxB = AddPoint(b);
        int idxC = AddPoint(c);

        // Set the vertex colors if fill color is provided
        if (fillColor.HasValue)
        {
            VertexColors[idxA] = fillColor.Value;
            VertexColors[idxB] = fillColor.Value;
            VertexColors[idxC] = fillColor.Value;
        }

        // Use the line color if provided, otherwise don't add the lines.
        if (linecolor.HasValue)
        {
            // If a line color is provided, add lines between the vertices.
            GloColorRGB lineCol = linecolor.Value;
            AddLine(idxA, idxB, lineCol, lineCol);
            AddLine(idxB, idxC, lineCol, lineCol);
            AddLine(idxC, idxA, lineCol, lineCol);
        }

        // Add the triangle to the list
        Triangles.Add(new GloMeshTriangle(idxA, idxB, idxC));

        // If a fill color is provided, also add a triangle color
        if (fillColor.HasValue)
        {
            GloMeshTriangleColour triangleColor = new GloMeshTriangleColour(Triangles.Count - 1, fillColor.Value);
            TriangleColors.Add(triangleColor);
        }
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

    public void SetLineColor(int lineIndex, GloColorRGB startColor, GloColorRGB endColor) => SetLineColor(new GloMeshLineColour(lineIndex, startColor, endColor));

    public void SetLineColor(GloMeshLineColour lineColor)
    {
        // As the line list is indexed by the line index, we search for the line index rather than use the ID as an array lookup.
        // We add the colors as a new entry in the LineColors list if it doesn't already exist.
        foreach (var color in LineColors)
        {
            if (color.Index == lineColor.Index)
            {
                // Update existing color
                LineColors[LineColors.IndexOf(color)] = lineColor;
                return;
            }
        }

        // else, add a new color entry
        LineColors.Add(lineColor);
    }

    public void SetAllLineColors(GloColorRGB startColor, GloColorRGB endColor)
    {
        // Set all line colors to the specified start and end colors
        for (int i = 0; i < Lines.Count; i++)
        {
            SetLineColor(i, startColor, endColor);
        }
    }

    public void SetAllLineColors(GloColorRGB color) => SetAllLineColors(color, color);

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Colors
    // --------------------------------------------------------------------------------------------

    public void SetTriangleColor(int triangleIndex, GloColorRGB color)
    {
        GloMeshTriangleColour tricolor = new GloMeshTriangleColour(triangleIndex, color);
        SetTriangleColor(tricolor);
    }

    public void SetTriangleColor(GloMeshTriangleColour tricolor)
    {
        // As with the lines, the triangle colors are stored by their index, so we search for the index
        foreach (var color in TriangleColors)
        {
            if (color.Index == tricolor.Index)
            {
                // Update existing color
                TriangleColors[TriangleColors.IndexOf(color)] = tricolor;
                return;
            }
        }

        // else, add a new color entry
        TriangleColors.Add(tricolor);
    }
}


