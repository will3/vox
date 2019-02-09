using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class DeployActors : MonoBehaviour
    {
        public int SearchDistance = 8;
        private HighlightHoveredSurface _highlight;
        private int _deployed;
        public int Number = 1;
        
        private void Start()
        {
            _highlight = gameObject.AddComponent<HighlightHoveredSurface>();
        }

        private void Update()
        {
            if (_deployed >= Number)
            {
                return;
            }
            
            var result = _highlight.Result;

            if (result == null || !Input.GetKeyDown(KeyCode.Mouse0)) return;

            var coord = Terrian.Instance.Routes.FindClosestLandingSpot(result.GetCoord(), SearchDistance);
            
            if (coord == null)
            {
                return;
            }
            
            Deploy(coord.Value);  
            
            _deployed += 1;
        }

        private void Deploy(Vector3Int coord)
        {
            Debug.Log(string.Format("Found landing spot at {0}", coord));
            
            var go = new GameObject("Dawg");

            var actor = go.AddComponent<Actor>();
            actor.Coord = coord;
            actor.Scale = new Vector3(1.0f, 1.5f, 1.0f) * 8.0f;
        }
    }
}