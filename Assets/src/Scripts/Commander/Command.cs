using UnityEngine;

namespace FarmVox
{
    class Command
    {
        public readonly CommandType type;
        public Vector3Int? startCoord;
        public Vector3Int? endCoord;

        public Bounds? Bounds
        {
            get
            {
                if (startCoord == null || endCoord == null)
                {
                    return null;
                }
                var start = startCoord.Value;
                var end = endCoord.Value;

                var bounds = new Bounds();
                bounds.min = new Vector3(
                    start.x < end.x ? start.x : end.x,
                    start.y < end.y ? start.y : end.y,
                    start.z < end.z ? start.z : end.z
                );
                bounds.max = new Vector3(
                    start.x > end.x ? start.x : end.x,
                    start.y > end.y ? start.y : end.y,
                    start.z > end.z ? start.z : end.z
                );
                return bounds;
            }
        }

        public Command(CommandType type)
        {
            this.type = type;
        }
    }
}