using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class VisionMap
    {
        Vector2Int origin;

        public Vector2Int Origin
        {
            get
            {
                return origin;
            }
        }

        int size;

        public int Size
        {
            get
            {
                return size;
            }
        }

        public readonly static int MaxVisionNumber = 512;

        public struct Vision {
            float x;
            float z;
            float radius;

            public Vision(float x, float z, float radius) {
                this.x = x;
                this.z = z;
                this.radius = radius;
            }

            public static int Stride {
                get {
                    return sizeof(float) * 3;    
                }
            }
        }

        HashSet<VisionSource> sources = new HashSet<VisionSource>();
        bool bufferDirty = false;
        Vision[] visionList;
        ComputeBuffer visionBuffer;

        public ComputeBuffer VisionBuffer
        {
            get
            {
                return visionBuffer;
            }
        }

        public VisionMap(int size, Vector2Int origin)
        {
            this.size = size;
            this.origin = origin;
            visionList = new Vision[MaxVisionNumber];
        }

        public void Add(VisionSource source) {
            if (sources.Count == MaxVisionNumber) {
                Debug.LogWarning("Max vision number reached, discard");
                return;
            }
            sources.Add(source);
            bufferDirty = true;
        }

        public void Remove(VisionSource source) {
            sources.Remove(source);
            bufferDirty = true;
        }

        public void UpdateBuffer() {
            if (visionBuffer == null) {
                visionBuffer = new ComputeBuffer(MaxVisionNumber, Vision.Stride);    
            }

            if (bufferDirty) {
                var index = 0;
                foreach (var source in sources) {
                    visionList[index] = new Vision(source.transform.position.x, source.transform.position.z, source.radius);
                    index++;
                }

                // break signal
                visionList[index] = new Vision(0, 0, 0);

                visionBuffer.SetData(visionList);
            }

            bufferDirty = false;
        }

        public void Dispose() {
            if (visionBuffer != null) {
                visionBuffer.Dispose();    
            }
        }
    }
}