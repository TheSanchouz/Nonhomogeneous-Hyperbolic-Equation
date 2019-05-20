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
        static Grid2D grid2D;

        public Form()
        {
            InitializeComponent();
            MathNet.Numerics.Control.ConfigureAuto();
            pictureBox4.Image = Image.FromFile("d:\\Nonhomogeneous-Hyperbolic-Equation\\Nonhomogeneous Hyperbolic Equation\\sl.png");

            chart.Series.Clear();
            chart.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas[0].AxisX.RoundAxisValues();
            //chart.ChartAreas[0].AxisX.Interval = 1;
            //chart.ChartAreas[0].AxisY.Interval = 1;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            trackBarCurrentLayer.Maximum = int.Parse(textBox_t_count.Text) - 1;

            double L = double.Parse(textBoxL.Text);
            double T = double.Parse(textBoxT.Text);

            double a = Math.Sqrt(double.Parse(textBoxA2.Text));
            double l = double.Parse(textBoxL.Text);
            double h = double.Parse(textBox_h.Text);
            double t = double.Parse(textBox_t.Text);

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


            Func<double, double> phi = expr_phi.Compile("x");
            Func<double, double> psi = expr_psi.Compile("x");
            Func<double, double> b = expr_b.Compile("x");

            chart.Series.Clear();
            chart.Series.Add("Начальное условие \u03D5(x)");
            chart.Series["Начальное условие \u03D5(x)"].ChartType = SeriesChartType.Spline;
            chart.Series["Начальное условие \u03D5(x)"].Color = Color.Blue;
            chart.Series["Начальное условие \u03D5(x)"].BorderWidth = 2;

            for (double i = 0; i <= L; i += h)
            {
                chart.Series["Начальное условие \u03D5(x)"].Points.AddXY(i, phi(i));
            }

            grid2D = new Grid2D(phi, psi, b, a, L, T, h, t);
            if (radioButtonHomogeneous.Checked == true)
            {
                grid2D.SolveHomogeneous();
            }
            else if (radioButtonNongomogeneous.Checked == true)
            {
                grid2D.SolveNonhomogeneous();
            }


            trackBarCurrentLayer.Value = int.Parse(textBox_t_count.Text) - 1;
            groupBoxCurrentLayer.Text = String.Format("Текущий слой: {0} (последний u(x, T))", trackBarCurrentLayer.Value);
            string number_layer = "Слой " + trackBarCurrentLayer.Value;
            if (trackBarCurrentLayer.Value == int.Parse(textBox_t_count.Text) - 1)
            {
                number_layer += " (последний u(x, T))";
            }
            chart.Series.Add(number_layer);
            chart.Series[number_layer].ChartType = SeriesChartType.Spline;
            chart.Series[number_layer].Color = Color.Black;
            chart.Series[number_layer].BorderWidth = 2;


            double[] layer = grid2D.GetLastLayer();
            for (int i = 0; i < layer.Length; i++)
            {
                chart.Series[number_layer].Points.AddXY(i * h, layer[i]);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(textBoxL.Text, out double L) && 
                double.TryParse(textBox_h.Text, out double h) &&
                (textBox_h.Focused || textBoxL.Focused))
            {
                double h_count = L / h + 1;
                textBox_h_count.Text = h_count.ToString();

                if ((int)h_count == h_count)
                {
                    buttonCalculate.Enabled = true;
                }
                else
                {
                    buttonCalculate.Enabled = false;
                    new ToolTip().Show("Должно быть целым!", textBox_h_count, 1000);
                }
            }
            else
            {
                buttonCalculate.Enabled = false;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(textBoxL.Text, out double L) && 
                int.TryParse(textBox_h_count.Text, out int h_count) &&
                (textBox_h_count.Focused || textBoxL.Focused))
            {
                double h = (L + 1) / h_count;
                textBox_h.Text = h.ToString();

                buttonCalculate.Enabled = true;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(textBoxT.Text, out double T) && 
                double.TryParse(textBox_t.Text, out double t) &&
                (textBox_t.Focused || textBoxT.Focused))
            {
                double t_count = T / t + 1;
                textBox_t_count.Text = t_count.ToString();

                if ((int)t_count == t_count)
                {
                    buttonCalculate.Enabled = true;
                }
                else
                {
                    buttonCalculate.Enabled = false;
                    new ToolTip().Show("Должно быть целым!", textBox_t_count, 1000);
                }
            }
            else
            {
                buttonCalculate.Enabled = false;
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(textBoxT.Text, out double T) && 
                int.TryParse(textBox_t_count.Text, out int t_count) &&
                (textBox_t_count.Focused || textBoxL.Focused))
            {
                double t = (T + 1) / t_count;
                textBox_t.Text = t.ToString();

                buttonCalculate.Enabled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox3_TextChanged(sender, e);
            textBox6_TextChanged(sender, e);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            textBox4_TextChanged(sender, e);
            textBox7_TextChanged(sender, e);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double h = double.Parse(textBox_h.Text);
            groupBoxCurrentLayer.Text = String.Format("Текущий слой: {0}", trackBarCurrentLayer.Value);
            string number_layer = "Слой " + trackBarCurrentLayer.Value;
            if (trackBarCurrentLayer.Value == int.Parse(textBox_t_count.Text) - 1)
            {
                number_layer += " (последний u(x, T))";
                groupBoxCurrentLayer.Text += " (последний u(x, T))";
            }

            chart.Series.RemoveAt(1);
            chart.Series.Add(number_layer);
            chart.Series[number_layer].ChartType = SeriesChartType.Spline;
            chart.Series[number_layer].Color = Color.Black;
            chart.Series[number_layer].BorderWidth = 2;

            double[] layer = grid2D.GetLayer(trackBarCurrentLayer.Value);
            for (int i = 0; i < layer.Length; i++)
            {
                chart.Series[number_layer].Points.AddXY(i * h, layer[i]);
            }
        }
    }
}
