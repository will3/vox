using UnityEngine;

namespace FarmVox
{
    public static class AppendBufferCounter
    {
        public static int Count(ComputeBuffer from)
        {
            var countBuffer = new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);

            ComputeBuffer.CopyCount(from, countBuffer, 0);
            int[] counter = new int[] { 0, 0, 0, 0 };
            countBuffer.GetData(counter);
            int count = counter[0];

            countBuffer.Dispose();

            return count;
        }
    }
}