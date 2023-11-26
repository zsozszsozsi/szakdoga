using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PCA
{
    private float[,] Data;
    public float[,] TransformedData;

    private int DimensionCount;   

    private int RowCount;
    private int ColCount;

    private float[] Mean;
    private float[] Deviation;
    private float[,] CovMatrix;
    private MathNet.Numerics.LinearAlgebra.Vector<Complex> EigenValues;
    private Matrix<float> EigenVectors;
    private Matrix<float> PrincipalComponents;
    private float[] Variance;

    private int[] SelectedCols;

    private List<int> ZeroDevCols = new List<int>();

    public PCA(float[,] data, int dimensionCount)
    {
        DimensionCount = dimensionCount;

        RowCount = data.GetLength(0);
        ColCount = data.GetLength(1);

        Variance = new float[ColCount];

        //Debug.Log(RowCount + ", row count");
        //Debug.Log(ColCount + ", col count");

        //Debug.Log("---Default---");
        Data = new float[RowCount,ColCount];
        
        TransformedData = new float[RowCount, DimensionCount];

        for (int i = 0; i < RowCount; i++)
        {
            //string tmp = i + ": ";
            for(int j = 0; j < ColCount; j++)
            {
                Data[i, j] = data[i, j];
                //tmp += Data[i, j] + "\t";
            }

            //Debug.Log(tmp);
        }

        Mean = new float[ColCount];
        Deviation = new float[ColCount];
        

    }

    public void Compute()
    {
        CalculateMean();
        CalculateDeviation();

        /*string mean = "";
        string std = "";
        for (int i = 0; i < ColCount; i++)
        {
            mean += Mean[i] + "\t";
            if(i % 28 == 0) mean += "\n";

            std += Deviation[i] + "\t";
            if (i % 28 == 0) std += "\n";

        }

        Debug.Log(mean);
        Debug.Log(std);*/
        RemoveZeroDeviationCols();
        Standardization();

        /*string tmp = "";
        for(int i = 0; i < RowCount; i++)
        {
            for(int j = 0; j < ColCount; j++)
            {
                tmp += Data[i, j] + "\t";
            }
            tmp += "\n";
        }
        Debug.Log(tmp);*/

        CovMatrix = new float[ColCount, ColCount];
        CalculateCoviranceMatrix();

        /*string tmp = "";
        for (int i = 0; i < ColCount; i++)
        {
            for (int j = 0; j < ColCount; j++)
            {
                tmp += CovMatrix[i, j] + "\t";
            }
            tmp += "\n";
        }
        Debug.Log(tmp);*/
        CalculateEigenVectors();

        /*string temp = "";
        for (int i = 0; i < Variance.Length; i++)
        {
            temp += Variance[i] + "\t";
        }
        //Debug.Log(temp);

        temp = "";
        for (int i = 0; i < EigenValues.Count; i++)
        {
            temp += EigenValues[i].Real + "\t";
        }*/

        //Debug.Log(temp);
        TransformData();

        //ReverseStandardization();

        /*for (int i = 0; i < EigenVectors.RowCount; i++)
        {
            string tmp = "";
            for (int j = 0; j < EigenVectors.ColumnCount; j++)
            {
                tmp += EigenVectors[i, j] + "\t";
            }
            //Debug.Log(tmp);
        }*/

        /*for(int i = 0; i < CovMatrix.GetLength(0); i++)
        {
            string tmp = "";
            for (int j = 0; j < CovMatrix.GetLength(1); j++)
            {
                tmp += CovMatrix[i, j] + "\t";
            }
            Debug.Log(tmp);
        }*/


    }

    /*
            features: 784
            i: row
            j: col
            
            data = [
                    F1,F2,F3...,F784
                    [1,2,3,4,...,784],
                    [1,2,3,4,...,784],
                    ...
                    [1,2,3,4,...,784]
                   ]

     */

    private void CalculateMean()
    {

        float[] sums = new float[ColCount];

        for(int row = 0; row < RowCount; row++)
        {
            for (int col = 0; col < ColCount; col++)
            {
                sums[col] += Data[row,col];
            }
        }

        for(int i = 0; i < sums.GetLength(0); i++)
        {
            Mean[i] = sums[i] / RowCount;
        }
    }

    private void CalculateDeviation()
    {
        float[] sums = new float[ColCount];

        for (int row = 0; row < RowCount; row++)
        {
            for (int col = 0; col < ColCount; col++)
            {
                sums[col] += Mathf.Pow(Data[row, col] - Mean[col], 2);
            }
        }

        for(int i = 0; i < sums.GetLength(0); i++)
        {
            Deviation[i] = Mathf.Sqrt(sums[i] / (RowCount-1) );
            if(Deviation[i] == 0)
            {
                ZeroDevCols.Add(i);
            }
        }
    }

    private void RemoveZeroDeviationCols()
    {
        var tmpData = new float[RowCount,ColCount-ZeroDevCols.Count];
        var tmpMean = new float[ColCount - ZeroDevCols.Count];
        var tmpDeviation = new float[ColCount - ZeroDevCols.Count];

        for (int i = 0, idx = 0; i < ColCount; i++)
        {
            if (ZeroDevCols.Contains(i))
            {
                continue;
            }
            tmpMean[idx] = Mean[i];
            tmpDeviation[idx] = Deviation[i];
            idx++;
        }

        for(int i = 0; i < RowCount; i++)
        {
            for(int j = 0, col = 0; j < ColCount; j++)
            {
                if (ZeroDevCols.Contains(j))
                {
                    continue;
                }
                tmpData[i,col] = Data[i,j];
                col++;
            }
        }

        Data = tmpData;
        Mean = tmpMean;
        Deviation = tmpDeviation;
        ColCount = Data.GetLength(1);
    }

    private void Standardization()
    {
        for (int row = 0; row < RowCount; row++)
        {
            for (int col = 0; col < ColCount; col++)
            {
                var Z = (Data[row, col] - Mean[col]) / Deviation[col];
                Data[row, col] = Z;
            }
        }
        
    }

    private float Cov(int col1, int col2)
    {
        // Cov = E(X*Y) - E(X)*E(Y)
        // E (X) == E(Y) = 0
        // Cov = E(X*Y)
        // Cov =  SUM{ (X - MEAN(X))(Y - MEAN(Y)) } / Number of data points
        // Because Standardization MEAN(X) and MEAN(Y) are equal to 0
        // Cov = SUM (X * Y) / Number of data points

        if (col1 == col2)
            return Var(col1);

        float sums = 0f;
        for(int row = 0; row < RowCount; row++)
        {
            sums += Data[row, col1] * Data[row, col2];
        }


        return sums/RowCount;
    }

    private float Var(int col)
    {
        float sums = 0f;

        for(int row = 0; row < RowCount; row++)
        {
            sums += Mathf.Pow(Data[row, col], 2);
        }
        return sums / RowCount;
    }

    private void CalculateCoviranceMatrix()
    {
        for (int col1 = 0; col1 < ColCount; col1++)
        {
            CovMatrix[col1, col1] = Cov(col1, col1);

            for (int col2 = col1+1; col2 < ColCount; col2++)
            {
                CovMatrix[col1, col2] = Cov(col1, col2);
                CovMatrix[col2, col1] = CovMatrix[col1, col2];
            }
        }
        
    }

    private void CalculateEigenVectors()
    {
        var matrix = Matrix<float>.Build.DenseOfArray(CovMatrix);
        Evd<float> evd = matrix.Evd(Symmetricity.Symmetric);

        EigenValues = evd.EigenValues;
        EigenVectors = evd.EigenVectors;

        var topValues = EigenValues.Real().OrderByDescending(x => x).ToArray();

        for(int i = 0; i < DimensionCount; i++)
        {
            Variance[i] = Convert.ToSingle(topValues[i] / topValues.Sum());
            Debug.Log(Variance[i]);
        }

        var tempCols = new float[DimensionCount][];
        int count = 0;
        SelectedCols = new int[DimensionCount];

        foreach(var item in topValues.Take(DimensionCount).ToArray())
        {
            int id = EigenValues.Real().ToList().IndexOf(item);
            SelectedCols[count] = id;
            tempCols[count] = EigenVectors.Column(id).ToArray();
            count++;
        }

        PrincipalComponents = Matrix<float>.Build.DenseOfColumnArrays(tempCols);

    }

    private void TransformData()
    { 
        // TransformedData =  Data * FeatureVector

        TransformedData = Matrix<float>.Build.DenseOfArray(Data).Multiply(PrincipalComponents).ToArray();


        for (int i = 0; i < TransformedData.GetLength(0); i++)
        {
            string tmp = "";
            for(int j = 0; j < TransformedData.GetLength(1); j++)
            {
                tmp += TransformedData[i, j] + "\t";
            }
            //Debug.Log(tmp);
        }
    }

    private void ReverseStandardization()
    {
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < TransformedData.GetLength(1); j++)
            {
                var Z = TransformedData[i, j] * Deviation[SelectedCols[j]] + Mean[SelectedCols[j]];
                TransformedData[i, j] = Z;
            }
        }
    }
}
