using FarmVox.Models;
using FarmVox.Objects;
using FarmVox.Voxel;
using UnityEngine;
using UnityEngine.AI;

namespace FarmVox.Scripts
{
    public class PlayerControl : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Camera targetCamera;
        public GameObject walls;
        public TextAsset sawmill;
        public TextAsset house;

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
                PlaceBuilding(house);
            }
        }

        private Vector3Int GetBuildingGrid()
        {
            var coord = GetCoord();
            return
                new Vector3Int(
                    Mathf.FloorToInt(coord.x / 6.0f) * 6,
                    coord.y,
                    Mathf.FloorToInt(coord.z / 6.0f) * 6);
        }

        private void PlaceBuilding(TextAsset asset)
        {
            var model = ModelLoader.Load(asset);
            var modelObject = new ModelObject(model);
            var coord = GetBuildingGrid();

            ObjectPlacer.Place(walls.GetComponent<Chunks>(), modelObject, coord, Time.frameCount % 4);
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

            var dest = agent.nextPosition + dir * 10;
            if (!agent.SetDestination(dest))
            {
                Debug.Log("Failed to set destination");
            }
        }
    }
}