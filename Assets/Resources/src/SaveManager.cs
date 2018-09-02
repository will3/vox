using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace FarmVox
{
    public class SaveManager
    {
        public void Save() {
            var terrian = Finder.FindTerrian();
            foreach (var chunk in terrian.DefaultLayer.Map.Values) 
            {
                SaveChunk(chunk, "default");
            }

            foreach (var chunk in terrian.WaterLayer.Map.Values) 
            {
                SaveChunk(chunk, "water");
            }

            //foreach (var chunk in terrian.TreeLayer.Map.Values)
            //{
            //    SaveChunk(chunk, "tree");
            //}
        }

        public void SaveChunk(Chunk chunk, string prefix) {
            var chunkData = new ChunkData();
            chunkData.data = chunk.Data;
            chunkData.colors = chunk.Colors;

            var path = pathForChunk(chunk, prefix);

            var bf = new BinaryFormatter();
            var file = File.Open(path, FileMode.Open);

            bf.Serialize(file, chunkData);

            file.Close();
        }

        public void LoadChunk(Chunk chunk, string prefix) {
            var path = pathForChunk(chunk, prefix);

            if (File.Exists(path)) {
                var file = File.Open(path, FileMode.Open);

                var bf = new BinaryFormatter();

                var chunkData = (ChunkData)bf.Deserialize(file);

                file.Close();

                chunk.SetColors(chunkData.colors);
                chunk.SetData(chunkData.data);
            }
        }

        string pathForChunk(Chunk chunk, string prefix) {
            return Application.persistentDataPath + "/" + "chunk-" + prefix + "-" + chunk.Origin.ToString();
        }

        [Serializable]
        class ChunkData {
            public float[] data;
            public Color[] colors;
        }
    }
}