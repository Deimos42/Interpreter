using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace laba2
{
    // Iden соотносится с num и size, myIden
    // Iden2 соотносится с num2 и size2, myIden2
    public class Iden //  unsafe
    {
        public int atr;
        public string id;

    }

    public class Iden2 //  unsafe
    {
        public int atr;
        public string id;

    }

    public class Mass // unsafe
    {
        public int size;
        public int size2;
        public Iden[] num = new Iden[100];
        public Iden2[] num2 = new Iden2[100];

        public Mass()
        {
            size = 0;
            Iden myIden = new Iden();
            myIden.atr = 0;
            myIden.id = null;
            num[0] = myIden;

            size2 = 0;
            Iden2 myIden2 = new Iden2();
            myIden2.atr = 0;
            myIden2.id = null;
            num2[0] = myIden2;
        }
        ~Mass()
        {
        }

        public int Inde(char nu1)
        {
            int fin1 = 0;
            int g = size; // возможно понадобится и size2
            string alph = "0123456789ЦИКЛциклAaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz ";
            bool flag3 = true;
            int k = 0;
            while (flag3 == true)
            {
                if (nu1 == alph[k])
                {
                    fin1 = k;
                    flag3 = false;
                    break;
                }
                else
                    k += 1;

                if (alph[k] == ' ')  // исключение
                {
                    fin1 = -1;
                    flag3 = false;
                    break;
                }

            }
            return fin1;
        }

        public void Add1(int el1, string el2)
        {
            int k2 = 0;
            string Ziph = "0123456789 ";
            char per;
            bool flag3 = true;
            char res = ' ';
            el2 = el2.Trim();


            per = el2[0];
            foreach (char i in Ziph)
            {
                if (i == per)
                {
                    foreach (char i2 in el2)
                    {
                        foreach (char i3 in Ziph)
                        {
                            res = i3;
                            if (i2 == i3)
                            {
                                res = i3;
                                break;
                            }
                        }
                        if (res == ' ')
                        {
                            flag3 = false;
                            break;
                        }
                    }
                }
            }

            if (flag3 == false)
            {
                // очистка таблиц
                MessageBox.Show("ошибка(в лексеме цифры не должны стоять перед буквами)");
                return;
            }

            // возможно стоит перенести в начало метода
            el2 = el2 + ' '; // можно реализовать функцию прибавляющую число пробелов равное

            Iden elem = new Iden();
            elem.atr = el1;
            elem.id = el2;
            num[size] = elem;
            size += 1;
        }


        public void Add2(int el1, string el2)
        {
            char pred1;
            char pred2;
            int pr1 = 0;
            int pr2 = 0;
            int k = 0;
            int k2 = 0;
            int index = 0; // номер буквы идентификатора
            int index2 = 0;
            bool flag = true;

            el2 = el2.Trim(); // удаляем пробелы в начале и конце строки
            int len_el2 = el2.Length;

            string Ziph = "0123456789 ";
            char per;
            bool flag3 = true;
            char res = ' ';

            per = el2[0];
            foreach (char i in Ziph)
            {
                if (i == per)
                {
                    foreach (char i2 in el2)
                    {
                        foreach (char i3 in Ziph)
                        {
                            res = i3;
                            if (i2 == i3)
                            {
                                res = i3;
                                break;
                            }
                        }
                        if (res == ' ')
                        {
                            flag3 = false;
                            break;
                        }
                    }
                }
            }

            if (flag3 == false)
            {
                //MessageBox.Show("ошибка(в лексеме цифры не должны стоять перед буквами)");
                return;
            }
            // возможно стоит перенести в начало метода
            el2 = el2 + ' ';

            // проверка на повторное вхождение идентификатора 
            while (k2 < size2)
            {
                if (el2 == num2[k2].id)
                {
                    flag = false;
                }
                k2 += 1;
            }

            if (flag == false)
            {
                return;
            }

            foreach (int i in el2)
            {
                k += 1;
            }

            if (size2 == 0)
            {
                Iden2 elem = new Iden2();
                elem.atr = el1;
                elem.id = el2;
                num2[size2] = elem;
                size2 += 1;
            }
            else
            {
                Iden2 elem = new Iden2();
                elem.atr = el1;
                elem.id = el2;
                num2[size2] = elem;
                int g = size2;
                char nu1 = num2[g - 1].id[0];
                char nu2 = num2[g].id[0];
                int fin1 = 0; // номер буквы в алфавите
                int fin2 = 0;
                bool flag2 = true;
                bool sort = true;

                while (flag2 == true)
                {
                    // предварительная проверка
                    for (index2 = 0; index2 <= index; index2++)
                    {
                        // можно еще добавить проверку nu2
                        nu1 = num2[g - 1].id[index2];
                        if (nu1 == ' ')
                        {
                            flag2 = false;
                            break;
                        }
                    }
                    if (flag2 == false)
                        break;

                    nu1 = num2[g - 1].id[index];
                    nu2 = num2[g].id[index];

                    fin1 = Inde(nu1);
                    fin2 = Inde(nu2);

                    if (fin2 < fin1)  // if (fin2 <= fin1)
                    {
                        // просматриваем предыдущую букву для правильной сортировки
                        if ((index > 0) & (g > 0))
                        {
                            pred1 = num2[g - 1].id[index - 1];
                            pred2 = num2[g].id[index - 1];
                            pr1 = Inde(pred1);
                            pr2 = Inde(pred2);
                            if (pr1 != pr2)
                            {
                                sort = false;
                            }
                        }

                        if (sort == true)
                        {
                            Iden2 elem2 = new Iden2();
                            elem2 = num2[g - 1];
                            num2[g - 1] = elem;
                            num2[g] = elem2;
                        }

                        g -= 1;

                        if (g == 0)
                        {
                            flag2 = false;
                        }
                    }

                    else if (fin2 == fin1)
                    {
                        if (index >= k)  //  if (index >= k)
                        {
                            if (index > 0)
                            {
                                flag2 = false;
                            }
                        }

                        if (g == 0)
                        {
                            g += 1;
                        }
                        index += 1;
                    }

                    else if (fin2 > fin1)
                    {
                        k = 0;
                        g -= 1;

                        if (g == 0)
                        {
                            flag2 = false;
                        }
                    }
                }
                size2 += 1;
            }
        }

        public void Analiz(string s)
        {
            //Mass L = new Mass();
            int leks = 0; // переменная отвечающая за то чтобы буквы и цифры не чередовались в лексеме
            int k = 0;
            int k3 = 0;
            bool flag = false;
            bool flag2 = false;
            int index = 0;
            int vxod = 0;
            int vixod = 0;
            string add = ""; // будем передавать её в Add в качестве el2 
            char elem_pred = ' ';
            char elem = s[index];
            string Alph = "ЦИКЛAaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";  //
            string Ziph = "0123456789";
            while (elem != '^') // проверяет строку //   while (elem != ';')
            {
                add = "";

                if (elem == '^')  // конец текста
                {
                    break;
                }

                /*
                if (elem == '\n')
                {
                    index += 1;
                    elem_pred = s[index - 1];
                    elem = s[index];
                }
                */


                if (elem == ' ')
                {
                    index += 1;
                    elem_pred = s[index - 1];
                    elem = s[index];
                }


                if (s[index] != ';')  // if (s[index] != '>')
                {
                    if (s[index + 1] != ';')  // if (s[index + 1] != '>')
                    {
                        if ((s[index] == 'i') & (s[index + 1] == 'f') & (s[index + 2] == '('))
                        {
                            //leks = 0;
                            add = "if";
                            Add1(101, add);  // 
                            add = "(";
                            Add1(15, add);  // 
                            index += 3;
                            elem_pred = s[index - 1];
                            elem = s[index];
                        }
                    }
                }

                if (s[index] != ';')
                {
                    if (s[index + 1] != ';')
                    {
                        if (s[index + 2] != ';')
                        {
                            if ((s[index] == 'e') & (s[index + 1] == 'l') & (s[index + 2] == 's') & (s[index + 3] == 'e'))
                            {
                                //leks = 0;
                                add = "else";
                                Add1(102, add);  // 
                                index += 4;
                                elem_pred = s[index - 1];
                                elem = s[index];
                            }
                        }
                    }
                }

                // блок ЦИКЛ
                if (s[index] != ';')
                {
                    if (s[index + 1] != ';')
                    {
                        if (s[index + 2] != ';')
                        {
                            if (s[index + 3] != ';')
                            {
                                if ((s[index] == 'Ц') & (s[index + 1] == 'И') & (s[index + 2] == 'К') & (s[index + 3] == 'Л')) // & (s[index + 4] == ' '
                                {
                                    //leks = 0;
                                    add = "ЦИКЛ";
                                    Add1(103, add);  // 
                                    index += 3; // index += 5
                                    elem_pred = s[index - 1];
                                    elem = s[index];
                                }

                            }
                        }
                    }
                }

                if (s[index] != ';')
                {
                    if (s[index + 1] != ';')
                    {
                        if (s[index + 2] != ';')
                        {
                            if (s[index + 3] != ';')
                            {
                                if (s[index + 4] != ';')
                                {
                                    if (s[index + 5] != ';')
                                    {
                                        if (s[index + 6] != ';')
                                        {
                                            if (s[index + 7] != ';')
                                            {
                                                if ((s[index] == 'П') & (s[index + 1] == 'О') & (s[index + 2] == 'К') & (s[index + 3] == 'А') & (s[index + 4] == '_') & (s[index + 5] == 'Н') & (s[index + 6] == 'Е') & (s[index + 7] == ' '))
                                                {
                                                    //leks = 0;
                                                    add = "ПОКА_НЕ";
                                                    Add1(104, add);  // 
                                                    index += 7;
                                                    elem_pred = s[index - 1];
                                                    elem = s[index];
                                                }


                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // проверяем вхождение
                // не забыть про проверку констант отдельно
                add = "";
                k = 0;
                vxod = 0;
                vixod = 0;
                flag = false;
                while (flag == false)
                {
                    if (Alph[k] == s[index])
                    {
                        flag = true;
                        break;
                    }
                    if ((Alph[k] == 'z') | (k == 40))
                    {
                        break;
                    }

                    k += 1;
                }

                k3 = 0;
                flag2 = false;
                while (flag2 == false)
                {
                    if (Ziph[k3] == s[index])
                    {
                        flag2 = true;
                        break;
                    }
                    if ((Ziph[k3] == '9') | (k3 == 10))
                        break;
                    k3 += 1;
                }


                if (flag == true) // значит наткнулись на букву
                {
                    //if (leks == 2)
                    //{
                    //    MessageBox.Show("ошибка(в лексеме цифры не должны стоять перед буквами)");
                    //    //return;
                    //    break;
                    //}
                    //leks = 1;
                    vxod = index;
                    while ((elem != ':') & (elem != '/') & (elem != '-') & (elem != '>') & (elem != '<') & (elem != ';') & (elem != '(') & (elem != ')') & (elem != ' ')) // & (elem != ' ')
                    {
                        index += 1;
                        elem_pred = s[index - 1];
                        elem = s[index];
                        vixod = index;
                    }

                    if ((elem == ':') | (elem == '/') | (elem == '-') | (elem == '>') | (elem == '<') | (elem == ')') | (elem == ';') | (elem == ' ')) // ?
                    {
                        //MessageBox.Show(" " +s[vxod] + " ");
                        //MessageBox.Show(" " + s[vixod] + " ");
                        if (s[vxod] == 'Л') //  if ((s[vxod] == 'Ц')
                        {
                            for (int k2 = vxod; k2 < vixod; k2++)
                            {
                                add = add + s[k2];
                            }
                            //MessageBox.Show("321");
                            //MessageBox.Show(add);
                        }
                        else
                        {
                            for (int k2 = vxod; k2 < vixod; k2++)
                            {
                                add = add + s[k2];
                            }
                            //MessageBox.Show("123");
                            //MessageBox.Show(add);
                            Add1(1, add); // добавление идентификатора в таблицу 
                            Add2(1, add);
                        }
                    }

                    /*
                    if(elem == ':')
                    {
                        index += 1;
                    }
                    */

                }
                else if (flag2 == true) // значит наткнулись на цифру
                {
                    /*
                    if (leks == 1)
                    {
                        MessageBox.Show("ошибка(в лексеме цифры не должны следовать после букв)");
                        //return;
                        break;
                    */

                    //leks = 2;
                    vxod = index;
                    while ((elem != ':') & (elem != '/') & (elem != '-') & (elem != '>') & (elem != '<') & (elem != ';') & (elem != '(') & (elem != ')') & (elem != ' ')) //
                    {
                        index += 1;
                        elem_pred = s[index - 1];
                        elem = s[index];
                        vixod = index;
                    }

                    if ((elem == ':') | (elem == '/') | (elem == '-') | (elem == '>') | (elem == '<') | (elem == ')') | (elem == ';') | (elem == ' ')) //
                    {
                        for (int k2 = vxod; k2 < vixod; k2++)
                        {
                            add = add + s[k2];
                        }
                        //MessageBox.Show(add);
                        Add1(2, add); // добавление идентификатора в таблицу 
                        Add2(2, add);
                    }

                    /*
                    if (elem == ':')
                    {
                        index += 1;
                    }
                    */
                }

                if (s[index] == ':')
                {
                    if (s[index + 1] == '=')
                    {
                        //leks = 0;
                        add = ":=";
                        Add1(12, add);
                        index += 2;
                        elem_pred = s[index - 1];
                        elem = s[index];
                    }
                }
                /*
                else if (s[index] == '=')
                {
                    //leks = 0;
                    add = "=";
                    Add1(12, add);
                    index += 1;
                    elem_pred = s[index - 1];
                    elem = s[index];
                }
                */

                else if (s[index] == '-')
                {
                    //leks = 0;
                    add = "-";
                    Add1(11, add);
                    index += 1;
                    elem_pred = s[index - 1];
                    elem = s[index];
                }
                else if (s[index] == '/')
                {
                    //leks = 0;
                    add = "/";
                    Add1(10, add);
                    index += 1;
                    elem_pred = s[index - 1];
                    elem = s[index];
                }
                else if (s[index] == '>')
                {
                    //leks = 0;
                    add = ">";
                    Add1(13, add);
                    index += 1;
                    elem_pred = s[index - 1];
                    elem = s[index];
                }
                else if (s[index] == '<')
                {
                    //leks = 0;
                    add = "<";
                    Add1(14, add);
                    index += 1;
                    elem_pred = s[index - 1];
                    elem = s[index];
                }

                // условия if и for можно сделать в отдельных блоках(в циклах while)
                // в if и for так же добавлять ключевые слова
                else if (s[index] == '(') // условие if
                {
                    //add = ")";   
                    //Add1(15, add);  // 
                    //leks = 0;
                    index += 1;
                    elem_pred = s[index - 1];
                    elem = s[index];
                }

                else if (s[index] == ')') // окончание условия if
                {
                    //leks = 0;
                    add = ")";   //  может быть пробел и не стоит добавлять как разделитель
                    Add1(16, add);  // 
                    index += 1;    // могут возникнуть проблемы с последующим пробелом
                    if (s[index] == ' ') // цикл for // может стоит убрать
                    {
                        index += 1;
                        elem_pred = s[index - 1];
                        elem = s[index];
                    }
                }

                else if (s[index] == ';')
                {
                    //MessageBox.Show("1");
                    //leks = 0;
                    add = ";";
                    Add1(17, add);  // здесь индекс не меняем так как это последний символ строки

                    index += 1;

                    if (s[index] == '^')
                    {
                        elem_pred = s[index - 1];
                        elem = s[index];
                    }
                    else if (s[index] == ' ')
                    {
                        index += 1;
                        elem_pred = s[index - 1];
                        elem = s[index];
                    }
                    else
                    {
                        index += 2;
                        elem_pred = s[index - 1];
                        elem = s[index];
                    }

                }

                //index += 1;
                //elem_pred = s[index - 1];
                //elem = s[index];
            }
            //Console.WriteLine("Введите строку: ");
            //string w = Console.ReadLine();
        }






    }


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
