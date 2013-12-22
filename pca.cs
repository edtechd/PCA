using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
//using testc

namespace ImageSegmentationLib
{
    class pcaClass
    {
        public byte[] data;
        public byte[] markers;

        public double[] vectors;

        public double[] Q;
        public double[] R;

        public double[] eigenvectors;
        public double[] eigenvalues;

        public double[] reducematrix = new double[0];

        public int width;
        public int height;

        // загрузка побайтовых данных из файла
        public void LoadData(string filename, int width, int height)
        {
            this.width = width;
            this.height = height;
            data = new byte[width * height];

            FileStream fs = new FileStream(filename, FileMode.Open);
            fs.Read(data, 0, width * height);
            fs.Close();
        }

        // загрузка побайтовых данных из файла + маркеров из другого файла
        public void LoadDataMarked(string filename, string markersfile, int width, int height)
        {
            this.width = width;
            this.height = height;
            data = new byte[width * height];
            markers = new byte[width * height];

            FileStream fs = new FileStream(filename, FileMode.Open);
            fs.Read(data, 0, width * height);
            fs.Close();
            fs = new FileStream(markersfile, FileMode.Open);
            fs.Read(markers, 0, width * height);
            fs.Close();
        }

        // построение векторов данных
        public void BuildVectors(int size)
        {
            int vsize = size * size * 4+4*size+1;
            int c = 0, c2=0;
            vectors = new double[vsize * width * height];

            for (int i = size; i < this.height - size; i++)
            //Parallel.For(size, this.height - size, delegate(int i)
            {

                for (int j = size; j < this.width - size; j++)
                {
                    c2 = 0;
                    for (int k = i - size; k <= i + size; k++)
                        for (int l = j - size; l <= j + size; l++)
                        {
                            vectors[vsize * c + c2] = data[width * k + l];
                            c2++;
                        }

                    c++;
                }
            } //);
        }

        // построение векторов данных для  размеченных данных
        public int BuildVectorsMarked(int size)
        {
            int vsize = size * size * 4 + 4 * size + 1;
            int c = 0, c2 = 0;
            int markerscount = 0;

            for (int i = 0; i < markers.Length; i++)
                if (markers[i] > 0)
                    markerscount++;

            vectors = new double[vsize * markerscount];

            for (int i = size; i < this.height - size; i++)
            //Parallel.For(size, this.height - size, delegate(int i)
            {

                for (int j = size; j < this.width - size; j++)
                {
                    if (markers[width * i + j] == 0)
                        continue;
                    c2 = 0;
                    for (int k = i - size; k <= i + size; k++)
                        for (int l = j - size; l <= j + size; l++)
                        {
                            vectors[vsize * c + c2] = data[width * k + l];
                            c2++;
                        }

                    c++;
                }
            } //);

            return markerscount;
        }

        public int LoadVectors(string filename, int width)
        {
            this.width = width;
            

            long numbytes = new FileInfo(filename).Length;

            vectors = new double[numbytes / sizeof(double)];
            byte[] data = new byte[numbytes];

            FileStream fs = new FileStream(filename, FileMode.Open);
            fs.Read(data, 0, data.Length);
            Buffer.BlockCopy(data, 0, vectors, 0, data.Length);
            fs.Close();
            return (int)(numbytes / sizeof(double) / width);
        }
        

        public double[] MultiplyMatrices(double[] b, double[] a, int w2, int h2, int w1, int h1)
        {
            double[] result = new double[w1 * h2];

            Parallel.For(0, w1, delegate(int i)
            {
                //for (int i = 0; i < w1; i++)
                for (int j = 0; j < h2; j++)
                    for (int k = 0; k < w2; k++)
                        result[w1 * j + i] += a[w1 * k + i] * b[w2 * j + k];  // r[i,j]+=a[i,k]*b[k,j]
            });

            return result;
        }

        public double[] MultiplyMatrices(ref double[] b, ref double[] a, int w2, int h2, int w1, int h1)
        {
            double[] result = new double[w1 * h2];


            for (int i = 0; i < w1; i++)
                for (int j = 0; j < h2; j++)
                    for (int k = 0; k < w2; k++)
                        result[w1 * j + i] += a[w1 * k + i] * b[w2 * j + k];  // r[i,j]+=a[i,k]*b[k,j]

            return result;
        }

        public double[] SumMatrixes(double[] a, double[] b, int w, int h, double koef2)
        {
            double[] result = new double[w * h];

            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    result[i * w + j] = a[i * w + j] + koef2 * b[i * w + j];

            return result;
        }

        public void SumMatrixes(ref double[] a,ref double[] b, int w, int h, double koef2)
        {            
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                     a[i * w + j] += koef2 * b[i * w + j];            
        }

        public double[] TransposeMatrix(double[] a, int w, int h)
        {
            double[] result = new double[w * h];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                        result[j * h + i] = a[i * w + j];

            return result;
        }

        public double[] TransposeMatrix(ref double[] a, int w, int h)
        {
            double[] result = new double[w * h];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    result[j * h + i] = a[i * w + j];

            return result;
        }

        public double[] UMatrix(int w, int h)
        {
            double[] result = new double[w * h];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    result[i * w + j] = 1;

            return result;

        }

        public double[] UnityMatrix(int w, int h)
        {
            double[] result = new double[w * h];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    result[i * w + j] = i==j ? 1 : 0;

            return result;

        }
        public double[] MultiplyMatrixCoef(double[] a, double koef)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] = a[i] * koef;
            return a;
        }

        public void MultiplyMatrixCoef(ref double[] a, double koef)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] = a[i] * koef;
        }

        // перемножение матрицы U (состоящей из единиц) на a
        // b = 1
        // w2 = h1
        // h2 = h1
        public double[] MultiplyUMatrices(double[] a, int w1, int h1)
        {
            double[] result = new double[w1 * h1];

            int count = 0;
          //  for (int i = 0; i < w1; i++)
            Parallel.For(0, w1, delegate(int i)
            {
               // Console.WriteLine(Convert.ToString(count++ * 100 / w1) + "% of U matrix completed");
                for (int j = 0; j < h1; j++)
                    for (int k = 0; k < h1; k++)
                        result[w1 * j + i] += a[w1 * k + i];
            });

            return result;
        }




        // подсчёт матрицы ковариации
        // алгоритм - http://stattrek.com/matrix-algebra/covariance-matrix.aspx
        //
        public double[] CovarianceMatrix(double[] m, int w, int h)
        {            

            double[] s = MultiplyUMatrices(m, w, h);
            SumMatrixes(ref m, ref s, w, h, -1.0 / h);
            double[] s2 = TransposeMatrix(ref m, w, h);
            double[] cov = MultiplyMatrices(s2, m, h, w, w, h);
            MultiplyMatrixCoef(ref cov, 1.0 / h);
            return cov;
        }

        // построение QR-декомпощиции методом вращений
        // описание метода вращения http://algorithmlib.org/rotation_slay

        public void QRDecomposition(double[] m, int w)
        {
            double a, b, c, s, t;

            double[] q = UnityMatrix(w, w);

            for (int i = 0; i < w; i++)
            {
                 for (int j = i + 1; j < w; j++)
               // Parallel.For(i + 1, w, delegate(int j)
                 {
                     a = m[i * w + i];
                     b = m[j * w + i];
                     c = a / Math.Sqrt(a * a + b * b);
                     s = b / Math.Sqrt(a * a + b * b);

                     for (int k = i; k < w; k++)
                     {
                         t = m[i * w + k];
                         m[i * w + k] = c * m[i * w + k] + s * m[j * w + k];
                         m[j * w + k] = -s * t + c * m[j * w + k];
                     }

                     // матрица Q находится перемножением транспонированных матриц вращения 
                     for (int k = 0; k < w; k++)
                     {
                         t = q[k * w + i];
                         q[k * w + i] = c * q[k * w + i] + s * q[k * w + j];
                         q[k * w + j] = -s * t + c * q[k * w + j];
                     }
                 } //);
            }
            Q = q;
            R = m;
        }

        public double[] BuildEigenVectors(double[] m, int w, int iterations)
        {
            QRDecomposition(m, w);
            eigenvalues = new double[w * w];
            double[] result = UnityMatrix(w,w);
            width = w;

            for (int i = 0; i < iterations; i++)
            {
                result = MultiplyMatrices(result, Q, w, w, w, w);
                eigenvalues = MultiplyMatrices(R, Q, w, w, w, w);
                QRDecomposition(eigenvalues, w);
            }

            eigenvectors = result;
            return result;
        }

        public void BuildreduceMatrix()
        {
            double max1 = eigenvalues[0], max2 = eigenvalues[width + 1];
            int max_i1 = 0, max_i2 = 1;
            double[] result = new double[2];


            for (int i = 2; i < width; i++)
            {
                if (eigenvalues[i * width + i] > max1)
                {
                    max2 = max1;
                    max_i2 = max_i1;
                    max1 = eigenvalues[i * width + i];
                    max_i1 = i;
                }
                else if (eigenvalues[i * width + i] > max2)
                {
                    max2 = eigenvalues[i * width + i];
                    max_i2 = i;
                }
            }

            reducematrix = new double[2 * width];

            for (int i = 0; i < width; i++)
            {
                reducematrix[i * 2] = eigenvectors[i * width + max_i1];
                reducematrix[i * 2 + 1] = eigenvectors[i * width + max_i2];
            }
        }

        
        public double[] ReduceVector(double[] source)
        {
            if (reducematrix.Length == 0)
            {
                BuildreduceMatrix();
               /* double max1 = eigenvalues[0], max2 = eigenvalues[width + 1];
                    int max_i1 = 0, max_i2 = 1;
                    double[] result = new double[2];
            

                    for (int i = 2; i < width; i++)
                    {
                        if (eigenvalues[i * width + i] > max1)
                        {
                            max2 = max1;
                            max_i2 = max_i1;
                            max1 = eigenvalues[i * width + i];
                            max_i1 = i;
                        }
                        else if (eigenvalues[i * width + i] > max2)
                        {
                            max2 = eigenvalues[i * width + i];
                            max_i2 = i;                    
                        }
                    }

                        reducematrix = new double[2 * width];

                        for (int i = 0; i < width; i++)
                        {
                            reducematrix[i * 2] = eigenvectors[i * width + max_i1];
                            reducematrix[i * 2 + 1] = eigenvectors[i * width + max_i2];
                        }*/
            }

            return MultiplyMatrices(source, reducematrix, width, 1, 2, width);
        }



        public double[] ReconstructVector(double[] source)
        {
            if (reducematrix.Length > 0)
            {
                return MultiplyMatrices(source, TransposeMatrix(reducematrix,2,width), 2, 1, width, 2);
            }

            return null;
        }

        public void SaveReduceMatrix(string filename)
        {
            if (reducematrix.Length > 0)
            {
                FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
                byte[] writedata = new byte[reducematrix.Length * sizeof(double)];
                Buffer.BlockCopy(reducematrix, 0, writedata, 0, writedata.Length);
                fs.Write(writedata, 0, writedata.Length);
                fs.Close();
            }
        }

        public void LoadReduceMatrix(string filename,int width)
        {
            this.width = width;
            byte[] data = new byte[width * 2 * sizeof(double)];
            FileStream fs = new FileStream(filename, FileMode.Open);
            fs.Read(data, 0, data.Length);
            fs.Close();
            reducematrix = new double[width * 2];
            Buffer.BlockCopy(data, 0, reducematrix, 0, data.Length);
        }

        public void SelfTest()
        {
            double[] m1 = { 90, 60, 90, 90, 90, 30, 60, 60, 60, 60, 60, 90, 30, 30, 30 };
            double[] cov = CovarianceMatrix(m1, 3, 5);

            double[] cov_r = { 504,360,180,360,360,0,180,0,720 };

                for (int i = 0; i < 9;i++ )
                    if (cov_r[i] != Math.Round(cov[i], 4))
                        {
                            throw new Exception("Covariation test failed.");
                        }

            BuildEigenVectors(cov, 3, 100);

            double[] r1 = { 0.65580225498014655, 0.38599879538810028, -0.6487898984431627, 0.42919779654868828, 0.51636641772155289, 0.74104991335750248, 0.62105768959147911, -0.7644413990675446, 0.1729644286867946};
            double[] r2 = { 910.069953041036, 0, 0, 0, 629.11038667632545, 0, 0, 0, 44.819660282638793};

            for (int i = 0; i < 9; i++)
            {
                if (Math.Round(r1[i],4) != Math.Round(eigenvectors[i], 4))
                    throw new Exception("Eigen vectors test failed.");
                if (Math.Round(r2[i], 4) != Math.Round(eigenvalues[i], 4))
                    throw new Exception("Eigen values test failed.");
            }

            double[] v = { 30, 30, 30 };
            double[] r = ReduceVector(v);

            double[] r3 = { 51.181732233609416, 4.1377144212632579 };

            for (int i = 0; i < 2; i++)
            {
                if (Math.Round(r3[i], 4) != Math.Round(r[i], 4))
                    throw new Exception("Vector reduce test failed.");
            }
        }
    }
}
