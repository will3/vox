using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class StorageBlock
    {
        public bool allocated;
        public readonly Vector3Int coord;
        float storage = 1.0f;

        public float Storage
        {
            get
            {
                return storage;
            }
        }

        List<Item> items = new List<Item>();

        public StorageBlock(Vector3Int coord)
        {
            this.coord = coord;
        }

        public void AddItem(Item item) {
            items.Add(item);
            storage -= item.weight;
        }

        public void RemoveItem(Item item) {
            items.Remove(item);
            storage += item.weight;
        }
    }
}