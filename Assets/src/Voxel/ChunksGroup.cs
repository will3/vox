using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class ChunksGroup
    {
        private IList<Chunks> list;
        public ChunksGroup(IList<Chunks> list) {
            this.list = list;
        }

        public bool Any(Vector3Int coord) {
            return Any(coord.x, coord.y, coord.z);
        }

        public bool Any(int i, int j, int k) {
            foreach(var chunks in list) {
                if (chunks.Get(i, j, k) > 0) {
                    return true;
                }
            }
            return false;
        }
    }
}
