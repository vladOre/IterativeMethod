using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IterativeMethod
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {        
                //Задаем начальные значения
                chart1.Series[0].Points.Clear();
                chart1.Series[1].Points.Clear();
                chart1.Series[2].Points.Clear();
                int Count_Points = 1000;
                double Null_value_X = 5;
                double Epsilon_Y = 0.00000001;
                double Epsilon_X = 0.000000001;
                double Left_border = 0;
                double Right_border = 1;
                double Null_lambda = 1;
                double Delta_border_T = Math.Abs(Left_border - Right_border) / Count_Points;
                double J0 = 0, J1 = 0;
                bool division = false;
                double[,] u_delta_T = new double[2, Count_Points];
                double[,] x_delta_T = new double[2, Count_Points];
                double[] J_delta_T = new double[2];
                double[] lambda;
                int First_round, round = -1;
                for (First_round=0; ; First_round++)
                {
                    round++;
                    double[] u0 = new double[Count_Points];// нулевой вектор
                    //for (int i = 0; i < Count_Points; i++)
                    //    u0[i] = 1;/* единичный вектор*/
                    double[] x0 = new double[Count_Points];            
                    double[] Sort_X = new double[Count_Points];
                    double[] Sort_X1 = new double[Count_Points/2];
                    lambda = new double[Count_Points];
                    lambda[0] = 1;
                    x0[0] = Null_value_X;
                    for (int i = 0; i < Count_Points - 1; i++)                 
                        x0[i+1] = x0[i] + Delta_border_T * u0[i];                    //пересчет исксов
                    J0 = Quality_Functional(x0, u0, Count_Points, Delta_border_T);
                    //лямба q-тая равна 0
                    for(int Second_round=0; Second_round<1;)
                    {
                        lambda[Count_Points - 1] = 0 - Null_lambda * x0[Count_Points - 1] * Delta_border_T;
                        for (int i = Count_Points - 2; i > 0; i--)                         
                            lambda[i] = lambda[i+1] - Null_lambda * x0[i] * Delta_border_T;                        
                        double[] u1 = new double[Count_Points];//!
                        double[] x1 = new double[Count_Points];//!
                        x1[0] = Null_value_X;
                        for (int i = 0; i < Count_Points - 1; i++)
                            u1[i] = lambda[i + 1] / Null_lambda;
                        u1[Count_Points - 1] = 0;
                        for (int i = 0; i < Count_Points - 1; i++)
                            x1[i+1] = x1[i] + Delta_border_T * u1[i];
                        J1= Quality_Functional(x1, u1, Count_Points, Delta_border_T);
                        for (int i = 0; i < Count_Points; i++)
                            Sort_X[i] = Math.Abs(x1[i] - x0[i]);
                        Array.Sort(Sort_X);
                        if(J1<J0)
                        {
                            if (Math.Abs(J1 - J0) < Epsilon_Y && Sort_X[Count_Points-1] < Epsilon_X)                           
                                Second_round = 2;
                            else
                            {
                                J0 = J1;
                                for (int i = 0; i < Count_Points; i++)
                                {
                                    x0[i] = x1[i];
                                    u0[i] = u1[i];
                                }
                                continue;
                            }
                        }
                        else
                        {
                            J1 = J0;
                            for (int i = 0; i < Count_Points; i++)
                            {
                                x1[i] = x0[i];
                                u1[i] = u0[i];
                            }
                            Second_round = 2;
                        }
                        J_delta_T[First_round % 2] = J1;
                        for (int j = 0; j < Count_Points; j++)
                        {
                            x_delta_T[First_round % 2, j] = x1[j];
                            u_delta_T[First_round % 2, j] = u1[j];
                        }
                    }
                    if(division == false)        // делили мы шаг       
                        division = true;                    
                    else
                    {
                        for (int i = 0; i < Count_Points / 2; i++)
                            Sort_X1[i] = Math.Abs(x_delta_T[0, i] - x_delta_T[1, 2 * i]);
                        Array.Sort(Sort_X1);
                        if (Math.Abs(J_delta_T[0] - J_delta_T[1]) < Epsilon_Y && Sort_X1[Count_Points / 2 - 1] < Epsilon_X)
                            break;                        
                        else
                        {
                            J_delta_T[Math.Abs(First_round % 2 - 1)] = J_delta_T[First_round % 2];
                            for (int j = 0; j < Count_Points; j++)
                            {
                                x_delta_T[Math.Abs(First_round % 2 - 1), j] = x_delta_T[First_round % 2, j];
                                u_delta_T[Math.Abs(First_round % 2 - 1), j] = u_delta_T[First_round % 2, j];
                            }
                            First_round--;
                        }
                    }
                    if (Count_Points > 1000000)// проверка на размерность массива чтобы не был больше ляма
                         break;
                    // перекладывание значений
                    Count_Points = 2 * Count_Points;
                    Delta_border_T = Math.Abs(Left_border - Right_border) / Count_Points;
                    double[,] service_u_delta_T = new double[2, Count_Points];
                    double[,] service_x_delta_T = new double[2, Count_Points];
                    for (int j = 0; j < Count_Points/2; j++)
                    {
                        service_u_delta_T[0, j] = u_delta_T[0, j];
                        service_u_delta_T[1, j] = u_delta_T[1, j];
                        service_x_delta_T[0, j] = x_delta_T[0, j];
                        service_x_delta_T[1, j] = x_delta_T[1, j];
                    }
                    u_delta_T = new double[2, Count_Points];
                    x_delta_T = new double[2, Count_Points];
                    for (int j = 0; j < Count_Points / 2; j++)
                    {
                        u_delta_T[0, j] = service_u_delta_T[0, j];
                        u_delta_T[1, j] = service_u_delta_T[1, j];
                        x_delta_T[0, j] = service_x_delta_T[0, j];
                        x_delta_T[1, j] = service_x_delta_T[1, j];
                    }
                }
                //вывод отрисовка
                textBox1.Text = $"{round}";
                textBox2.Text = $"{J_delta_T[First_round % 2]}";
                int s = 0;
                for (double k = Left_border; k < Right_border; k += 0.001)
                {
                    chart1.Series[0].Points.AddXY(k, x_delta_T[First_round % 2, s]);// численный x
                    chart1.Series[1].Points.AddXY(k, u_delta_T[First_round % 2, s]);
                    chart1.Series[2].Points.AddXY(k, ((Math.Pow(x_delta_T[First_round % 2, s], 2) + Math.Pow(u_delta_T[First_round % 2, s], 2)) / -2) +  u_delta_T[First_round % 2, s] * lambda[s]);// внимательно функция понтрягина
                    chart1.Series[3].Points.AddXY(k, 0.6*Math.Exp(k)+ 4.42*Math.Exp(-k)); //формула для икса
                    s += Count_Points / 1000;/*Convert.ToInt32(k * 1000);*//* Convert.ToInt32(k*1000/Delta_border_T);*/
                }                
                this.Refresh();                
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка!");
            }
        }
        private double Quality_Functional(double[] x0, double[] u0, int Count_Points, double Delta_border_T)
        {
            double J = 0;
            for (int i = 0; i < Count_Points; i++) 
            {
                J += (Math.Pow(u0[i], 2) + Math.Pow(x0[i], 2)) * Delta_border_T;
            }
            J /= 2;
            return J;
        }
    }
}
