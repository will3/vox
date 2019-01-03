using System;
using System.Collections.Generic;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Scripts.Commands
{
    public class SaveCommand : IDevCommand
    {
        private SaveManager _saveManager = new SaveManager();
        
        public IEnumerable<string> Names
        {
            get { return new[] {"save"}; }
        }
        
        public void Execute(string[] args)
        {
            var name = args[1];

            if (args.Length <= 1)
            {
                throw new Exception("Usage: save [name]");
            }

            _saveManager.SaveBiome(name, Terrian.Instance.Config);
        }
    }
}