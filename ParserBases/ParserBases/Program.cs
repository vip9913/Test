using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ParserBases
{
    /// <summary>    
    /// Программа для парсинга входных данных подключений клиентов с БД
    /// alex_velig@mail.ru 89107810197 координаты для связи 
    /// Загружает данные из файла 
    /// Анализирует их и выдает подходящие в ввиде некоторого набора файлов в каталоге запуска
    /// или указанном каталоге пользователя
    /// Не прошедшие проверку оседают тут же в виде bad-files 
    /// Примерное содержание файла с исходными данными приводится в конце листинга
    /// </summary>
    class Program
    {
        private const int ERROR_BAD_ARGUMENTS = 0xA0;       
        private const int ERROR_INVALID_COMMAND_LINE = 0x667;
        private string path = Environment.CurrentDirectory;
        Encoding ascii   = Encoding.ASCII;
        Encoding unicode = Encoding.Unicode;
        Encoding utf8    = Encoding.UTF8;
        private string filename = "";
        private int users=2;//количество пользователей которым сольем файлы для обработки (частей)
        public int part = 0;//счетчик верных блоков данных в исходном файле

        public List<string> list { get; private set; } //массив строк загружаемого файла в память согласно условию поставленной задачи (ОЗУ)
        public bool structure_test = true;       
        
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
            ClearFilesDir();
            Block block = new Block();
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length != 2) 
                {
                    Environment.ExitCode = ERROR_BAD_ARGUMENTS;
                    Console.WriteLine("Error bad arguments (no input *.txt file)!");
                    Console.WriteLine(@"Run program as \\ParsesBases example.txt");
                    return;
                }
                filename = args[1];                
                if (File.Exists(path + filename)) return;
                LoadFile(filename);
                if (!structure_test)
                {
                    Console.WriteLine("Bad structure input file! ");
                    Environment.Exit(-1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error input data. Bad input file! " + e.Message);
                Environment.Exit(-1);
            }
            Console.WriteLine("Файл - " + filename + " успешно захвачен и обрабатывается...\n");

            //проверка данных                  
            bool key_bad = false;
            foreach (var b in block.Load(list))
            {
                var str = b.Title + "\n\r" + string.Join("\n\r", b.Body)+"\n\r";
                if (str.Contains("!!!"))
                {
                    using (StreamWriter stream = new StreamWriter(path + @"\\bad_data.txt", true))
                        stream.WriteLine(str);
                    key_bad = true;
                }
                else
                    using (StreamWriter stream = new StreamWriter(path + @"\\base_all.txt", true))
                    {
                        stream.WriteLine(b.Title);
                        foreach (var s in b.Body)
                        {
                            stream.WriteLine(s);
                        }                        
                        part++;
                        stream.WriteLine("");
                    }
            }

            //разбиваем блоки по заданным количествам частей и пишем их в файлы base_part.txt                       
            int portion = (part%users)>0? part / users+1: part / users;
            int i = 1, count=0;
            try
            {
                LoadFile(@"base_all.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine("Повреждение промежуточных данных. Перезапустите программу! " + e.Message);
                Environment.Exit(-1);
            }

            foreach (var b in block.Load(list))
            {
                count++;
                if (count > portion)
                {
                    count = 1; i++;
                }
                using (StreamWriter stream = new StreamWriter(path + @"\\base_" + i.ToString() + ".txt", true))
                        stream.WriteLine(b.Title);
                foreach (var s in b.Body)
                {                    
                    using (StreamWriter stream = new StreamWriter(path + @"\\base_" + i.ToString() + ".txt", true))
                        stream.WriteLine(s);
                }
                using (StreamWriter stream = new StreamWriter(path + @"\\base_" + i.ToString() + ".txt", true))
                    stream.WriteLine("");
            }
            Console.WriteLine("Обработанные данные ищите в каталоге - " + path + " с именем base_*.txt \n\r");
            if (key_bad) Console.WriteLine("\n\r В исходных данных есть ошибки смотрите файл bad_data.txt");
        }

        /// <summary>
        /// Зачищаем рабочую директорию программы от мусора
        /// </summary>
        private void ClearFilesDir()
        {
            if (File.Exists(path + @"\\bad_data.txt")) File.Delete(path + @"\\bad_data.txt"); //подчищаем за собой мусор       
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo file in dir.GetFiles("base_*.txt"))
            {
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка очистки директории рабочей программы от старого мусора base_*.txt " + e.Message);
                }
            }
        }

        /// <summary>
        /// загружает файл указанный в параметрах запуска консоли
        /// </summary>
        public void LoadFile(string filename)
        {            
            list = new List<string>();
            //проверка структуры                       
            foreach (string s in File.ReadAllLines(filename,utf8))
            {
                list.Add(s);

                //пустой заголовок
                if (s.Trim().StartsWith("[") &&
                    s.Trim().EndsWith("]") &&
                    s.Trim().Length <= 2)
                    structure_test = false;

                //пустой или некорректный параметр Connect остальные нас не истерисуют в данной задаче                             
                if (s.Trim().StartsWith("Connect") &&  (!s.Contains("=File=") && !s.Contains("=Srvr=")))
                    structure_test = false; 
                if (s.Trim().StartsWith("Connect") && s.Contains("=Srvr=") && !s.Contains(@";Ref=")) structure_test = false;
            }

            list.Add("");//не знаю есть ли в исходном файле пустая строчка разделитель блока в конце
            
            //if (list.Count == 0) structure_test = true;//на предмет пустого входного файла в техзадании непонятно указано пустой вообще или пустой с верной структурой

            //попытка реализовать ИИ по восстановлению структуры пока неудачная (мало информации по некорректным файлам)
            for (int i = 0; i < list.Count; i++)
            {
                string temp = list[i].ToString().Trim();
                if (temp.Length!=0) BlockValidate(temp, i);
            }
        }

       /// <summary>
       /// Проверка корректности блока данных т.е. того что между двумя заголовками []
       /// </summary>
       /// <param name="line">исходная строка</param>
       /// <param name="i">номер строки в списке</param>
       /// <returns></returns>                      
        private bool BlockValidate(string line, int i)
        {
            //предположительно нужно сделать сканирование строк с начала в конец и обратно так корректнее получится
            if (i - 1 > 0)
            {
                if (line.StartsWith("[") && line.EndsWith("]")
                && (list[i--].ToString().Trim() == "")
                && (i++ < list.Count && list[i++].ToString().Trim().Contains("Connect"))) return true;
            }
            else
            {
                if (line.StartsWith("[") && line.EndsWith("]")
                && (list[i].ToString().Trim() == "")
                && (i++ < list.Count && list[i++].ToString().Trim().Contains("Connect"))) return true;
            }
            return false;          
        }               
    }

    /// <summary>
    /// Класс для работы с блоками данных
    /// </summary>
    public class Block
    {
        public string Title;
        public IList<string> Body;

        public IEnumerable<Block> Load(List<string> list)
        {
            Block ret = null;
            foreach (string line in list)
            {
                if (line.Length == 0 && ret != null)
                {
                    yield return ret;
                    ret = null;
                    continue;
                }
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    ret = new Block { Title = line.Trim(), Body = new List<string>() };
                    continue;
                }
                if (ret != null && line.StartsWith("Connect"))
                {
                    if (line.Contains("File")) //проверка путей файловой БД
                    {
                        if (FilePathValid(line)) ret.Body.Add(line);
                        else ret.Body.Add(line + "... что-то не так с путями...!!!");
                    }
                    if (line.Contains("Srvr")) //проверка путей серверной БД
                    {
                        if (SrvrNameValid(line)) ret.Body.Add(line);
                        else ret.Body.Add(line + "... что-то не так с настройками серверной БД ...!!!");
                    }
                }
            }
        } 
        
        /// <summary>
        /// Проверка валидности пути к файлу
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool FilePathValid(string path)
        {
            // ",      0022
            // <,      003C
            // >,      003E
            // |,      007C
            if (path.Length == 0) return false;
            string p = path.Substring(path.IndexOf('"'), path.LastIndexOf('"') - path.IndexOf('"')).Trim('"');           
            if ((path==null) ||
                (p.IndexOfAny(Path.GetInvalidPathChars()) != -1)) return false;
            return true;
        }

        /// <summary>
        /// Проверка валидности серверного пути
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public bool SrvrNameValid(string srvName)
        {
            string subSrv = srvName.Substring(srvName.IndexOf('"') + 1, srvName.IndexOf(';') - 2 - srvName.IndexOf('"') - 1);
            string subRef = srvName.Substring(srvName.IndexOf("Ref=") + 5, srvName.LastIndexOf('"') - srvName.IndexOf("Ref=")-5);
            if (srvName != null && subSrv.Length > 0 && subRef.Length > 0) return true;
            //string pattern= @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            //    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";            
            //if (Regex.IsMatch(srvName, pattern, RegexOptions.IgnoreCase))  return false;
            return false;
        }
    }
}


//Пример файла для обработки
//[KTA_NDS]
//Connect=File="\\clusterfs126\users\91776\DB\Accounting3";
//ID=e74bf314-e943-4830-bd93-befc04aa714e
//External = 0

//[МХМ]
//Connect=File="s:\usersdata\643343\МХТ";

//[Оптовик]
//        Connect=Srvr="host371:1741";Ref="432345_base01";
//ID=e74bf314-e943-4830-bd93-36aba8c5a66d
//External = 1

//[ООО & quot; Сити&quot;]
//Connect=Srvr="host166";Ref="635445_base23";

