using System;

public static class GloTestLangStrings
{
    // Usage: GloTestLangStrings.RunTests(testLog);
    public static void RunTests(GloTestLog testLog)
    {
        try
        {
            TestLangStrings(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("GloTestLangStrings RunTests // Exception: ", false, ex.Message);
            return;
        }
    }

    // Test to create a basic color - convert it to JSON (text) and then back again, to ensure all the lists are correctly serialized and deserialized.
    public static void TestLangStrings(GloTestLog testLog)
    {
        // Test the retrieval of strings in different languages
        {
            GloLangStrings.Initialize("./Common/CoreApp/LangStrings.txt");

            GloLangStrings.Instance.SetActiveLanguage("English");
            testLog.AddComment($"GloLangStrings Get (English) Hello: {GloLangStrings.Instance.Get("Hello")}");

            testLog.AddComment($"GloLangStrings GetForLanguage (Spanish) Hello: {GloLangStrings.Instance.GetForLanguage("Hello", "Spanish")}");

            // List languages
            var languages = GloLangStrings.Instance.GetAvailableLanguages();
            testLog.AddComment("GloLangStrings Languages: " + string.Join(", ", languages));
        }

    }
}





