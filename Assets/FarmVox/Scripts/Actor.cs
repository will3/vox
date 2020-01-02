using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace FarmVox.Scripts
{
    public class Actor : MonoBehaviour
    {
        public NavMeshAgent agent;

        public float speed = 15;
        public float waterSpeedMultiplier = 0.4f;

        private void Start()
        {
            agent.updatePosition = false;
        }

        private void Update()
        {
            transform.position = agent.nextPosition;

            if (!agent.enabled)
            {
                return;
            }

            if (!NavMesh.SamplePosition(agent.nextPosition, out var hit, 1f, NavMesh.AllAreas))
            {
                return;
            }

            var inWater = (hit.mask & 1 << 4) > 0;
            var speedMultiplier = inWater ? waterSpeedMultiplier : 1;
            agent.speed = speed * speedMultiplier;
        }
    }
}