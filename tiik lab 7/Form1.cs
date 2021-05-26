using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace tiik_lab_7
{
    public partial class Form1 : Form
    {
        //private int hammingBase_ = 0;   //второе число в хэмминге
        //private int hammingFinal_ = 0;  //первое число в хэмминге


        public Form1()
        {
            InitializeComponent();
        }


        /// <summary>
        /// ввод, обработка и вывод данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (Hamming.HammingBase == 0 || (textBox1.Text.Length % Hamming.HammingBase != 0 && textBox1.Text.Length % Hamming.HammingBase < 3))  //если не выбрано
                return;

            var codes = SplitToArray (textBox1.Text, Hamming.HammingBase); //получение массива входных данных, разбитых по размеру второго параметра хэмминга, для обработки

            textBox2.Text = Hamming.Encode(codes);
        }


        /// <summary>
        /// вывод и обработка проверки ошибок и исправления их
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (Hamming.HammingFinal == 0 || (textBox3.Text.Length % Hamming.HammingFinal != 0 && textBox3.Text.Length % Hamming.HammingFinal < 3)) //если не выбрано
                return;

            var codes = SplitToArray (textBox3.Text, Hamming.HammingFinal);

            var res = "";
            var errors = Hamming.FindError (codes, out res);

            if (errors.Length == 0)
                label2.Text = "Ошибок не обнаружено";
            else {
                label2.Text = "";
                foreach (var error in errors) {
                    label2.Text += "Ошибка обнаружена в " + error + " бите!\n";
                }
            }
            textBox4.Text = res;
        }


        /// <summary>
        /// строка к массиву строк по длине
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private string[] SplitToArray(string str, int len)
        {
            str = str.Replace(" ", "");
            string[] res = new string[str.Length % len == 0
                ? str.Length / len
                : str.Length / len + 1];

            for (int i = 0; i < res.Length; i++) {
                if (str.Length < len)
                    res[i] = str;
                else {
                    res[i] = str.Substring(0, len);
                    str = str.Substring(len);
                }
            }

            return res;
        }


        /// <summary>
        /// задание параметров хэмминга
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Hamming.HammingBase = Convert.ToInt32(comboBox1.Text);
            Hamming.HammingFinal = Hamming.HammingBase + Hamming.NeedToAddBits (Hamming.HammingBase);
            label1.Text = "Код Хэмминга (" + Hamming.HammingFinal + ", " + Hamming.HammingBase + ").";
        }
    }
}
