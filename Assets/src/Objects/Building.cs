using UnityEngine;

namespace FarmVox
{
    public class Building
    {
        public readonly VoxelModel Model;
        
        public Building(string name)
        {
            Model = VoxLoader.LoadData("house");
            Model.Offset = new Vector3Int(-3, 0, -3);
        }
    }
}