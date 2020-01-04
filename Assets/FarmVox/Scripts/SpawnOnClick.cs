using System;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class SpawnOnClick : MonoBehaviour
    {
        public GameObject prefab;
        public Camera mainCamera;

        private void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Input.GetKeyUp(KeyCode.Mouse0))
            {
                return;
            }

            if (!Physics.Raycast(ray, out var hit))
            {
                return;
            }

            var go = Instantiate(prefab);
            go.transform.position = hit.point;
        }
    }
}