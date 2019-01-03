using System.Collections.Generic;

namespace FarmVox.Scripts.Commands
{
    public class ListCommand : IDevCommand
    {
        private readonly SaveManager _saveManager = new SaveManager();
        
        public IEnumerable<string> Names
        {
            get { return new[] {"list", "ls"}; }
        }
        
        public void Execute(string[] args)
        {
            var names = _saveManager.ListBiomes();

            foreach (var name in names)
            {
                DevConsole.Instance.AddLine(name);
            }
        }
    }
}