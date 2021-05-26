using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace tiik_lab_7
{
    public partial class Form1 : Form
    {
        private int hammingBase_ = 0;   //второе число в хэмминге
        private int hammingFinal_ = 0;  //первое число в хэмминге


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
            if (hammingBase_ == 0)  //если не выбрано
                return;

            var codes = SplitToArray (textBox1.Text, hammingBase_); //получение массива входных данных, разбитых по размеру второго параметра хэмминга, для обработки

            textBox2.Text = "";
            for (int i = 0; i < codes.Length; i++) {    //обход этого массива
                var init = codes[i].ToBitArray();   //конверт строки в бит массив
                var arr = CompleteBits(init, false);    //добавление места для контрольных битов
                CalculateControlBytes(ref arr, init.Length);    //заполнение векторов для контрольных битов

                textBox2.Text += arr.ToString2() + " "; //вывод результата
            }
            textBox2.Text = textBox2.Text.TrimEnd();
        }


        /// <summary>
        /// вывод и обработка проверки ошибок и исправления их
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (hammingFinal_ == 0) //если не выбрано
                return;

            var codes = SplitToArray (textBox3.Text, hammingFinal_);

            label2.Text = "";
            textBox4.Text = "";
            for (int i = 0; i < codes.Length; i++) {
                var init = codes[i].ToBitArray();
                var arr = CompleteBits (init, true);
                CalculateControlBytes(ref arr, init.Length - init.NumberOfControlBits());
                var res = CompareAndCorrect (ref init, arr);

                if (res != -1)
                    label2.Text += "Ошибка обнаружена в " + (res + 1) * (i + 1) + " бите!\n";

                textBox4.Text += init.ToString2() + " ";
            }
            textBox4.Text = textBox4.Text.TrimEnd();

            if (label2.Text == "")
                label2.Text = "Ошибка не обнаружена";
        }


        /// <summary>
        /// после вычисления контрольных битов для сообщения предположительно с ошибкой, сравнение контрольных битов исходных с получившимися. сумма позиций несовпадающих битов будет указывать на место с ошибкой, бит в котором надо просто инвертировать
        /// </summary>
        /// <param name="original"></param>
        /// <param name="calculated"></param>
        /// <returns></returns>
        private int CompareAndCorrect (ref BitArray original, BitArray calculated)
        {
            int errorIndex = 0;
            for (int i = 0; i < original.NumberOfControlBits(); i++) {
                var currPower = (int)Math.Pow (2, i) - 1;
                if (original[currPower] != calculated[currPower])
                    errorIndex += currPower + 1;
            }

            if (--errorIndex != -1)
                original[errorIndex] = !original[errorIndex];

            return errorIndex;
        }


        /// <summary>
        /// добавление контрольных битов при кодировании или уже имеющихся на нулевые при проверке ошибки
        /// </summary>
        /// <param name="number"></param>
        /// <param name="toReplace"></param>
        /// <returns></returns>
        private BitArray CompleteBits(BitArray number, bool toReplace)
        {
            BitArray array;

            if (!toReplace)
                array = new BitArray (number.Length + MaxPowerOfTwo (number.Length));
            else
                array = new BitArray(number.Length);

            for (int i = 0, k = 0; i < array.Length; i++)
            {
                if (((i + 1) & i) == 0) {
                    array[i] = false;
                    if (toReplace)
                        k++;
                }
                else
                    array[i] = number[k++];
            }

            return array;
        }


        /// <summary>
        /// заполнение массивов для контрольных битов
        /// </summary>
        /// <param name="numberLen"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private void CalculateControlBytes(ref BitArray arr, int numberLen)
        {
            for (int i = 0; i < MaxPowerOfTwo (numberLen); i++) {
                var currPow = (int)Math.Pow(2, i) - 1;
                int count = 0;
                for (int j = currPow; j < arr.Length; j += (currPow + 1) * 2) {
                    for (int k = j; k < j + currPow + 1; k++) {
                        count += (arr[k] == true ? 1 : 0);
                    }
                }

                arr[currPow] = (count % 2 == 1);
            }
        }


        /// <summary>
        /// вычисление значения, для получения первого числа из первого (вычисления макс колва бит, которые можно прибавить к исх строке)
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        private int MaxPowerOfTwo(int len)
        {
            List <bool> list = new List <bool>();

            int i = 0, c = 0;
            for (; c < len; i++, c++) {
                if (((i + 1) & i) == 0) {
                    list.Add(false);
                    c--;
                }
                else
                    list.Add(true);
                    
            }

            if (list.Last())
                return i - c;
            else
                return i - c - 1;
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
            hammingBase_ = Convert.ToInt32(comboBox1.Text);
            hammingFinal_ = hammingBase_ + MaxPowerOfTwo (hammingBase_);
            label1.Text = "Код Хэмминга (" + hammingFinal_ + ", " + hammingBase_ + ").";
        }
    }
}
