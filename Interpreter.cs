using System.Collections.Generic;
using System.IO;

namespace KizhiPart1
{
    public class Interpreter
    {
        private readonly CommandExecutor _commandExecutor;

        private bool _isPreviousCommandExecuted = true;

        public Interpreter(TextWriter writer)
        {
            _commandExecutor = new CommandExecutor(writer);
        }

        public void ExecuteLine(string command)
        {
            if (!_isPreviousCommandExecuted) return;

            var commandParts = command.Split(' ');
            var commandForExecute = CreateCommandFromCommandParts();
            _isPreviousCommandExecuted = _commandExecutor.TryExecute(commandForExecute);


            Command CreateCommandFromCommandParts()
            {
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

        public CommandWithValue(string name, string variableName, int value)
            : base(name, variableName)
        {
            Value = value;
        }

        public override string ToString() => $"{Name} {VariableName} {Value}";
    }

    internal class CommandExecutor
    {
        private readonly TextWriter _writer;

        private readonly Dictionary<string, int> _variableNameToValue = new Dictionary<string, int>();

        public CommandExecutor(TextWriter writer)
        {
            _writer = writer;
        }

        public bool TryExecute(Command command)
        {
            if (command.Name != "set" && !MemoryContainsVariableWithName(command.VariableName))
                return false;

            if (command is CommandWithValue valueCommand)
                Execute(valueCommand);
            else
                Execute(command);

            return true;
        }

        private bool MemoryContainsVariableWithName(string name)
        {
            if (_variableNameToValue.ContainsKey(name)) return true;

            _writer.WriteLine("Переменная отсутствует в памяти");
            return false;
        }

        private void Execute(CommandWithValue valueCommand)
        {
            switch (valueCommand.Name)
            {
                case "set":
                    _variableNameToValue[valueCommand.VariableName] = valueCommand.Value;
                    break;
                case "sub":
                    _variableNameToValue[valueCommand.VariableName] -= valueCommand.Value;
                    break;
            }
        }

        private void Execute(Command command)
        {
            switch (command.Name)
            {
                case "rem":
                    _variableNameToValue.Remove(command.VariableName);
                    break;
                case "print":
                    _writer.WriteLine(_variableNameToValue[command.VariableName]);
                    break;
            }
        }
    }
}