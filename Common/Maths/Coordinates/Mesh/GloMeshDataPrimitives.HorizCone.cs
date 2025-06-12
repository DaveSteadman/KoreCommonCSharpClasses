using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Static class to create GloMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class GloMeshDataPrimitives
{
    public enum ConeStyle { Cone, CroppedCone }

    public static GloMeshData HorizCone(
        float length,         // Distance from apex to base center.
        float majorAxis,      // Radius along Y.
        float minorAxis,      // Radius along Z.
        int baseNumPoints,
        GloColorRGB color,
        ConeStyle style = ConeStyle.Cone,
        int numDots = 5)      // Number of dots for dotted lines.
    {
        GloMeshData mesh = new GloMeshData();

        // Define key points.
        // Apex at origin.
        int idxApex = mesh.AddPoint(new GloXYZVector(0, 0, 0), null, color);
        // Base center along +X.
        int idxBaseCenter = mesh.AddPoint(new GloXYZVector(length, 0, 0), null, color);

        // Generate base points on an ellipse in the YZ plane (centered at baseCenter).
        List<int> basePointIndices = new List<int>();
        for (int i = 0; i < baseNumPoints; i++)
        {
            double angle = 2 * Math.PI * i / baseNumPoints;
            double offsetY = majorAxis * Math.Cos(angle);
            double offsetZ = minorAxis * Math.Sin(angle);

            // Base point is offset from the base center.
            GloXYZVector basePoint = new GloXYZVector(length, offsetY, offsetZ);
            int idx = mesh.AddPoint(basePoint, null, color);
            basePointIndices.Add(idx);
        }

        // Create the cone's base by connecting each base point to the base center
        // and linking consecutive points.
        for (int i = 0; i < basePointIndices.Count; i++)
        {
            int currentIdx = basePointIndices[i];
            int nextIdx = basePointIndices[(i + 1) % basePointIndices.Count];

            if (style == ConeStyle.Cone)
            {
                // Draw solid lines.
                mesh.AddLine(mesh.Vertices[idxBaseCenter], mesh.Vertices[currentIdx], color, color);
                mesh.AddLine(mesh.Vertices[currentIdx], mesh.Vertices[nextIdx], color, color);
            }
        }

        // Create the lateral sides: connect the apex with every base point.
        foreach (int idx in basePointIndices)
        {
            mesh.AddLine(mesh.Vertices[idxApex], mesh.Vertices[idx], color, color);
        }

        return mesh;
    }


    public static GloMeshData Cone(
        GloXYZPoint apex,         // Apex point of the cone.
        GloXYZVector centerLine,  // Center line vector of the cone.
        double baseRadius,        // Radius perpendicular to the center line at the base.
        int baseNumPoints)
    {
        GloMeshData mesh = new GloMeshData();

        // Define the key points
        GloXYZPoint apexPoint = apex;
        GloXYZPoint baseCenterpoint = apexPoint + centerLine;

        // Create the list of base points around the base center point, perpendicular to the center line.
        List<GloXYZPoint> basePoints = new List<GloXYZPoint>();
        for (int i = 0; i < baseNumPoints; i++)
        {
            double angle = 2 * Math.PI * i / baseNumPoints;
            GloXYZVector offset = new GloXYZVector(
                baseRadius * Math.Cos(angle),
                baseRadius * Math.Sin(angle),
                0); // Assuming the cone is upright, so Z offset is zero.
            GloXYZPoint basePoint = baseCenterpoint + offset;
            basePoints.Add(basePoint);
        }


       return mesh;
   }



}

