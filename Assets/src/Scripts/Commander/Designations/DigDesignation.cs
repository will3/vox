using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class DigDesignation : Designation
    {
        public DigDesignation(BoundsInt bounds) {
            this.bounds = bounds;
            type = DesignationType.Dig;
        }

        Dictionary<Vector3Int, Task> tasks = new Dictionary<Vector3Int, Task>();

        public override void Start() {
            bounds = Boxes.AdjustBounds(bounds);
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

            var coordsEnumerator = BoundCoords.LoopCoords(bounds);
            while (coordsEnumerator.MoveNext())
            {
                var coord = coordsEnumerator.Current;

                if (chunks.Get(coord) > 0)
                {
                    var task = new DigTask(coord);
                    task.designation = this;
                    var yDiff = task.GetCoord().y - bounds.max.y;
                    task.priority = yDiff * 2.0f;
                    AddTask(task);
                }
            }
        }

		public override void Update()
		{
		}

        void AddTask(Task task) {
            tasks[task.GetCoord()] = task;
            TaskMap.Instance.AddTask(task);
        }

        protected override string GetName()
        {
            return "dig";
        }
	}
}