using System;

public static class GloTestMesh
{
    // Usage: GloTestMesh.RunTests(testLog);
    public static void RunTests(GloTestLog testLog)
    {
        TestBasicCube(testLog);

    }

    public static void TestBasicCube(GloTestLog testLog)
    {
        // Test for basic cube mesh creation
        var cubeMesh = GloMeshDataPrimitives.BasicCube(1.0f, new GloColorRGB(255, 0, 0));

        cubeMesh.AddUV(new GloXYVector(0.0f, 0.0f));
        cubeMesh.AddUV(new GloXYVector(1.0f, 0.0f));
        cubeMesh.AddUV(new GloXYVector(1.0f, 1.0f));
        cubeMesh.AddUV(new GloXYVector(0.0f, 1.0f));

        string cubeJSON = GloMeshDataIO.ToJson(cubeMesh);
        Console.WriteLine($"Cube Mesh JSON:{cubeJSON}");

    }

}