using UnityEngine;

namespace FarmVox
{
    public partial class GenTerrianGPU
    {
        class ColorGradientBuffers : System.IDisposable
        {
            public readonly ComputeBuffer intervalsBuffer;
            public readonly ComputeBuffer colorsBuffer;

            public ColorGradientBuffers(ComputeBuffer intervalsBuffer, ComputeBuffer colorsBuffer)
            {
                this.intervalsBuffer = intervalsBuffer;
                this.colorsBuffer = colorsBuffer;
            }

            public void Dispose()
            {
                intervalsBuffer.Dispose();
                colorsBuffer.Dispose();
            }
        }
    }
}