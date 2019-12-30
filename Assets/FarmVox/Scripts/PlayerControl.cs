using UnityEngine;
using UnityEngine.AI;

namespace FarmVox.Scripts
{
    public class PlayerControl : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Camera targetCamera;

        private void Start()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private void Update()
        {
            var right = Input.GetAxisRaw("Horizontal");
            var forward = Input.GetAxisRaw("Vertical");

            if (!agent.isOnNavMesh)
            {
                return;
            }

            if (Mathf.Abs(right) < Mathf.Epsilon && Mathf.Abs(forward) < Mathf.Epsilon)
            {
                agent.isStopped = true;
                agent.ResetPath();
                return;
            }

            agent.isStopped = false;
            var inVector = (transform.position - targetCamera.transform.position).normalized;
            var rightVector = Vector3.Cross(Vector3.up, inVector);
            var forwardVector = Vector3.Cross(rightVector, Vector3.up);

            var dir = rightVector * right + forwardVector * forward;

            var dest = agent.nextPosition + dir * 10;
            if (!agent.SetDestination(dest))
            {
                Debug.Log("Failed to set destination");
            }
        }
    }
}