using System.Collections.Generic;
using FarmVox.Terrain;

namespace FarmVox.Scripts.Commands
{
    public class LoadCommand : IDevCommand
    {
        private SaveManager _saveManager = new SaveManager();
        
        public IEnumerable<string> Names
        {
            get { return new[] {"load"}; }
        }
        
        public void Execute(string[] args)
        {
            var name = args[1];
            var config = _saveManager.LoadBiome(name);
            Terrian.Instance.ReloadConfig(config);
        }
    }
}