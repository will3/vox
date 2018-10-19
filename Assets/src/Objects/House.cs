using UnityEngine;

namespace FarmVox
{
    class House {
        static VoxelModel model = new VoxelModel("house");

        public void Add(Chunks chunks, Vector3Int position)
        {
            var offset = new Vector3Int(-3, 0, -3);
            model.Add(chunks, position, offset);
        }
    }
}