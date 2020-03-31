using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace KizhiPart1
{
    [TestFixture]
    public class InterpreterTests
    {
        private readonly string[] _commandsSeparator = {"\r\n"};

        void TestInterpreter(string[] commands, string[] expectedOutput)
        {
            string[] actualOutput;
            using (var sw = new StringWriter())
            {
                var interpreter = new Interpreter(sw);

                foreach (var command in commands)
                {
                    interpreter.ExecuteLine(command);
                }

                actualOutput = sw.ToString().Split(_commandsSeparator, StringSplitOptions.RemoveEmptyEntries);
            }

            Assert.AreEqual(expectedOutput.Length, actualOutput.Length);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Set()
        {
            TestInterpreter(new[]
                {
                    "set m 20",
                    "print m"
                },
                new[] {"20"});
        }

        [Test]
        public void Sub()
        {
            TestInterpreter(new[]
            {
                "set m 20", "sub m 19", "print m"
            }, new[] {"1"});
        }

        [Test]
        public void SubAfterRem()
        {
            TestInterpreter(new[]
            {
                "set m 20",
                "rem m",
                "sub m 20"
            }, new[] {"Переменная отсутствует в памяти"});
        }

        [Test]
        public void PrintAfterRem()
        {
            TestInterpreter(new[]
            {
                "set m 20",
                "rem m",
                "print m"
            }, new[] {"Переменная отсутствует в памяти"});
        }

        [Test]
        public void RemAfterRem()
        {
            TestInterpreter(new[]
            {
                "set m 20",
                "rem m",
                "rem m"
            }, new[] {"Переменная отсутствует в памяти"});
        }

        [Test]
        public void DontExecuteAfterNotFound()
        {
            TestInterpreter(new[]
            {
                "print a",
                "set a 5",
                "print a"
            }, new[] {"Переменная отсутствует в памяти"});
        }

        [Test]
        public void VariableValueIsZero()
        {
            Assert.Throws<ArgumentException>(() =>
                TestInterpreter(new[] {"set m 0"}, new string[0])
            );
        }
    }
}