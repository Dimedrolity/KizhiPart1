using System;
using System.Collections.Generic;
using System.IO;

namespace KizhiPart1
{
    public class Interpreter
    {
        private readonly CommandExecutor _commandExecutor;

        public Interpreter(TextWriter writer)
        {
            _commandExecutor = new CommandExecutor(writer);
        }

        public void ExecuteLine(string command)
        {
            if (!_commandExecutor.IsPreviousCommandExecuted) return;

            var commandForExecute = CreateCommandFromString();
            _commandExecutor.Execute(commandForExecute);


            Command CreateCommandFromString()
            {
                var commandParts = command.Split(' ');

                if (commandParts.Length <= 2)
                    return new Command(commandParts[0], commandParts[1]);

                var commandValue = int.Parse(commandParts[2]);
                return new CommandWithValue(commandParts[0], commandParts[1], commandValue);
            }
        }
    }

    internal class Command
    {
        public string Name { get; }
        public string VariableName { get; }

        public Command(string name, string variableName)
        {
            Name = name;
            VariableName = variableName;
        }

        // для удобства при дебаге
        public override string ToString() => $"{Name} {VariableName}";
    }

    internal class CommandWithValue : Command
    {
        public int Value { get; }

        public CommandWithValue(string name, string variableName, int value) : base(name, variableName)
        {
            Value = value;
        }

        public override string ToString() => $"{Name} {VariableName} {Value}";
    }

    internal class CommandExecutor
    {
        public bool IsPreviousCommandExecuted { get; private set; } = true;

        private readonly VariablesStorage _variablesStorage = new VariablesStorage();

        private readonly TextWriter _writer;

        public CommandExecutor(TextWriter writer)
        {
            _writer = writer;
        }

        public void Execute(Command command)
        {
            if (command.Name != "set" && !MemoryContainsVariableWithName(command.VariableName))
            {
                IsPreviousCommandExecuted = false;
                return;
            }

            if (command is CommandWithValue commandWithValue)
                ExecuteCommandWithValue(commandWithValue);
            else
                ExecuteCommand(command);
        }

        private bool MemoryContainsVariableWithName(string variableName)
        {
            if (_variablesStorage.ContainsVariableWithName(variableName)) return true;

            _writer.WriteLine("Переменная отсутствует в памяти");
            return false;
        }

        private void ExecuteCommandWithValue(CommandWithValue commandWithValue)
        {
            switch (commandWithValue.Name)
            {
                case "set":
                    _variablesStorage.SetValueOfVariableWithName(commandWithValue.VariableName, commandWithValue.Value);
                    break;
                case "sub":
                    var currentValue = _variablesStorage.GetValueOfVariableWithName(commandWithValue.VariableName);
                    var valueAfterSub = currentValue - commandWithValue.Value;
                    _variablesStorage.SetValueOfVariableWithName(commandWithValue.VariableName, valueAfterSub);
                    break;
            }
        }

        private void ExecuteCommand(Command command)
        {
            switch (command.Name)
            {
                case "rem":
                    _variablesStorage.RemoveVariableWithName(command.VariableName);
                    break;
                case "print":
                    var value = _variablesStorage.GetValueOfVariableWithName(command.VariableName);
                    _writer.WriteLine(value);
                    break;
            }
        }
    }

    internal class VariablesStorage
    {
        private readonly Dictionary<string, int> _variableNameToValue = new Dictionary<string, int>();

        public bool ContainsVariableWithName(string variableName)
        {
            return _variableNameToValue.ContainsKey(variableName);
        }

        public int GetValueOfVariableWithName(string variableName)
        {
            return _variableNameToValue[variableName];
        }

        public void SetValueOfVariableWithName(string variableName, int value)
        {
            if (value <= 0)
                throw new ArgumentException(
                    $"Попытка присвоить в переменную '{variableName}' значение '{value}'.\n" +
                    "Значениями переменных могут быть только натуральные числа");

            _variableNameToValue[variableName] = value;
        }

        public void RemoveVariableWithName(string variableName)
        {
            _variableNameToValue.Remove(variableName);
        }
    }
}