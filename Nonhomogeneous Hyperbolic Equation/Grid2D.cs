using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonhomogeneous_Hyperbolic_Equation
{
    class Grid2D
    {
        private double[,] grid;
        private readonly Func<double, double> phi;
        private readonly Func<double, double> psi;
        private readonly Func<double, double> b;
        private readonly double a;
        private readonly double L;
        private readonly double T;
        private readonly double h;
        private readonly double t;

        public Grid2D(
            Func<double, double> phi,
            Func<double, double> psi,
            Func<double, double> b, 
            double a,
            double L,
            double T,
            double h,
            double t)
        {
            this.phi = phi;
            this.psi = psi;
            this.b = b;
            this.a = a;
            this.L = L;
            this.T = T;
            this.h = h;
            this.t = t;

            grid = new double[(int)(T / t) + 1, (int)(L / h) + 1];
        }

        public void SolveHomogeneous()
        {
            int dim_L = (int)(L / h);
            int dim_T = (int)(T / t);

            // первый слой
            for (int j = 0; j <= dim_L; j++)
            {
                grid[0, j] = phi(h * j);
            }

            // второй слой без граничных точек
            for (int j = 1; j <= dim_L - 1; j++)
            {
                grid[1, j] = phi(h * j) + psi(h * j) * t +
                    ((grid[0, j + 1] - 2 * grid[0, j] + grid[0, j - 1]) * (a * a) * (t * t)) / (2 * (h * h));
            }
            // граничые точки
            grid[1, 0] = (4 * grid[1, 1] - grid[1, 2]) / 3;
            grid[1, dim_L] = (-grid[1, dim_L - 2] + 4 * grid[1, dim_L - 1]) / 3;

            // шаблон неявный T
            // конвеер
            for (int i = 2; i <= dim_T; i++)
            {
                double[,] coeff = new double[dim_L + 1, dim_L + 1];
                double[] b_coeff = new double[dim_L + 1];
           

                coeff[0, 0] = -3;
                coeff[0, 1] = 4;
                coeff[0, 2] = -1;
                b_coeff[0] = 0;

                for (int j = 1; j <= dim_L - 1; j++)
                {
                    coeff[j, j - 1] = (a * a) / (h * h);
                    coeff[j, j]     = (-2 * a * a) / (h * h) - 1 / (t * t);
                    coeff[j, j + 1] = (a * a) / (h * h);
                    b_coeff[j] = (-2 * grid[i - 1, j] + grid[i - 2, j]) / (t * t);
                }

                coeff[dim_L, dim_L - 2] = 1;
                coeff[dim_L, dim_L - 1] = -4;
                coeff[dim_L, dim_L] = 3;
                b_coeff[dim_L] = 0;


                var Matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(coeff);
                var b_vector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(b_coeff);
                var solution = Matrix.Solve(b_vector);

                for (int j = 0; j <= dim_L; j++)
                {
                    grid[i, j] = solution[j];
                }
            }
        }

        public void SolveNonhomogeneous()
        {
            int dim_L = (int)(L / h);
            int dim_T = (int)(T / t);

            // первый слой
            for (int j = 0; j <= dim_L; j++)
            {
                grid[0, j] = phi(h * j);
            }

            // второй слой без граничных точек
            for (int j = 1; j <= dim_L - 1; j++)
            {
                double intergral = MathNet.Numerics.Integration.NewtonCotesTrapeziumRule.IntegrateAdaptive(b, 0, L, 1e-5);

                grid[1, j] = phi(h * j) + psi(h * j) * t +
                    ((grid[0, j + 1] - 2 * grid[0, j] + grid[0, j - 1]) * (a * a) * (t * t)) / (2 * (h * h)) +
                    (grid[0, j] * b(j * h) + grid[0, j] * intergral) * (t * t) / 2;
            }
            // граничые точки
            grid[1, 0] = (4 * grid[1, 1] - grid[1, 2]) / 3;
            grid[1, dim_L] = (-grid[1, dim_L - 2] + 4 * grid[1, dim_L - 1]) / 3;

            // шаблон неявный T
            // конвеер
            for (int i = 2; i <= dim_T; i++)
            {
                double[,] coeff = new double[dim_L + 1, dim_L + 1];
                double[] b_coeff = new double[dim_L + 1];


                coeff[0, 0] = -3;
                coeff[0, 1] = 4;
                coeff[0, 2] = -1;
                b_coeff[0] = 0;

                for (int j = 1; j <= dim_L - 1; j++)
                {
                    double intergral = MathNet.Numerics.Integration.NewtonCotesTrapeziumRule.IntegrateAdaptive(b, 0, L, 1e-5);

                    coeff[j, j - 1] = (a * a) / (h * h);
                    coeff[j, j]     = (-2 * a * a) / (h * h) - 1 / (t * t);
                    coeff[j, j + 1] = (a * a) / (h * h);
                    b_coeff[j] = (-2 * grid[i - 1, j] + grid[i - 2, j]) / (t * t) -
                        grid[i - 1, j] * b(j * h) - grid[0, j] * intergral;
                }

                coeff[dim_L, dim_L - 2] = 1;
                coeff[dim_L, dim_L - 1] = -4;
                coeff[dim_L, dim_L] = 3;
                b_coeff[dim_L] = 0;


                var Matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(coeff);
                var b_vector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(b_coeff);
                var solution = Matrix.Solve(b_vector);

                for (int j = 0; j <= dim_L; j++)
                {
                    grid[i, j] = solution[j];
                }
            }
        }

        public double[] GetLayer(int i)
        {
            double[] layer = new double[(int)(L / h) + 1];
            for (int j = 0; j <= (int)(L / h); j++)
            {
                layer[j] = grid[i, j];
            }

            return layer;
        }

        public double[] GetLastLayer()
        {
            double[] layer = new double[(int)(L / h) + 1];
            for (int j = 0; j <= (int)(L / h); j++)
            { 
                layer[j] = grid[(int)(T / t), j];
            }

            return layer;
        }
    }
}
