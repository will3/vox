using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox
{
    public class TaskMap
    {
        static TaskMap instance;

        public static TaskMap Instance {
            get {
                if (instance == null) {
                    instance = new TaskMap(TerrianConfig.Instance.BoundsInt);
                }
                return instance;
            }
        }

        Octree<Task> taskTree;

        public TaskMap(BoundsInt bounds) {
            taskTree = new Octree<Task>(bounds);
        }

        public void AddTask(Task task)
        {
            var coord = task.GetCoord();
            if (!taskTree.Set(coord, task)) {
                taskTree.Set(coord, task);
                throw new System.Exception("failed to add task to map");
            }
        }

        public void RemoveTask(Task task) {
            var coord = task.GetCoord();
            taskTree.Remove(coord);
        }

        public Task FindTask(Vector3Int from)
        {
            var maxTry = 5;
            var radius = 16;

            for (var i = 0; i < maxTry; i++)
            {
                var results = Find(from, radius);

                if (results.Count > 0)
                {
                    return GetBest(from, results);
                }

                radius *= 2;
            }

            return null;
        }

        Task GetBest(Vector3 from, List<Task> tasks) {
            var minValue = Mathf.Infinity;
            Task closestTask = null;
            foreach (var task in tasks)
            {
                var diff = task.GetCoord() + new Vector3(0.5f, 0.5f, 0.5f) - from;
                var dis = diff.magnitude;
                var value = dis - task.priority * 10;
                if (value < minValue)
                {
                    minValue = value;
                    closestTask = task;
                }
            }
            return closestTask;
        }

        Task GetRandom(List<Task> tasks) {
            var index = Mathf.FloorToInt(Random.Range(0.0f, 1.0f) * tasks.Count);
            return tasks.ElementAt(index);
        }

        Task GetClosest(Vector3 from, List<Task> tasks) {
            var minDis = Mathf.Infinity;
            Task closestTask = null;
            foreach (var task in tasks) {
                var diff = task.GetCoord() + new Vector3(0.5f, 0.5f, 0.5f) - from;
                var dis = diff.magnitude;
                if (dis < minDis) {
                    minDis = dis;
                    closestTask = task;
                }
            }
            return closestTask;
        }

        List<Task> Find(Vector3Int from, int radius) {
            var extent = new Vector3Int(radius, radius, radius);

            var bounds = new BoundsInt();
            bounds.min = from - extent;
            bounds.max = from + extent;

            var results = taskTree.Search(bounds);
            return results;
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            taskTree.Visit((coord, value) =>
            {
                var pos = coord + new Vector3(0.5f, 0.5f, 0.5f);
                Gizmos.DrawCube(pos, new Vector3(1.02f, 1.02f, 1.02f));
            });

            Gizmos.color = Color.blue;
        }
    }
}