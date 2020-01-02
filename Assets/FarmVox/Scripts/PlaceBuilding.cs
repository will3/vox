using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class PlaceBuilding : MonoBehaviour
    {
        public Structures structures;
        public BuildingTileTracker buildingTileTracker;
        public BuildingTiles tiles;
        public float buildingHighlightAmount = 0.6f;
        public TextAsset structuresText;
        public StructureType structureType;
        private Dictionary<StructureType, StructureData> _structureData;

        private void Start()
        {
            var structures = JsonUtility
                .FromJson<StructureDataRoot>(structuresText.text)
                .structures;
            _structureData = structures.ToDictionary(d => d.type, d => d);
        }

        private void Update()
        {
            var structureData = _structureData[structureType];

            var buildingTile = buildingTileTracker.CurrentTile;
            if (buildingTile != null)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    if (tiles.TryFindTilesAround(transform.position, structureData.numGrids, out var ts))
                    {
                        foreach (var t in ts)
                        {
                            t.SetHighlightAmount(buildingHighlightAmount);
                        }
                    }
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    if (tiles.TryFindTilesAround(transform.position, structureData.numGrids, out var ts))
                    {
                        structures.TryPlaceBuilding(ts, structureData);
                    }
                }
            }
        }
    }
}