using System;

public static class GloTestMath
{
    public static void RunTests(GloTestLog testLog)
    {
        TestValueUtilsBool(testLog);
        TestValueUtilsInt(testLog);
        TestValueUtilsFloat(testLog);
        TestFloat1DArray_Basics(testLog);
    }

    public static void TestValueUtilsBool(GloTestLog testLog)
    {
        testLog.AddResult("BoolToStr(true)",      (GloValueUtils.BoolToStr(true)    == "True"));
        testLog.AddResult("BoolToStr(false)",     (GloValueUtils.BoolToStr(false)   == "False"));

        testLog.AddResult("StrToBool(\"True\")",  (GloValueUtils.StrToBool("True")  == true));
        testLog.AddResult("StrToBool(\"False\")", (GloValueUtils.StrToBool("False") == false));
        testLog.AddResult("StrToBool(\"true\")",  (GloValueUtils.StrToBool("true")  == true));
        testLog.AddResult("StrToBool(\"false\")", (GloValueUtils.StrToBool("false") == false));
        testLog.AddResult("StrToBool(\"y\")",     (GloValueUtils.StrToBool("y")     == true));
        testLog.AddResult("StrToBool(\"n\")",     (GloValueUtils.StrToBool("n")     == false));

        testLog.AddResult("StrToBool(\" \")",     (GloValueUtils.StrToBool(" ")     == false));
        testLog.AddResult("StrToBool(\"1212\")",  (GloValueUtils.StrToBool("1212")  == false));
    }

    public static void TestValueUtilsInt(GloTestLog testLog)
    {
        testLog.AddResult("Clamp( 0, 1, 10)", (GloValueUtils.Clamp( 0, 1, 10) == 1));
        testLog.AddResult("Clamp( 5, 1, 10)", (GloValueUtils.Clamp( 5, 1, 10) == 5));
        testLog.AddResult("Clamp(11, 1, 10)", (GloValueUtils.Clamp(11, 1, 10) == 10));

        testLog.AddResult("Wrap( 0, 1, 10)", (GloValueUtils.Wrap( 0, 1, 10) == 10));
        testLog.AddResult("Wrap( 5, 1, 10)", (GloValueUtils.Wrap( 5, 1, 10) == 5));
        testLog.AddResult("Wrap(11, 1, 10)", (GloValueUtils.Wrap(11, 1, 10) == 1));
    }

    public static void TestValueUtilsFloat(GloTestLog testLog)
    {
        // Test for Modulo operation
        testLog.AddResult("Modulo 1.1 % 1.0",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.Modulo(1.1f, 1.0f), 0.1f));
        testLog.AddResult("Modulo 2.1 % 1.0",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.Modulo(2.1f, 1.0f), 0.1f));
        testLog.AddResult("Modulo -0.1 % 1.0", GloValueUtils.EqualsWithinTolerance(GloValueUtils.Modulo(-0.1f, 1.0f), 0.9f));

        // Test for LimitToRange operation
        testLog.AddResult("LimitToRange 1.1 in 0-1", GloValueUtils.EqualsWithinTolerance(GloValueUtils.LimitToRange(1.1f, 0f, 1f), 1f));
        testLog.AddResult("LimitToRange -5 in 0-1",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.LimitToRange(-5f, 0f, 1f), 0f));
        testLog.AddResult("LimitToRange 0.5 in 0-1", GloValueUtils.EqualsWithinTolerance(GloValueUtils.LimitToRange(0.5f, 0f, 1f), 0.5f));

        // Test for WrapToRange operation
        testLog.AddResult("WrapToRange 1.1 in 1-2",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.WrapToRange(1.1f, 1f, 2f), 1.1f));
        testLog.AddResult("WrapToRange 3.1 in 1-2",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.WrapToRange(3.1f, 1f, 2f), 1.1f));
        testLog.AddResult("WrapToRange -1.5 in 1-2", GloValueUtils.EqualsWithinTolerance(GloValueUtils.WrapToRange(-1.5f, 1f, 2f), 1.5f));

        // Test for DiffInWrapRange operation
        testLog.AddResult("DiffInWrapRange 1 to 350 in 0-360",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.DiffInWrapRange(0f, 360f, 1f, 350f), -11f));
        testLog.AddResult("DiffInWrapRange 1 to 5 in 0-360",    GloValueUtils.EqualsWithinTolerance(GloValueUtils.DiffInWrapRange(0f, 360f, 1f, 5f), 4f));
        testLog.AddResult("DiffInWrapRange 340 to 20 in 0-360", GloValueUtils.EqualsWithinTolerance(GloValueUtils.DiffInWrapRange(0f, 360f, 340f, 20f), 40f));

        // Test for IndexFromFraction operation
        testLog.AddResult($"IndexFromFraction 0.1 in 0-10",   GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromFraction(0.1f, 0, 10), 1));
        testLog.AddResult($"IndexFromFraction 0.2 in 0-100",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromFraction(0.2f, 0, 100), 20));
        testLog.AddResult($"IndexFromFraction 0.49 in 0-5",   GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromFraction(0.49f, 0, 5), 2));
        testLog.AddResult($"IndexFromFraction 0.50 in 0-5",   GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromFraction(0.50f, 0, 5), 2));
        testLog.AddResult($"IndexFromFraction 0.6  in 0-5",   GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromFraction(0.6f, 0, 5), 3));

        // Test for IndexFromIncrement operation
        testLog.AddResult("IndexFromIncrement 0.1 from 0 increment 1",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromIncrement(0f, 1f, 0.1f), 0));
        testLog.AddResult("IndexFromIncrement 1.1 from 0 increment 1",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromIncrement(0f, 1f, 1.1f), 1));
        testLog.AddResult("IndexFromIncrement 13.1 from 0 increment 1", GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromIncrement(0f, 1f, 13.1f), 13));

        // Test for IsInRange operation
        testLog.AddResult("IsInRange 0 in 0-1",   GloValueUtils.IsInRange(0f, 0f, 1f));
        testLog.AddResult("IsInRange 1 in 0-1",   GloValueUtils.IsInRange(1f, 0f, 1f));
        testLog.AddResult("!IsInRange -1 in 0-1", !GloValueUtils.IsInRange(-1f, 0f, 1f));
        testLog.AddResult("!IsInRange 2 in 0-1",  !GloValueUtils.IsInRange(2f, 0f, 1f));

        // Test for Interpolate operation
        testLog.AddResult("Interpolate 0.1 between 0 and 1",    GloValueUtils.EqualsWithinTolerance(GloValueUtils.Interpolate(0f, 1f, 0.1f), 0.1f));
        testLog.AddResult("Interpolate 0.9 between 0 and 100",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.Interpolate(0f, 100f, 0.9f), 90f));
        testLog.AddResult("Interpolate 1.1 between 0 and 100",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.Interpolate(0f, 100f, 1.1f), 110f));
        testLog.AddResult("Interpolate -0.1 between 0 and 100", GloValueUtils.EqualsWithinTolerance(GloValueUtils.Interpolate(0f, 100f, -0.1f), -10f));
    }

    public static void TestFloat1DArray_Basics(GloTestLog testLog)
    {
        GloFloat1DArray array = new GloFloat1DArray(5);
        array[0] = 1.0f;
        array[1] = 2.0f;
        array[2] = 3.0f;
        array[3] = 4.0f;
        array[4] = 5.0f;

        // Test Length
        testLog.AddResult("Array Length", array.Length == 5);

        // Test individual element access
        testLog.AddResult("Array[0]", GloValueUtils.EqualsWithinTolerance(array[0], 1.0f));
        testLog.AddResult("Array[1]", GloValueUtils.EqualsWithinTolerance(array[1], 2.0f));
        testLog.AddResult("Array[2]", GloValueUtils.EqualsWithinTolerance(array[2], 3.0f));
        testLog.AddResult("Array[3]", GloValueUtils.EqualsWithinTolerance(array[3], 4.0f));
        testLog.AddResult("Array[4]", GloValueUtils.EqualsWithinTolerance(array[4], 5.0f));

        // Test Max
        testLog.AddResult("Array Max", GloValueUtils.EqualsWithinTolerance(array.Max(), 5.0f));

        // Test Min
        testLog.AddResult("Array Min", GloValueUtils.EqualsWithinTolerance(array.Min(), 1.0f));

        // Test Average
        testLog.AddResult("Array Average", GloValueUtils.EqualsWithinTolerance(array.Average(), 3.0f));

        // Test Sum
        testLog.AddResult("Array Sum", GloValueUtils.EqualsWithinTolerance(array.Sum(), 15.0f));

        // Additional tests for boundary conditions and invalid inputs
        // Assuming GloFloat1DArray handles negative indices or out-of-bound indices gracefully
        // testLog.Add("Array[-1]", GloValueUtils.EqualsWithinTolerance(array[-1], /* expected value */));
        // testLog.Add("Array[5]", GloValueUtils.EqualsWithinTolerance(array[5], /* expected value */));
    }

}