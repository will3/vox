using System;
using System.Collections.Generic;

namespace FarmVox.Scripts.Commands
{
    public class RemoveCommand : IDevCommand
    {
        private readonly SaveManager _saveManager = new SaveManager();
        
        public IEnumerable<string> Names
        {
            get
            {
                return new[]
                {
                    "remove", "rm"
                };
            }
        }

        public void Execute(string[] args)
        {
            if (args.Length <= 1)
            {
                throw new Exception("Usage rm [name]");
            }
            
            _saveManager.RemoveBiome(args[1]);
        }
    }
}