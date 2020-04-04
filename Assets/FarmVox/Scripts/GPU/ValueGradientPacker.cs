namespace FarmVox.Scripts.GPU
{
    public static class ValueGradientPacker
    {
        public static float[] PackValueGradient(ValueGradient valueGradient, int segments = 64)
        {
            // x segments has x + 1 values, +2 for min and max t
            var results = new float[segments + 1 + 2];

            var keys = valueGradient.Curve.keys;
            var t1 = keys[0].time;
            var t2 = keys[keys.Length - 1].time;

            results[0] = t1;
            results[1] = t2;

            const int offset = 2;

            for (var i = 0; i <= segments; i++)
            {
                var r = i / (float) segments;
                var t = t1 + (t2 - t1) * r;
                var v = valueGradient.GetValue(t);

                results[i + offset] = v;
            }

            return results;
        }
    }
}