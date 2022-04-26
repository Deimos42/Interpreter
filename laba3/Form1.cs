using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace laba2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {         
            this.dataGridView1.Rows.Clear();
            this.dataGridView2.Rows.Clear();
            Mass L = new Mass();
            string s;
            string g = "";
            int k2 = 0;
            s = textBox1.Text;
            s = s + '^'; // символ конца
            int k = 0;

            int j = 0;
            int inde = 0;
            string add_str = "";
            string[] strok = new string[30];

            while (s[j] != '^')  
            {

                //if(j != 0)  //
                //    j += 2; // j += 2;
                add_str = "";
                while (s[j] != ';')
                {
                    add_str = add_str + s[j];
                    //MessageBox.Show(add_str);
                    j += 1;
                }
                if (s[j] == ';')
                {
                    add_str = add_str + s[j];
                    //MessageBox.Show(add_str);
                    j += 1;
                    if ((s[j] != '^') & (s[j] != ' ')) // ? теперь s[j] = '>' или первый символ новой строки
                    {
                        j += 2;
                    }

                }

                strok[inde] = add_str;
                inde += 1;
                //MessageBox.Show("yes");

            }

            string s3 = "";
            j = 0;
            k = 0;
            k2 = 0;

            /*
            try 
            {	  
                L.Analiz(s);               
            }
            catch (Exception)
            {
                this.dataGridView1.Rows.Clear();
                this.dataGridView2.Rows.Clear();
                MessageBox.Show("ошибка(в лексеме цифры не должны стоять перед буквами)");
            }
            */


            L.Analiz(s);
            dataGridView1.Rows.Add();   // добавление строки 
            MessageBox.Show("size: " + L.size);

            L.Add_L(0, "v");  // символ конца для таблицы L
            L.Add_L(24, "<S>");
            bool flag_S;

            flag_S = L.Semant();

            // TODO: подумать оставлять ли оповещение о неправильной лексеме или убирать
            // TODO: разобраться с допусками кода с "+" и "-"(не всегда правильно выделяются лексемы)
            if (flag_S == true)
                MessageBox.Show("Допустить");
            else
                MessageBox.Show("Отвергнуть");


            // заполнение таблицы лексем          
            while (k < L.size)
            {
                dataGridView1.Rows[k].Cells[0].Value = L.num[k].id;  // L.num[k].atr;
                dataGridView1.Rows[k].Cells[1].Value = L.num[k].atr;
                dataGridView1.Rows.Add();	// добавление строки 
                k += 1;
            }
            

            /*
            dataGridView2.Rows.Add();
            k = 0;
            // заполнение таблицы идентификаторов и констант
            while (k < L.size2)
            {
                dataGridView2.Rows[k].Cells[0].Value = L.num2[k].id;
                dataGridView2.Rows[k].Cells[1].Value = L.num2[k].atr;
                dataGridView2.Rows.Add();	// добавление строки 
                k += 1;
            }
            */

            dataGridView2.Rows.Add();
            k = 0;
            // заполнение таблицы идентификаторов и констант
            while (k < L.sizeL)
            {
                dataGridView2.Rows[k].Cells[0].Value = L.numL[k].id;
                dataGridView2.Rows[k].Cells[1].Value = L.numL[k].atr;
                dataGridView2.Rows.Add();	// добавление строки 
                k += 1;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            string fileText = System.IO.File.ReadAllText(filename, Encoding.UTF8);
            textBox1.Text = fileText;
            //MessageBox.Show("Файл открыт");
        }
    }
}
