using System.Collections.Generic;

#nullable enable


// GloSubMeshData: Handles the organization of sub-mesh data within a larger mesh structure.
// - contains properties for the sub-mesh name, mesh data, scale, offset, and rotation.

public class GloSubMeshData
{
    public string       Name     { get; set; } = string.Empty;
    public GloMeshData  Mesh     { get; set; } = new GloMeshData();
    public double       Scale    { get; set; } = 1.0; // Scale applies to mesh after any offset translation and rotation
    public GloXYZVector Offset   { get; set; } = new GloXYZVector(0.0, 0.0, 0.0);
    public GloAttitude  Rotation { get; set; } = new GloAttitude(0.0, 0.0, 0.0);

    public GloSubMeshData()
    {
        Clear();
    }

    public void Clear()
    {
        Name     = "SubMesh";
        Mesh     = new GloMeshData();
        Scale    = 1.0;
        Offset   = new GloXYZVector(0.0, 0.0, 0.0);
        Rotation = new GloAttitude(0.0, 0.0, 0.0);
    }
}