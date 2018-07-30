using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class StorageDesignation : Designation
    {
        Dictionary<Vector3Int, Task> tasks = new Dictionary<Vector3Int, Task>();

        public StorageDesignation(BoundsInt bounds) {
            this.bounds = bounds;
            type = DesignationType.Storage;
        }

		public override void Start()
		{
            bounds = Boxes.AdjustBounds(bounds);
            CreateBox();
            RemoveAnyTrees();
            MarkStorageBlocks();
        }

        void RemoveAnyTrees() {
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

        void MarkStorageBlocks() {
            var enumerator = BoundCoords.LoopCoords(bounds);
            var chunks = Finder.FindTerrian().DefaultLayer;
            while (enumerator.MoveNext()) {
                var coord = enumerator.Current;
                if (chunks.IsSurfaceCoord(coord)) {
                    var storageBlock = new StorageBlock(coord);
                    StorageMap.Instance.Add(storageBlock);
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