using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using ZedGraph;
using System;

namespace TeorVer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        void ShellSort(double[] mass)
        {
            int i, j, step;
            double tmp1;
            int n = mass.Length;
            for (step = n / 2; step > 0; step /= 2)
            {
                for (i = step; i < n; i++)
                {
                    tmp1 = mass[i];
                    for (j = i; j >= step; j -= step)
                    {
                        if (tmp1 < mass[j - step])
                        {
                            mass[j] = mass[j - step];
                        }
                        else
                        {
                            break;
                        }
                    }
                    mass[j] = tmp1;
                }
            }
        }


        private double Gamma(double Num)
        {
            if (Num > 1)
            {
                double Number = (Num - 1) * Gamma(Num - 1);
                return Number;
            }
            if (Num == 1.0)
            {
                return 1.0;
            }

            return Math.Pow(3.14159, 0.5);
        }

        private double Hi_Quadrat(double R0, double k)
        {
            double gamma = Gamma((k - 1.0) / 2.0);
            double result = 0;
            double exp2 = 1;
            for (int i = 0; i < k - 1; i++)
            {
                exp2 *= 2.0;
            }
            exp2 = Math.Pow(exp2, 0.5);
            if (k == 2)
            {
                for (int i = 2; i <= 1000; i++)
                {
                    result += (Math.Exp(-R0 * ((double)i - 1.0) / 1000) * Math.Pow(R0 * ((double)i - 1.0) / 1000, (k - 1.0) / 2 - 1) / exp2 / gamma +
                               Math.Exp(-R0 * (double)i / 1000) * Math.Pow(R0 * (double)i / 1000, (k - 1.0) / 2 - 1) / exp2 / gamma) * R0 / 2000;
                }
                return result;
            }
            for (int i = 1; i <= 1000; i++)
            {
                result += (Math.Exp(-R0 * ((double)i - 1.0) / 1000) * Math.Pow(R0 * ((double)i - 1.0) / 1000, (k - 1.0) / 2 - 1) / exp2 / gamma +
                           Math.Exp(-R0 * (double)i / 1000) * Math.Pow(R0 * (double)i / 1000, (k - 1.0) / 2 - 1) / exp2 / gamma) * R0 / 2000;
            }
            return result;
        }




        private void button1_Click(object sender, EventArgs e)
        {

            //1 лаба
            dataGridView1.Rows.Clear();

            Random rnd = new Random(DateTime.Now.Second);

            double sigma = Convert.ToDouble(textBox1.Text);
            int Number_steps = Convert.ToInt32(textBox2.Text);
            double[] y = new double[Number_steps];

            for (int i = 0; i < Number_steps; i++)
            {

                y[i] = sigma * Math.Sqrt(-2 * Math.Log(rnd.NextDouble()));
            }

            ShellSort(y);
            for (int i = 0; i < y.Length; i++)
            {
                dataGridView1.Rows.Add();

                dataGridView1.Rows[i].Cells[0].Value = i;
                dataGridView1.Rows[i].Cells[1].Value = y[i];
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView5.Rows.Clear();


            int Number_steps = Convert.ToInt32(textBox2.Text);
            double sigma = Convert.ToDouble(textBox1.Text);
            double[] y = new double[Number_steps];

            double[] NumberExpInDistance = new Double[dataGridView4.RowCount];
            for (int i = 0; i < Number_steps; i++)
            {
                y[i] = (double)dataGridView1.Rows[i].Cells[1].Value;
            }

            double[] q = new Double[dataGridView4.RowCount];

            //Вычисление R0
            double R0 = 0;
            double sum = 0;


            q[0] = 1 - Math.Exp(-Convert.ToDouble(dataGridView4[0, 0].Value) * Convert.ToDouble(dataGridView4[0, 0].Value) / (2 * sigma * sigma));
            sum += q[0];
            for (int i = 1; i < dataGridView4.RowCount - 1; i++)
            {

                q[i] = -Math.Exp(-Convert.ToDouble(dataGridView4[0, i].Value) * Convert.ToDouble(dataGridView4[0, i].Value) / (2 * sigma * sigma))
                      + Math.Exp(-Convert.ToDouble(dataGridView4[0, i - 1].Value) * Convert.ToDouble(dataGridView4[0, i - 1].Value) / (2 * sigma * sigma));
                sum += q[i];
            }

            q[q.Length - 1] = 1 - sum;
            NumberExpInDistance[0] = 0;
            double alpha = Convert.ToDouble(textBox6.Text);
            int Numflag = 0;
            for (int j = Numflag; j < y.Length; j++)
            {
                if (y[j] <= Convert.ToDouble(dataGridView4[0, 0].Value))
                {
                    NumberExpInDistance[0] += 1;
                }
                else
                {
                    Numflag = j;
                    break;
                }
            }
            for (int i = 1; i < dataGridView4.RowCount - 1; i++)
            {
                for (int j = Numflag; j < y.Length; j++)
                {
                    if (y[j] <= Convert.ToDouble(dataGridView4[0, i].Value) && y[j] > Convert.ToDouble(dataGridView4[0, i - 1].Value))
                    {
                        NumberExpInDistance[i] += 1;
                    }
                    else
                    {
                        Numflag = j;
                        break;
                    }
                }
            }
            for (int j = Numflag; j < y.Length; j++)
            {
                if (y[j] > Convert.ToDouble(dataGridView4[0, dataGridView4.RowCount - 1].Value))
                {
                    NumberExpInDistance[NumberExpInDistance.Length - 1] += 1;
                }
            }
            for (int i = 0; i < q.Length; i++)
            {
                if (q[i] != 0)
                {
                    R0 += Math.Pow(NumberExpInDistance[i] - (double)Number_steps * q[i], 2) / ((double)Number_steps * q[i]);
                }
            }
            //Вывод
            double FR = 1 - Hi_Quadrat(R0, q.Length);
            textBox7.Text = Convert.ToString(FR);
            for (int i = 0; i < q.Length; i++)
            {
                dataGridView5.Rows.Add(i, q[i], NumberExpInDistance[i]);
            }
            if (FR > alpha)
            {
                textBox8.Text = "Гипотеза принята";
            }
            else
            {
                textBox8.Text = "Гипотеза отвергнута";
            }

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
