float4 sampleColorGradient32(float4 gradient[33], float v) {
    float segments = 32;

    if (v <= 0) {
        return gradient[0];
    }

    if (v >= 1) {
        return gradient[segments];
    }

    float i = v * segments;
    int i1 = floor(i);
    int i2 = i1 + 1;
    float4 v1 = gradient[i1];
    float4 v2 = gradient[i2];
    float r = i - i1;

    return v1 * (1 - r) + v2 * r;
}

float sampleValueGradient64(float curve[67], float v) {
    float segments = 64;
    int offset = 2;

    float t1 = curve[0];
    float t2 = curve[1];

    if (v < t1) {
        return curve[offset];
    }

    if (v > t2) {
        return curve[segments + offset];
    }

    float i = (v - t1) / (t2 - t1) * segments;

    int i1 = floor(i);

    int i2 = i + 1;
    float r = i - i1;

    float v1 = curve[i1 + offset];
    float v2 = curve[i2 + offset];

    return v1 * (1 - r) + v2 * r;
}