using System;
using System.Collections.Generic;

#nullable enable

namespace KoreCommon;

public partial class KoreMeshData
{
    // -----------------------------------------------------------------------------
    // Validity functions
    // -----------------------------------------------------------------------------

    public void RemoveOrphanedPoints()
    {
        // Determine which vertices are referenced by lines or triangles
        var used = new HashSet<int>();
        foreach (var l in Lines)
        {
            used.Add(l.A);
            used.Add(l.B);
        }
        foreach (var t in Triangles)
        {
            used.Add(t.A);
            used.Add(t.B);
            used.Add(t.C);
        }

        var indexMap = new Dictionary<int, int>();
        var newVerts = new List<KoreXYZVector>();
        var newNormals = new List<KoreXYZVector>();
        var newUVs = new List<KoreXYVector>();
        var newColors = new List<KoreColorRGB>();

        bool hasNormals = Normals.Count > 0;
        bool hasUVs = UVs.Count > 0;
        bool hasColors = VertexColors.Count > 0;

        for (int i = 0; i < Vertices.Count; i++)
        {
            if (used.Contains(i))
            {
                int newIdx = newVerts.Count;
                indexMap[i] = newIdx;
                newVerts.Add(Vertices[i]);
                if (i < Normals.Count) newNormals.Add(Normals[i]);
                if (i < UVs.Count) newUVs.Add(UVs[i]);
                if (i < VertexColors.Count) newColors.Add(VertexColors[i]);
            }
        }

        Vertices = newVerts;
        if (hasNormals) Normals = newNormals;
        if (hasUVs) UVs = newUVs;
        if (hasColors) VertexColors = newColors;

        // Renumber existing indices
        for (int i = 0; i < Lines.Count; i++)
        {
            var l = Lines[i];
            Lines[i] = new KoreMeshLine(indexMap[l.A], indexMap[l.B]);
        }
        for (int i = 0; i < Triangles.Count; i++)
        {
            var t = Triangles[i];
            Triangles[i] = new KoreMeshTriangle(indexMap[t.A], indexMap[t.B], indexMap[t.C]);
        }
    }

    public void RemoveDuplicatePoints(double tolerance = KoreConsts.ArbitrarySmallDouble)
    {
        var indexMap = new Dictionary<int, int>();
        var uniqueVerts = new List<KoreXYZVector>();
        var uniqueNormals = new List<KoreXYZVector>();
        var uniqueUVs = new List<KoreXYVector>();
        var uniqueColors = new List<KoreColorRGB>();

        bool hasNormals = Normals.Count > 0;
        bool hasUVs = UVs.Count > 0;
        bool hasColors = VertexColors.Count > 0;

        for (int i = 0; i < Vertices.Count; i++)
        {
            var v = Vertices[i];
            int found = -1;
            for (int j = 0; j < uniqueVerts.Count; j++)
            {
                var u = uniqueVerts[j];
                if (KoreValueUtils.EqualsWithinTolerance(v.X, u.X, tolerance) &&
                    KoreValueUtils.EqualsWithinTolerance(v.Y, u.Y, tolerance) &&
                    KoreValueUtils.EqualsWithinTolerance(v.Z, u.Z, tolerance))
                {
                    found = j;
                    break;
                }
            }

            if (found >= 0)
            {
                indexMap[i] = found;
            }
            else
            {
                int newIdx = uniqueVerts.Count;
                indexMap[i] = newIdx;
                uniqueVerts.Add(v);
                if (i < Normals.Count) uniqueNormals.Add(Normals[i]);
                if (i < UVs.Count) uniqueUVs.Add(UVs[i]);
                if (i < VertexColors.Count) uniqueColors.Add(VertexColors[i]);
            }
        }

        Vertices = uniqueVerts;
        if (hasNormals) Normals = uniqueNormals;
        if (hasUVs) UVs = uniqueUVs;
        if (hasColors) VertexColors = uniqueColors;

        for (int i = 0; i < Lines.Count; i++)
        {
            var l = Lines[i];
            Lines[i] = new KoreMeshLine(indexMap[l.A], indexMap[l.B]);
        }
        for (int i = 0; i < Triangles.Count; i++)
        {
            var t = Triangles[i];
            Triangles[i] = new KoreMeshTriangle(indexMap[t.A], indexMap[t.B], indexMap[t.C]);
        }
    }

    public void RemoveOrphanedLines()
    {
        Lines.RemoveAll(line => line.A < 0 || line.A >= Vertices.Count || line.B < 0 || line.B >= Vertices.Count);
    }

    public void RemoveDuplicateLines()
    {
        var uniqueLines = new HashSet<(int, int)>();
        Lines.RemoveAll(line => !uniqueLines.Add((Math.Min(line.A, line.B), Math.Max(line.A, line.B))));
    }

    public void RemoveOrphanedTriangles()
    {
        Triangles.RemoveAll(triangle => triangle.A < 0 || triangle.A >= Vertices.Count ||
                                          triangle.B < 0 || triangle.B >= Vertices.Count ||
                                          triangle.C < 0 || triangle.C >= Vertices.Count);
    }

    public void RemoveDuplicateTriangles()
    {
        var uniqueTriangles = new HashSet<(int, int, int)>();
        Triangles.RemoveAll(triangle =>
        {
            int[] idx = { triangle.A, triangle.B, triangle.C };
            Array.Sort(idx);
            return !uniqueTriangles.Add((idx[0], idx[1], idx[2]));
        });
    }

    public void RemoveOrphanedLineColors()
    {
        LineColors.RemoveAll(lineColor => lineColor.Index < 0 || lineColor.Index >= Lines.Count);
    }

    public void RemoveDuplicateLineColors()
    {
        var uniqueLineColors = new HashSet<int>();
        LineColors.RemoveAll(lineColor => !uniqueLineColors.Add(lineColor.Index));
    }

    public void RemoveOrphanedTriangleColors()
    {
        TriangleColors.RemoveAll(triangleColor => triangleColor.Index < 0 || triangleColor.Index >= Triangles.Count);
    }

    public void RemoveDuplicateTriangleColors()
    {
        var uniqueTriangleColors = new HashSet<int>();
        TriangleColors.RemoveAll(triangleColor => !uniqueTriangleColors.Add(triangleColor.Index));
    }
}
