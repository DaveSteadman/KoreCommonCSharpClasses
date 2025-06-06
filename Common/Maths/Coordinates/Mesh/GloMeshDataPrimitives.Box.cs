using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


// Static class to create GloMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class GloMeshDataPrimitives
{
    // Usage: var cubeMesh = GloMeshDataPrimitives.BasicCube(1.0f, new GloColorRGB(255, 0, 0));
    public static GloMeshData BasicCube(float size, GloColorRGB color)
    {
        var mesh = new GloMeshData { Name = "Cube" };

        // Define the vertices of the cube
        int v0 = mesh.AddPoint(new GloXYZVector(-size, -size, -size), null, color);
        int v1 = mesh.AddPoint(new GloXYZVector( size, -size, -size), null, color);
        int v2 = mesh.AddPoint(new GloXYZVector( size,  size, -size), null, color);
        int v3 = mesh.AddPoint(new GloXYZVector(-size,  size, -size), null, color);
        int v4 = mesh.AddPoint(new GloXYZVector(-size, -size,  size), null, color);
        int v5 = mesh.AddPoint(new GloXYZVector( size, -size,  size), null, color);
        int v6 = mesh.AddPoint(new GloXYZVector( size,  size,  size), null, color);
        int v7 = mesh.AddPoint(new GloXYZVector(-size,  size,  size), null, color);

        // Lines
        mesh.AddLine(v0, v1, color, color);
        mesh.AddLine(v1, v5, color, color);
        mesh.AddLine(v5, v4, color, color);
        mesh.AddLine(v4, v0, color, color);
        mesh.AddLine(v2, v3, color, color);
        mesh.AddLine(v3, v7, color, color);
        mesh.AddLine(v7, v6, color, color);
        mesh.AddLine(v6, v2, color, color);
        mesh.AddLine(v0, v3, color, color);
        mesh.AddLine(v1, v2, color, color);
        mesh.AddLine(v4, v7, color, color);
        mesh.AddLine(v5, v6, color, color);

        // Triangles
        mesh.AddTriangle(v0, v1, v2); mesh.AddTriangle(v0, v2, v3);
        mesh.AddTriangle(v4, v5, v6); mesh.AddTriangle(v4, v6, v7);
        mesh.AddTriangle(v0, v1, v5); mesh.AddTriangle(v0, v5, v4);
        mesh.AddTriangle(v1, v2, v6); mesh.AddTriangle(v1, v6, v5);
        mesh.AddTriangle(v2, v3, v7); mesh.AddTriangle(v2, v7, v6);
        mesh.AddTriangle(v3, v0, v4); mesh.AddTriangle(v3, v4, v7);

        mesh.MakeValid();
        return mesh;
    }

    // ---------------------------------------------------------------------------------------------

    public static GloMeshData SizedBox(
        double sizeUp, double sizeDown,
        double sizeLeft, double sizeRight,
        double sizeFront, double sizeBack,
        GloColorRGB color)
    {
        // Create a new GloMeshData object
        var mesh = new GloMeshData() { Name = "SizedBox" };

        // Define 8 unique vertices for the rectangular box
        // Front face vertices:
        int v0 = mesh.AddPoint(new GloXYZVector(-sizeLeft,  -sizeDown, -sizeFront), null, color); // Lower left front
        int v1 = mesh.AddPoint(new GloXYZVector( sizeRight, -sizeDown, -sizeFront), null, color); // Lower right front
        int v2 = mesh.AddPoint(new GloXYZVector( sizeRight,  sizeUp,   -sizeFront), null, color); // Upper right front
        int v3 = mesh.AddPoint(new GloXYZVector(-sizeLeft,   sizeUp,   -sizeFront), null, color); // Upper left front

        // Back face vertices:
        int v4 = mesh.AddPoint(new GloXYZVector(-sizeLeft,  -sizeDown, sizeBack),   null, color); // Lower left back
        int v5 = mesh.AddPoint(new GloXYZVector( sizeRight, -sizeDown, sizeBack),   null, color); // Lower right back
        int v6 = mesh.AddPoint(new GloXYZVector( sizeRight,  sizeUp,   sizeBack),   null, color); // Upper right back
        int v7 = mesh.AddPoint(new GloXYZVector(-sizeLeft,   sizeUp,   sizeBack),   null, color); // Upper left back

        // Define edges (lines)
        // Lines
        mesh.AddLine(v0, v1, color, color);
        mesh.AddLine(v1, v5, color, color);
        mesh.AddLine(v5, v4, color, color);
        mesh.AddLine(v4, v0, color, color);
        mesh.AddLine(v2, v3, color, color);
        mesh.AddLine(v3, v7, color, color);
        mesh.AddLine(v7, v6, color, color);
        mesh.AddLine(v6, v2, color, color);
        mesh.AddLine(v0, v3, color, color);
        mesh.AddLine(v1, v2, color, color);
        mesh.AddLine(v4, v7, color, color);
        mesh.AddLine(v5, v6, color, color);

        // Triangles
        mesh.AddTriangle(v0, v1, v2); mesh.AddTriangle(v0, v2, v3);
        mesh.AddTriangle(v4, v5, v6); mesh.AddTriangle(v4, v6, v7);
        mesh.AddTriangle(v0, v1, v5); mesh.AddTriangle(v0, v5, v4);
        mesh.AddTriangle(v1, v2, v6); mesh.AddTriangle(v1, v6, v5);
        mesh.AddTriangle(v2, v3, v7); mesh.AddTriangle(v2, v7, v6);
        mesh.AddTriangle(v3, v0, v4); mesh.AddTriangle(v3, v4, v7);

        return mesh;
    }

}
