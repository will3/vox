using UnityEngine;
using UnityEngine.AI;

namespace FarmVox.Scripts
{
    public class PlayerControl : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Camera targetCamera;
        public Walls walls;
        public BuildingTileTracker buildingTileTracker;

        private void Start()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            agent.updatePosition = false;
        }

        private void Update()
        {
            UpdateMovement();
            
            transform.position = agent.nextPosition;

            var buildingTile = buildingTileTracker.CurrentBuildingTile;
            if (Input.GetKeyDown(KeyCode.Space) && buildingTile != null)
            {
                var structureType = StructureType.House;

                if (walls.CanPlaceBuilding(buildingTile, structureType))
                {
                    walls.PlaceBuilding(buildingTile, structureType);
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