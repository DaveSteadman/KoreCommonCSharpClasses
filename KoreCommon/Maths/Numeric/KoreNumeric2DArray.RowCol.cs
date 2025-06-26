
// global using KoreFloat2DArray  = KoreNumeric2DArray<float>;
// global using KoreDouble2DArray = KoreNumeric2DArray<double>;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace KoreCommon;

public partial class KoreNumeric2DArray<T> where T : struct, INumber<T>
{
    // --------------------------------------------------------------------------------------------
    // MARK: Row Col Access
    // --------------------------------------------------------------------------------------------

    public KoreNumeric1DArray<T> GetRow(int row)
    {
        KoreNumeric1DArray<T> rowArray = new KoreNumeric1DArray<T>(Width);
        for (int x = 0; x < Width; x++)
            rowArray[x] = Data[x, row];
        return rowArray;
    }

    public void SetRow(int row, KoreNumeric1DArray<T> rowArray)
    {
        if (rowArray.Length != Width)
            throw new ArgumentException("Length of rowArray must be equal to Width of the array.", nameof(rowArray));

        for (int x = 0; x < Width; x++)
            Data[x, row] = rowArray[x];
    }

    public KoreNumeric1DArray<T> GetCol(int col)
    {
        KoreNumeric1DArray<T> colArray = new KoreNumeric1DArray<T>(Height);
        for (int y = 0; y < Height; y++)
            colArray[y] = Data[col, y];
        return colArray;
    }

    public void SetCol(int col, KoreNumeric1DArray<T> colArray)
    {
        if (colArray.Length != Height)
            throw new ArgumentException("Length of colArray must be equal to Height of the array.", nameof(colArray));

        for (int y = 0; y < Height; y++)
            Data[col, y] = colArray[y];
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Edge functions
    // --------------------------------------------------------------------------------------------

    // array.GetEdge(KoreFloat2DArray.Edge.Top)

    public KoreNumeric1DArray<T> GetEdge(Edge e)
    {
        KoreNumeric1DArray<T> edgeArray;
        switch (e)
        {
            case Edge.Top:
                edgeArray = new KoreNumeric1DArray<T>(Width);
                for (int x = 0; x < Width; x++)
                    edgeArray[x] = Data[x, 0];
                break;

            case Edge.Bottom:
                edgeArray = new KoreNumeric1DArray<T>(Width);
                for (int x = 0; x < Width; x++)
                    edgeArray[x] = Data[x, Height - 1];
                break;

            case Edge.Left:
                edgeArray = new KoreNumeric1DArray<T>(Height);
                for (int y = 0; y < Height; y++)
                    edgeArray[y] = Data[0, y];
                break;

            case Edge.Right:
                edgeArray = new KoreNumeric1DArray<T>(Height);
                for (int y = 0; y < Height; y++)
                    edgeArray[y] = Data[Width - 1, y];
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(e), "Invalid edge specified");
        }
        return edgeArray;
    }

    public void SetEdge(KoreNumeric1DArray<T> edgeArray, Edge e)
    {
        switch (e)
        {
            case Edge.Top:
                if (edgeArray.Length != Width)
                    throw new ArgumentException("Length of edgeArray must be equal to Width of the array.", nameof(edgeArray));

                for (int x = 0; x < Width; x++)
                    Data[x, 0] = edgeArray[x];
                break;

            case Edge.Bottom:
                if (edgeArray.Length != Width)
                    throw new ArgumentException("Length of edgeArray must be equal to Width of the array.", nameof(edgeArray));

                for (int x = 0; x < Width; x++)
                    Data[x, Height - 1] = edgeArray[x];
                break;

            case Edge.Left:
                if (edgeArray.Length != Height)
                    throw new ArgumentException("Length of edgeArray must be equal to Height of the array.", nameof(edgeArray));

                for (int y = 0; y < Height; y++)
                    Data[0, y] = edgeArray[y];
                break;

            case Edge.Right:
                if (edgeArray.Length != Height)
                    throw new ArgumentException("Length of edgeArray must be equal to Height of the array.", nameof(edgeArray));

                for (int y = 0; y < Height; y++)
                    Data[Width - 1, y] = edgeArray[y];
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(e), "Invalid edge specified");
        }
    }


}
