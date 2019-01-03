using System.Collections.Generic;
using System.IO;
using System.Linq;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Scripts.Commands
{
    public class SaveManager
    {
        private readonly FileStorage _fileStorage = new FileStorage();

        private const string BiomeDir = "biome";
        
        public void SaveBiome(string name, TerrianConfig config)
        {
            _fileStorage.Save(BiomeDir, name, config);
        }

        public TerrianConfig LoadBiome(string name)
        {
            return _fileStorage.Load<TerrianConfig>(BiomeDir, name);
        }

        public IEnumerable<string> ListBiomes()
        {
            return _fileStorage.List(BiomeDir);
        }

        public void RemoveBiome(string name)
        {
            _fileStorage.Remove(BiomeDir, name);
        }
    }
}