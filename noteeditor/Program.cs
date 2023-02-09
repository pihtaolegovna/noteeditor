using System;
using System.IO;
using System.Linq;

namespace noteeditor
{
    internal class Program
    {
        public static int init = 6;

        public static void Main(string[] args) // чтение пути, обращение к Read
        {
            Console.WriteLine("Введите путь до файла (с названием), который вы хотите открыть.\r\n---------------------------------------------------------------");

            string path = Console.ReadLine();

            if (!File.Exists(path))
            {
                Console.WriteLine("Файл не существует! Проверьте правильность введённого пути.");
                return;
            }

            Console.WriteLine("Введите кол-во записей в вашем дневнике (по умолчанию шесть): ");


            string init1 = Console.ReadLine();

            if (!string.IsNullOrEmpty(init1))
            {
                init = Convert.ToInt32(init1);
            }

            ReadnSave.Read(path);
        }
    }
}