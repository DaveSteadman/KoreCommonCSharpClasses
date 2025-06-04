using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// MagSphere - A sphere where the radius of each point is defined by an input array of floats.
// The radius is the distance from the center of the sphere to the point on the surface of the sphere.
// The radius is defined in spherical coordinates, with the azimuth and elevation angles defining the position of the point on the sphere.
// The color of a point is the magnitude of the radius, defined by an input color range.

public static partial class GloMeshDataPrimitives
{
    public static GloMeshData MagSphere(GloXYZVector center, GloFloat2DArray radiusList, GloColorRange colorRange)
    {
        GloMeshData returnedMesh = new GloMeshData() { Name = "MagSphere" };

        int vertSegments  = radiusList.Height - 1;
        int horizSegments = radiusList.Width - 1;

        double vertAngInc  = 180f / (double)vertSegments;
        double horizAngInc = 360f / (double)horizSegments;

        double maxRadius = radiusList.MaxVal();
        double minRadius = radiusList.MinVal();

        // Hold the indices of the MeshData.Vertices for this sphere
        List<int>   sphereVertexIndices = new List<int>();

        // Define the points on the sphere surface
        for (int i = 0; i < vertSegments+1; i++)
        {
            for (int j = 0; j < horizSegments; j++)
            {
                // Lookup the radius
                float radius = radiusList[j, i];

                // Calculate the angles
                double azRads = GloAngle.DegsToRads(90f - (vertAngInc * i));
                double elRads = GloAngle.DegsToRads(horizAngInc * j);

                // Calculate the x,y,z
                double y = radius * Math.Sin(azRads);
                double r = radius * Math.Cos(azRads);
                double x = r * Math.Cos(elRads);
                double z = r * Math.Sin(elRads);

                // Determine the final vertex position with the center offset
                GloXYZVector v = new GloXYZVector(x, y, z) + center;

                // Add the vertex
                int index = returnedMesh.AddPoint(v);

                // Add the normal
                GloXYZVector n = new GloXYZVector(x, y, z).Normalize();
                returnedMesh.AddNormal(n);

                // Add the UV
                double uvXFraction = (double)j / (double)horizSegments;
                double uvYFraction = (double)i / (double)vertSegments;
                returnedMesh.AddUV(new GloXYVector(uvXFraction, uvYFraction));

                // Calculate the fraction through the range, and add the color for that fraction
                double radiusFraction = (radius - minRadius) / (maxRadius - minRadius);
                returnedMesh.AddColor(colorRange.GetColor((float)radiusFraction));

                // Add the index to the sphereVertexIndices - to calculate the triangles later
                sphereVertexIndices.Add(index);
            }
        }

        // Define the MeshData.Triangles
        for (int row = 0; row < (vertSegments); row++)
        {
            int rowStart     = row       * horizSegments;
            int nextRowStart = (row + 1) * horizSegments;

            for (int i = 0; i < (horizSegments); i++)
            {
                int index1 = sphereVertexIndices[rowStart + i];
                int index2 = sphereVertexIndices[rowStart + (i + 1) % horizSegments];
                int index3 = sphereVertexIndices[nextRowStart + i];
                int index4 = sphereVertexIndices[nextRowStart + (i + 1) % horizSegments];

                returnedMesh.AddTriangle(index1, index4, index2);
                returnedMesh.AddTriangle(index1, index3, index4);

                returnedMesh.AddLine(index1, index2, returnedMesh.VertexColors[index1], returnedMesh.VertexColors[index2]);
                returnedMesh.AddLine(index1, index3, returnedMesh.VertexColors[index1], returnedMesh.VertexColors[index3]);

                // returnedMesh.AddLine(index1, index4, returnedMesh.VertexColors[index1], returnedMesh.VertexColors[index4]);
                // returnedMesh.AddLine(index2, index3, returnedMesh.VertexColors[index2], returnedMesh.VertexColors[index3]);
                // returnedMesh.AddLine(index2, index4, returnedMesh.VertexColors[index2], returnedMesh.VertexColors[index4]);
                // returnedMesh.AddLine(index3, index4, returnedMesh.VertexColors[index3], returnedMesh.VertexColors[index4]);
            }
        }

        return returnedMesh;
    }
}
