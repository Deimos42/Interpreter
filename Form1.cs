using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

// теперь в качестве начального символа выступает совокупность операторов вместо <S>
namespace СинтаксическийАнализ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Типы лексем
        const int CIKL = 103;
        //const int DO       = 1;
        const int WHILE_NOT = 104;
        const int IF          = 101;
        const int IF_SPIS     = 25;
        const int ELSE        = 102;
        //const int ENDIF       = 5;
        const int LEFT        = 15; // (
        const int RIGHT       = 16; // )
        const int SEMICOLON   = 17; // ;
        const int LT          = 14; // <
        const int GT          = 13; // > 
        const int SET         = 12; // :=  
        const int EOF         = 99; // конец программы
        const int IDENT       = 1; // идентификатор
        const int NUM         = 2; // число
        //const int COMMA       = 15; // to
        const int SUB         = 11; // -
        const int DELE         = 10; // /
        const int ERROR       = -6; // лексическая ошибка

        // Нетерминалы
        const int PROGRAMMA   = 24;
        const int OPER        = 26;
        const int SOV_OP      = 29;
        const int E           = 30;
        const int E_SP        = 31;
        const int T           = 32;
        const int T_SP        = 33;
        const int F           = 34;
        const int LOG_VIR     = 27;
        const int LOG_OP      = 28;

        const int MARKER_DNA  = 0;
        const int PRISV       = 39;
        const int PER_PO_0    = 40;
        const int BEZUSL_PER  = 41;
        const int METKA       = 42;
        const int ВЫЧЕСТЬ     = 43;
        const int ДЕЛЕНИЕ      = 44;
        const int OMORE       = 45;
        const int OLOW        = 46;
        const int O_PER_PO_SR = 47;
        const int O_UVEL_NA_1 = 48;

        const int DOPUST      = 49;
        const int ERR         = -1;

        const int MAX_LEX     = 1000; // Максимальное количество лексем

        // Состояния конечного автомата при лексическом разборе
        const int НачалоЛексемы               = 50;
        const int ПродолжениеИдентификатора   = 51;
        const int ПродолжениеЧисла            = 52;
        //const int ПродолжениеКомментария      = 3;
        //const int НачалоЗавершенияКомментария = 4;
        //const int ЗавершениеКомментария       = 5;

        struct pamiat
        {
            public string Name;
            public int value;
        };

        pamiat[] Pam = new pamiat[100];
        int[,] PrCode = new int[100, 4];
        int[] Magazin = new int[500];
        int Pam_top;
        int PrCode_top;
        int table_top;


        // Данные элемента списка - название идентификатора и его атрибут
        public struct Elem
        {
            public string code;    // Название идентификатора (оно же является ключом)
            public int attr;       // Атрибут
        };

        // Элемент списка
        public class Node
        {
            public Node(Elem data)
            {
                Data = data;
            }
            public Elem Data { get; set; }
            public Node Next { get; set; }
        }

        // Односвязный список
        public class LinkedList
        {
            // Головной элемент
            Node head;

            // Количество элементов в списке
            int count;

            public int Count { get { return count; } }
            public bool IsEmpty { get { return count == 0; } }

            // Очистка списка
            public void Clear()
            {
                head = null;
                count = 0;
            }

            // Содержит ли список элемент
            public int Contains(string code)
            {
                int i;
                Node current = head;

                for (i = 0; current != null; current = current.Next, ++i)
                {
                    if (current.Data.code == code)
                        return i;
                    current = current.Next;
                }
                return -1;
            }

            // Вернуть символьное представление элемента таблицы
            public string Elem (int n)
            {
                Node current = head;

                while (n-- != 0)
                    current = current.Next;
                return current.Data.code;
            }

            // Добавление элемента в конец списка
            public int AppendEnd(Elem data)
            {
                int i;
                Node t;
                Node node = new Node(data);

                // Список пуст?
                if (head == null)
                {
                    head = node;
                    ++count;
                    return 0;
                }

                // Находим конец списка
                for (i = 0, t = head; t.Next != null; t = t.Next, ++i)
                    ;

                t.Next = node;
                ++count;
                return i + 1;
            }
        }

        class Hash
        {
            // Таблица идентификаторов
            LinkedList[] table;

            // Размер хеш-таблицы
            uint size_table;

            // Текущее число занятых элементов таблицы
            int size_cur;

            // Индекс в таблице, определенный при помощи хэш-функции
            uint key;

            // Позиция элемента в списке
            int pos;

            // Инициализация таблицы для установки всех ее элементов
            // в состояние свободно
            void init()
            {
                int i;

                for (i = size_cur = 0; i < size_table; ++i)
                    table[i] = null;
            }

            // Вернуть хеш-число для символьного ключа
            uint DJBHash(string str)
            {
                uint hash = 5381;
                int i;

                for (i = 0; i < str.Length; ++i)
                {
                    hash = ((hash << 5) + hash) + str[i];
                }
                return hash;
            }

            // Конструктор таблицы по умолчанию
            public Hash()
            {
                table = new LinkedList[size_table = 101];
                init();
            }

            // Конструктор таблицы размером n
            public Hash(int n)
            {
                table = new LinkedList[size_table = (uint)n];
                init();
            }

            // Вставить в таблицу элемент
            // return: true  - элемент вставлен
            //         false - таблица уже содержит добавляемый элемент
            public bool insert_elem(string code, int attr)
            {
                // Такой элемент уже есть в таблице?
                if (search_elem(code))
                    return false;

                Elem data = new Elem();
                data.code = code;
                data.attr = attr;

                // По вычисленному индексу еще нет списка?
                if (table[key] == null)
                {
                    // Создаем список
                    table[key] = new LinkedList();
                }

                // Помещаем в вычисленный список элемент
                pos = table[key].AppendEnd(data);

                // Увеличим число элементов таблицы
                ++size_cur;

                return true;
            }

            // Поиск в таблице элемента  с параметром code
            // return: true, если элемент найден; false - не найден
            public bool search_elem(string code)
            {
                // Вычисляем значение хеш-функции по ключу элемента
                key = DJBHash(code);

                // Приводим это значение к величине, не превышающее
                // размер хеш-таблицы
                key %= size_table;

                // По вычисленному индексу еще нет списка
                if (table[key] == null)
                    return false;

                // Содержится ли элемент в списке?
                return (pos = table[key].Contains(code)) >= 0;
            }

            // Вернуть номер списка и позицию элемента в списке идентификатора или константы
            public int getind ()
            {
                return (int)(key << 16) | pos;
            }

            // Символьное представление идентификатора или константы
            public string getStr(int n)
            {
                return table[(uint)(n >> 16)].Elem(n &= 0xFFFF);
            }
        }

        public Lex lex;

        public class Lex
        {
            string str;
            string[] prog;

            // Текущее состояние автомата
            int Состояние;

            // Текст лексической ошибки
            string txt_error;

            // Индекс символа в исходной строке
            int ind;

            // Номер исходной строки
            int num;

            // Позиция идентификатора или числовой константы в соответствующей таблице
            int pos;

            // Двоичное значение числа
            int val = 0;

            // Текущий сканируемый символ исходной программы
            char c;

            // Признак чтения символа из файла
            bool yes_c;

            // Текущий идентификатор
            string ident;

            // Количество лексем в программе
            public int cnt_lex;

            // Индекс начала лексемы
            public int beg_ind;

            // Номер строки с лексемой
            public int beg_num;

            // Программа в виде лексем
            // [0] тип лексемы
            // [1] номер строки, где встретилась лексема
            // [2] позиция в строке с лексемой
            // [3] для идентификатра и числа индекс в соответствующей таблице
            public int[,] prog_lex = new int[4, MAX_LEX];


            Hash Table;

            public Lex(string[] p)
            {
                prog = p;
                ind = -1;
                num = 0;
                yes_c = false;
                c = Input();
                cnt_lex = 0;
                Состояние = НачалоЛексемы;
                yes_c = true;
                Table = new Hash();
            }

            // Символьное представление идентификатора или константы
            public string getStr (int n)
            {
                return Table.getStr (n);
            }

            // Чтение идентификатора
            public string getIdent()
            {
                return ident;
            }

            // Чтение числовой константы
            public int getNum()
            {
                return val;
            }

            // Чтение номера строки с лексической ошибкой
            public int getLineNum()
            {
                return beg_num;
            }

            // Чтение позиции в строке с лексической ошибкой
            public int getLinePos()
            {
                return beg_ind;
            }

            // Чтение позиции в таблице идентификатора или числовой константы
            public int getPos()
            {
                return pos;
            }

            // Чтение очередного символа программы
            // В случае конца программы возвращается 0
            // Для конца строки возвращается '\n'
            char Input()
            {
                // Символ уже прочитан при сканировании предыдущей лексемы?
                if (yes_c)
                {
                    if (c != 0)
                        yes_c = false;
                    return c;
                }

                // Надо читать очередную строку?
                if (ind == -1)
                {
                    // Конец исходной программы?
                    if (num == prog.Length)
                        return (char)0;

                    str = prog[num++];

                    // Устанавливаем индекс чтения очередного символа исходной строки
                    ind = 0;
                }

                // Прочитали конец строки?
                if (ind == str.Length)
                {
                    // Устанавливаем признак чтения очередной строки
                    ind = -1;

                    // Возвращаем признак конца строки
                    return '\n';
                }

                // Возвращаем очередной символ строки
                return str[ind++];
            }

            // Вернуть наименование лексической ошибки
            public string Error()
            {
                return txt_error;
            }

            // Прочитать очередную лексему
            // Если ошибок нет, тип лексемы содержится в type
            public int lex()
            {
                char c2;
                // Переход по состояниям автомата
                for (; ; )
                {
                    switch (Состояние)
                    {
                        case НачалоЛексемы:
                            // Игнорируем пустые символы, оставаясь в том же состоянии
                            while ((c = Input()) == ' ' || c == '\t' || c == '\n')
                                ;

                            // Координаты начала лексемы
                            beg_ind = ind;
                            beg_num = num;

                            // Встретили конец программы?
                            if (c == 0)
                            {
                                yes_c = true;
                                return EOF;
                            }

                            // Это начало числовой константы?
                            if (Char.IsDigit(c))
                            {
                                // Изменяем состояние, продолжаем разбор
                                Состояние = ПродолжениеЧисла;

                                // Устанавливаем значение старшей цифры числа
                                val = c - '0';
                                continue;
                            }

                            //if (c == '_' || c == 'Ц' || c == 'И' || c == 'К' || c == 'Л' || c == 'П' || c == 'О' || c == 'К' || c == 'А' || c == 'Н' || c == 'Е' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
                            if (c == '_' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
                            {
                                // Изменяем состояние, продолжаем разбор
                                Состояние = ПродолжениеИдентификатора;

                                // Устанавливаем значение старшего символа идентификатора
                                ident = c.ToString();
                                continue;
                            }

                            

                          
                            // Распознавание разделителей
                            Состояние = НачалоЛексемы;
                            switch (c)
                            {
                                case '(':
                                    yes_c = false;
                                    return LEFT;

                                case ')':
                                    yes_c = false;
                                    return RIGHT;

                                case ';':
                                    yes_c = false;
                                    return SEMICOLON;
                                

                                case ':': // =
                                    yes_c = false;
                                    c2 = Input();
                                    if(c2 == '=')
                                        return SET;
                                    return ERROR;
                                  

                                case '-':
                                    yes_c = false;
                                    return SUB;

                                case '/':
                                    yes_c = false;
                                    return DELE;

                                case '<':
                                    return LT;

                                case '>':
                                    return GT;
                            }

                            txt_error = "Неизвестный символ";
                            return ERROR;
                        


                       
                        case ПродолжениеИдентификатора:
                            // Остаемся в том же состоянии при сканировании допустимых символов идентификатора
                            // Продолжаем собирать идентификатор
                            //if ((c = Input()) == '_' || (c = Input()) == 'Ц' || (c = Input()) == 'И' || (c = Input()) == 'К' || (c = Input()) == 'Л' || (c = Input()) == 'П' || (c = Input()) == 'О' || (c = Input()) == 'К' || (c = Input()) == 'А' || (c = Input()) == 'Н' || (c = Input()) == 'Е' || Char.IsDigit(c) || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
                            if ((c = Input()) == '_' || Char.IsDigit(c) || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
                            {
                                ident += c.ToString();
                                continue;
                            }

                            // Идентификатор закончился, устанавливаем начальное состояние автомата
                            Состояние = НачалоЛексемы;

                            yes_c = true;

                            // Может это служебное слово?
                            if (ident == "CIKL")
                                return CIKL;

                            if (ident == "WHILE_NOT")
                                return WHILE_NOT;
                            if (ident == "if")
                                return IF;

                            if (ident == "else")
                                return ELSE;

                            // Нет, это обычный идентификатор, поместим его в таблицу
                            Table.insert_elem(ident, 0);
                            pos = Table.getind();
                            return IDENT;

                        case ПродолжениеЧисла:
                            // Остаемся в том же состоянии при сканировании цифр
                            // Продолжаем собирать число
                            if (Char.IsDigit(c = Input()))
                            {
                                val = val * 10 + c - '0';
                                continue;
                            }

                            // Число не может вплотную примыкать к идентификатору
                            //if (c == '_' || c == 'Ц' || c == 'И' || c == 'К' || c == 'Л' || c == 'П' || c == 'О' || c == 'К' || c == 'А' || c == 'Н' || c == 'Е' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
                            if (c == '_' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
                            {
                                txt_error = "Ошибочная запись числа";
                                return ERROR;
                            }
                            yes_c = true;

                            // Поместим числовую константу в таблицу
                            Table.insert_elem(val.ToString(), 1);
                            pos = Table.getind();

                            // Устанавливаем начальное состояние автомата
                            Состояние = НачалоЛексемы;

                            // Возвращаем лексему
                            return NUM;
                    }
                }
            }
        }

        // Лексический анализ
        private bool Лексика()
        {
            lex = new Lex(richTextBox1.Lines);
            int i, j;

            do
            {
                if ((i = lex.lex()) == ERROR)
                {
                    if ((i = lex.getLinePos()) < 0)
                        i = 1;


                    MessageBox.Show(lex.Error(), "Лексическая ошибка в строке " + lex.getLineNum().ToString() + " в позиции " + i.ToString());
                    return false;
                }

                if (lex.cnt_lex == MAX_LEX)
                {
                    MessageBox.Show("Слишком большая программа");
                    return false;
                }

                // Сохраним тип лексемы
                lex.prog_lex[0, lex.cnt_lex] = i;

                // Сохраним номер строки и позиции лексемы
                lex.prog_lex[1, lex.cnt_lex] = lex.beg_num - 1;
                lex.prog_lex[2, lex.cnt_lex] = lex.beg_ind;

                // Для идентификатора и числа - позиция в соответствующей таблице
                lex.prog_lex[3, lex.cnt_lex] = lex.getPos();

                // Увеличим размер программы
                ++lex.cnt_lex;
            } while (i != EOF);

            lex.prog_lex[1, lex.cnt_lex] = lex.prog_lex[1, lex.cnt_lex - 1];
            lex.prog_lex[2, lex.cnt_lex] = lex.prog_lex[2, lex.cnt_lex - 1];
            return true;
        }

        // Синтаксический анализ
        private void Синтаксис()
        {
            string str = "";
            int kluch = 0;
            int top = 0;
            int sdvig = 1;
            bool flag;
            int ptr, p, adr = 0;
            int i = 0, j, k = 0;

            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();

            if (!Лексика())
                return;

            Pam_top = 0;
            PrCode_top = 0;
            table_top = 0;

            Magazin[top] = MARKER_DNA;
            top++;
            Magazin[top] = SOV_OP;    //PROGRAMMA;
            while (Magazin[top] != DOPUST && Magazin[top] != ERR)
            {
                if (sdvig == 1)
                {
                    // Тип лексемы
                    i = lex.prog_lex[0, k];

                    // Символьное представление идентификатора или константы
                    str = lex.getStr (lex.prog_lex[3, k++]);
                }
                switch (Magazin[top])
                {                  
                    case PROGRAMMA://Верхний магазинный символ Программа(<S>)
                        switch (i)
                        {
                            case IDENT:
                                Magazin[top] = SOV_OP;
                                sdvig = 0;
                                break;

                            case IF: 
                                Magazin[top] = SOV_OP;
                                sdvig = 0;
                                break;
                            
                            case ELSE:
                                top--;
                                sdvig = 0;
                                break;
                            
                           /*
                            case ENDIF:
                                top--;
                                sdvig = 0;
                                break;
                            */

                            case CIKL:
                                Magazin[top] = SOV_OP;
                                sdvig = 0;
                                break;
                                
                            case WHILE_NOT:  //
                                top--;
                                sdvig = 0;
                                break;
                            
                            case EOF:
                                Magazin[top] = SOV_OP;
                                sdvig = 0;
                                break;
                            
                            
                            /*
                            case EOF:
                                top--;
                                sdvig = 0;
                                break;
                            */
                            
                            
                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case SOV_OP: //верхний магазинный символ Совокупность операторов
                        switch (i)
                        {
                            case IDENT:
                                Magazin[top] = SOV_OP;
                                top++;
                                Magazin[top] = OPER;
                                sdvig = 0;
                                break;

                            case IF:
                                Magazin[top] = SOV_OP;
                                top++;
                                Magazin[top] = OPER;
                                sdvig = 0;
                                break;

                            case CIKL: //
                                Magazin[top] = SOV_OP;
                                top++;
                                Magazin[top] = OPER;
                                sdvig = 0;
                                break;

                            case EOF:
                                top--;
                                sdvig = 0;
                                break;

                            case WHILE_NOT:
                                top--;
                                sdvig = 0;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case OPER: //Верхний магазинный символ Оператор
                        switch (i)
                        {
                            case IDENT:
                                {
                                    Magazin[top] = SEMICOLON;
                                    top++;
                                    Magazin[top] = -1;
                                    top++;
                                    Magazin[top] = -1;
                                    top++;
                                    Magazin[top] = PRISV;
                                    top++;
                                    Magazin[top] = top - 3;
                                    top++;
                                    Magazin[top] = E;
                                    top++;   
                                    Magazin[top] = SET;
                                    sdvig = 1;
                                    flag = false;
                                    j = 0;
                                    while (j < Pam_top && !flag)
                                    {
                                        if (Pam[j].Name == str)
                                        {
                                            flag = true;
                                            adr = j;
                                        }
                                        j++;
                                    }
                                    if (!flag)
                                    {
                                        Pam[Pam_top].Name = str;
                                        adr = Pam_top;
                                        Pam_top++;
                                    }
                                    Magazin[top - 4] = adr;
                                };
                                break;

                            case IF: // сделать с if-списком
                                //Magazin[top] = ENDIF; //
                                //top++;

                                Magazin[top] = table_top;
                                table_top++;
                                top++;

                                
                                Magazin[top] = METKA;
                                top++;
                                Magazin[top] = OPER;  // SOV_OP
                                top++;
                                Magazin[top] = table_top;
                                table_top++;
                                top++;
                                Magazin[top] = METKA;
                                top++;
                                Magazin[top] = table_top - 2;
                                top++;

                                Magazin[top] = BEZUSL_PER;
                                top++;
                                Magazin[top] = ELSE;
                                top++;
                                
                                

                                //Magazin[top] = IF_SPIS; //
                                //top++;

                                Magazin[top] = OPER;   // // SOV_OP
                                top++;
                                Magazin[top] = table_top - 1;
                                top++;
                                Magazin[top] = -1;
                                top++; 
                                Magazin[top] = PER_PO_0;
                                top++;
                                Magazin[top] = RIGHT;
                                top++;
                                Magazin[top] = top - 3;
                                top++;
                                Magazin[top] = LOG_VIR;
                                top++;
                                Magazin[top] = LEFT;
                                sdvig = 1;
                                break;

                            case CIKL:
                                {
                                    Magazin[top] = SEMICOLON;
                                    top++;
                                    Magazin[top] = table_top;
                                    top++;
                                    Magazin[top] = -1;
                                    top++;
                                    Magazin[top] = PER_PO_0;
                                    top++;
                                    Magazin[top] = top - 2;
                                    top++;
                                    Magazin[top] = LOG_VIR;
                                    top++;                            
                                    Magazin[top] = WHILE_NOT; //
                                    top++;
                                    Magazin[top] = table_top;
                                    table_top++;
                                    top++;
                                    Magazin[top] = METKA;
                                    top++;
                                    Magazin[top] = SOV_OP;  // SOV_OP
                                    top++;
                                    Magazin[top] = table_top;
                                    table_top++;
                                    top++;
                                    Magazin[top] = METKA;
                                    top++;
                                    Magazin[top] = table_top - 1;
                                    top++;
                                    Magazin[top] = BEZUSL_PER;
                                    //top++;
                                    //Magazin[top] = SOV_OP;  
                                    //top++;  
                                    sdvig = 1; 
                                }
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case E: //Верхний магазинный символ <E>
                        switch (i)
                        {
                            case IDENT:
                                Magazin[top] = -1;
                                top++;
                                Magazin[top] = E_SP;
                                top++;
                                Magazin[top] = top - 2;
                                top++;
                                Magazin[top] = T;
                                sdvig = 0;
                                break;

                            case NUM:
                                Magazin[top] = -1;
                                top++;
                                Magazin[top] = E_SP;
                                top++;
                                Magazin[top] = top - 2;
                                top++;
                                Magazin[top] = T;
                                sdvig = 0;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case E_SP: //Верхний магазинный символ <E-список>
                        switch (i)
                        {
                            case SEMICOLON:
                                top--;
                                Magazin[Magazin[top - 1]] = Magazin[top];     //
                                top -= 2;                                 //
                                sdvig = 0; 
                                break;

                            case SUB:
                                p = Magazin[top - 1];
                                Pam[Pam_top].Name = Pam[p].Name + " - ";
                                top--;
                                Magazin[top] = Pam_top;
                                top++;
                                Magazin[top] = E_SP;
                                top++;
                                Magazin[top] = Pam_top;
                                Pam_top++;
                                top++;
                                Magazin[top] = -1;
                                top++;
                                Magazin[top] = p;
                                top++;
                                Magazin[top] = ВЫЧЕСТЬ;
                                top++;
                                Magazin[top] = top - 3;
                                top++;
                                Magazin[top] = T;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case F: //Верхний магазинный символ <F>
                        switch (i)
                        {
                            case IDENT:
                                top--;
                                flag = false;
                                j = 0;
                                while (j < Pam_top && !flag)
                                {
                                    if (Pam[j].Name == str)
                                    {
                                        flag = true;
                                        adr = j;
                                    }
                                    j++;
                                }
                                if (!flag)
                                {
                                    Pam[Pam_top].Name = str;
                                    adr = Pam_top;
                                    Pam_top++;
                                }
                                Magazin[Magazin[top]] = adr;
                                top--;
                                sdvig = 1;
                                break;

                            case NUM:
                                top--;
                                flag = false;
                                j = 0;
                                while (j < Pam_top && !flag)
                                {
                                    if (Pam[j].Name == str)
                                    {
                                        flag = true;
                                        adr = j;
                                    }
                                    j++;
                                }
                                if (!flag)
                                {
                                    Pam[Pam_top].Name = str;
                                    Int32.TryParse(str, out Pam[Pam_top].value);
                                    adr = Pam_top;
                                    Pam_top++;
                                }
                                Magazin[Magazin[top]] = adr;
                                top--;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case T: //Верхний магазинный символ <T>
                        switch (i)
                        {
                            case IDENT:
                                Magazin[top] = -1;
                                top++;
                                Magazin[top] = T_SP;
                                top++;
                                Magazin[top] = top - 2;
                                top++;
                                Magazin[top] = F;
                                sdvig = 0;
                                break;

                            case NUM:
                                Magazin[top] = -1;
                                top++;
                                Magazin[top] = T_SP;
                                top++;
                                Magazin[top] = top - 2;
                                top++;
                                Magazin[top] = F;
                                sdvig = 0;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case T_SP: //Верхний магазинный символ <T-список>
                        switch (i)
                        {
                            case SEMICOLON:
                                top--;
                                Magazin[Magazin[top - 1]] = Magazin[top]; //
                                top -= 2;                                 //
                                sdvig = 0;
                                break;

                            case SUB:
                                top--;
                                Magazin[Magazin[top - 1]] = Magazin[top]; //
                                top -= 2;                                 //
                                sdvig = 0;
                                break;

                            case DELE:
                                p = Magazin[top - 1];
                                top--;
                                Pam[Pam_top].Name = Pam[p].Name + " / ";
                                Magazin[top] = Pam_top;
                                top++;
                                Magazin[top] = T_SP;
                                top++;
                                Magazin[top] = Pam_top;
                                Pam_top++;
                                top++;
                                Magazin[top] = -1;
                                top++;
                                Magazin[top] = p;
                                top++;
                                Magazin[top] = ДЕЛЕНИЕ;
                                top++;
                                Magazin[top] = top - 3;
                                top++;
                                Magazin[top] = F;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case LOG_VIR: //Верхний магазинный символ <логическое выражение>  //
                        switch (i)
                        {
                            case IDENT:
                                Magazin[top] = -1;
                                top++;

                                Magazin[top] = LOG_OP;
                                top++;

                                Magazin[top] = top - 2;
                                top++;
                                Magazin[top] = F;
                                sdvig = 0;
                                break;

                            case NUM:
                                Magazin[top] = -1;
                                top++;
                                Magazin[top] = LOG_OP;
                                top++;
                                Magazin[top] = top - 2;
                                top++;
                                Magazin[top] = F;
                                sdvig = 0;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case LOG_OP: //Верхний магазинный символ <логический оператор>
                        switch (i)
                        {
                            case GT:
                                top--;
                                p = Magazin[top];
                                Pam[Pam_top].Name = Pam[p].Name + ("> ");
                                Magazin[top] = Pam_top;
                                Pam_top++;
                                top++;
                                Magazin[top] = -1;
                                top++;
                                Magazin[top] = p;
                                top++;
                                Magazin[top] = OMORE;
                                top++;
                                Magazin[top] = top - 3;
                                top++;
                                Magazin[top] = F;
                                sdvig = 1;
                                break;

                            case LT:
                                top--;
                                p = Magazin[top];
                                Pam[Pam_top].Name = Pam[p].Name + ("< ");
                                Magazin[top] = Pam_top;
                                Pam_top++;
                                top++;
                                Magazin[top] = -1;
                                top++;
                                Magazin[top] = p;
                                top++;
                                Magazin[top] = OLOW;
                                top++;
                                Magazin[top] = top - 3;
                                top++;
                                Magazin[top] = F;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case IDENT:
                        switch (i)
                        {
                            case IDENT:
                                {
                                    flag = false;
                                    j = 0;
                                    while (j < Pam_top && !flag)
                                    {
                                        if (Pam[j].Name == str)
                                        {
                                            flag = true;
                                            adr = j;
                                        }
                                        j++;
                                    }
                                    if (!flag)
                                    {
                                        Pam[Pam_top].Name = str;
                                        adr = Pam_top;
                                        Pam_top++;
                                    }
                                    ptr = Magazin[top - 1];
                                    while (ptr != -1)
                                    {
                                        p = Magazin[ptr];
                                        Magazin[ptr] = adr;
                                        ptr = p;
                                    }
                                    top -= 2;
                                    sdvig = 1;
                                }
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case NUM:
                        switch (i)
                        {
                            case NUM:
                                flag = false;
                                j = 0;
                                while (j < Pam_top && !flag)
                                {
                                    if (Pam[j].Name == str)
                                    {
                                        flag = true;
                                        adr = j;
                                    }
                                    j++;
                                }
                                if (!flag)
                                {
                                    Pam[Pam_top].Name = str;
                                    Int32.TryParse(str, out Pam[Pam_top].value);
                                    adr = Pam_top;
                                    Pam_top++;
                                }
                                ptr = Magazin[top - 1];
                                while (ptr != -1)
                                {
                                    p = Magazin[ptr];
                                    Magazin[ptr] = adr;
                                    ptr = p;
                                }
                                top -= 2;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case SET:
                        switch (i)
                        {
                            case SET:
                                top--;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;
                    /*
                    case COMMA: // to  - здесь не нужно
                        switch (i)
                        {
                            case COMMA:
                                top--;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;
                    */

                    case SEMICOLON:
                        switch (i)
                        {
                            case SEMICOLON:
                                top--;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case ELSE:
                        switch (i)
                        {
                            case ELSE:
                                top--;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;
                    


                    case WHILE_NOT:
                        switch (i)
                        {
                            case WHILE_NOT:
                                top--;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case LEFT:
                        switch (i)
                        {
                            case LEFT:
                                top--;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;
                    /*
                    case DO:
                        switch (i)
                        {
                            case DO:
                                top--;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;
                    */

                    case RIGHT:
                        switch (i)
                        {
                            case RIGHT:
                                top--;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case SUB:
                        switch (i)
                        {
                            case SUB:
                                top--;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case DELE:
                        switch (i)
                        {
                            case DELE:
                                top--;
                                sdvig = 1;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case MARKER_DNA:
                        switch (i)
                        {
                            case EOF:
                                Magazin[top] = DOPUST;
                                break;

                            default:
                                Magazin[top] = ERR;
                                break;
                        }
                        break;

                    case PRISV:
                        PrCode[PrCode_top, 0] = Magazin[top];
                        PrCode[PrCode_top, 1] = Magazin[top - 1];
                        PrCode[PrCode_top, 2] = Magazin[top - 2];
                        sdvig = 0;
                        PrCode_top++;
                        dataGridView2.Rows.Add(kluch, "Присвоить ( " + Magazin[top - 1].ToString() + " , " + Magazin[top - 2].ToString() + " )");
                        kluch++;
                        top -= 3;

                        break;

                    case PER_PO_0:
                        PrCode[PrCode_top, 0] = Magazin[top];
                        PrCode[PrCode_top, 1] = Magazin[top - 1];
                        PrCode[PrCode_top, 2] = Magazin[top - 2];
                        dataGridView2.Rows.Add(kluch, "Переход_по_0 ( " + Magazin[top - 1].ToString() + " , " + Magazin[top - 2].ToString() + " )");
                        kluch++;
                        top -= 3;
                        sdvig = 0;
                        PrCode_top++;
                        break; 

                    case BEZUSL_PER:
                        PrCode[PrCode_top, 0] = Magazin[top];
                        PrCode[PrCode_top, 1] = Magazin[top - 1];
                        dataGridView2.Rows.Add(kluch, "Безусловный_переход ( " + Magazin[top - 1].ToString() + " )");
                        kluch++;
                        top -= 2;
                        sdvig = 0;
                        PrCode_top++;
                        break;

                    case O_PER_PO_SR:
                        PrCode[PrCode_top, 0] = Magazin[top];
                        PrCode[PrCode_top, 1] = Magazin[top - 1];
                        PrCode[PrCode_top, 2] = Magazin[top - 2];
                        PrCode[PrCode_top, 3] = Magazin[top - 3];
                        dataGridView2.Rows.Add(kluch, "Пер._по_сравн. ( " + Magazin[top - 1].ToString() + " , " + Magazin[top - 2].ToString() + " , " + Magazin[top - 3].ToString() + " )");
                        kluch++;
                        top -= 4;
                        sdvig = 0;
                        PrCode_top++;
                        break;

                    case O_UVEL_NA_1:
                        PrCode[PrCode_top, 0] = Magazin[top];
                        PrCode[PrCode_top, 1] = Magazin[top - 1];
                        dataGridView2.Rows.Add(kluch, "Увеличить_на_1 ( " + Magazin[top - 1].ToString() + " )");
                        kluch++;
                        PrCode_top++;
                        top -= 2;
                        sdvig = 0;
                        break;

                    case METKA:
                        PrCode[PrCode_top, 0] = Magazin[top];
                        PrCode[PrCode_top, 1] = Magazin[top - 1];
                        dataGridView2.Rows.Add(kluch, "Метка ( " + Magazin[top - 1].ToString() + " )");
                        kluch++;
                        top -= 2;
                        sdvig = 0;
                        PrCode_top++;
                        break;

                    case ВЫЧЕСТЬ:
                        Pam[Magazin[top - 3]].Name = Pam[Magazin[top - 1]].Name + " - " + Pam[Magazin[top - 2]].Name;
                        PrCode[PrCode_top, 0] = Magazin[top];
                        PrCode[PrCode_top, 1] = Magazin[top - 1];
                        PrCode[PrCode_top, 2] = Magazin[top - 2];
                        PrCode[PrCode_top, 3] = Magazin[top - 3];
                        dataGridView2.Rows.Add(kluch, "Вычесть ( " + Magazin[top - 1].ToString() + " , " + Magazin[top - 2].ToString() + " , " + Magazin[top - 3].ToString() + " )");
                        kluch++;
                        top -= 4;
                        sdvig = 0;
                        PrCode_top++;
                        break;

                    case ДЕЛЕНИЕ:
                        Pam[Magazin[top - 3]].Name = Pam[Magazin[top - 1]].Name + " / " + Pam[Magazin[top - 2]].Name;
                        PrCode[PrCode_top, 0] = Magazin[top];
                        PrCode[PrCode_top, 1] = Magazin[top - 1];
                        PrCode[PrCode_top, 2] = Magazin[top - 2];
                        PrCode[PrCode_top, 3] = Magazin[top - 3];
                        dataGridView2.Rows.Add(kluch, "Деление ( " + Magazin[top - 1].ToString() + " , " + Magazin[top - 2].ToString() + " , " + Magazin[top - 3].ToString() + " )");
                        kluch++;
                        top -= 4;
                        sdvig = 0;
                        PrCode_top++;
                        break;


                    case OMORE:
                        Pam[Magazin[top - 3]].Name = Pam[Magazin[top - 1]].Name + "> " + Pam[Magazin[top - 2]].Name;
                        PrCode[PrCode_top, 0] = Magazin[top];
                        PrCode[PrCode_top, 1] = Magazin[top - 1];
                        PrCode[PrCode_top, 2] = Magazin[top - 2];
                        PrCode[PrCode_top, 3] = Magazin[top - 3];
                        dataGridView2.Rows.Add(kluch, "Больше? ( " + Magazin[top - 1].ToString() + " , " + Magazin[top - 2].ToString() + " , " + Magazin[top - 3].ToString() + " )");
                        kluch++;
                        Magazin[Magazin[top - 4]] = Magazin[top - 3];
                        top -= 5;
                        sdvig = 0;
                        PrCode_top++;
                        break;

                    case OLOW:
                        Pam[Magazin[top - 3]].Name = Pam[Magazin[top - 1]].Name + "< " + Pam[Magazin[top - 2]].Name;
                        PrCode[PrCode_top, 0] = Magazin[top];
                        PrCode[PrCode_top, 1] = Magazin[top - 1];
                        PrCode[PrCode_top, 2] = Magazin[top - 2];
                        PrCode[PrCode_top, 3] = Magazin[top - 3];
                        dataGridView2.Rows.Add(kluch, "Меньше? ( " + Magazin[top - 1].ToString() + " , " + Magazin[top - 2].ToString() + " , " + Magazin[top - 3].ToString() + " )");
                        kluch++;
                        Magazin[Magazin[top - 4]] = Magazin[top - 3];
                        top -= 5;
                        sdvig = 0;
                        PrCode_top++;
                        break;
                }
            }
            if (Magazin[top] == ERR)
            {
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();

                --k;

                if ((i = lex.prog_lex[2, k]) < 0)
                    i = 1;

                /*
                // Получить позицию ошибки
                j = richTextBox1.GetFirstCharIndexFromLine(lex.prog_lex[1, k]) + i - 1;

                // Выделить один символ
                richTextBox1.Select(j, 1);

                // Окрасить его
                richTextBox1.SelectionBackColor = Color.Red;
                richTextBox1.SelectionColor = Color.White;

                // Убрать выделение
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                */

                MessageBox.Show(lex.Error(), "Синтаксическая ошибка в строке " + (lex.prog_lex[1, k] + 1).ToString() + " в позиции " + i.ToString());
                return;
            }

            j = 0;
            while (j < PrCode_top)
            {
                switch (PrCode[j, 0])
                {
                    case PRISV:
                        Pam[PrCode[j, 1]].value = Pam[PrCode[j, 2]].value;
                        j++;
                        break;

                    case PER_PO_0:
                        {
                            int p1 = j + 1;
                            bool flag1 = false;
                            if (Pam[PrCode[j, 1]].value == 0)
                            {
                                while (!flag1)
                                {
                                    if (PrCode[p1, 0] == METKA && PrCode[p1, 1] == PrCode[j, 2])
                                        flag1 = true;
                                    else
                                        p1++;
                                }
                                j = p1 + 1;
                            }
                            else
                                j = p1;
                        }
                        break;

                    case BEZUSL_PER:
                        {
                            int p1 = 0;
                            bool flag1 = false;
                            while (!flag1)
                            {
                                if (PrCode[p1, 0] == METKA && PrCode[p1, 1] == PrCode[j, 1])
                                    flag1 = true;
                                else
                                    p1++;

                            }
                            j = p1 + 1;
                        }
                        break;

                    case METKA:
                        j++;
                        break;

                    case ВЫЧЕСТЬ:
                        Pam[PrCode[j, 3]].value = Pam[PrCode[j, 1]].value - Pam[PrCode[j, 2]].value;
                        j++;
                        break;

                    case ДЕЛЕНИЕ:
                        Pam[PrCode[j, 3]].value = Pam[PrCode[j, 1]].value / Pam[PrCode[j, 2]].value;
                        j++;
                        break;

                    case OMORE:
                        {
                            if (Pam[PrCode[j, 1]].value > Pam[PrCode[j, 2]].value)
                                Pam[PrCode[j, 3]].value = 1;
                            else
                                Pam[PrCode[j, 3]].value = 0;
                            j++;
                        }
                        break;

                    case OLOW:
                        {
                            if (Pam[PrCode[j, 1]].value < Pam[PrCode[j, 2]].value)
                                Pam[PrCode[j, 3]].value = 1;
                            else
                                Pam[PrCode[j, 3]].value = 0;
                            j++;
                        }
                        break;

                    case O_PER_PO_SR:
                        {
                            int p1 = j + 1;
                            bool flag1 = false;
                            if (Pam[PrCode[j, 1]].value >= Pam[PrCode[j, 2]].value)
                            {
                                while (!flag1)
                                {
                                    if (PrCode[p1, 0] == METKA && PrCode[p1, 1] == PrCode[j, 3])
                                        flag1 = true;
                                    else
                                        p1++;
                                }
                                j = p1 + 1;
                            }
                            else
                                j = p1;
                        }
                        break;

                    case O_UVEL_NA_1:
                        Pam[PrCode[j, 1]].value++;
                        j++;
                        break;
                }
            }
            for (int ii = 0; ii < Pam_top; ii++)
            {
                dataGridView1.Rows.Add(ii, Pam[ii].Name, Pam[ii].value);
            }
        }

        // Выполнение синтаксического анализа
        private void button1_Click(object sender, EventArgs e)
        {
            Синтаксис();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var column1 = new DataGridViewColumn();
            column1.HeaderText = "№";                             // Текст в шапке
            column1.Width = 30;                                   // Ширина колонки
            column1.ReadOnly = true;                              // Значение в этой колонке нельзя править
            column1.Frozen = true;                                // Флаг, что данная колонка всегда отображается на своем месте
            column1.CellTemplate = new DataGridViewTextBoxCell(); // Тип колонки

            var column2 = new DataGridViewColumn();
            column2.HeaderText = "Имя";
            column2.Width = 100;
            column2.ReadOnly = true;
            column2.Frozen = true;
            column2.CellTemplate = new DataGridViewTextBoxCell();

            var column3 = new DataGridViewColumn();
            column3.HeaderText = "Значение";
            column3.Width = 180;
            column3.ReadOnly = true;
            column3.Frozen = true;
            column3.CellTemplate = new DataGridViewTextBoxCell();

            dataGridView1.Columns.Add(column1);
            dataGridView1.Columns.Add(column2);
            dataGridView1.Columns.Add(column3);

            dataGridView1.AllowUserToAddRows = false;            // Запрещаем пользователю самому добавлять строки
            dataGridView1.RowHeadersVisible = false;             // Убираем крайний левый столбец

            ///////////////////////////////////////
            var column4 = new DataGridViewColumn();
            column4.HeaderText = "№";
            column4.Width = 30;
            column4.ReadOnly = true;
            column4.Frozen = true;
            column4.CellTemplate = new DataGridViewTextBoxCell();

            var column5 = new DataGridViewColumn();
            column5.HeaderText = "Команда";
            column5.Width = 250;
            column5.ReadOnly = true;
            column5.Frozen = true;
            column5.CellTemplate = new DataGridViewTextBoxCell();

            dataGridView2.Columns.Add(column4);
            dataGridView2.Columns.Add(column5);

            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.RowHeadersVisible = false;
        }

        // Сброс ошибки
        /*
        private void button2_Click(object sender, EventArgs e)
        {
            
            // Выделить весь текст
            richTextBox1.SelectAll();

            // Окрасить его в стандартный цвет
            richTextBox1.SelectionBackColor = Color.White;
            richTextBox1.SelectionColor = Color.Black;

            // Убрать выделение
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            
        }
        */

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            string fileText = System.IO.File.ReadAllText(filename);
            richTextBox1.Text = fileText;
            //MessageBox.Show("Файл открыт");
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
