using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{
    public class House
    {
        public Vector3Int offset;
        public List<Voxel> voxels = new List<Voxel>();

        public House(int houseWidth, int houseHeight, int houseLength)
        {
            var height = houseHeight + Mathf.CeilToInt(houseWidth / 2.0f) + 1;
            var width = 1 + houseWidth + 1;
            var length = houseLength;

            voxels.Clear();
            var midI = (width - 1) / 2;
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    for (var k = 0; k < length; k++)
                    {
                        var isRoof = (j - houseHeight) == i || (j - houseHeight) == (width - 1 - i);
                        var isWall =
                            (i != 0 && i != width - 1) &&
                            //(k != 0 && k != length - 1) &&
                            ((j - houseHeight) < i && (j - houseHeight) < (width - 1 - i));
                        //var isWall = false;

                        if (isRoof)
                        {
                            var v = new Voxel();
                            v.value = 1;
                            v.color = Colors.GetColor("#B04212");
                            v.type = VoxelType.Roof;
                            voxels.Add(v);
                        }

                        if (isWall)
                        {
                            var v = new Voxel();
                            v.value = 1;
                            v.color = Colors.GetColor("#A9B1B4");
                            v.type = VoxelType.Roof;
                            voxels.Add(v);
                        }
                    }
                }
            }
            offset = new Vector3Int(width / 2, height / 2, length / 2) * -1;
        }
    }
}