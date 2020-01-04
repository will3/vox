float getVision(float3 worldPos, float3 playerPosition, float visionRange, float gridSize, float blurRange) {
    float2 xz = worldPos.xz;
    float2 playerXz = playerPosition.xz;
    
    float2 gridXz = floor(xz / gridSize);
    float2 gridPlayerXz = floor(playerXz / gridSize);

    float dis = visionRange - max(abs(gridPlayerXz.x - gridXz.x), abs(gridPlayerXz.y - gridXz.y)) * gridSize;
 
    return 1;
    
    if (dis < 0) {
        return 0;
    }
    
    if (dis < blurRange) {
        return dis / blurRange;
    }
    
    return 1;
}