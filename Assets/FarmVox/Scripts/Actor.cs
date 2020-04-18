using System.Collections.Generic;
using FarmVox.Voxel;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace FarmVox.Scripts
{
    public class Actor : MonoBehaviour
    {
        public NavMeshAgent agent;

        public float speed = 15;
        public float walkAnimationSpeed = 14f;
        public float waterSpeedMultiplier = 0.4f;
        public Animator animator;
        private Vector3 _lastPosition;
        private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");

        private void Start()
        {
            agent.updatePosition = false;
        }

        private void Update()
        {
            transform.position = agent.nextPosition;

            UpdateWaterSpeedMultiplier();

            var position = transform.position;

            if (_lastPosition != Vector3.zero)
            {
                var velocity = (_lastPosition.GetXz() - position.GetXz()).magnitude;
                animator.SetFloat(WalkSpeed, velocity * walkAnimationSpeed);
            }

            _lastPosition = position;
        }

        private void UpdateWaterSpeedMultiplier()
        {
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