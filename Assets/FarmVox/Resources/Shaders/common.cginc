float4 sampleColor(float4 gradient[8], float intervals[8], int size, float banding, float ratio) {
    if (size == 1) {
        return gradient[0];
    }
    if (ratio < 0) {
        ratio = 0;
    }
    if (ratio > 1.0) {
        ratio = 1.0;
    }

    if (banding > 0) {
        ratio = floor(ratio * banding) / banding;
    }

    for (int i = 0; i < size - 1; i++) {
        float ra = intervals[i];
        float rb = intervals[i + 1];

        if (ratio >= ra && ratio <= rb) {
            float r = (ratio - ra) / (rb - ra);
            return lerp(gradient[i], gradient[i + 1], r);
        }
    }

    return float4(0, 0, 0, 0);
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