using System;
using System.IO;

namespace ParserBases
{
    /// <summary>
    /// alex_velig@mail.ru 89107810197
    /// 
    /// Программа для парсинга входных данных
    /// Загружает данные из файла 
    /// Анализирует их и выдает подходящие в ввиде некоторого набора файлов в каталоге запуска
    /// или указанном каталоге пользователя
    /// Не прошедшие проверку оседают тут же в виде bad-files 
    /// Примерное содержание файла с исходными данными приводится
    /// </summary>


    class Program
    {
        private const int ERROR_BAD_ARGUMENTS = 0xA0;       
        private const int ERROR_INVALID_COMMAND_LINE = 0x667;

        public string[] lines;//массив строк загружаемого файла в память согласно условию поставленной задачи (ОЗУ)

        static void Main(string[] args)
        {           
            Program myProg = new Program();
            myProg.Start();
            
            Console.ReadKey();//не забыть убрать в рабочей программе
        }

        /// <summary>
        /// основной модуль
        /// </summary>
        void Start()
        {            
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length == 0) Environment.ExitCode= ERROR_BAD_ARGUMENTS;
                LoadFile();                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error input data. Bad structure input file! "+e.Message);
                Environment.Exit(-1);  //Console.Out.WriteLine("-1");                
            }

            //заглушка для обработки
            Analizer analizer = new Analizer();
            analizer.Read();
            analizer.BadData();
        }

        /// <summary>
        /// загружает файл указанный в параметрах запуска консоли
        /// </summary>
        private void LoadFile()
        {
            string patch= Environment.CurrentDirectory;            
            //string patch=@"C:\Test\ParserBases\";
            string filename = "1.txt";
            string coding = "";
            //  if (!(patch + filename).) return;
            lines= File.ReadAllLines(filename);
        }
    }
}
