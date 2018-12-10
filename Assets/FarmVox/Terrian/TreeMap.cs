using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class Tree
    {
        public readonly HashSet<Vector3Int> trunkCoords;
        public readonly HashSet<Vector3Int> stumpCoords;
        public readonly Vector3Int pivot;

        public bool markedForRemoval;
        public bool removedTrunk;
        public bool removedStump;

        public Tree(HashSet<Vector3Int> stumpCoords, HashSet<Vector3Int> trunkCoords, Vector3Int pivot)
        {
            this.stumpCoords = stumpCoords;
            this.trunkCoords = trunkCoords;
            this.pivot = pivot;
        }
    }

    public class TreeMap
    {
        readonly HashSet<Vector3Int> trees = new HashSet<Vector3Int>();
        BoundsInt bounds;
        Octree<Tree> map;

        public TreeMap(BoundsInt bounds)
        {
            this.bounds = bounds;
            map = new Octree<Tree>(bounds);
        }

        //Dictionary<Vector3Int, Tree> treeLookup = new Dictionary<Vector3Int, Tree>();

        public void AddTree(Tree tree)
        {
            if (!map.Set(tree.pivot, tree))
            {
                map.Set(tree.pivot, tree);
                //Debug.LogWarning("Failed to add tree at " + tree.pivot.ToString());
            }

            //foreach (var coord in tree.trunkCoords)
            //{
            //    treeLookup[coord] = tree;
            //}

            //foreach (var coord in tree.stumpCoords)
            //{
            //    treeLookup[coord] = tree;
            //}
        }

        public void RemoveTree(Tree tree)
        {
            map.Remove(tree.pivot);

            //foreach (var coord in tree.trunkCoords)
            //{
            //    treeLookup.Remove(coord);
            //}
        }

        public Tree FindTree(Vector3Int coord)
        {
            //if (treeLookup.ContainsKey(coord))
            //{
            //    return treeLookup[coord];
            //}
            return null;
        }

        public bool HasTrees(BoundsInt b)
        {
            return map.Any(b);
        }

        public List<Tree> Search(BoundsInt bounds)
        {
            return map.Search(bounds);
        }
    }
}
