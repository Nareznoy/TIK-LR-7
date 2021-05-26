using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tiik_lab_7
{
    static class Hamming
    {
        public static int HammingBase { get; set; }   //второе число в хэмминге
        public static int HammingFinal { get; set; }  //первое число в хэмминге

        static Hamming()
        {
            HammingBase = 0;
            HammingFinal = 0;
        }


        public static string Encode (string[] codes)
        {
            string output = "";
            for (int i = 0; i < codes.Length; i++)
            {    //обход этого массива
                var init = codes[i].ToBitArray();   //конверт строки в бит массив
                var arr = CompleteBits(init, false);    //добавление места для контрольных битов
                CalculateControlBytes(ref arr, init.Length);    //заполнение векторов для контрольных битов

                output += arr.ToString2() + " "; //вывод результата
            }

            return output.TrimEnd();
        }


        public static int[] FindError (string[] codes, out string correctedText)
        {
            correctedText = "";
            List <int> errors = new List<int>();
            for (int i = 0; i < codes.Length; i++)
            {
                var init = codes[i].ToBitArray();
                var arr = CompleteBits(init, true);
                CalculateControlBytes(ref arr, init.Length - init.NumberOfControlBits());
                var res = CompareAndCorrect(ref init, arr);

                if (res != -1)
                    errors.Add((res + 1) * (i + 1));

                correctedText += init.ToString2() + " ";
            }

            correctedText = correctedText.TrimEnd();
            return errors.ToArray();
        } 


        /// <summary>
        /// добавление контрольных битов при кодировании или уже имеющихся на нулевые при проверке ошибки
        /// </summary>
        /// <param name="number"></param>
        /// <param name="toReplace"></param>
        /// <returns></returns>
        public static BitArray CompleteBits(BitArray number, bool toReplace)
        {
            BitArray array;

            if (!toReplace)
                array = new BitArray(number.Length + NeedToAddBits(number.Length));
            else
                array = new BitArray(number.Length);

            for (int i = 0, k = 0; i < array.Length; i++)
            {
                if (((i + 1) & i) == 0)
                {
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
        public static void CalculateControlBytes(ref BitArray arr, int numberLen)
        {
            for (int i = 0; i < NeedToAddBits(numberLen); i++)
            {
                var currPow = (int)Math.Pow(2, i) - 1;
                int count = 0;
                for (int j = currPow; j < arr.Length; j += (currPow + 1) * 2)
                {
                    for (int k = j; k < j + currPow + 1 && k < arr.Length; k++)
                    {
                        count += (arr[k] == true ? 1 : 0);
                    }
                }

                arr[currPow] = (count % 2 == 1);
            }
        }


        /// <summary>
        /// после вычисления контрольных битов для сообщения предположительно с ошибкой, сравнение контрольных битов исходных с получившимися. сумма позиций несовпадающих битов будет указывать на место с ошибкой, бит в котором надо просто инвертировать
        /// </summary>
        /// <param name="original"></param>
        /// <param name="calculated"></param>
        /// <returns></returns>
        public static int CompareAndCorrect(ref BitArray original, BitArray calculated)
        {
            int errorIndex = 0;
            for (int i = 0; i < original.NumberOfControlBits(); i++)
            {
                var currPower = (int)Math.Pow(2, i) - 1;
                if (original[currPower] != calculated[currPower])
                    errorIndex += currPower + 1;
            }

            if (--errorIndex != -1)
                original[errorIndex] = !original[errorIndex];

            return errorIndex;
        }


        /// <summary>
        /// вычисление значения, для получения первого числа из первого (вычисления макс колва бит, которые можно прибавить к исх строке)
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static int NeedToAddBits(int len)
        {
            List<bool> list = new List<bool>();

            int i = 0, c = 0;
            for (; c < len; i++, c++)
            {
                if (((i + 1) & i) == 0)
                {
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
    }
}
