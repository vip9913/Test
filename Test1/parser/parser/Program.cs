using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace parser
{
    class Program
    {
        static void Main(string[] args)
        {
        //...
        foreach (var b in Block.Load("test.txt"))
        {
            var str = b.Title + "\n\t" + string.Join("\n\t", b.Body);
        Console.WriteLine(str);
        }

        Console.ReadKey();
        //...
        }
    }

    /// <summary>
    /// Класс для работы с блоками данных
    /// </summary>
class Block
{
    public string Title;
    public IList<string> Body;

    public static IEnumerable<Block> Load(string path)
    {
        Block ret = null;
            //foreach (var line in File.ReadLines(path).Select(l => l.Trim()))
            foreach (var line in File.ReadLines(path))
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
                ret.Body.Add(line);               
        }
    }
}


  //public class XMLValidate
  //  {
  //      public bool Validate(string Text, out string Error)
  //      {
  //          try
  //          {
  //              XmlDocument doc = new XmlDocument();
  //              doc.LoadXml(Text);
  //              Error = "";
  //              return true;
  //          }
  //          catch (Exception ex)
  //          {
  //              Error = $"Неверный формат XML строки: {ex.Message}";
  //              return false;
  //          }
  //      }
  //  }


}