using System;

public static class GloTestList2D
{
    // GloTestList2D.RunTests(testLog)
    public static void RunTests(GloTestLog testLog)
    {
        Test2DList(testLog);

    }

    public static void Test2DList(GloTestLog testLog)
    {
        {
            GloNumeric2DArray<double> list = new GloNumeric2DArray<double>(3, 3);

            var size = list.Size;


            testLog.AddResult("GloNumeric2DArray Test2DList Size", (size.Width == 3 && size.Height == 3));

        }
    }
}