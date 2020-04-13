using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Scripts.Voxel
{
    public class DefaultBuffers : MonoBehaviour
    {
        private readonly Dictionary<Vector2Int, ComputeBuffer> _buffers = new Dictionary<Vector2Int, ComputeBuffer>();

        public ComputeBuffer GetBuffer(int count, int stride)
        {
            var key = new Vector2Int(count, stride);
            if (_buffers.TryGetValue(key, out var buffer))
            {
                return buffer;
            }

            buffer = new ComputeBuffer(count, stride);
            _buffers[key] = buffer;
            return buffer;
        }

        private void OnDestroy()
        {
            foreach (var buffer in _buffers.Values)
            {
                buffer.Dispose();
            }
        }
    }
}