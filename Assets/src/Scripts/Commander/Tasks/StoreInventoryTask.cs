using System.Linq;
using UnityEngine;

namespace FarmVox
{
    public class StoreInventoryTask : Task
    {
        public StoreInventoryTask() {
            type = TaskType.StoreInventory;
        }

        public override Vector3Int GetCoord()
        {
            return new Vector3Int();
        }

        StorageBlock storageBlock;

        public override void Perform(Actor actor)
        {
            if (storageBlock == null) {
                var coord = Vectors.FloorToInt(actor.transform.position);
                storageBlock = StorageMap.Instance.Find(coord, 1.0f);
                storageBlock.allocated = true;
            }

            // Cant find storage, done
            if (storageBlock == null) {
                done = true;
                return;
            }

            var item = actor.Items.Last();

            actor.RemoveItem(item);
            storageBlock.AddItem(item);

            // Find a new storage block
            if (storageBlock.Storage <= 0.0f) {
                storageBlock = null;
            }

            // Cleared all items
            if (!actor.Items.Any()) {
                done = true;
            }
        }
    }
}