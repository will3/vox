using UnityEngine;

namespace FarmVox
{
    public class VoxelRaycastResult
    {
        public readonly RaycastHit hit;

        public VoxelRaycastResult(RaycastHit hit) {
            this.hit = hit;
        }

        public Vector3Int GetCoord()
        {
            var point = hit.point - hit.normal * 0.1f;
            return new Vector3Int(Mathf.FloorToInt(point.x),
                                  Mathf.FloorToInt(point.y),
                                  Mathf.FloorToInt(point.z));
        }

        public Color GetColor(Chunks chunks) {
            return chunks.GetColor(GetCoord());
        }

        public Mesh GetQuad()
        {
            int d = 0;
            var front = false;
            if (Mathf.Abs(hit.normal.x) > Mathf.Epsilon)
            {
                d = 0;
                front = hit.normal.x > 0;
            }
            else if (Mathf.Abs(hit.normal.y) > Mathf.Epsilon)
            {
                d = 1;
                front = hit.normal.y > 0;
            }
            else if (Mathf.Abs(hit.normal.z) > Mathf.Epsilon)
            {
                d = 2;
                front = hit.normal.z > 0;
            }

            return Cube.GetQuad(d, front);
        }

        public int FindNoneDefaultLayer()
        {
            if (hit.collider == null) {
                return 0;
            }
            return FindNoneDefaultLayer(hit.collider.gameObject);
        }

        int FindNoneDefaultLayer(GameObject gameObject) {
            if (gameObject.layer != 0)
            {
                return gameObject.layer;
            }

            var parentTransform = gameObject.transform.parent;
            if (parentTransform != null)
            {
                return FindNoneDefaultLayer(parentTransform.gameObject);
            }

            return 0;
        }
    }
}