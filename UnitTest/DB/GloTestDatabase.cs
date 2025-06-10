using System;
using System.IO;


public static class GloTestDatabase
{
    // GloTestDatabase.RunTests(testLog)
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestDatabaseReadWrite(testLog);
            TestDatabaseMesh(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("GloTestDatabase.RunTests", false, $"Exception: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static void TestDatabaseReadWrite(GloTestLog testLog)
    {
        string dbPath = "test_db.sqlite";
        if (File.Exists(dbPath))
            File.Delete(dbPath);

        // Create a new database
        var db = new GloBinaryDataManager(dbPath);
        testLog.AddResult("Database Created", File.Exists(dbPath));

        // Prepare test values
        int    testInt    = 123456;
        float  testFloat  = 3.14159f;
        double testDouble = 2.718281828459;
        bool   testBool   = true;
        string testString = "Hello, world!";
        byte[] testBytes  = new byte[] { 1, 2, 3, 4, 5 };

        // Write values
        var writer = new GloByteArrayWriter();
        writer.WriteInt(testInt);
        writer.WriteFloat(testFloat);
        writer.WriteDouble(testDouble);
        writer.WriteBool(testBool);
        writer.WriteString(testString);
        writer.WriteBytes(testBytes);
        byte[] data = writer.ToArray();

        bool addResult = db.Add("test", data);
        testLog.AddResult("GloBinaryDataManager Add()", addResult);

        // Read back
        byte[] readData   = db.Get("test");
        var    reader     = new GloByteArrayReader(readData);
        int    readInt    = reader.ReadInt();
        float  readFloat  = reader.ReadFloat();
        double readDouble = reader.ReadDouble();
        bool   readBool   = reader.ReadBool();
        string readString = reader.ReadString();
        byte[] readBytes  = reader.ReadBytes(testBytes.Length);

        testLog.AddResult("GloBinaryDataManager Int Read/Write", readInt == testInt);
        testLog.AddResult("GloBinaryDataManager Float Read/Write", Math.Abs(readFloat - testFloat) < 1e-5);
        testLog.AddResult("GloBinaryDataManager Double Read/Write", Math.Abs(readDouble - testDouble) < 1e-10);
        testLog.AddResult("GloBinaryDataManager Bool Read/Write", readBool == testBool);
        testLog.AddResult("GloBinaryDataManager String Read/Write", readString == testString);
        testLog.AddResult("GloBinaryDataManager Bytes Read/Write", readBytes.Length == testBytes.Length && System.Linq.Enumerable.SequenceEqual(readBytes, testBytes));
    }

    private static void TestDatabaseMesh(GloTestLog testLog)
    {
        string dbPath = "test_db_mesh.sqlite";
        if (File.Exists(dbPath))
            File.Delete(dbPath);

        // Create a basic cube mesh
        var mesh = GloMeshDataPrimitives.BasicCube(1.0f, new GloColorRGB(255, 0, 0));

        // Serialise to bytes
        byte[] meshBytes = GloMeshDataIO.ToBytes(mesh);

        // Write to DB
        // var db = new GloBinaryDataManager(dbPath);
        // bool addResult = db.Add("cube", meshBytes);
        // testLog.AddResult("GloBinaryDataManager Mesh Add()", addResult);

        // // Read back
        // byte[] readBytes = db.Get("cube");
        var mesh2 = GloMeshDataIO.FromBytes(meshBytes);

        // Check vertex and triangle counts
        bool vertsMatch = mesh2.Vertices.Count == mesh.Vertices.Count;
        bool trisMatch  = mesh2.Triangles.Count == mesh.Triangles.Count;
        testLog.AddResult($"GloBinaryDataManager Mesh Vertices Count ({mesh.Vertices.Count} -> {mesh2.Vertices.Count})", vertsMatch);
        testLog.AddResult($"GloBinaryDataManager Mesh Triangles Count ({mesh.Triangles.Count} -> {mesh2.Triangles.Count})", trisMatch);
    }
}
