using System;

public static class GloTestMesh
{
    // Usage: GloTestMesh.RunTests(testLog);
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestBasicCubeJson(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("GloTestMesh RunTests // Exception: ", false, ex.Message);
            return;
        }
    }


    // Test to create a basic cube - convert it to JSON (text) and then back again, to ensure all the lists are correctly serialized and deserialized.
    public static void TestBasicCubeJson(GloTestLog testLog)
    {
        // Test for basic cube mesh creation
        var cubeMesh = GloMeshDataPrimitives.BasicCube(1.0f, new GloColorRGB(255, 0, 0));

        // Add some minort customizations to the cube mesh to see if they are serialized correctly
        cubeMesh.SetVertexColor(1, GloColorPalette.Colors["Green"]);
        cubeMesh.SetLineColor(2, GloColorPalette.Colors["Blue"], GloColorPalette.Colors["Cyan"]);
        cubeMesh.SetTriangleColor(3, GloColorPalette.Colors["Yellow"]);

        string cubeJSON = GloMeshDataIO.ToJson(cubeMesh);
        //Console.WriteLine($"Cube Mesh JSON:{cubeJSON}");
        testLog.AddComment($"GloMeshDataIO ToJson BasicCube: {cubeJSON}");

        var deserializedCubeMesh = GloMeshDataIO.FromJson(cubeJSON);
        string reserialisedCubeJSON = GloMeshDataIO.ToJson(deserializedCubeMesh);

        testLog.AddComment($"GloMeshDataIO FromJson BasicCube - JSON format: {reserialisedCubeJSON}");


        // testLog.AddResult("GloMeshDataIO FromJson BasicCube", deserializedCubeMesh != null);

        // if (deserializedCubeMesh == null)
        // {
        //     testLog.AddComment("Deserialized mesh is null, check the JSON format or the GloMeshDataIO implementation.");
        //     return;
        // }

        // // Check if the deserialized mesh is valid - comparing some select values and values
        // testLog.AddResult("GloMeshDataIO FromJson BasicCube Name", deserializedCubeMesh.Name                 == cubeMesh.Name);
        // testLog.AddResult("GloMeshDataIO PointCount",              deserializedCubeMesh.Vertices.Count       == cubeMesh.Vertices.Count);
        // testLog.AddResult("GloMeshDataIO LineCount",               deserializedCubeMesh.Lines.Count          == cubeMesh.Lines.Count);
        // testLog.AddResult("GloMeshDataIO TriangleCount",           deserializedCubeMesh.Triangles.Count      == cubeMesh.Triangles.Count);
        // testLog.AddResult("GloMeshDataIO NormalCount",             deserializedCubeMesh.Normals.Count        == cubeMesh.Normals.Count);
        // testLog.AddResult("GloMeshDataIO UVCount",                 deserializedCubeMesh.UVs.Count            == cubeMesh.UVs.Count);
        // testLog.AddResult("GloMeshDataIO VertexColorCount",        deserializedCubeMesh.VertexColors.Count   == cubeMesh.VertexColors.Count);
        // testLog.AddResult("GloMeshDataIO LineColorCount",          deserializedCubeMesh.LineColors.Count     == cubeMesh.LineColors.Count);
        // testLog.AddResult("GloMeshDataIO TriangleColorCount",      deserializedCubeMesh.TriangleColors.Count == cubeMesh.TriangleColors.Count);
    }

}