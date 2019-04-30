using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using WpfMath;
using MathNet;

namespace Nonhomogeneous_Hyperbolic_Equation
{
    public partial class Form : System.Windows.Forms.Form
    {
        static TexFormulaParser texFormulaParser = new TexFormulaParser();

        public Form()
        {
            InitializeComponent();

            chart.Series.Clear();
            chart.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas[0].AxisX.RoundAxisValues();
            chart.ChartAreas[0].AxisX.Interval = 1;
            chart.ChartAreas[0].AxisY.Interval = 1;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            double a = Math.Sqrt(double.Parse(textBox1.Text));
            double l = double.Parse(textBox2.Text);
            double h = double.Parse(textBox3.Text);
            double t = double.Parse(textBox4.Text);

            string cos1 = "cos(pi*x/" + Convert.ToString(l) + ")";
            string cos2 = "cos(2*pi*x/" + Convert.ToString(l) + ")";
            string sin1 = "sin(pi*x/" + Convert.ToString(l) + ")";
            string sin2 = "sin(2*pi*x/" + Convert.ToString(l) + ")";

            // phi
            string phi_coeff_1 = textBox_phi_coeff_1.Text;
            string phi_coeff_2 = textBox_phi_coeff_2.Text;
            string phi_coeff_3 = textBox_phi_coeff_3.Text;

            string formula_phi = phi_coeff_1 + "+" +
                phi_coeff_2 + "*" + cos1 + "+" +
                phi_coeff_3 + "*" + cos2;

            MathNet.Symbolics.SymbolicExpression expr_phi = MathNet.Symbolics.SymbolicExpression.Parse(formula_phi);


            // psi
            string psi_coeff_1 = textBox_psi_coeff_1.Text;
            string psi_coeff_2 = textBox_psi_coeff_2.Text;
            string psi_coeff_3 = textBox_psi_coeff_3.Text;

            string formula_psi = psi_coeff_1 + "+" +
                psi_coeff_2 + "*" + cos1 + "+" +
                psi_coeff_3 + "*" + cos2;

            MathNet.Symbolics.SymbolicExpression expr_psi = MathNet.Symbolics.SymbolicExpression.Parse(formula_psi);


            // b
            string b_coeff_1 = textBox_b_coeff_1.Text;
            string b_coeff_2 = textBox_b_coeff_2.Text;
            string b_coeff_3 = textBox_b_coeff_3.Text;
            string b_coeff_4 = textBox_b_coeff_4.Text;
            string b_coeff_5 = textBox_b_coeff_5.Text;

            string formula_b = b_coeff_1 + "+" +
                b_coeff_2 + "*" + cos1 + "+" +
                b_coeff_3 + "*" + sin1 + "+" +
                b_coeff_4 + "*" + cos2 + "+" +
                b_coeff_5 + "*" + sin2;

            MathNet.Symbolics.SymbolicExpression expr_b = MathNet.Symbolics.SymbolicExpression.Parse(formula_b);

            pictureBox1.BeginInvoke((MethodInvoker)(() =>
            {
                string latex_formula_phi =
                    "\\phi\\left(x\\right)=" +
                    MathNet.Symbolics.LaTeX.Format(expr_phi.Expression).Replace("\\pix", "{\\pi}x");

                TexFormula texFormula = texFormulaParser.Parse(latex_formula_phi);
                byte[] pngBytes = texFormula.RenderToPng(100.0, 0.0, 0.0, "Arial");
                pictureBox1.Image = Image.FromStream(new MemoryStream(pngBytes));
            }));
            pictureBox2.BeginInvoke((MethodInvoker)(() =>
            {
                string latex_formula_psi =
                    "\\psi\\left(x\\right)=" +
                    MathNet.Symbolics.LaTeX.Format(expr_psi.Expression).Replace("\\pix", "{\\pi}x");

                TexFormula texFormula = texFormulaParser.Parse(latex_formula_psi);
                byte[] pngBytes = texFormula.RenderToPng(100.0, 0.0, 0.0, "Arial");
                pictureBox2.Image = Image.FromStream(new MemoryStream(pngBytes));
            }));
            pictureBox3.BeginInvoke((MethodInvoker)(() =>
            {
                string latex_formula_b =
                    "b\\left(x\\right)=" +
                    MathNet.Symbolics.LaTeX.Format(expr_b.Expression).Replace("\\pix", "{\\pi}x");

                TexFormula texFormula = texFormulaParser.Parse(latex_formula_b);
                byte[] pngBytes = texFormula.RenderToPng(100.0, 0.0, 0.0, "Arial");
                pictureBox3.Image = Image.FromStream(new MemoryStream(pngBytes));
            }));

            //new Task(() =>
            //{
            //    while (true)
            //    {
            //        string latex_formula_phi =
            //            "\\phi\\left(x\\right)=" +
            //            MathNet.Symbolics.LaTeX.Format(expr_phi.Expression).Replace("\\pix", "{\\pi}x");

            //        try
            //        {
            //            byte[] pngBytes = new TexFormula(
            //                new TexFormulaParser().Parse(
            //                    latex_formula_phi)).RenderToPng(100.0, 0.0, 0.0, "Arial");
            //            pictureBox1.Image = Image.FromStream(new MemoryStream(pngBytes));

            //            break;
            //        }
            //        catch (Exception) { }
            //    }
            //}).Start();
            //new Task(() =>
            //{
            //    while (true)
            //    {
            //        string latex_formula_psi =
            //            "\\psi\\left(x\\right)=" +
            //            MathNet.Symbolics.LaTeX.Format(expr_psi.Expression).Replace("\\pix", "{\\pi}x");

            //        try
            //        {
            //            byte[] pngBytes = new TexFormula(
            //                new TexFormulaParser().Parse(
            //                    latex_formula_psi)).RenderToPng(100.0, 0.0, 0.0, "Arial");
            //            pictureBox2.Image = Image.FromStream(new MemoryStream(pngBytes));

            //            break;
            //        }
            //        catch (Exception) { }
            //    }
            //}).Start();
            //new Task(() =>
            //{
            //    while (true)
            //    {
            //        string latex_formula_b =
            //            "b\\left(x\\right)=" +
            //            MathNet.Symbolics.LaTeX.Format(expr_b.Expression).Replace("\\pix", "{\\pi}x");

            //        try
            //        {
            //            byte[] pngBytes = new TexFormula(
            //                new TexFormulaParser().Parse(
            //                    latex_formula_b)).RenderToPng(100.0, 0.0, 0.0, "Arial");
            //            pictureBox3.Image = Image.FromStream(new MemoryStream(pngBytes));

            //            break;
            //        }
            //        catch (Exception) { }
            //    }
            //}).Start();


            var A = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(new double[,]
            {
                { 3, 2, -1 },
                { 2, -2, 4 },
                { -1, 0.5, -1}
            });
            var b1 = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(new double[] { 1, -2, 0 });
            var x1 = A.Solve(b1);
            Console.WriteLine(x1);


            Func<double, double> phi = expr_phi.Compile("x");
            Func<double, double> psi = expr_psi.Compile("x");
            Func<double, double> b = expr_b.Compile("x");

            Console.WriteLine(MathNet.Numerics.Integration.NewtonCotesTrapeziumRule.IntegrateAdaptive(phi, 0, l, 1e-5));

            chart.Series.Clear();
            chart.Series.Add("Начальные условия");
            chart.Series["Начальные условия"].ChartType = SeriesChartType.Spline;
            chart.Series["Начальные условия"]["LineTension"] = "0.2";
            chart.Series["Начальные условия"].Color = Color.Gray;
            chart.Series["Начальные условия"].BorderWidth = 2;


            double dx = 0.1;
            for (double x = 0.0; x <= 7; x += dx)
            {
                chart.Series["Начальные условия"].Points.AddXY(x, phi(x));
            }
        }
    }
}
