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

            double C = a * (t / h);

            //
            // первый слой
            //
            for (int j = 0; j <= dim_L; j++)
            {
                grid[0, j] = phi(h * j);
            }
            //

            //
            // второй слой
            //
            double[,] coeff_second = new double[dim_L + 1, dim_L + 1];
            double[] b_coeff_second = new double[dim_L + 1];

            coeff_second[0, 0] = -3;
            coeff_second[0, 1] = 4;
            coeff_second[0, 2] = -1;
            b_coeff_second[0] = 0;
            for (int j = 1; j <= dim_L - 1; j++)
            {
                coeff_second[j, j - 1] = 1 / (h * h);
                coeff_second[j, j] = -2 / (h * h) - 1 / t;
                coeff_second[j, j + 1] = 1 / (h * h);
                b_coeff_second[j] = -grid[0, j] / t;
            }
            coeff_second[dim_L, dim_L - 2] = 1;
            coeff_second[dim_L, dim_L - 1] = -4;
            coeff_second[dim_L, dim_L] = 3;
            b_coeff_second[dim_L] = 0;

            var matrix_2layer = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(coeff_second);
            var b_vector_2layer = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(b_coeff_second);
            var solution_2layer = matrix_2layer.Solve(b_vector_2layer);

            for (int j = 0; j <= dim_L; j++)
            {
                grid[1, j] = solution_2layer[j];
            }
            //

            //
            // шаблон неявный T
            //
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


                var matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(coeff);
                var b_vector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(b_coeff);
                var solution = matrix.Solve(b_vector);

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

            double C = a * (t / h);

            //
            // первый слой
            //
            for (int j = 0; j <= dim_L; j++)
            {
                grid[0, j] = phi(h * j);
            }
            //

            //
            // второй слой
            //
            double[,] coeff_second = new double[dim_L + 1, dim_L + 1];
            double[] b_coeff_second = new double[dim_L + 1];

            coeff_second[0, 0] = -3;
            coeff_second[0, 1] = 4;
            coeff_second[0, 2] = -1;
            b_coeff_second[0] = 0;
            double integral_2layer = Integrate(0);
            for (int j = 1; j <= dim_L - 1; j++)
            {
                coeff_second[j, j - 1] = 1 / (h * h);
                coeff_second[j, j] = -2 / (h * h) - 1 / t;
                coeff_second[j, j + 1] = 1 / (h * h);
                b_coeff_second[j] = -grid[0, j] / t +
                    grid[j - 1, j] * b(j * h) - grid[j - 1, j] * integral_2layer;
            }
            coeff_second[dim_L, dim_L - 2] = 1;
            coeff_second[dim_L, dim_L - 1] = -4;
            coeff_second[dim_L, dim_L] = 3;
            b_coeff_second[dim_L] = 0;

            var matrix_2layer = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(coeff_second);
            var b_vector_2layer = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(b_coeff_second);
            var solution_2layer = matrix_2layer.Solve(b_vector_2layer);
            for (int j = 0; j <= dim_L; j++)
            {
                grid[1, j] = solution_2layer[j];
            }
            //

            //
            // шаблон неявный T
            //
            for (int i = 2; i <= dim_T; i++)
            {
                double[,] coeff = new double[dim_L + 1, dim_L + 1];
                double[] b_coeff = new double[dim_L + 1];


                coeff[0, 0] = -3;
                coeff[0, 1] = 4;
                coeff[0, 2] = -1;
                b_coeff[0] = 0;

                double integral = Integrate(i - 1);
                for (int j = 1; j <= dim_L - 1; j++)
                {
                    coeff[j, j - 1] = (a * a) / (h * h);
                    coeff[j, j]     = (-2 * a * a) / (h * h) - 1 / (t * t);
                    coeff[j, j + 1] = (a * a) / (h * h);
                    b_coeff[j] = (-2 * grid[i - 1, j] + grid[i - 2, j]) / (t * t) -
                        grid[i - 1, j] * b(j * h) + grid[i - 1, j] * integral;
                }

                coeff[dim_L, dim_L - 2] = 1;
                coeff[dim_L, dim_L - 1] = -4;
                coeff[dim_L, dim_L] = 3;
                b_coeff[dim_L] = 0;


                var matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(coeff);
                var b_vector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(b_coeff);
                var solution = matrix.Solve(b_vector);

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

        private double Integrate(int i)
        {
            int count = (int)(L / h) + 1;
            double integral = 0.0;

            if (count % 2 != 0)
            {
                for (int j = 1; j <= count - 1; j += 2)
                {
                    integral += grid[i, j - 1] * b((j - 1) * h) + 
                        4 * grid[i, j] * b(j * h) + 
                        grid[i, j + 1] * b((j + 1) * h);
                }

                integral *= (h / 3);
            }
            else
            {
                for (int j = 1; j <= count - 4; j += 2)
                {
                    integral += grid[i, j - 1] * b((j - 1) * h) +
                        4 * grid[i, j] * b(j * h) +
                        grid[i, j + 1] * b((j + 1) * h);
                }

                integral *= h / 3;

                double integral_3_8 = (3 * h / 8) * (grid[i, count - 4] * b(L - 3 * h) + 
                    3 * grid[i, count - 3] * b(L - 2 * h) + 
                    3 * grid[i, count - 2] * b(L - 1 * h) + 
                    grid[i, count - 1] * b(L));

                integral += integral_3_8;
            }

            return integral;
        }
    }
}
