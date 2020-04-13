using System.Collections.Generic;

namespace FarmVox.Scripts
{
    public class CommandManager
    {
        private CommandManager()
        {
        }

        private static CommandManager _instance;

        public static CommandManager Instance => _instance ?? (_instance = new CommandManager());

        private readonly Dictionary<string, ICommand> _map = new Dictionary<string, ICommand>();

        public void Add(ICommand command)
        {
            _map[command.CommandName.ToLowerInvariant()] = command;
        }

        public ICommand Get(string commandName)
        {
            return _map.TryGetValue(commandName.ToLowerInvariant(), out var value) ? value : null;
        }
    }
}