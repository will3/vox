using System;

namespace FarmVox.Scripts
{
    [Serializable]
    public class StoneConfig
    {
        public ColorGradient color = new ColorGradient("#B19F9B");
        public Noise noise;
        public ValueGradient heightCurve = new ValueGradient(1, 0);
    }
}