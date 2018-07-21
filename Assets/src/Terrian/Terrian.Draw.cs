using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        public void Draw(Chunks chunks, Vector3Int origin, Transform transform, Material material, TerrianChunk terrianChunk)
        {
            if (!chunks.HasChunk(origin))
            {
                return;
            }

            var chunk = chunks.GetChunk(origin);

            if (!chunk.Dirty)
            {
                return;
            }

            if (chunk.Mesh != null)
            {
                UnityEngine.Object.Destroy(chunk.Mesh);
                UnityEngine.Object.Destroy(chunk.GameObject);
            }

            var mesh = VoxelMesher.MeshGPU(chunk, terrianChunk);

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
            chunk.Dirty = false;
        }
    }
}
