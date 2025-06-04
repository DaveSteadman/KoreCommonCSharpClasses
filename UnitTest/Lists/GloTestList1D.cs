using System;

public static class GloTestList1D
{
    // GloTestList1D.RunTests(testLog)
    public static void RunTests(GloTestLog testLog)
    {
        Test1DList(testLog);

    }

    public static void Test1DList(GloTestLog testLog)
    {
        {
            GloNumeric1DArray<double> list = new GloNumeric1DArray<double>(10);

            // sums to 45
            for (int i = 0; i < 10; i++)
                list[i] = i;
            testLog.Add("GloNumeric1DArray List<double> Sum", GloValueUtils.EqualsWithinTolerance(list.Sum(), 45.0));

            testLog.Add("GloNumeric1DArray Length", GloValueUtils.EqualsWithinTolerance(list.Length, 10));
            list.Add(10.0);
            testLog.Add("GloNumeric1DArray Length After Add", GloValueUtils.EqualsWithinTolerance(list.Length, 11));
            list.RemoveAtIndex(3);
            testLog.Add("GloNumeric1DArray Length After RemoveAtIndex", GloValueUtils.EqualsWithinTolerance(list.Length, 10));
        }
    }
}