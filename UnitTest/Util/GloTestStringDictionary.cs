using System;
using System.Diagnostics;

public static class GloTestStringDictionary
{
    public static void RunTests(GloTestLog testLog)
    {
        TestRoundTrip(testLog);
    }

    private static void TestRoundTrip(GloTestLog testLog)
    {
        // Create dictionary, write values of differing types
        var original = new GloStringDictionary();
        original.Set("name", "Cube01");
        original.Set("note", "unit cube");
        GloStringDictionaryOperations.WriteDouble(original, "size", 1.5);
        GloStringDictionaryOperations.WriteBool(original, "visible", true);

        // Export to JSON
        string serializedJson = original.ExportJson(indented: false);
        Console.WriteLine($"Serialized JSON: {serializedJson}");


        // Import from JSON
        var restored = new GloStringDictionary();
        restored.ImportJson(serializedJson);

        // Test values are correctly imported, as basic strings
        testLog.AddResult("restored.Get(name) == Cube01", (restored.Get("name") == "Cube01"), restored.ExportJson(indented: false));
        testLog.AddResult("restored.Get(note) == unit cube", (restored.Get("note") == "unit cube"));
        testLog.AddResult("restored.Get(visible) == True", (restored.Get("visible") == "True"));

        // Test missing values
        testLog.AddResult("restored.Has(notexist)", (restored.Has("notexist") == false));

        // Test values, read back in their original types
        testLog.AddResult(
            "ReadDouble(size) == 1.5",
            GloValueUtils.EqualsWithinTolerance(GloStringDictionaryOperations.ReadDouble(restored, "size"), 1.5));

        testLog.AddResult(
            "ReadDouble(visible) returns fallback -1 (invalid type)",
            GloValueUtils.EqualsWithinTolerance(GloStringDictionaryOperations.ReadDouble(restored, "visible"), -1));

        testLog.AddResult(
            "ReadBool(visible) == true", GloStringDictionaryOperations.ReadBool(restored, "visible") == true);

    }
}