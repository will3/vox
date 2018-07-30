using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class StorageBlock
    {
        public bool allocated;
        public readonly Vector3Int coord;
        float storage = 4.0f;

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

            var yOffset = items.Count;
            var coordAbove = coord + new Vector3Int(0, yOffset, 0);

            var buildingLayer = Finder.FindTerrian().BuildingLayer;

            buildingLayer.Set(coordAbove, 1);
            buildingLayer.SetColor(coordAbove, item.color);
        }

        public void RemoveItem(Item item) {
            items.Remove(item);
            storage += item.weight;
        }
    }
}