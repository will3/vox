using System.Collections.Generic;
using FarmVox.Terrain;

namespace FarmVox.Scripts.Commands
{
    public interface IDevCommand
    {
        IEnumerable<string> Names { get; }

        void Execute(string[] args);
    }

    public class ReloadCommand : IDevCommand
    {
        public IEnumerable<string> Names
        {
            get { return new[] {"re", "reload"}; }
        }
        
        public void Execute(string[] args)
        {
            Terrian.Instance.ReloadConfig();    
        }
    }
}