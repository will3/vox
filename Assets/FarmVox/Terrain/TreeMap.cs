using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class TreeMap
    {
        private readonly Octree<Tree> _map;

        public TreeMap(BoundsInt bounds)
        {
            _map = new Octree<Tree>(bounds);
        }

        public void AddTree(Tree tree)
        {
            _map.Set(tree.Pivot, tree);
        }

        public void RemoveTree(Tree tree)
        {
            _map.Remove(tree.Pivot);
        }

        public bool HasTrees(BoundsInt b)
        {
            return _map.Any(b);
        }

        public List<Tree> Search(BoundsInt b)
        {
            return _map.Search(b);
        }

        public void Clear()
        {
            _map.Clear();
        }
    }
}
