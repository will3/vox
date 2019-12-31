using UnityEngine;
using UnityEngine.AI;

namespace FarmVox.Scripts
{
    public class PlaceAgentOnNavMesh : MonoBehaviour
    {
        public NavMeshAgent agent;
        public int areaMask;
        private bool _hasPlacedAgent;

        private void Update()
        {
            if (_hasPlacedAgent)
            {
                return;
            }

            if (!NavMesh.SamplePosition(transform.position, out var hit, 1000, areaMask))
            {
                return;
            }

            transform.position = hit.position;
            agent.enabled = true;
            _hasPlacedAgent = true;
        }
    }
}