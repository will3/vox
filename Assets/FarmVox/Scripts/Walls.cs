using System;
using System.Collections.Generic;
using System.Linq;
using FarmVox.Models;
using FarmVox.Objects;
using FarmVox.Terrain;
using FarmVox.Voxel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace FarmVox.Scripts
{
    [RequireComponent(typeof(Chunks))]
    public class Walls : MonoBehaviour
    {
        public Chunks chunks;
        public GameObject structurePrefab;
        public GameObject wallPrefab;
        public Terrian terrian;

        public TextAsset sawmill;
        public TextAsset house;
        public TextAsset wall;

        public int wallHeight = 6;
        public int buildingTileToleranceVertical = 4;

        private readonly QuadTree<GameObject> _structures = new QuadTree<GameObject>(32);
        private const int GridSize = 6;

        public void PlaceBuilding(StructureType structureType, Vector3Int coord)
        {
            var buildingTile = GetBuildingTile(coord);
            var buildingCoord = buildingTile.buildingCoord;
            var modelObject = LoadObject(structureType, coord);
            var size = modelObject.GetSize();

            var structureGo = Instantiate(GetStructurePrefab(structureType), transform);
            structureGo.transform.position = buildingCoord + new Vector3(size.x / 2f, 0, size.z / 2.0f);
            var volume = structureGo.GetComponent<NavMeshModifierVolume>();
            volume.size = size;
            volume.center = new Vector3(0, size.y / 2.0f, 0);

            ObjectPlacer.Place(chunks, modelObject, buildingCoord, Time.frameCount % 4);

            _structures.Add(buildingCoord, structureGo);
        }

        public bool CanPlaceBuilding(Vector3Int coord, StructureType structureType)
        {
            var buildingTile = GetBuildingTile(coord);
            var buildingCoord = buildingTile.buildingCoord;
            var modelObject = LoadObject(structureType, coord);
            var size = modelObject.GetSize();

            var hasExistingBuilding = _structures.Search(new BoundsInt(buildingCoord, size)).Any();

            return !hasExistingBuilding &&
                   buildingTile.isBuildable;
        }

        private BuildingTile GetBuildingTile(Vector3Int coord)
        {
            var startX = Mathf.FloorToInt(coord.x / (float) GridSize) * GridSize;
            var startZ = Mathf.FloorToInt(coord.z / (float) GridSize) * GridSize;
            var fromY = coord.y;

            var tile = new Dictionary<Vector2Int, Vector3Int?>();

            for (var i = 0; i < GridSize; i++)
            {
                for (var k = 0; k < GridSize; k++)
                {
                    var c = new Vector3Int(startX + i, fromY, startZ + k);
                    var result = SearchVertical(c, 1) ??
                                 SearchVertical(c, -1);

                    tile[new Vector2Int(c.x, c.z)] = result;
                }
            }

            return new BuildingTile(
                tile,
                new Vector2Int(startX, startZ),
                buildingTileToleranceVertical);
        }

        private Vector3Int? SearchVertical(Vector3Int coord, int dir, int searchDistance = 6)
        {
            for (var i = 0; i < searchDistance; i++)
            {
                var next = coord + Vector3Int.up * (i * dir);
                if (!terrian.HasGround(next))
                {
                    return next + Vector3Int.down;
                }
            }

            return null;
        }

        private IPlaceableObject LoadObject(StructureType structureType, Vector3Int coord)
        {
            if (structureType == StructureType.Wall)
            {
                var height = wallHeight + terrian.Config.GroundHeight - coord.y;
                return new WallObject(height);
            }

            var asset = GetAsset(structureType);
            var model = ModelLoader.Load(asset);
            return new ModelObject(model);
        }

        private GameObject GetStructurePrefab(StructureType structureType)
        {
            return structureType == StructureType.Wall ? wallPrefab : structurePrefab;
        }

        private TextAsset GetAsset(StructureType structureType)
        {
            switch (structureType)
            {
                case StructureType.House:
                    return house;
                case StructureType.Sawmill:
                    return sawmill;
                default:
                    throw new ArgumentOutOfRangeException(nameof(structureType), structureType, null);
            }
        }
    }

    public class BuildingTile
    {
        private readonly Dictionary<Vector2Int, Vector3Int?> _map;

        public BuildingTile(
            [NotNull] Dictionary<Vector2Int, Vector3Int?> map,
            Vector2Int gridCoord,
            int buildingTileToleranceVertical)
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));

            var total = map.Count;

            var nonEmptyTiles = map.Values.Where(v => v != null).Select(v => v.Value).ToArray();

            minY = nonEmptyTiles.Min(v => v.y);
            var maxY = nonEmptyTiles.Max(v => v.y);

            isBuildable =
                nonEmptyTiles.Length / (float) total > 0.8f &&
                maxY - minY < buildingTileToleranceVertical;

            var ys = nonEmptyTiles.Select(v => v.y).OrderByDescending(y => y).ToArray();
            var mediumY = ys[Mathf.FloorToInt(ys.Length / (float) 2)];

            buildingCoord = new Vector3Int(gridCoord.x, minY, gridCoord.y);
        }

        public bool isBuildable { get; }

        public IEnumerable<Vector3Int> GetSurfaceCoords()
        {
            return _map.Values.Where(v => v != null).Select(v => v.Value);
        }

        public Vector3Int buildingCoord { get; }

        public int minY { get; }
    }

    public enum StructureType
    {
        House,
        Sawmill,
        Wall
    }
}