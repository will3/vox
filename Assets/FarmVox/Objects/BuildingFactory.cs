using UnityEngine;

namespace FarmVox.Objects
{
    public static class BuildingFactory
    {
        public static Building GetHouse()
        {
            var names = new[] { "house", "house1" };
            return new Building
            {
                Name = BuildingType.House,
                ModelName = names[Random.Range(0, names.Length)],
                Offset = new Vector3Int(-3, 0, -3)
            };
        }

        public static Building GetWall()
        {
            return new Building
            {
                Name = BuildingType.Wall,
                ModelName = "wall2",
                Offset = new Vector3Int(-3, 0, -3)
            };
        }
    }
}