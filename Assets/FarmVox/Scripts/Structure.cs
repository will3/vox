using FarmVox.Scripts.Voxel;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class Structure : MonoBehaviour
    {
        public Chunks chunks;
        public Ground ground;

        public Vector3Int origin;
        public int gridSize = 3;
        public Vector2Int numGrids;
        public StructureType structureType;
        public int wallHeight = 24;
        
        public Color wallColor = new Color(0.4f, 0.4f, 0.4f);

        private void Start()
        {
            if (structureType == StructureType.Wall)
            {
                DrawWall();
            }
        }

        private void DrawWall()
        {
            var sizeX = numGrids.x * gridSize;
            var sizeZ = numGrids.y * gridSize;

            for (var i = 0; i < sizeX; i++)
            {
                for (var k = 0; k < sizeZ; k++)
                {
                    for (var j = origin.y; j < wallHeight; j++)
                    {
                        var coord = new Vector3Int(origin.x + i, j, origin.z + k);
                        var isGround = ground.IsGround(coord);

                        if (j == wallHeight - 1)
                        {
                            if ((i == 0 || i == sizeX - 1) && (k == 0 || k == sizeZ - 1))
                            {
                                chunks.Set(coord, 1.0f);
                                chunks.SetColor(coord, wallColor);
                            }

                            continue;
                        }

                        if (!isGround)
                        {
                            chunks.Set(coord, 1.0f);
                            chunks.SetColor(coord, wallColor);
                        }
                    }
                }
            }
        }
    }
}