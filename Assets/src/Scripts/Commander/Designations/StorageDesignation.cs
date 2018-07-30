using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class StorageDesignation : Designation
    {
        Dictionary<Vector3Int, Task> tasks = new Dictionary<Vector3Int, Task>();

        public StorageDesignation(BoundsInt bounds) {
            this.bounds = bounds;
        }

		public override void Start()
		{
            AdjustBounds();
            CreateBox();

            CreateSurfaceTasks();
        }

        void CreateSurfaceTasks() {
            var terrian = Finder.FindTerrian();
            var chunks = terrian.DefaultLayer;

            var trees = terrian.TreeMap.Search(bounds);

            foreach (var tree in trees)
            {
                if (!tree.markedForRemoval)
                {
                    tree.markedForRemoval = true;
                    var task = new RemoveTreeTask(tree);
                    task.designation = this;
                    var yDiff = task.GetCoord().y - bounds.max.y;
                    task.priority = yDiff * 2.0f + 20.0f;
                    AddTask(task);
                }
            }
        }

        void AddTask(Task task) {
            tasks[task.GetCoord()] = task;
            TaskMap.Instance.AddTask(task);
        }

		public override void Update()
        {
            throw new System.NotImplementedException();
        }

        protected override string GetName()
        {
            return "storage";
        }
    }
}