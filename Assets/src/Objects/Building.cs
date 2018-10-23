using UnityEngine;

namespace FarmVox
{
    public class Building
    {
        public readonly VoxelModel Model;
        
        public Building(string name)
        {
            Model = new VoxelModel("house", new Vector3Int(-3, 0, -3));
        }
    }
}