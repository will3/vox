using UnityEngine;
using System.Collections;

public class Voxel {
    public Voxel(float value, Color color) {
        this.value = value;
        this.color = color;
    }
    public float value;
    public Color color;
}

public class House 
{
    public Array3<Voxel> Shape;

    public House(int houseWidth, int houseHeight, int length) {
        var height = houseHeight + Mathf.CeilToInt(houseWidth / 2.0f);
        var width = 1 + houseWidth + 1;

        Shape = new Array3<Voxel>(width, height, length);
        var midI = (width - 1) / 2;
        for (var i = 0; i < width; i ++) {
            for (var j = 0; j < height; j ++) {
                for (var k = 0; k < length; k ++) {
                    var isRoof = (j - houseHeight) == i || (j - houseHeight) == (width - 1 - i);
                    var isWall = (i != 0 && i != width - 1) && ((j - houseHeight) < i || (j - houseHeight) < (width - 1 - i));

                    if (isRoof) {
                        var v = new Voxel(1, Colors.roof);
                        Shape.Set(i, j, k, v);
                    }

                    if (isWall) {
                        var v = new Voxel(1, Colors.wall);
                        Shape.Set(i, j, k, v);
                    }
                }
            }
        }
    }
}