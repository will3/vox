float4 sampleColor(float4 gradient[8], float intervals[8], int size, float banding, float ratio) {
    return gradient[0];
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

float sampleValue(float values[8], float keys[8], int size, float ratio) {
    if (ratio < keys[0]) {
        ratio = keys[0];
    }
    if (ratio > keys[size - 1]) {
        ratio = keys[size - 1];
    }

    for (int i = 0; i < size - 1; i++) {
        float ra = keys[i];
        float rb = keys[i + 1];

        if (ratio >= ra && ratio <= rb) {
            float r = (ratio - ra) / (rb - ra);
            float va = values[i];
            float vb = values[i + 1];
            return va + (vb - va) * r;
        }
    }
    return 0;
}