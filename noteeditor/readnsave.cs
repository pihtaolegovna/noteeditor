using Newtonsoft.Json;
using noteeditor;
using System.Collections.Generic;
using System.IO;
using System;
using System.Xml.Serialization;

namespace ConsoleApp3
{
    internal class ReadnSave
    {
        public static void Read(string path) // чтение файлов, десериализация, вывод на консоль
        {
            Console.Clear();
            Console.WriteLine("Сохранить файл в одном из трёх форматов (txt, json, xml) - F1. Закрыть программу - Escape\r\n-----------------------------------------------------------------------------------------");

            List<Diary> result = new(); // создание коллекции моделей

            if (path.Contains(".txt"))
            {
                string[] txt = File.ReadAllLines(path);

                Diary newDiary = new(); // создание модели

                string authorMarkerWord = "Автор: ";

                for (int i = 0; i < txt.Length; i++)
                {
                    if (txt[i].Contains(authorMarkerWord))
                    {
                        newDiary.author = txt[i].Replace(authorMarkerWord, "") + "\r\n";
                    }
                }

                for (int j = 0; j < txt.Length / 3; j++)
                {
                    txt[2 + (j * 3)] += "\r\n";
                    newDiary.date[j] = txt[2 + (j * 3)];
                }

                for (int k = 0; k < txt.Length / 3; k++)
                {
                    txt[3 + (k * 3)] += "\r\n";
                    newDiary.content[k] = txt[3 + (k * 3)];
                }

                result.Add(newDiary); // добавление модели в коллекцию

                foreach (Diary diary in result)
                {
                    Console.WriteLine("->Автор: " + diary.author);

                    for (int j = 0; j < diary.date.Length; j++)
                    {
                        Console.WriteLine("  " + diary.date[j] + "  " + diary.content[j]);
                    }

                    Console.SetCursorPosition(2, 2);
                }

            }

            else if ((path.Contains(".xml")) || (path.Contains(".json")))
            {
                if (path.Contains(".xml"))
                {
                    XmlSerializer xml = new(typeof(List<Diary>));

                    using FileStream fs = new(path, FileMode.Open);

                    result = (List<Diary>)xml.Deserialize(fs);
                }

                else if (path.Contains(".json"))
                {
                    string json = File.ReadAllText(path);

                    result = JsonConvert.DeserializeObject<List<Diary>>(json);
                }

                foreach (Diary diary in result)
                {
                    Console.WriteLine("->Автор: " + diary.author + "\r\n");

                    for (int j = 0; j < diary.date.Length; j++)
                    {
                        Console.WriteLine(diary.date[j] + diary.content[j]);
                    }

                    Console.SetCursorPosition(0, 2);
                }
            }

            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.F1)
                {
                    Save(path, result);
                }

                else if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }

                else if ((key.Key == ConsoleKey.UpArrow) || (key.Key == ConsoleKey.DownArrow) || (key.Key == ConsoleKey.Enter))
                {
                    result = CursorMove(key, result);
                }

            } while (true);
        }

        static void Save(string path, List<Diary> result) // сохранение файлов, сериализация
        {
            Console.Clear();
            Console.WriteLine("Сохранить файл в одном из трёх форматов (txt, json, xml) - F1. Закрыть программу - Escape\r\n-----------------------------------------------------------------------------------------\r\n");

            path = StringEdit(path, "", 2).Replace("\r\n", "");
            Console.SetCursorPosition(0, 3);

            if (File.Exists(path))
            {
                Console.WriteLine("Файл с таким названием уже существует. Программа перезапишет его. Нажмите любую клавишу, чтобы продолжить.");
                ConsoleKeyInfo key = Console.ReadKey(true);
            }

            if (path.Contains(".txt"))
            {
                string txt = "";

                foreach (Diary diary in result)
                {
                    txt += "Автор: " + diary.author + "\r\n";

                    for (int i = 0; i < diary.date.Length; i++)
                    {
                        txt += diary.date[i] + diary.content[i] + "\r\n";
                    }
                }

                File.WriteAllText(path, txt);
            }

            else if (path.Contains(".xml"))
            {
                XmlSerializer xml = new(typeof(List<Diary>));
                using FileStream fs = new(path, FileMode.Create);
                xml.Serialize(fs, result);
            }

            else if (path.Contains(".json"))
            {
                string json = JsonConvert.SerializeObject(result);
                File.WriteAllText(path, json);
            }

            Console.WriteLine("Успешно!");
        }

        static int lineIndex = 0;

        static List<Diary> CursorMove(ConsoleKeyInfo key, List<Diary> result) // стрелочки для текста
        {
            int[] tops = new int[1 + Program.init * 2];
            tops[0] = 2;
            int[] tops2 = new int[1 + Program.init * 2];
            tops2[0] = 2;

            int[] contentLenght = new int[Program.init + 1];

            for (int i = 0; i < contentLenght.Length - 1; i++)
            {
                contentLenght[i + 1] = result[0].content[i].Length / 120;
            }

            for (int i = 0; i < contentLenght.Length - 1; i++)
            {
                contentLenght[i + 1] += contentLenght[i];
            }

            for (int i = 0; i < Program.init; i++)
            {
                tops[1 + i * 2] = 4 + i * 3 + contentLenght[i];
                tops[2 + i * 2] = 5 + i * 3 + contentLenght[i];
            }

            for (int i = 0; i < Program.init; i++)
            {
                tops2[i + 1] = 4 + i * 3 + contentLenght[i];
                tops2[i + 7] = 5 + i * 3 + contentLenght[i];
            }

            if (key.Key == ConsoleKey.UpArrow)
            {
                if (!(lineIndex == 0))
                {
                    lineIndex--;
                }

                Console.SetCursorPosition(0, tops[lineIndex + 1]);
                Console.Write("  ");
            }

            else if (key.Key == ConsoleKey.DownArrow)
            {
                if (!(lineIndex == tops.Length - 1))
                {
                    lineIndex++;
                }

                Console.SetCursorPosition(0, tops[lineIndex - 1]);
                Console.Write("  ");
            }

            else if (key.Key == ConsoleKey.Enter)
            {
                LineFindToStringEdit(result, tops, lineIndex);
            }

            Console.SetCursorPosition(0, tops[lineIndex]);
            Console.Write("->");

            return result;
        }

        static void LineFindToStringEdit(List<Diary> result, int[] tops, int lineIndex)
        {
            int[] ints = new int[tops.Length];

            for (int i = 0; i <= tops.Length - 1; i++)
            {
                ints[i] = i;
            }

            string additionalString = "  ";

            if (lineIndex == 0)
            {
                additionalString = "  Автор: ";

                result[0].author = StringEdit(result[0].author, additionalString, tops[0]);
            }

            else if (lineIndex % 2 != 0)
            {
                result[0].date[lineIndex / 2] = StringEdit(result[0].date[lineIndex / 2], additionalString, tops[lineIndex]);
            }

            else if (lineIndex % 2 == 0)
            {
                result[0].content[lineIndex / 2 - 1] = StringEdit(result[0].content[lineIndex / 2 - 1], additionalString, tops[lineIndex]);
            }
        }

        static string StringEdit(string @string, string additionalString, int cursorTop)
        {
            ConsoleKeyInfo key;

            do
            {
                Console.SetCursorPosition(additionalString.Length, cursorTop);

                for (int i = 0; i < @string.Length + 1; i++)
                {
                    Console.Write(" ");
                }

                Console.SetCursorPosition(0, cursorTop);

                Console.Write(additionalString + @string);


                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace)
                {
                    @string = @string.Remove(@string.Length - 1, 1);
                }

                else if (key.Key == ConsoleKey.Enter)
                {

                }

                else
                {
                    @string += key.KeyChar;
                }

            } while (key.Key != ConsoleKey.Enter);

            return @string + "\r\n";
        }

    }
}