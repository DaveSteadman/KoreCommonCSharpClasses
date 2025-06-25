using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace KoreCommon;

public static partial class KoreNumeric1DArrayOps<T> where T : struct, INumber<T>
{
    // --------------------------------------------------------------------------------------------
    // MARK: Create Array Series
    // --------------------------------------------------------------------------------------------

    // Functions to create arrays based on start and step values.
    // Usage: var array = KoreNumeric1DArrayOps.CreateArray(0, 1, 10);
    public static KoreNumeric1DArray<T> CreateArrayByStep(T start, T step, int count)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than zero.");
        var array = new KoreNumeric1DArray<T>(count);
        for (int i = 0; i < count; i++)
        {
            array[i] = start + step * T.CreateChecked(i);
        }
        return array;
    }

    // Create a sequence of values from start to end, with a best-fit division of the remaining range between the two.
    // Upscales to double precision to avoid integer rounding hassles.
    // Usage: var array = KoreNumeric1DArrayOps<int>.CreateArray(0, 8, 4) => [0, 2, 5, 8]
    public static KoreNumeric1DArray<T> CreateArrayByCount(T start, T end, int count)
    {
        if (count < 2) throw new ArgumentOutOfRangeException(nameof(count), "Count must be at least 2.");

        var array = new KoreNumeric1DArray<T>(count);

        // Promote to double for precise interpolation
        double startD = double.CreateChecked(start);
        double endD = double.CreateChecked(end);
        double range = endD - startD;

        for (int i = 0; i < count; i++)
        {
            double t = (double)i / (count - 1);
            double value = startD + range * t;

            array[i] = T.CreateChecked(value); // Cast back to T
        }

        return array;
    }


}


