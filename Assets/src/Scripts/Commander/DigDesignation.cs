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
            var terrian = Finder.FindTerrian();
            var chunks = terrian.DefaultLayer;

            var y = end.y;

            for (var x = start.x; x <= end.x; x++)
            {
                for (var z = start.z; z <= end.z; z++)
                {
                    var result = FindSurfaceCoord(x, y, z);
                    if (result != null) {
                        var coord = result.GetCoord();

                        var layer = result.FindNoneDefaultLayer();

                        if (layer == UserLayers.trees) {
                            var tree = terrian.TreeMap.FindTree(coord);
                            if (tree == null) {
                                throw new System.Exception("cannot find tree in treemap");
                            }
                            if (!tree.markedForRemoval) {
                                tree.markedForRemoval = true;
                                AddTask(new RemoveTreeTask(tree));
                            }
                        } else if (layer == UserLayers.terrian) {
                            //AddTask(new DigTask(coord));
                        }
                    }
                }
            }
		}

        void AddTask(Task task) {
            tasks[task.GetCoord()] = task;
            TaskMap.Instance.AddTask(task);
        }

        RaycastResult FindSurfaceCoord(int x, int y, int z) {
            var yOffset = 2;
            var maxTries = 5;

            for (var i = 0; i < maxTries; i++) {
                var result = FindSurfaceCoord(x, y, z, yOffset);
                if (result != null) {
                    return result;
                }
                yOffset *= 2;
            }

            return null;
        }

        RaycastResult FindSurfaceCoord(int x, int y, int z, int yOffset) {
            var coord = new Vector3Int(x, y + yOffset, z);
            var ray = new Ray(coord + new Vector3(0.5f, 0.5f, 0.5f), Vector3.down);

            var result = VoxelRaycast.TraceRay(ray, 1 << UserLayers.terrian | 1 << UserLayers.trees);

            return result;
        }
	}
}