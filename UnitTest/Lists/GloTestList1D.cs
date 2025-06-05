using System;
using System.Text;

public static class GloTestList1D
{
    // GloTestList1D.RunTests(testLog)
    public static void RunTests(GloTestLog testLog)
    {
        Test1DList(testLog);
        Test1DIntList(testLog);
    }

    private static void Test1DList(GloTestLog testLog)
    {
        {
            GloNumeric1DArray<double> list = new GloNumeric1DArray<double>(10);

            // sums to 45
            for (int i = 0; i < 10; i++)
                list[i] = i;
            testLog.AddResult("GloNumeric1DArray List<double> Sum", GloValueUtils.EqualsWithinTolerance(list.Sum(), 45.0));

            testLog.AddResult("GloNumeric1DArray Length", GloValueUtils.EqualsWithinTolerance(list.Length, 10));
            list.Add(10.0);
            testLog.AddResult("GloNumeric1DArray Length After Add", GloValueUtils.EqualsWithinTolerance(list.Length, 11));
            list.RemoveAtIndex(3);
            testLog.AddResult("GloNumeric1DArray Length After RemoveAtIndex", GloValueUtils.EqualsWithinTolerance(list.Length, 10));
        }
    }

    private static void Test1DIntList(GloTestLog testLog)
    {
        {
            var list = GloNumeric1DArrayOperations<int>.CreateArrayByStep(0, 1, 10);

            // print the array
            StringBuilder sb = new StringBuilder();
            sb.Append("GloNumeric1DArray List<int>: GloNumeric1DArrayOperations<int>.CreateArrayByStep(0, 1, 10); => ");
            sb.AppendJoin(", ", list);
            testLog.AddComment(sb.ToString());
        }
        {
            StringBuilder sb = new StringBuilder();

            var list = GloNumeric1DArrayOperations<int>.CreateArrayByCount(0, 8, 4);
            sb.Append("GloNumeric1DArray List<int>: GloNumeric1DArrayOperations<int>.CreateArrayByCount(0, 8, 4); => ");
            sb.AppendJoin(", ", list);
            testLog.AddComment(sb.ToString());
        }
        {
            StringBuilder sb = new StringBuilder();

            var list = GloNumeric1DArrayOperations<float>.CreateArrayByCount(0, 10, 20);
            sb.Append("GloNumeric1DArray List<float>: GloNumeric1DArrayOperations<float>.CreateArrayByCount(0, 10, 21); => ");
            sb.AppendJoin(", ", list.Select(x => x.ToString("F2")));
            testLog.AddComment(sb.ToString());
        }
    }
}


