using System;
using System.IO;
using System.Collections.Generic;

namespace AlgorithmForRemovingUnreachableStates
{
    class NandE
    {
        private HashSet<char> n;
        private HashSet<char> e;
        public HashSet<char> N
        {
            get { return n; }
            set { n = value; }
        }
        public HashSet<char> E
        {
            get { return e; }
            set { e = value; }
        }
        public NandE()
        {
            n = new HashSet<char>();
            e = new HashSet<char>();
        }
        public NandE(List<string> listOfOfRightChars)
        {
            n = new HashSet<char>();
            e = new HashSet<char>();
            foreach (String c in listOfOfRightChars)
            {
                if (c != "\\e")
                {
                    if (IsN(c[0]))
                    {
                        n.Add(c[0]);
                    }
                    else if (IsE(c[0]))
                    {
                        e.Add(c[0]);
                    }
                }
            }
        }
        public string GetCountInfo()
        {
            return $"Count of N: {n.Count}; Count of E: {e.Count}";
        }
        public void PrintCountInfo()
        {
            Console.WriteLine(GetCountInfo());
        }
        //Является ли символ нетерминалом
        private bool IsN(char c) { return Char.IsUpper(c); }

        //Является ли символ терминалом
        private bool IsE(char c) { return Char.IsLower(c) || Char.IsDigit(c); }
    }
    class Product
    {
        private char left;
        private string right;
        private int countOfRightChars;
        public NandE RightChars;
        public char Left
        {
            get { return left; }
            set { left = value;}
        }
        public string Right
        {
            get { return right; }
            set { right = value; }
        }
        public int CountOfRightChars
        {
            get { return countOfRightChars; }
            set { countOfRightChars = value; }
        }
        public Product() : this("Empty->Empty")
        {
        }
        public Product(string pString)
        {
            String[] str = pString.Split("->");
            if (str[0].Length > 1 && Char.IsLower(str[0][0]))
                throw new Exception("Слева должен быть 1 нетерминал");
            this.left = str[0][0];
            this.right = str[1];
            CountOfRightChars = CalculateCountOfRightChars();

            List<string> lst = CreateListOfRightChars();
            RightChars = new NandE(lst);
        }
        public Product(string xLeft, string xRight)
        {
            if (xLeft.Length > 1 && Char.IsLower(xLeft[0]))
                throw new Exception("Слева должен быть 1 нетерминал");
            this.left = xLeft[0];
            this.right = xRight;
            CountOfRightChars = CalculateCountOfRightChars();
        }
        public string GetFullString()
        {
            return $"{left}->{right}";
        }
        public void Print()
        {
            Console.WriteLine(GetFullString());
        }
        private List<String> CreateListOfRightChars()
        {
            List<String> lst = new List<string>();
            bool f = true; //флаг для \e
            for(int i = 0 ; i < right.Length; i++)
            {
                f = true;
                if (right[i] != '|') //если прочли символ
                {
                    if (right[i] == '\\') //Встретили ли мы начало \e?
                    {
                        if ((i + 1) < right.Length && right[i + 1] == 'e')
                        {      //встретили \e
                            lst.Add(right[i].ToString() + right[i+1].ToString());  //Добавили символ \e
                            f = false; //т.к. добавили символ \e
                            i++; //Чтобы пропустить \e
                        }
                    }
                    if (f)//Если не добавили \e
                    {
                        lst.Add(right[i].ToString());  //Добавили символ
                    }
                }
            }
            return lst;
        }
        private int CalculateCountOfRightChars()
        {
            int k = 0;
            for (int i = 0; i < right.Length; i++)
            {
                if (right[i] != '|') //если прочли символ
                {
                    if (right[i] == '\\') //Встретили ли мы начало \e?
                    {
                        if ( (i+1)< right.Length && right[i + 1] == 'e')
                        {      //встретили \e
                            i++; //Чтобы пропустить \e
                        }
                    }
                    k++;//Прочли символ
                }
            }
            return k;
        }
    }
    class Program
    {
        //Чтение строки из файла
        static string MyReadFile(ref string inPath)
        {
            string sourceString = "";
            try
            {
                using (var f = new StreamReader(inPath))
                {
                    sourceString = f.ReadLine();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            return sourceString;
        }
        //Визуальная запись в файл
        static void MyVisualWriteFile(ref string outComfortablePath, ref Product[] p, ref HashSet<Char> n, bool myAppend)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(outComfortablePath, myAppend, System.Text.Encoding.Default))
                {
                    sw.WriteLine("Дано:");
                    sw.WriteLine();
                    for (int i = 0; i < p.Length; i++)
                    {
                        sw.WriteLine(p[i].GetFullString());
                    }
                    sw.WriteLine("-----------------------");
                    sw.WriteLine("После алгоритма удаления недостижимых символов:");
                    sw.WriteLine();
                    for (int i = 0; i < p.Length; i++)
                    {
                        if (n.Contains(p[i].Left))
                        {
                            sw.WriteLine(p[i].GetFullString());
                        }
                    }
                    sw.WriteLine("*****************************");
                    sw.WriteLine("*****************************");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {
            string inPath = @"in.txt";
            string outPath = @"out.txt";
            string outComfortablePath = @"outComfortable.txt";
            string outExamplePath = @"outComfortableExamples.txt";


            string sourceString = MyReadFile(ref inPath); //Прочли строку

            String[] pStrings = sourceString.Split(new char[] { ' ', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            Product[] p = new Product[pStrings.Length];
            for(int i = 0; i < pStrings.Length; i++)
            {
                p[i] = new Product(pStrings[i]);
            }
            int n_old_count = 0; // Для 3 шага
            HashSet<Char> n_old = new HashSet<Char>(); //Нетерминалы для того, чтобы лишний раз по ним не проходиться
            HashSet<Char> n = new HashSet<Char>(); //Нетерминалы будущие

            //Шаг 1
            //Y0
            if (p.Length > 0)
            {
                n.Add(p[0].Left);
                n_old_count = 1;
            }

            //Шаг 2
            //Y1...Yn
            while (true)
            {
                //Пробегаем по всем левым частям
                for (int i = 0; i < p.Length; i++)
                {
                    //Пройти строку, чтобы узнать новые N
                    if (n.Contains(p[i].Left) && !n_old.Contains(p[i].Left))
                    {
                        //Добавляем правую часть из тех, которые уже есть в нашем новом мн-ве N
                        foreach (char c in p[i].RightChars.N)
                        {
                            n.Add(c);
                        }
                        n_old.Add(p[i].Left);
                        i = 0;//После добавления идем назад по к началу списка
                    }
                }

                //Шаг 3
                if (n.Count == n_old_count)
                {
                    break;
                }
                else
                {
                    n_old_count = n.Count;
                }
            }

            //Шаг 4

            //Запись в файл
            try
            {
                using (StreamWriter sw = new StreamWriter(outPath, false, System.Text.Encoding.Default))
                {
                    for (int i = 0; i < p.Length; i++)
                    {
                        if (n.Contains(p[i].Left))
                        {
                            sw.Write(p[i].GetFullString() + " ");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //----------------------------------------------

            //Визуальная запись в файл до/после
            MyVisualWriteFile(ref outComfortablePath, ref p, ref n, false);

            //----------------------------------------------
            //Запись примеров в файл до/после без перезаписывания

            MyVisualWriteFile(ref outExamplePath, ref p, ref n, true);
        }
    }
}
