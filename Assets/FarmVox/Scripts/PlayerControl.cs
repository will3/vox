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
        public BuildingTiles tiles;
        public float buildingHighlightAmount = 0.6f;

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

            var buildingTile = buildingTileTracker.CurrentTile;
            if (buildingTile != null)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    if (tiles.TryFind2By2(transform.position, 0, out var ts))
                    {
                        foreach (var t in ts)
                        {
                            t.SetHighlightAmount(buildingHighlightAmount);
                        }
                    }
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    var structureType = StructureType.House;
                    if (tiles.TryFind2By2(transform.position, 0, out var ts))
                    {
                        walls.TryPlaceBuilding(ts, structureType);
                    }
                }
            }
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