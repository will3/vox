using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public enum DesignationType
    {
        Dig
    }

    public class DigDesignation : MonoBehaviour
    {
        public DesignationType type;
        public Vector3Int start;
        public Vector3Int end;

        Dictionary<Vector3Int, Task> tasks = new Dictionary<Vector3Int, Task>();

		void Start()
		{
            var chunks = Finder.FindTerrian().DefaultLayer;

            var y = end.y;

            for (var x = start.x; x <= end.x; x++)
            {
                for (var z = start.z; z <= end.z; z++)
                {
                    var coord = FindSurfaceCoord(x, y, z);
                    if (coord != null) {
                        AddTask(new DigTask(coord.Value));
                    }
                }
            }
		}

        void AddTask(Task task) {
            tasks[task.coord] = task;
            TaskMap.Instance.AddTask(task);
        }

        Vector3Int? FindSurfaceCoord(int x, int y, int z) {
            var yOffset = 2;
            var maxTries = 5;

            for (var i = 0; i < maxTries; i++) {
                var coord = FindSurfaceCoord(x, y, z, yOffset);
                if (coord != null) {
                    return coord;
                }
                yOffset *= 2;
            }

            return null;
        }

        Vector3Int? FindSurfaceCoord(int x, int y, int z, int yOffset) {
            var coord = new Vector3Int(x, y + yOffset, z);
            var ray = new Ray(coord + new Vector3(0.5f, 0.5f, 0.5f), Vector3.down);
            var result = VoxelRaycast.TraceRay(ray, 1 << UserLayers.terrian);

            if (result != null)
            {
                return result.GetCoord();
            }
            return null;
        }
	}
}