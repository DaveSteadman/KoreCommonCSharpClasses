using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Static class to create GloMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class GloMeshDataPrimitives
{
    public static GloMeshData Cylinder(GloXYZVector p1, GloXYZVector p2, double p1radius, double p2radius, int sides, bool endsClosed)
    {
        var mesh = new GloMeshData() { Name = "Cylinder" };

        // Direction and length
        GloXYZVector axis   = p2 - p1;
        double   height = axis.Magnitude;
        if (height < 1e-6f || sides < 3) return mesh;

        axis = axis.Normalize();

        // Find a vector not parallel to axis for basis
        GloXYZVector up      = Math.Abs(GloXYZVector.DotProduct(axis, GloXYZVector.Up)) < 0.99f ? GloXYZVector.Up : GloXYZVector.Right;
        GloXYZVector side    = GloXYZVector.CrossProduct(axis, up).Normalize();
        GloXYZVector forward = GloXYZVector.CrossProduct(axis, side).Normalize();

        // Generate circle points for both ends
        var   p1Circle  = new List<GloXYZVector>();
        var   p2Circle  = new List<GloXYZVector>();
        double angleStep = Math.Tau / sides;

        for (int i = 0; i < sides; i++)
        {
            double   angle  = i * angleStep;
            GloXYZVector offset = (Math.Cos(angle) * side + Math.Sin(angle) * forward);
            p1Circle.Add(p1 + offset * p1radius);
            p2Circle.Add(p2 + offset * p2radius);
        }

        // Side faces (quads split into triangles)
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;
            // First triangle
            mesh.AddTriangle(p1Circle[i], p2Circle[i], p2Circle[next]);
            // Second triangle
            mesh.AddTriangle(p1Circle[i], p2Circle[next], p1Circle[next]);
        }

        // End caps
        if (endsClosed)
        {
            // p1 cap
            for (int i = 1; i < sides - 1; i++)
                mesh.AddTriangle(p1, p1Circle[i], p1Circle[i + 1]);

            // p2 cap
            for (int i = 1; i < sides - 1; i++)
                mesh.AddTriangle(p2, p2Circle[i + 1], p2Circle[i]);
        }

        // --- Add lines for wireframe ---
        GloColorRGB lineColor = new GloColorRGB(200, 200, 200); // Or choose as needed

        // Longitudinal lines
        for (int i = 0; i < sides; i++)
        {
            mesh.AddLine(p1Circle[i], p2Circle[i], lineColor, lineColor);
        }

        // Rings at each end
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;
            mesh.AddLine(p1Circle[i], p1Circle[next], lineColor, lineColor);
            mesh.AddLine(p2Circle[i], p2Circle[next], lineColor, lineColor);
        }

        return mesh;
    }
}
