using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public enum DesignationType
    {
        Dig
    }

    public interface IDesignation {
        void UpdateTasks();
    }

    public class DigDesignation : IDesignation
    {
        public DesignationType type;
        public Vector3Int start;
        public Vector3Int end;

        Dictionary<Vector3Int, Task> tasks = new Dictionary<Vector3Int, Task>();

		void UpdateTasks()
		{
            var terrian = Finder.FindTerrian();
            var chunks = terrian.DefaultLayer;

            var y = end.y;

            // Update y bounds;

            int maxY = end.y;
            int minY = start.y;

            for (var x = start.x; x <= end.x; x++)
            {
                for (var z = start.z; z <= end.z; z++)
                {
                    var result = FindGroundCoord(x, y, z);
                    if (result != null) {
                        var coord = result.GetCoord();

                        if (coord.y > maxY) {
                            maxY = coord.y;
                        }

                        if (coord.y < minY) {
                            minY = coord.y;
                        }
                    }
                }
            }

            start.y = minY;
            end.y = maxY;

            var bounds = new Bounds();
            bounds.min = start;
            bounds.max = end;

            var trees = terrian.TreeMap.Search(bounds);

            foreach (var tree in trees) {
                if (!tree.markedForRemoval) {
                    tree.markedForRemoval = true;
                    var task = new RemoveTreeTask(tree);
                    var yDiff = task.GetCoord().y - bounds.max.y;
                    task.priority = yDiff * 2.0f + 20.0f;
                    AddTask(task);
                }
            }

            var coordsEnumerator = BoundCoords.LoopCoords(bounds);
            while(coordsEnumerator.MoveNext()) {
                var coord = coordsEnumerator.Current;

                if (chunks.Get(coord) > 0) {
                    var task = new DigTask(coord);
                    var yDiff = task.GetCoord().y - bounds.max.y;
                    task.priority = yDiff * 2.0f;
                    AddTask(task);
                }
            }
		}

        void AddTask(Task task) {
            tasks[task.GetCoord()] = task;
            TaskMap.Instance.AddTask(task);
        }

        RaycastResult FindGroundCoord(int x, int y, int z) {
            var yOffset = 2;
            var maxTries = 5;

            for (var i = 0; i < maxTries; i++) {
                var result = FindGroundCoord(x, y, z, yOffset);
                if (result != null) {
                    return result;
                }
                yOffset *= 2;
            }

            return null;
        }

        RaycastResult FindGroundCoord(int x, int y, int z, int yOffset) {
            var coord = new Vector3Int(x, y + yOffset, z);
            var ray = new Ray(coord + new Vector3(0.5f, 0.5f, 0.5f), Vector3.down);

            var result = VoxelRaycast.TraceRay(ray, 1 << UserLayers.terrian);

            return result;
        }
	}
}