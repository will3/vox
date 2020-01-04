using System;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class BuildingTileTracker : MonoBehaviour
    {
        public int searchNumGrids = 3;
        public float highlightSearchDistance = 20.0f;
        public float highlightDistance = 5.0f;
        public float maxOpacity = 0.2f;
        public float highlightPowCurve = 0.4f;
        public bool shouldHighlightCurrentTile = true;
        public float currentTileHighlightAmount = 0.3f;

        public BuildingTiles tiles;

        private Vector3Int _lastCoord;

        public BuildingTile CurrentTile { get; private set; }

        private void Start()
        {
            if (tiles == null)
            {
                tiles = FindObjectOfType<BuildingTiles>();
            }
        }

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

        private void UpdateNearbyTiles(Vector3 coord)
        {
            CurrentTile = tiles.GetOrCreateBuildingTile(coord);

            if (CurrentTile == null)
            {
                return;
            }

            for (var i = -searchNumGrids; i <= searchNumGrids; i++)
            {
                for (var k = -searchNumGrids; k <= searchNumGrids; k++)
                {
                    if (i == 0 && k == 0)
                    {
                        continue;
                    }

                    tiles.GetOrCreateBuildingTile(CurrentTile, i, k);
                }
            }
        }

        private void HighlightNearbyTiles(Vector3 coord)
        {
            var ts = tiles.Search(coord, highlightSearchDistance);

            foreach (var tile in ts)
            {
                if (shouldHighlightCurrentTile && tile == CurrentTile)
                {
                    tile.SetHighlightAmount(currentTileHighlightAmount);
                    continue;
                }

                var distance = tile.CalcDistance(coord);
                var r = Mathf.Clamp01(1 - distance / highlightDistance);
                r = Mathf.Pow(r, highlightPowCurve);
                var amount = r * maxOpacity;
                tile.SetHighlightAmount(amount);
            }
        }
    }
}