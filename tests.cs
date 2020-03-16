using System.IO;
using NUnit.Framework;
using NUnit;

namespace KizhiPart1
{
    [TestFixture]
    public class tests
    {
        [Test]
        public void Set()
        {
            var path = @"testSet.txt";
            var sw = new StreamWriter(path);
            sw.AutoFlush = true;
            var interpreter = new Interpreter(sw);
            interpreter.ExecuteLine("set m 20");
            interpreter.ExecuteLine("print m");

            sw.Close();
            var sr = new StreamReader(path);
            var line = sr.ReadLine();
            Assert.AreEqual(new[] {"20"},
                new[] {line});
        }

        [Test]
        public void Sub()
        {
            var path = @"testSub.txt";
            var sw = new StreamWriter(path);
            sw.AutoFlush = true;
            var interpreter = new Interpreter(sw);
            interpreter.ExecuteLine("set m 20");
            interpreter.ExecuteLine("sub m 20");
            interpreter.ExecuteLine("print m");

            sw.Close();
            var sr = new StreamReader(path);
            var line = sr.ReadLine();
            Assert.AreEqual(new[] {"0"},
                new[] {line});
        }

        [Test]
        public void SubAfterRem()
        {
            var path = @"testSubAfterRem.txt";
            var sw = new StreamWriter(path);
            sw.AutoFlush = true;
            var interpreter = new Interpreter(sw);

            interpreter.ExecuteLine("set m 20");
            interpreter.ExecuteLine("rem m");
            interpreter.ExecuteLine("sub m 20");

            sw.Close();
            var sr = new StreamReader(path);
            var line = sr.ReadLine();
            Assert.AreEqual(new[] {"Переменная отсутствует в памяти"},
                new[] {line});
        }

        [Test]
        public void PrintAfterRem()
        {
            var path = @"testPrintAfterRem.txt";
            var sw = new StreamWriter(path);
            sw.AutoFlush = true;
            var interpreter = new Interpreter(sw);

            interpreter.ExecuteLine("set m 20");
            interpreter.ExecuteLine("rem m");
            interpreter.ExecuteLine("print m");


            sw.Close();
            var sr = new StreamReader(path);
            var line = sr.ReadLine();
            Assert.AreEqual(new[] {"Переменная отсутствует в памяти"},
                new[] {line});
        }

        [Test]
        public void RemAfterRem()
        {
            var path = @"testRemAfterRem.txt";
            var sw = new StreamWriter(path);
            sw.AutoFlush = true;
            var interpreter = new Interpreter(sw);

            interpreter.ExecuteLine("set m 20");
            interpreter.ExecuteLine("rem m");
            interpreter.ExecuteLine("rem m");


            sw.Close();
            var sr = new StreamReader(path);
            var line = sr.ReadLine();
            Assert.AreEqual(new[] {"Переменная отсутствует в памяти"},
                new[] {line});
        }

        [Test]
        public void DontWriteAfterNotFound()
        {
            var path = @"testDontWriteAfterNotFound.txt";
            var sw = new StreamWriter(path);
            sw.AutoFlush = true;
            var interpreter = new Interpreter(sw);

            interpreter.ExecuteLine("print a");
            interpreter.ExecuteLine("set a 5");
            interpreter.ExecuteLine("print a");

            sw.Close();
            var sr = new StreamReader(path);
            var line = sr.ReadLine();
            var line2 = sr.ReadLine();
            Assert.AreEqual(new[] {"Переменная отсутствует в памяти", null},
                new[] {line, line2});
        }
    }
}