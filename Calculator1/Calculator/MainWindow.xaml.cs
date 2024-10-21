using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography.X509Certificates;

namespace Calculator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary
    /// 
    public partial class MainWindow : Window
    {
        List<string> operations = new List<string>() { "+", "-", "*", "/", "√", "%"};
        string current_operation = null;
        List<string> HISTORY = new List<string>();
        public MainWindow()
        {
            InitializeComponent();      
        }

        public void print_num(object sender, RoutedEventArgs e)
        {
            if (table.Text.Contains("∞"))
            {
                table.Text = "";
            }
            if (table.Text.Contains("не число"))
            {
                table.Text = "";
            }
            if (table.Text.Contains("E"))
            {
                table.Text = "";
            }
            table.Text += sender.ToString().Split(':')[1].Trim();
            current_operation = null;
        }

        public void print_oper(object sender, RoutedEventArgs e)
        {
            if (table.Text.Contains("∞"))
            {
                table.Text = "";
            }
            if (table.Text.Contains("не число"))
            {
                table.Text = "";
            }
            if (table.Text.Contains("E"))
            {
                table.Text = "";
            }
            if (table.Text.Length == 0 && (sender.ToString().Split(':')[1].Trim() != "-"))
            {
                return;
            }
            if (current_operation == null)
            {
                if (sender.ToString().Split(':')[1].Trim() != current_operation)
                {
                    table.Text += sender.ToString().Split(':')[1].Trim();
                    current_operation = sender.ToString().Split(':')[1].Trim();
                }
            }
            else if (table.Text.Length > 1)
            {
                сhangeOperator(sender.ToString().Split(':')[1].Trim());
            }
        }

        private void kor_butt_Click(object sender, RoutedEventArgs e)
        {
            if (table.Text.Length == 0)
            {
                return;
            }
            Regex regex = new Regex(@"\+\-\*\/");
            MatchCollection matches = regex.Matches(table.Text);
            if (matches.Count == 0)
            {
                double number;
                bool isNumeric = double.TryParse(table.Text, out number);

                if (!isNumeric)
                {
                    MessageBox.Show("Введите корректное число!");
                }
                else if (number < 0)
                {
                    MessageBox.Show("Корень из отрицательного числа!");
                }
                else
                {
                    table.Text = (Math.Pow(number, 0.5)).ToString();
                }
            }
        }
        

        private void ravno_butt_Click(object sender, RoutedEventArgs e)
        {
            if (table.Text.Length > 11)
            {
                MessageBox.Show("Вы вышли за диапазон", "Предупреждение");
                table.Text = "";
                return;
            }
            if (table.Text.Contains("не число") || table.Text.Contains("∞") || table.Text.Contains("-∞") || table.Text.Contains("-0,0"))
            {
                table.Text = "";
            }
            if (table.Text.Length == 0) { return; }
            if (table.Text.Contains("∞") || table.Text.Contains("не число"))
            {
                table.Text = "";
            }

            if (table.Text.Length > 0 && operations.Contains(table.Text.Substring(table.Text.Length-1)))
            {
                MessageBox.Show("Закончите выражение");
                return;
            }
            
            string stroka = table.Text.Replace(',', '.').Replace("%", "/100");

            if (table.Text.StartsWith("*") || table.Text.StartsWith("/") || table.Text.StartsWith("%") || stroka.Contains("/,") || stroka.Contains("*,"))
            {
                MessageBox.Show("Ошибочное выражение");
                table.Text = "";
                return;
            }
            if(table.Text.Contains("/"))
            {
                int indexOfSlash = table.Text.IndexOf('/');
                string denominatorPart = table.Text.Substring(indexOfSlash + 1).Trim();
                if (double.TryParse(denominatorPart, out double denominator))
                {
                    
                    if (denominator == 0)
                    {
                        MessageBox.Show("Попытка деления на ноль");
                        return;
                    }
                }
            }
            eval();
        }

        public bool cheked_number()
        { 
            List<string> dives = new List<string>(table.Text.Replace("%", "/100").Split('/'));
            foreach (string dive in dives)
            {
                if (dive.Contains("+") || dive.Contains("-") || dive.Contains("/") || dive.Contains("*") || dive.Contains("%"))
                {
                    continue;
                }
                if(dives.IndexOf(dive) != 0 && Convert.ToDouble(dive) == 0)
                {
                    MessageBox.Show("Попытка деления на ноль");
                    return false;
                }
            }
            return true;
        }
        public void eval()
        {
            if (!cheked_number()) return;
            СhangeSign();
            СhangeSign();

            string history = $"{table.Text}";

            string expression = table.Text.Replace(',', '.').Replace("%", "/100");

            object result = new DataTable().Compute($"1.0 * {expression}", null);

            if (result == null)
            {
                MessageBox.Show("Ошибка: нечисловое выражение или переполнение");
                table.Text = "не число";
                return;
            }

            double computedResult;
            bool isValidDouble = double.TryParse(result.ToString(), out computedResult);

            if (!isValidDouble || computedResult > double.MaxValue || computedResult < double.MinValue)
            {
                MessageBox.Show("Ошибка: переполнение");
                table.Text = "не число";
            }
            else
            {
                table.Text = computedResult.ToString();
            }

            current_operation = null;

            history += " = " + table.Text;
            HISTORY.Add(history);
        }

        public void remove_butt_Click(object sender, RoutedEventArgs e)
        {
            if(table.Text == "не число")
            {
                table.Text = "";
            }    
            if (table.Text.Length > 0)
            {
                table.Text = table.Text.Substring(0, table.Text.Length - 1);
                if (table.Text.Length > 0 && operations.Contains(table.Text.Substring(table.Text.Length - 1)))
                {
                    current_operation = table.Text.Substring(table.Text.Length - 1);
                }
                else
                {
                    current_operation = null;
                }
            }
        }

        private void CA_butt_Click(object sender, RoutedEventArgs e)
        {
            table.Text = "";
            current_operation = null;
        }

        private void comma_butt_Click(object sender, RoutedEventArgs e)
        {
            if (table.Text == "не число")
            {
                table.Text = "";
            }
            if (table.Text.Contains("E"))
            {
                return;
            }
            if (table.Text.Length == 0)
            {
                return;
            }
            if (operations.Contains(table.Text.Substring(table.Text.Length - 1)))
            {
                return;
            }
            else
            {
                string[] parts = table.Text.Split(new[] { '+', '-', '*', '/', '%' });
                string lastPart = parts.Last();

                if (!lastPart.Contains(","))
                {
                    table.Text += ",";
                }
            }
        }

        public void сhangeOperator(string newOperator)
        {
            if (table.Text.Length == 0)
            {
                return;
            }
            string[] parts = table.Text.Split(new[] { '+', '-', '*', '/', '%' });
            if (parts.Length > 1)
            {
                int lastOperatorIndex = table.Text.LastIndexOfAny(new char[] { '+', '-', '*', '/', '%' });
                table.Text = table.Text.Substring(0, lastOperatorIndex) + newOperator + table.Text.Substring(lastOperatorIndex + 1);
            }
        }

        public void СhangeSign()
        {
            if(table.Text.Contains("-∞"))
            {
                table.Text = "";
            }
            if (table.Text.Length == 0)
            {
                return;
            }
            string[] parts = table.Text.Split(new[] { '+', '*', '/', '%' });
            string lastPart = parts.Last();

            if (double.TryParse(lastPart, out double number))
            {
                if(number > 0)
                {
                    number = -number;
                }
                else if (number < 0)
                {
                    number = number*-1;
                }
                table.Text = table.Text.Substring(0, table.Text.LastIndexOf(lastPart)) + number.ToString();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                remove_butt_Click(sender, e);
            }
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                if (table.Text.Contains("∞") || table.Text.Contains("не число") || table.Text.Contains("E"))
                {
                    table.Text = "";
                }
                table.Text += (e.Key - Key.D0).ToString();
            }
        }

        private void table_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if(textBox.SelectionLength > 0)
            {
                e.Handled = true;
            }
        }

        private void change_sign_butt_Click(object sender, RoutedEventArgs e)
        {
            СhangeSign();
        }


        private void history_Click(object sender, RoutedEventArgs e)
        {
            string history = "";
            if (HISTORY != null && HISTORY.Count > 0)
            {
                int countToShow = Math.Min(4, HISTORY.Count);
                for (int i = 0; i < countToShow; i++)
                {
                    history += HISTORY[HISTORY.Count - 1 - i] + "\n";
                }
            }
            MessageBox.Show(history);
        }
    }
}
