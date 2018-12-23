using UnityEngine;

namespace FarmVox.GPU
{
    public class ColorGradientBuffers : System.IDisposable
    {
        private readonly ComputeBuffer _intervalsBuffer;
        private readonly ComputeBuffer _colorsBuffer;

        public ColorGradientBuffers(ComputeBuffer intervalsBuffer, ComputeBuffer colorsBuffer)
        {
            _intervalsBuffer = intervalsBuffer;
            _colorsBuffer = colorsBuffer;
        }

        public void Dispose()
        {
            _intervalsBuffer.Dispose();
            _colorsBuffer.Dispose();
        }
    }
}