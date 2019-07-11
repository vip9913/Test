using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ParserBases.Tests
{
    [TestClass()]
    public class BlockTestsFileName
    {        
        /// <summary>
        /// Проверка правильности метода определения ошибочных путей
        /// </summary>
        [TestMethod()]
        public void FilePathValidTest()
        {          
            Block b = new Block();                   

            string path = @"""\\clusterfs126\users\91776\DB\Accounting3""";            
            var actual = b.FilePathValid(path);
            var expected = true;
            Assert.AreEqual(actual, expected);

            path = @"""s:\\usersdata\43\<М>""";
            actual = b.FilePathValid(path);
            expected = false;
            Assert.AreEqual(actual, expected);

            path = @"""s:\\usersdata\43\ХТ|yu""";
            actual = b.FilePathValid(path);
            expected = false;
            Assert.AreEqual(actual, expected);

            path = @"""s:\\usersdata\43\ХТ▼""";
            actual = b.FilePathValid(path);
            expected = false;
            Assert.AreEqual(actual, expected);
        }
    }
}