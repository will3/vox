using Priority_Queue;
using UnityEngine;

namespace FarmVox
{
    class RouteNode : FastPriorityQueueNode
    {
        public RouteNode(Vector3Int node)
        {
            this.coord = node;
        }

        public readonly Vector3Int coord;
        public float cost = 0.0f;
    }
}