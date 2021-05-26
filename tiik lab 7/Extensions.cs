using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tiik_lab_7
{
    static class Extensions
    {
        /// <summary>
        /// получение элемента строки с конца
        /// </summary>
        /// <param name="number"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool At(this int number, int pos)
        {
            var temp = Convert.ToString(number, 2);
            if (temp.Length - pos >= 0)
                return Convert.ToString(number, 2)[temp.Length - pos] == '1';
            else
                return false;
        }


        /// <summary>
        /// бин строку в бит массив
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static BitArray ToBitArray(this string str)
        {
            var temp = new BitArray(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                temp[i] = str[i] == '1';
            }

            return temp;
        }


        /// <summary>
        /// превращение бит массива в бин строку
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string ToString2(this BitArray arr)
        {
            string res = "";
            foreach (bool elem in arr)
            {
                res += (elem == true ? "1" : "0");
            }

            return res;
        }


        /// <summary>
        /// вычисления количества контрольных битов в сообщении на проверку
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static int NumberOfControlBits(this BitArray arr)
        {
            int length = arr.Length;
            int power = 0;
            while (true)
            {
                if (Math.Pow(2, power) > length)
                    return power;

                power++;
            }
        }
    }
}
