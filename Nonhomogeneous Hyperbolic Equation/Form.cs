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
            trackBar1.Maximum = int.Parse(textBox7.Text) - 1;

            double L = double.Parse(textBox2.Text);
            double T = double.Parse(textBox5.Text);

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


            Func<double, double> phi = expr_phi.Compile("x");
            Func<double, double> psi = expr_psi.Compile("x");
            Func<double, double> b = expr_b.Compile("x");

            chart.Series.Clear();
            chart.Series.Add("Начальное условие \u03D5(x)");
            chart.Series["Начальное условие \u03D5(x)"].ChartType = SeriesChartType.Spline;
            chart.Series["Начальное условие \u03D5(x)"].Color = Color.Gray;
            chart.Series["Начальное условие \u03D5(x)"].BorderWidth = 2;

            for (double i = 0; i <= L; i += h)
            {
                chart.Series["Начальное условие \u03D5(x)"].Points.AddXY(i, phi(i));
            }

            grid2D = new Grid2D(phi, psi, b, a, L, T, h, t);
            backgroundWorker1.RunWorkerAsync();
            grid2D.SolveHomogeneous(ref backgroundWorker1);

            trackBar1.Value = int.Parse(textBox7.Text) - 1;
            groupBox6.Text = String.Format("Текущий слой: {0} (последний)", trackBar1.Value);
            string number_layer = "Слой " + trackBar1.Value;
            if (trackBar1.Value == int.Parse(textBox7.Text) - 1)
            {
                number_layer += " (последний)";
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
            if (double.TryParse(textBox2.Text, out double L) && 
                double.TryParse(textBox3.Text, out double h) &&
                (textBox3.Focused || textBox2.Focused))
            {
                double h_count = L / h + 1;
                textBox6.Text = h_count.ToString();

                if ((int)h_count == h_count)
                {
                    button1.Enabled = true;
                }
                else
                {
                    button1.Enabled = false;
                    new ToolTip().Show("Должно быть целым!", textBox6, 1000);
                }
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(textBox2.Text, out double L) && 
                int.TryParse(textBox6.Text, out int h_count) &&
                (textBox6.Focused || textBox2.Focused))
            {
                double h = (L + 1) / h_count;
                textBox3.Text = h.ToString();

                button1.Enabled = true;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(textBox5.Text, out double T) && 
                double.TryParse(textBox4.Text, out double t) &&
                (textBox4.Focused || textBox5.Focused))
            {
                double t_count = T / t + 1;
                textBox7.Text = t_count.ToString();

                if ((int)t_count == t_count)
                {
                    button1.Enabled = true;
                }
                else
                {
                    button1.Enabled = false;
                    new ToolTip().Show("Должно быть целым!", textBox7, 1000);
                }
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(textBox5.Text, out double T) && 
                int.TryParse(textBox7.Text, out int t_count) &&
                (textBox7.Focused || textBox2.Focused))
            {
                double t = (T + 1) / t_count;
                textBox4.Text = t.ToString();

                button1.Enabled = true;
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
            double h = double.Parse(textBox3.Text);
            groupBox6.Text = String.Format("Текущий слой: {0}", trackBar1.Value);
            string number_layer = "Слой " + trackBar1.Value;
            if (trackBar1.Value == int.Parse(textBox7.Text) - 1)
            {
                number_layer += " (последний)";
                groupBox6.Text += " (последний)";
            }

            chart.Series.RemoveAt(1);
            chart.Series.Add(number_layer);
            chart.Series[number_layer].ChartType = SeriesChartType.Spline;
            chart.Series[number_layer].Color = Color.Black;
            chart.Series[number_layer].BorderWidth = 2;

            double[] layer = grid2D.GetLayer(trackBar1.Value);
            for (int i = 0; i < layer.Length; i++)
            {
                chart.Series[number_layer].Points.AddXY(i * h, layer[i]);
            }
        }
    }
}
