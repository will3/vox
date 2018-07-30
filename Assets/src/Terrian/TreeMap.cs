using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class Tree {
        public readonly HashSet<Vector3Int> trunkCoords;
        public readonly HashSet<Vector3Int> stumpCoords;
        public readonly Vector3Int pivot;

        public bool markedForRemoval;
        public bool removedTrunk;

        public Tree(HashSet<Vector3Int> stumpCoords, HashSet<Vector3Int> trunkCoords, Vector3Int pivot) {
            this.stumpCoords = stumpCoords;
            this.trunkCoords = trunkCoords;
            this.pivot = pivot;
        }
    }

    public class TreeMap
    {
        readonly HashSet<Vector3Int> trees = new HashSet<Vector3Int>();
        Bounds bounds;
        Octree<Tree> map;
        public TreeMap(Bounds bounds) {
            this.bounds = bounds;
            map = new Octree<Tree>(
                Vectors.FloorToInt(bounds.min),
                Vectors.FloorToInt(bounds.size));
        }

        Dictionary<Vector3Int, Tree> treeLookup = new Dictionary<Vector3Int, Tree>();

        public void AddTree(Tree tree) {
            if (!map.Add(tree.pivot, tree)) {
                throw new System.Exception("Failed to add tree");
            }

            foreach (var coord in tree.trunkCoords) {
                treeLookup[coord] = tree;
            }

            foreach (var coord in tree.stumpCoords) {
                treeLookup[coord] = tree;
            }
        }

        public void RemoveTree(Tree tree) {
            map.Remove(tree.pivot);

            foreach (var coord in tree.trunkCoords) {
                treeLookup.Remove(coord);
            }
        }

        public Tree FindTree(Vector3Int coord) {
            if (treeLookup.ContainsKey(coord)) {
                return treeLookup[coord];
            }
            return null;
        }

        public bool HasTrees(Bounds b)
        {
            return map.Any(b);
        }
    }
}
