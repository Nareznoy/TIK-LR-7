using System;
using System.Collections;

namespace TiK_LR_7
{
    static class Hamming
    {
        private static int _hammingBase = 7;
        private static int _hammingFinal = 11;

        public static int HammingFinal { get { return _hammingFinal; } }
        public static int HammingBase { get { return _hammingBase; } }

        private static int _controlBitsNumbers;


        /// <summary>
        ///     Синтезирование кода Хэмминга для входной строки
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string Encode (string inputStr)
        {
            BitArray inputBitArray = inputStr.ToBitArray();   //преобразование строки в бинарный массив
            BitArray arrayWithControlBites = new BitArray(inputBitArray.Length + (_hammingFinal - _hammingBase));

            // добавление контрольных битов
            for (int i = 0, k = 0; i < arrayWithControlBites.Length; i++)
            {
                if (((i + 1) & i) == 0)
                {
                    arrayWithControlBites[i] = false;
                }
                else
                    arrayWithControlBites[i] = inputBitArray[k++];
            }

            GetControlBytes(ref arrayWithControlBites);    //заполнение векторов для контрольных битов

            return arrayWithControlBites.ToBinString();
        }


        /// <summary>
        ///     Поиск и исправление ошибки во входной строке
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="fixedString"></param>
        /// <returns></returns>
        public static int FindError(string inputStr, out string fixedString)
        {
            int errors = 0;

            BitArray inputBitArray = inputStr.ToBitArray();
            BitArray arrayWithControlBites = new BitArray(inputBitArray.Length); ;

            // замена контрльных битов на нулевые
            for (int i = 0, k = 0; i < arrayWithControlBites.Length; i++)
            {
                if (((i + 1) & i) == 0)
                {
                    arrayWithControlBites[i] = false;
                    k++;
                }
                else
                    arrayWithControlBites[i] = inputBitArray[k++];
            }

            /// вычисления количества контрольных битов в сообщении на проверку
            int length = arrayWithControlBites.Length;
            int power = 0;
            while (true)
            {
                if (Math.Pow(2, power) > length)
                {
                    _controlBitsNumbers = power;
                    break;
                }
                power++;
            }

            GetControlBytes(ref arrayWithControlBites);

            int res = CorrectArray(ref inputBitArray, arrayWithControlBites);
            if (res != -1)
                errors = (res + 1);

            fixedString = inputBitArray.ToBinString();
            return errors;
        }



        /// <summary>
        ///     Заполнение массивов контрольных битов
        /// </summary>
        /// <param name="inputBitArray"></param>
        public static void GetControlBytes(ref BitArray inputBitArray)
        {
            for (int i = 0; i < (_hammingFinal - _hammingBase); i++)
            {
                int currentPow = (int)Math.Pow(2, i) - 1;
                int count = 0;
                for (int j = currentPow; j < inputBitArray.Length; j += (currentPow + 1) * 2)
                {
                    for (int k = j; k < j + currentPow + 1 && k < inputBitArray.Length; k++)
                    {
                        count += (inputBitArray[k] == true ? 1 : 0);
                    }
                }

                inputBitArray[currentPow] = (count % 2 == 1);
            }
        }


        /// <summary>
        ///     Корректирование входного бинарного массива
        /// </summary>
        /// <param name="inputArray"></param>
        /// <param name="calculatedArray"></param>
        /// <returns></returns>
        public static int CorrectArray(ref BitArray inputArray, BitArray calculatedArray)
        {
            int errorIndex = 0;
            for (int i = 0; i < _controlBitsNumbers; i++)
            {
                var currPower = (int)Math.Pow(2, i) - 1;
                if (inputArray[currPower] != calculatedArray[currPower])
                    errorIndex += currPower + 1;
            }

            if (--errorIndex != -1)
                inputArray[errorIndex] = !inputArray[errorIndex];

            return errorIndex;
        }


        /// <summary>
        ///     Возвращает входную строку в виде бинарного массива
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static BitArray ToBitArray(this string inputStr)
        {
            var temp = new BitArray(inputStr.Length);
            for (int i = 0; i < inputStr.Length; i++)
            {
                temp[i] = inputStr[i] == '1';
            }

            return temp;
        }


        /// <summary>
        ///     Возвращает входной бинарный массив в виде строки
        /// </summary>
        /// <param name="inputBitArray"></param>
        /// <returns></returns>
        public static string ToBinString(this BitArray inputBitArray)
        {
            string outputStr = "";
            foreach (bool elem in inputBitArray)
            {
                outputStr += (elem == true ? "1" : "0");
            }

            return outputStr;
        }
    }
}
