using UnityEngine;

namespace FarmVox.Voxel
{
    public class VoxelRaycastResult
    {
        private readonly RaycastHit _hit;

        public VoxelRaycastResult(RaycastHit hit) {
            _hit = hit;
        }

        public Vector3Int GetCoord()
        {
            var point = _hit.point - _hit.normal * 0.1f;
            return new Vector3Int(Mathf.FloorToInt(point.x),
                                  Mathf.FloorToInt(point.y),
                                  Mathf.FloorToInt(point.z));
        }

        public Color GetColor(Chunks chunks) {
            return chunks.GetColor(GetCoord());
        }

        public Mesh GetQuad()
        {
            var d = Axis.X;
            var front = false;
            if (Mathf.Abs(_hit.normal.x) > Mathf.Epsilon)
            {
                d = Axis.X;
                front = _hit.normal.x > 0;
            }
            else if (Mathf.Abs(_hit.normal.y) > Mathf.Epsilon)
            {
                d = Axis.Y;
                front = _hit.normal.y > 0;
            }
            else if (Mathf.Abs(_hit.normal.z) > Mathf.Epsilon)
            {
                d = Axis.Z;
                front = _hit.normal.z > 0;
            }

            return new CubeMeshBuilder().AddQuad(d, front, new Vector3()).Build();
        }
    }
}