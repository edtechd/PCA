using System;
using System.Collections.Generic;
using System.Text;

namespace ImageSegmentationLib
{
    class Program
    {
        static void ProcessData()
        {
            double time1 = DateTime.Now.TimeOfDay.TotalMilliseconds;
            int w = 441, h = 10000;

            pcaClass pca = new pcaClass();
            Console.WriteLine("Loading data...");
            pca.LoadData("pca_array.bin", 600, 100);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + " Building vectors...");
            pca.BuildVectors(10);
            PrintMatrix(pca.vectors, 9, 9);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + " Calculating covariance matrix...");
            double[] cov = pca.CovarianceMatrix(pca.vectors, w, h);
            //PrintMatrix(cov, 9, 9);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + " Calculating eigen vectors...");
            pca.BuildEigenVectors(cov, w, 100);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + " Reducing vector...");

        }

        static void ProcessDataMarkers()
        {
            double time1 = DateTime.Now.TimeOfDay.TotalMilliseconds;
            int w = 441, h = 10000;

            pcaClass pca = new pcaClass();
            Console.WriteLine("Loading data...");
            pca.LoadDataMarked("pca_array.bin", "pca_markers.bin", 600, 600);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + " Building vectors...");
            int c = pca.BuildVectorsMarked(10);
            Console.WriteLine(c.ToString() + " vectors loaded.");
            //PrintMatrix(pca.vectors, 9, 9);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + " Calculating covariance matrix...");
            double[] cov = pca.CovarianceMatrix(pca.vectors, w, h);
            //PrintMatrix(cov, 9, 9);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + " Calculating eigen vectors...");
            pca.BuildEigenVectors(cov, w, 100);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + " Building reduce matrix...");
            pca.BuildreduceMatrix();
            pca.SaveReduceMatrix("reduce1.bin");
        }

        static void PreciseVectors()
        {
            double time1 = DateTime.Now.TimeOfDay.TotalMilliseconds;
            int w = 441;

            pcaClass pca = new pcaClass();
            Console.WriteLine("Loading data...");
            //pca.LoadDataMarked("pca_array.bin", "pca_markers.bin", 600, 600);
            int num = pca.LoadVectors("vectors_cc0000.bin", 441);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) +" "+num.ToString() + " vectors loaded.");
            //PrintMatrix(pca.vectors, 9, 9);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + " Calculating covariance matrix...");
            double[] cov = pca.CovarianceMatrix(pca.vectors, w, num);
            //PrintMatrix(cov, 9, 9);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + " Calculating eigen vectors...");
            pca.BuildEigenVectors(cov, w, 100);
            Console.WriteLine(Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + " Building reduce matrix...");
            pca.BuildreduceMatrix();
            pca.SaveReduceMatrix("reduce3.bin");
        }

        static void TestData()
        {
            pcaClass pca = new pcaClass();
            double[] m = { 90, 60, 90, 90, 90, 30, 60, 60, 60, 60, 60, 90, 30, 30, 30 };
           int w = 3, h = 5;

            double[] v = { 30, 30, 30, 30, 30, 30, 30, 30, 30 };

            double[] cov = pca.CovarianceMatrix(m, w, h);

            pca.BuildEigenVectors(cov,3,100);
            double[] r = pca.ReduceVector(v);
            PrintMatrix(r, 2, 1);

            pca.SaveReduceMatrix("reduce1.bin");
            pca = new pcaClass();

            pca.LoadReduceMatrix("reduce1.bin",3);
            r = pca.ReduceVector(v);
            PrintMatrix(r, 2, 1);


            //double[] r = pca.ReduceVector(v);
            //PrintMatrix(r, 2, 1);
            //Console.WriteLine("Total time: "+Convert.ToString(DateTime.Now.TimeOfDay.TotalMilliseconds - time1) + ".");


            /*
            double[] e = { 1, 0, 0, 0, 1, 0, 0, 0, 1 };

            double[] m2 = { 504, 360, 180, 360, 360, 0, 180, 0, 720 };
            double[] m3 = { 504, 360, 180, 360, 360, 0, 180, 0, 720 };

            double[] m4 = { 11, 1, 1, 11 };

            double[] v = { 30, 30, 30 };

            //pca.QRDecomposition(m2, 3);
            double[] a = pca.BuildEigenVectors(m2, 3,40);
            PrintMatrix(a, 3, 3);
            double[] r = pca.ReduceVector(v);
            PrintMatrix(pca.eigenvalues, 3, 3);
             * 
            PrintMatrix(r, 2, 1);
            PrintMatrix(pca.ReconstructVector(r), 3, 1);*/

            /*
            PrintMatrix(pca.vectors, 9, 3);
            PrintMatrix(pca.CovarianceMatrix(pca.vectors,9,3), 9, 9);*/


            /*
             * PrintMatrix(m, 3, 5);
            //PrintMatrix(e, 3, 3);

            double[] a = pca.SumMatrixes(m, pca.MultiplyMatrices(m, pca.UMatrix(h, h), w, h, h, h), w, h, -1.0/h);
            double[] cov = pca.MultiplyMatrixCoef( pca.MultiplyMatrices(a, pca.TransposeMatrix(a, w, h), w, h, h, w),1.0/h);
            PrintMatrix(a, w, h);
            PrintMatrix(pca.TransposeMatrix(a, w, h), h, w);
            PrintMatrix(cov, w, w);
            
            //PrintMatrix(pca.MultiplyMatrices(m,e,2,3,3,3), 2, 3);
             * */
        }

        static void Main(string[] args)
        {
            pcaClass pca = new pcaClass();
            pca.SelfTest();
            //TestData();
            //ProcessDataMarkers();
            PreciseVectors();
        }

        static void PrintMatrix(double[] m, int w, int h)
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    Console.Write(String.Format("{0:F4}   ", m[i * w + j]));
                    //Console.Write(" ");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }
    }
}
