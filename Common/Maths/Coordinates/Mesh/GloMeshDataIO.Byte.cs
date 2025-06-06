using System;
using System.IO;
using System.Collections.Generic;


public static partial class GloMeshDataIO
{
    // --------------------------------------------------------------------------------------------
    // MARK: ToBytes
    // --------------------------------------------------------------------------------------------

    public static byte[] ToBytes(GloMeshData mesh)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        // Write name
        bw.Write(mesh.Name ?? "");

        // Vertices
        bw.Write(mesh.Vertices.Count);
        foreach (var v in mesh.Vertices)
        {
            bw.Write(v.X);
            bw.Write(v.Y);
            bw.Write(v.Z);
        }

        // Lines
        bw.Write(mesh.Lines.Count);
        foreach (var l in mesh.Lines)
        {
            bw.Write(l.A);
            bw.Write(l.B);
        }

        // Triangles
        bw.Write(mesh.Triangles.Count);
        foreach (var t in mesh.Triangles)
        {
            bw.Write(t.A);
            bw.Write(t.B);
            bw.Write(t.C);
        }

        // Normals
        bw.Write(mesh.Normals.Count);
        foreach (var n in mesh.Normals)
        {
            bw.Write(n.X);
            bw.Write(n.Y);
            bw.Write(n.Z);
        }

        // UVs
        bw.Write(mesh.UVs.Count);
        foreach (var uv in mesh.UVs)
        {
            bw.Write(uv.X);
            bw.Write(uv.Y);
        }

        // VertexColors
        bw.Write(mesh.VertexColors.Count);
        foreach (var c in mesh.VertexColors)
            WriteColor(bw, c);

        // Lines colours
        bw.Write(mesh.LineColors.Count);
        foreach (var lc in mesh.LineColors)
        {
            bw.Write(lc.Index);
            WriteColor(bw, lc.StartColor);
            WriteColor(bw, lc.EndColor);
        }

        // Triangles colours
        bw.Write(mesh.TriangleColors.Count);
        foreach (var tc in mesh.TriangleColors)
        {
            bw.Write(tc.Index);
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

        // Name
        mesh.Name = br.ReadString();

        // Vertices
        int vCount = br.ReadInt32();
        for (int i = 0; i < vCount; i++)
            mesh.Vertices.Add(new GloXYZVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));

        // Lines
        int lCount = br.ReadInt32();
        for (int i = 0; i < lCount; i++)
            mesh.AddLine(br.ReadInt32(), br.ReadInt32());

        // Triangles
        int tCount = br.ReadInt32();
        for (int i = 0; i < tCount; i++)
            mesh.AddTriangle(br.ReadInt32(), br.ReadInt32(), br.ReadInt32());

            // Normals
        int nCount = br.ReadInt32();
        for (int i = 0; i < nCount; i++)
            mesh.AddNormal(new GloXYZVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));

        // UVs
        int uvCount = br.ReadInt32();
        for (int i = 0; i < uvCount; i++)
            mesh.AddUV(new GloXYVector(br.ReadSingle(), br.ReadSingle()));

        // VertexColors
        int vcCount = br.ReadInt32();
        for (int i = 0; i < vcCount; i++)
            mesh.VertexColors.Add(ReadColor(br));

        // Lines colours
        int lcCount = br.ReadInt32();
        for (int i = 0; i < lcCount; i++)
            mesh.SetLineColor(br.ReadInt32(), ReadColor(br), ReadColor(br));

        // Triangles colours
        int tcCount = br.ReadInt32();
        for (int i = 0; i < tcCount; i++)
            mesh.SetTriangleColor(br.ReadInt32(), ReadColor(br));

        return mesh;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Colour
    // --------------------------------------------------------------------------------------------

    private static void WriteColor(BinaryWriter bw, GloColorRGB c)
    {
        bw.Write(c.R);
        bw.Write(c.G);
        bw.Write(c.B);
        bw.Write(c.A);
    }

    private static GloColorRGB ReadColor(BinaryReader br)
    {
        return new GloColorRGB(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
    }
}