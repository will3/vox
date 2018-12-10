using UnityEngine;

namespace FarmVox.GPU
{
    public static class AppendBufferCounter
    {
        public static int Count(ComputeBuffer from)
        {
            var countBuffer = new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);

            ComputeBuffer.CopyCount(from, countBuffer, 0);
            var counter = new[] { 0, 0, 0, 0 };
            countBuffer.GetData(counter);
            var count = counter[0];

            countBuffer.Dispose();

            return count;
        }
    }
}