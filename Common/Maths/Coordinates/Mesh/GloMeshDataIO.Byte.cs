using System;
using System.IO;
using System.Collections.Generic;

public static partial class GloMeshDataIO
{
    // --------------------------------------------------------------------------------------------
    // MARK: ToBytes
    // --------------------------------------------------------------------------------------------

    // Write a structure to bytes. We explicitly cast some of the types, to ensure that the correct
    // types are written to the byte array.

    public static byte[] ToBytes(GloMeshData mesh)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        // Vertices
        bw.Write((int)mesh.Vertices.Count);
        foreach (var v in mesh.Vertices)
        {
            bw.Write((double)v.X);
            bw.Write((double)v.Y);
            bw.Write((double)v.Z);
        }

        // Lines
        bw.Write((int)mesh.Lines.Count);
        foreach (var l in mesh.Lines)
        {
            bw.Write((int)l.A);
            bw.Write((int)l.B);
        }

        // Triangles
        bw.Write((int)mesh.Triangles.Count);
        foreach (var t in mesh.Triangles)
        {
            bw.Write((int)t.A);
            bw.Write((int)t.B);
            bw.Write((int)t.C);
        }

        // Normals
        bw.Write((int)mesh.Normals.Count);
        foreach (var n in mesh.Normals)
        {
            bw.Write((double)n.X);
            bw.Write((double)n.Y);
            bw.Write((double)n.Z);
        }

        // UVs
        bw.Write((int)mesh.UVs.Count);
        foreach (var uv in mesh.UVs)
        {
            bw.Write((double)uv.X);
            bw.Write((double)uv.Y);
        }

        // Vertex colors
        bw.Write((int)mesh.VertexColors.Count);
        foreach (var c in mesh.VertexColors)
            WriteColor(bw, c);

        // Line colors
        bw.Write((int)mesh.LineColors.Count);
        foreach (var lc in mesh.LineColors)
        {
            bw.Write((int)lc.Index);
            WriteColor(bw, lc.StartColor);
            WriteColor(bw, lc.EndColor);
        }

        // Triangle colors
        bw.Write((int)mesh.TriangleColors.Count);
        foreach (var tc in mesh.TriangleColors)
        {
            bw.Write((int)tc.Index);
            WriteColor(bw, tc.Color);
        }

        bw.Flush();
        return ms.ToArray();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: FromBytes
    // --------------------------------------------------------------------------------------------

    public static GloMeshData FromBytes(byte[] data)
    {
        var mesh = new GloMeshData();
        using var ms = new MemoryStream(data);
        using var br = new BinaryReader(ms);

        // Vertices
        int vCount = br.ReadInt32();
        for (int i = 0; i < vCount; i++)
        {
            double x = br.ReadDouble();
            double y = br.ReadDouble();
            double z = br.ReadDouble();
            mesh.Vertices.Add(new GloXYZVector(x, y, z));
        }

        // Lines
        int lCount = br.ReadInt32();
        for (int i = 0; i < lCount; i++)
        {
            int a = br.ReadInt32();
            int b = br.ReadInt32();
            mesh.AddLine(a, b);
        }

        // Triangles
        int tCount = br.ReadInt32();
        for (int i = 0; i < tCount; i++)
        {
            int a = br.ReadInt32();
            int b = br.ReadInt32();
            int c = br.ReadInt32();
            mesh.AddTriangle(a, b, c);
        }

        // Normals
        int nCount = br.ReadInt32();
        for (int i = 0; i < nCount; i++)
            mesh.AddNormal(new GloXYZVector(br.ReadDouble(), br.ReadDouble(), br.ReadDouble()));

        // UVs
        int uvCount = br.ReadInt32();
        for (int i = 0; i < uvCount; i++)
            mesh.AddUV(new GloXYVector(br.ReadDouble(), br.ReadDouble()));

        // Vertex colors
        int vcCount = br.ReadInt32();
        for (int i = 0; i < vcCount; i++)
            mesh.VertexColors.Add(ReadColor(br));

        // Line colors
        int lcCount = br.ReadInt32();
        for (int i = 0; i < lcCount; i++)
            mesh.SetLineColor(br.ReadInt32(), ReadColor(br), ReadColor(br));

        // Triangle colors
        int tcCount = br.ReadInt32();
        for (int i = 0; i < tcCount; i++)
            mesh.SetTriangleColor(br.ReadInt32(), ReadColor(br));

        return mesh;
    }

    // --------------------------------------------------------------------------------------------

    public static bool TryFromBytes(byte[] data, out GloMeshData mesh)
    {
        // Initialise the return values to a blank mesh default value
        mesh = new GloMeshData();
        try
        {
            // Read the mesh (as successfully as we can)
            mesh = FromBytes(data);

            // Return true if we successfully read the mesh
            return true;
        }
        catch
        {
            // If we hit an error, return false and the mesh object will remain in whatever state it reached.
            return false;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Colour
    // --------------------------------------------------------------------------------------------

    private static void WriteColor(BinaryWriter bw, GloColorRGB c)
    {
        bw.Write((byte)c.R);
        bw.Write((byte)c.G);
        bw.Write((byte)c.B);
        bw.Write((byte)c.A);
    }

    private static GloColorRGB ReadColor(BinaryReader br)
    {
        return new GloColorRGB(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
    }
}