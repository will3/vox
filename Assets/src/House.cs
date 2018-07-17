using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{
    public class Voxel
    {
        public Voxel(float value, Color color)
        {
            this.value = value;
            this.color = color;
        }
        public float value;
        public Color color;
    }

    public class House
    {
        public Array3<Voxel> shape;

        public Array3<Voxel> Shape
        {
            get
            {
                return shape;
            }
        }

        public Vector3Int offset;

        public House(int houseWidth, int houseHeight, int houseLength)
        {
            var height = houseHeight + Mathf.CeilToInt(houseWidth / 2.0f) + 1;
            var width = 1 + houseWidth + 1;
            var length = houseLength;

            shape = new Array3<Voxel>(width, height, length);
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
                            var v = new Voxel(1, Colors.roof);
                            Shape.Set(i, j, k, v);
                        }

                        if (isWall)
                        {
                            var v = new Voxel(1, Colors.brick);
                            Shape.Set(i, j, k, v);
                        }
                    }
                }
            }
            offset = new Vector3Int(width / 2, height / 2, length / 2) * -1;
        }


        public bool CanPrint(Vector3Int pos, RoadMap roadMap) {

        }

        public void Print(Vector3Int pos, Chunks chunks, RoadMap roadMap) {
            for (var i = 0; i < shape.Width; i++)
            {
                for (var j = 0; j < shape.Height; j++)
                {
                    for (var k = 0; k < shape.Depth; k++)
                    {
                        var voxel = shape.Get(i, j, k);
                        if (voxel == null)
                        {
                            continue;
                        }
                        if (voxel.value <= 0)
                        {
                            continue;
                        }
                        var x = pos.x + i + offset.x;
                        var y = pos.y + j + offset.y;
                        var z = pos.z + k + offset.z;
                        chunks.Set(x, y, z, voxel.value);
                        chunks.SetColor(x, y, z, voxel.color);
                    }
                }
            }

            var padding = 3;
            var startI = pos.x + offset.x - padding;
            var endI = startI + shape.Width + padding;
            var startK = pos.z + offset.z - padding;
            var endK = startK + shape.Depth + padding;

            for (var i = startI; i < endI; i++) {
                for (var k = startK; k < endK; k++) {
                    roadMap.RemoveXZ(new Vector2Int(i, k));
                }
            }
        }
    }
}