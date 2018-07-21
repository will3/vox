using UnityEngine;

namespace FarmVox
{
    public class MeshWorker : IWorker
    {
        private Chunk chunk;
        private TerrianChunk terrianChunk;
        Transform transform;
        Material material;
        private bool done;

        public MeshWorker(Chunk chunk, TerrianChunk terrianChunk, Transform transform, Material material)
        {
            this.chunk = chunk;
            this.terrianChunk = terrianChunk;
            this.transform = transform;
            this.material = material;
        }

        public void Start()
        {
            if (chunk.Mesh != null)
            {
                UnityEngine.Object.Destroy(chunk.Mesh);
                UnityEngine.Object.Destroy(chunk.GameObject);
            }

            var mesh = VoxelMesher.MeshGPU(chunk, terrianChunk);
            var origin = chunk.Origin;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            GameObject go = new GameObject("Mesh" + origin.ToString());
            go.transform.parent = transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = material;
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.transform.localPosition = chunk.Origin;

            chunk.Mesh = mesh;
            chunk.GameObject = go;

            done = true;
        }

        public bool IsDone()
        {
            return done;
        }
    }
}
