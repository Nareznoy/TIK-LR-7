using System;
using System.Windows.Forms;

namespace TiK_LR_7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        /// <summary>
        ///     Обработчик события нажатия на кнопку "Вычислить код"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = Hamming.Encode(textBox1.Text);
        }


        /// <summary>
        ///     Обработчик события нажатия на кнопку "Проверить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            int errorIndex = Hamming.FindError (textBox3.Text, out string outString);

            if (errorIndex == 0)
                label2.Text = "Данные приняты верно";
            else {
                label2.Text = "";
                    label2.Text += "Ошибка в " + errorIndex + " бите\n";
            }
            textBox4.Text = outString;
        }
    }
}
