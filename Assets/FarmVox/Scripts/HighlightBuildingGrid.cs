using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class HighlightBuildingGrid : MonoBehaviour
    {
        public HeightMap.Tile HoveredTile;
        private HeightMap.Tile _lastDrawnTile;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        private void Start()
        {
            
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();    
        }
        
        // Update is called once per frame
        public void Update()
        {
            var result = VoxelRaycast.TraceMouse(1 << UserLayers.Terrian);
            if (result == null)
            {
                HoveredTile = null;
                _meshRenderer.enabled = false;
                return;
            } 
            
            var coord = result.GetCoord();

            HoveredTile = Finder.FindTerrian().heightMap.GetTile(coord);

            if (HoveredTile == null)
            {
                _meshRenderer.enabled = false;
                return;
            }
            
            if (!HoveredTile.CanBuild) {
                _meshRenderer.enabled = false;
                return;
            } 
            
            // Update mesh
            if (HoveredTile == _lastDrawnTile) return;
            
            Destroy(_meshFilter.mesh);
            _meshRenderer.enabled = true;
            var meshBuilder = new CubeMeshBuilder();
            foreach (var tileCoord in HoveredTile.Coords)
            {
                meshBuilder.AddQuad(Axis.Y, true, tileCoord);
            }

            _meshFilter.mesh = meshBuilder.Build();
            _lastDrawnTile = HoveredTile;
        }
    }
}