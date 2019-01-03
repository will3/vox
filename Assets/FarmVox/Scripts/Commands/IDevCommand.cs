using System.Collections.Generic;
using FarmVox.Terrain;

namespace FarmVox.Scripts.Commands
{
    public interface IDevCommand
    {
        IEnumerable<string> Names { get; }

        void Execute(string[] args);
    }
}