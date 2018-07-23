using Priority_Queue;
using UnityEngine;

namespace FarmVox
{
    class RouteNode : FastPriorityQueueNode
    {
        public RouteNode(Vector3Int node)
        {
            this.node = node;
        }

        readonly Vector3Int node;
        public float cost = 0.0f;

        //public void SetCost(float cost) {
        //    this.cost = cost;
        //    Priority = cost;
        //}

        //public float GetCost() {
        //    return cost;
        //}
    }
}