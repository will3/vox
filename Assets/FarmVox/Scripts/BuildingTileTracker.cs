using System.Collections.Generic;
using System.Linq;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class BuildingTileTracker : MonoBehaviour
    {
        public float highlightDistance = 10.0f;
        public int searchRange = 3;
        public float maxOpacity = 0.2f;
        public float highlightPowCurve = 0.4f;
        public BuildingTiles tiles;

        private Vector3Int _lastCoord;

        public BuildingTile CurrentBuildingTile { get; private set; }

        private void Update()
        {
            var coord = GetCoord();

            if (_lastCoord != coord)
            {
                UpdateNearbyTiles(coord);
                HighlightNearbyTiles(coord);
            }

            _lastCoord = coord;
        }

//        private void ShowBuildingGridIfNeeded()
//        {
//            if (!Input.GetKey(KeyCode.Space))
//            {
//                return;
//            }
//
//            var coord = GetCoord();
//
//            var vector = Camera.main.transform.position - actor.transform.position;
//            vector.Normalize();
//            var dir = Vector3.ProjectOnPlane(vector, Vector3.up);
//            var xDir = dir.x > 0 ? 1 : -1;
//            var zDir = dir.z > 0 ? 1 : -1;
//            var xz = GetGridXZ(coord);
//
//            const int yRange = 10;
//
//            var a = new Vector3Int(xz.x, coord.y - yRange, xz.y);
//            var b = a + new Vector3Int(xDir * gridSize, yRange * 2, zDir * gridSize);
//            var bounds = BoundsHelper.CalcBounds(a, b);
//
//            var found = _tiles.Search(bounds);
//
//            foreach (var tile in found)
//            {
//                tile.SetHighlightAmount(0.4f);
//            }
//        }

        private Vector3Int GetCoord()
        {
            var position = transform.position;
            return new Vector3Int(
                Mathf.FloorToInt(position.x),
                Mathf.FloorToInt(position.y),
                Mathf.FloorToInt(position.z));
        }

        private void UpdateNearbyTiles(Vector3Int coord)
        {
            CurrentBuildingTile = tiles.GetOrCreateBuildingTile(coord);

            for (var i = -searchRange; i <= searchRange; i++)
            {
                for (var k = -searchRange; k <= searchRange; k++)
                {
                    if (i == 0 && k == 0)
                    {
                        continue;
                    }

                    var next = coord + new Vector3Int(i, 0, k) * tiles.gridSize;
                    tiles.GetOrCreateBuildingTile(next);
                }
            }
        }

        private void HighlightNearbyTiles(Vector3Int coord)
        {
            var center = coord + new Vector3(0.5f, 0.5f, 0.5f);
            var size = new Vector3(20, 8, 20);
            var bounds = new Bounds(center, size);
            var ts = tiles.Search(bounds);

            foreach (var tile in ts)
            {
                var distance = (center - tile.bounds.center).magnitude;
                var r = Mathf.Clamp01(1 - distance / highlightDistance);
                r = Mathf.Pow(r, highlightPowCurve);
                var amount = r * maxOpacity;
                tile.SetHighlightAmount(amount);
            }
        }
    }
}