// HomeWork template 1.4 // date: 17.10.2023

using Service;
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

/// QUESTIONS ///
/// 1. 

// HomeWork 11 : [Text statistic] --------------------------------

namespace IDA_C_sh_HomeWork
{
    internal class Program
    {
        static void Main(string[] args)
        {

            MainMenu.MainMenu mainMenu = new MainMenu.MainMenu();

            do
            {
                Console.Clear();
                mainMenu.Show_menu();
                if (mainMenu.User_Choice_Handle() == 0) break;
                Console.ReadKey();
            } while (true);
            // Console.ReadKey();
        }

        public static void Task_1(string work_name)
        /* Задание 
        Подсчитать сколько раз слово встречается в заданном тексте.
        Вывести статистику по тексту в виде json и xml файлов
        Текст:
        Вот дом, Который построил Джек. А это пшеница, Которая в темном чулане хранится В доме, Который построил
        Джек. А это веселая птица-синица, Которая часто ворует пшеницу, Которая в тёмном чулане хранится В доме,
        Который построил Джек.*/
        { Console.WriteLine("\n***\t{0}\n\n", work_name);

            string original_text = "Вот дом, Который построил Джек.\n" +
                    "А это пшеница, которая в темном чулане хранится в доме, который построил Джек.\n" +
                    "А это веселая птица-синица, которая часто ворует пшеницу, которая в тёмном чулане хранится в доме, который построил Джек.";
            Console.WriteLine("Original text:\n" + original_text);

            char[] splitters = new char[] { ' ', '\n', ',', '.' };


            //string[] words_array = original_text.Split(' ', StringSplitOptions.TrimEntries);

            // Получим массив уникальных слов встречающихся в тексте
            var words_array_distinct = original_text.ToLower().Split(splitters, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            // Получим массив всех слов встречающихся в тексте
            var words_array_full = original_text.ToLower().Split(splitters, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToArray();
            // Будем хранить используемые слова и кол-во их повторений в SortedList
            SortedList<string, int> text_statistics = new SortedList<string, int>();

            /*   var words_array = original_text
                                       .Split(' ')
                                       .Distinct()
                                       .OrderBy(x => x)
                                       .ToArray();*/

            /*var words_array = original_text
                                .Split(splitters)
                                .Distinct()
                                .OrderBy(x => x)
                                .ToArray();*/


            /*     foreach (var word in words_array)
              {
                  *//*Regex regex_1 = new Regex(word.Trim(splitters), RegexOptions.IgnoreCase);
                  MatchCollection matches = regex_1.Matches(original_text);
                  text_statistics.Add(word.Trim(splitters), matches.Count);*/

            /*     Regex regex_1 = new Regex(word, RegexOptions.IgnoreCase);
                 MatchCollection matches = regex_1.Matches(original_text);
                 text_statistics.Add(word, matches.Count);*//*
             }*/

            // Долго и упорно пытался собрать статистику используя Regex, но не смог организовать правильный подсчет таких слов как
            // "A" и "в" (слова предлоги из одной буквы) - при использовании Regex у меня считались в том числе просто вхождения
            // этих букв в другие слова. 
            // В итоге запилил свой подсчет и довольно аккуратно вышло           
            foreach (var uniq_word in words_array_distinct)
            {
                int count = 0;
                foreach (var word in words_array_full)
                    if (uniq_word == word) count++;
                text_statistics.Add(uniq_word, count);
            }



            // JSON File section
            string json_filename = "Text_statistics.json";
            Serialization_to_JSON_file(text_statistics, json_filename);

            Console.WriteLine(("\n\n--- through JSON: serialization -> write to file -> read from file -> deserialization ---\n" +
                "Text statistics (case ignored):\n\n\t[word]\t\t[quantity]").PadLeft(20));
            foreach (var element in Deserialization_from_JSON_file(json_filename))
                Console.WriteLine((element.Key + " \t" + element.Value).PadLeft(20));

            // XML File section
            string xml_filename = "Text_statistics.xml";

            /// Данные с сайта https://learn.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer?view=net-7.0
            /*   Serialization of ArrayList and Generic List
            The XmlSerializer cannot serialize or deserialize the following:
            1. Arrays of ArrayList
            2. Arrays of List<T>*/

            /// Данная проблема подтверждается исключением, возникающим при попытке
            // XmlSerializer xmlSerializer = new XmlSerializer(typeof(SortedList<string, int>)):
            /*  The type System.Collections.Generic.SortedList`2[
            [System.String, System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e],
            [System.Int32, System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]] 
            is not supported because it implements IDictionary.*/


            //Serialization_to_XML_file(text_statistics, xml_filename); не работает, причины выше


            ///В этой связи поробуем преобразовать SortedList<string, int>) в массив KeyValuePair<TKey, TValue>[]
            var text_statistics_array = text_statistics.ToArray();
            Serialization_to_XML_file_2(text_statistics_array, xml_filename);

            Console.WriteLine(("\n\n--- through XML: serialization -> write to file -> read from file -> deserialization ---\n" +
                "Text statistics (case ignored):\n\n\t[word]\t\t[quantity]").PadLeft(20));
            foreach (var element in Deserialization_from_XML_file_2(xml_filename))
                Console.WriteLine((element.Key + " \t" + element.Value).PadLeft(20));
        }

        static void Serialization_to_JSON_file(SortedList<string, int> text_statistics, string json_filename)
        {

            JsonSerializerOptions options = new JsonSerializerOptions();

            var data_to_json_string = JsonSerializer.Serialize(text_statistics);

            new FileStream(json_filename, FileMode.OpenOrCreate).Close();
            using (StreamWriter streamWriter_1 = new StreamWriter(json_filename))
            {
                streamWriter_1.WriteLine(data_to_json_string);
            }
        }
        static SortedList<string, int> Deserialization_from_JSON_file(string json_filename)
        {
            string read_result;
            StreamReader streamReader_1 = new StreamReader(json_filename);
            read_result = streamReader_1.ReadToEnd();
            return JsonSerializer.Deserialize<SortedList<string, int>>(read_result);
        }
        static void Serialization_to_XML_file(SortedList<string, int> text_statistics, string xml_filename)
        {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SortedList<string, int>));
            var file_stream_1 = new FileStream(xml_filename, FileMode.OpenOrCreate);
            xmlSerializer.Serialize(file_stream_1, text_statistics);
            file_stream_1.Close();

         /*   var data_to_xml_string = XMLSerializer.Serialize(text_statistics);

            new FileStream(json_filename, FileMode.OpenOrCreate).Close();
            using (StreamWriter streamWriter_1 = new StreamWriter(json_filename))
            {
                streamWriter_1.WriteLine(data_to_json_string);
            }*/
        }
        static void Serialization_to_XML_file_2(KeyValuePair<string, int>[] text_statistics_array, string xml_filename)
        {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(KeyValuePair<string, int>[]));

            //var file_stream_1 = new FileStream(xml_filename, FileMode.OpenOrCreate);
            //xmlSerializer.Serialize(file_stream_1, text_statistics_array);
            //file_stream_1.Close();

            TextWriter writer = new StreamWriter(xml_filename);
            xmlSerializer.Serialize(writer, text_statistics_array);
            writer.Close();

        }
        static SortedList<string, int> Deserialization_from_XML_file(string xml_filename)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SortedList<string, int>));

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(xml_filename, FileMode.Open);
            // Declare an object variable of the type to be deserialized.
            SortedList<string, int> po;
            /* Use the Deserialize method to restore the object's state with
            data from the XML document. */
            po = (SortedList<string, int>)xmlSerializer.Deserialize(fs);


     /*       string read_result;
            StreamReader streamReader_1 = new StreamReader(xml_filename);
            read_result = streamReader_1.ReadToEnd();*/


            return po;
        }
        static KeyValuePair<string, int>[] Deserialization_from_XML_file_2(string xml_filename)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(KeyValuePair<string, int>[]));

            FileStream fs = new FileStream(xml_filename, FileMode.Open);
            KeyValuePair<string, int>[] po;
            po = (KeyValuePair<string, int>[])xmlSerializer.Deserialize(fs);

            return po;
        }


    }// class Program
}// namespace