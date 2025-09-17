// <fileheader>

using System;
using System.Collections.Generic;

#nullable enable

namespace KoreCommon;

// KoreMiniMesh: A minimalist mesh definition, stripping away textures and UVs to favour exclusively colored materials.
// - Normals are calculated on the fly for a triangle surface.

// COORDINATE SYSTEM SPECIFICATION:
// - X+: Right, Y+: Up, Z-: Forward (Godot native)
// - Triangle winding: clockwise when viewed from outside (Godot native)

public readonly record struct KoreColorMeshTri(int A, int B, int C, KoreColorRGB Color);

public partial class KoreColorMesh
{
    // Core attribute buffers (deduplicated)
    public Dictionary<int, KoreXYZVector> Vertices = []; // unique XYZ
    public Dictionary<int, KoreColorMeshTri> Triangles = []; // per-face corner refs

    // Counters for unique IDs
    public int NextVertexId   = 0;
    public int NextTriangleId = 0;
}



