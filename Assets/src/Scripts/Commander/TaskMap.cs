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
                    instance = new TaskMap(1024);
                }
                return instance;
            }
        }

        Octree<Task> taskTree;
        Octree<Task> assignedTasksTree;

        public TaskMap(int size) {
            taskTree = new Octree<Task>(
                new Vector3Int(-size / 2, -size / 2, -size / 2), 
                new Vector3Int(size, size, size));

            assignedTasksTree = new Octree<Task>(
                new Vector3Int(-size / 2, -size / 2, -size / 2), 
                new Vector3Int(size, size, size));
        }

        public void AddTask(Task task)
        {
            var coord = task.GetCoord();
            if (!taskTree.Add(coord, task)) {
                taskTree.Add(coord, task);
                throw new System.Exception("failed to add task to map");
            }
        }

        public void AssignedTask(Task task) {
            var coord = task.GetCoord();
            if (!taskTree.Remove(coord)) {
                throw new System.Exception("unexpected");
            }

            assignedTasksTree.Add(coord, task);
        }

        public Task FindTask(Vector3 from)
        {
            var maxTry = 5;
            var radius = 16;

            for (var i = 0; i < maxTry; i++)
            {
                var results = Find(from, radius);

                if (results.Count > 0)
                {
                    return GetClosest(from, results);
                }

                radius *= 2;
            }

            return null;
        }

        public void FinishedTask(Task task) {
            if (!assignedTasksTree.Remove(task.GetCoord())) {
                throw new System.Exception("Failed to remove assigned task?");
            }
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

        List<Task> Find(Vector3 from, float radius) {
            var bounds = new Bounds();
            bounds.center = from;
            bounds.extents = new Vector3(radius, radius, radius);

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

            assignedTasksTree.Visit((coord, value) =>
            {
                var pos = coord + new Vector3(0.5f, 0.5f, 0.5f);
                Gizmos.DrawCube(pos, new Vector3(1.02f, 1.02f, 1.02f));
            });
        }
    }
}