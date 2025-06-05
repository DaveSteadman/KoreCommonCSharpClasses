using System;
using System.Collections.Generic;
using System.Text;

// File to contain a test entry and the overall test log, such that we can run tests and record the
// outputs with enough information to isolate a single test failure.
// Includes a simple output report we can print every time and a detailed report when we need to debug.

// ------------------------------------------------------------------------

public enum GloTestLogEntryType { Test, Comment, Separator}

public enum GloTestLogResult { Pass, Fail, Untested }

// ------------------------------------------------------------------------


public struct GloTestLogEntry
{
    public string Name;
    public GloTestLogEntryType EntryType;
    public GloTestLogResult Result;
    public string Comment;

}

public class GloTestLog
{
    private List<GloTestLogEntry> ResultList = new List<GloTestLogEntry>();

    // --------------------------------------------------------------------------------------------
    // Add test results
    // --------------------------------------------------------------------------------------------

    public void AddResult(string name, bool result, string comment = "")
    {
        ResultList.Add(
            new GloTestLogEntry()
            {
                Name = name,
                EntryType = GloTestLogEntryType.Test,
                Result = result ? GloTestLogResult.Pass : GloTestLogResult.Fail,
                Comment = comment
            });
    }

    public void AddComment(string comment)
    {
        ResultList.Add(
            new GloTestLogEntry()
            {
                Name = "",
                EntryType = GloTestLogEntryType.Comment,
                Result = GloTestLogResult.Untested,
                Comment = comment
            });
    }

    public void AddSeparator()
    {
        ResultList.Add(new GloTestLogEntry()
        {
            Name = "",
            EntryType = GloTestLogEntryType.Separator,
            Result = GloTestLogResult.Untested,
            Comment = ""
        });
    }

    // --------------------------------------------------------------------------------------------
    // Report generation
    // --------------------------------------------------------------------------------------------

    public bool OverallPass()
    {
        bool result = true;

        // Find each entry that is a test result, and then ensure its a pass
        foreach (GloTestLogEntry entry in ResultList)
        {
            if (entry.EntryType == GloTestLogEntryType.Test)
            {
                if (entry.Result != GloTestLogResult.Pass)
                {
                    result = false;
                    break; // No need to check further, we have a fail
                }
            }
        }
        return result;
    }

    // --------------------------------------------------------------------------------------------

    public string OneLineReport()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        int passCount = 0;
        int failCount = 0;
        int untestedCount = 0;
        int testCount = 0;
        foreach (var entry in ResultList)
        {
            if (entry.EntryType == GloTestLogEntryType.Test)
            {
                testCount++;
                if (entry.Result == GloTestLogResult.Pass) { passCount++; }
                if (entry.Result == GloTestLogResult.Fail) { failCount++; }
                if (entry.Result == GloTestLogResult.Untested) { untestedCount++; }
            }
        }

        string passFailStr = "PASS";
        if (failCount > 0)
        {
            passFailStr = "FAIL";
        }
        else
        {
            if (untestedCount > 0)
                passFailStr += " (with Untested: " + untestedCount + ")";
        }

        return $"Overall:{passFailStr} // Time:{timestamp} // NumTests:{testCount} Passes:{passCount} Fails:{failCount} Untested:{untestedCount}";
    }

    // --------------------------------------------------------------------------------------------

    public string FullReport()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var entry in ResultList)
        {
            switch (entry.EntryType)
            {
                case GloTestLogEntryType.Separator:
                    sb.AppendLine("--------------------------------------------------");
                    break;

                case GloTestLogEntryType.Comment:
                    sb.AppendLine($"COMMENT: {entry.Comment}");
                    break;

                case GloTestLogEntryType.Test:
                    string resultStr = ResultToString(entry.Result);
                    string comment = string.IsNullOrEmpty(entry.Comment) ? "" : $" // {entry.Comment}";
                    sb.AppendLine($"TEST: {entry.Name} // {resultStr}{comment}");
                    break;
            }
        }

        return sb.ToString();
    }


    // --------------------------------------------------------------------------------------------

    private string ResultToString(GloTestLogResult result)
    {
        return result switch
        {
            GloTestLogResult.Pass => "PASS",
            GloTestLogResult.Fail => "FAIL",
            GloTestLogResult.Untested => "UNT",
            _ => "UNKNOWN"
        };
    }
}
