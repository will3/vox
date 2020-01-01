using UnityEngine;
using UnityEngine.AI;

namespace FarmVox.Scripts
{
    public class PlayerControl : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Camera targetCamera;
        public Walls walls;

        private void Start()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private void Update()
        {
            UpdateMovement();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                var coord = GetCoord();
                var structureType = StructureType.House;
                if (walls.CanPlaceBuilding(coord, structureType))
                {
                    walls.PlaceBuilding(structureType, coord);    
                }
            }
        }

        private Vector3Int GetCoord()
        {
            var position = transform.position;
            return new Vector3Int((int) position.x, (int) position.y, (int) position.z);
        }

        private void UpdateMovement()
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

            agent.Move(Time.deltaTime * agent.speed * dir);
        }
    }
}