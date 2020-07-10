int getVoxelShadow(
    int3 coord,
    int size,
    float3 lightDir,
    StructuredBuffer<int> shadowMap00,
    StructuredBuffer<int> shadowMap01,
    StructuredBuffer<int> shadowMap10,
    StructuredBuffer<int> shadowMap11) {
    float yDiff = coord.y;
    float3 shadowCoord = coord + yDiff * lightDir;
    int x = shadowCoord.x;
    int z = shadowCoord.z;

    int i = 0;
    int j = 0;
    int u = x;
    int v = z;

    if (lightDir.x < 0 && x < 0) {
        i = 1;
        u += size;
    } else if (lightDir.x > 0 && x >= size) {
        i = 1;
        u -= size;
    }

    if (lightDir.z < 0 && z < 0) {
        j = 1;
        v += size;
    } else if (lightDir.z > 0 && z >= size) {
        j = 1;
        v -= size;
    }

    if (u < 0 || u >= size || v < 0 || v >= size) {
        //return 99;
    }

    int index = u * size + v;

    if (i == 0) {
        if (j == 0) {
            return shadowMap00[index];
        } else if (j == 1) {
            return shadowMap01[index];
        }
    } else if (i == 1) {
        if (j == 0) {
            return shadowMap10[index];
        } else if (j == 1) {
            return shadowMap11[index];
        }
    }

    return 99;
}