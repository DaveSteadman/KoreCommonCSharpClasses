using System;

using KoreCommon;
namespace KoreCommon.UnitTest;


public static class KoreTestLangStrings
{
    // Usage: KoreTestLangStrings.RunTests(testLog);
    public static void RunTests(KoreTestLog testLog)
    {
        try
        {
            TestLangStrings(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("KoreTestLangStrings RunTests // Exception: ", false, ex.Message);
            return;
        }
    }

    // Test to create a basic color - convert it to JSON (text) and then back again, to ensure all the lists are correctly serialized and deserialized.
    public static void TestLangStrings(KoreTestLog testLog)
    {
        // Test the retrieval of strings in different languages
        {
            KoreLangStrings.Initialize("./KoreCommon/CoreApp/LangStrings.txt");

            KoreLangStrings.Instance.SetActiveLanguage("English");
            testLog.AddComment($"KoreLangStrings Get (English) Hello: {KoreLangStrings.Instance.Get("Hello")}");

            testLog.AddComment($"KoreLangStrings GetForLanguage (Spanish) Hello: {KoreLangStrings.Instance.GetForLanguage("Hello", "Spanish")}");

            // List languages
            var languages = KoreLangStrings.Instance.GetAvailableLanguages();
            testLog.AddComment("KoreLangStrings Languages: " + string.Join(", ", languages));
        }
    }
}





