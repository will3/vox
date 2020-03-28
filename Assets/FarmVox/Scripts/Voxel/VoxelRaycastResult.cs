using FarmVox.Scripts.Voxel;
using UnityEngine;

namespace FarmVox.Voxel
{
    public class VoxelRaycastResult
    {
        public readonly RaycastHit Hit;

        public VoxelRaycastResult(RaycastHit hit) {
            Hit = hit;
        }

        public Vector3Int GetCoord()
        {
            var point = Hit.point - Hit.normal * 0.1f;
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
            if (Mathf.Abs(Hit.normal.x) > Mathf.Epsilon)
            {
                d = Axis.X;
                front = Hit.normal.x > 0;
            }
            else if (Mathf.Abs(Hit.normal.y) > Mathf.Epsilon)
            {
                d = Axis.Y;
                front = Hit.normal.y > 0;
            }
            else if (Mathf.Abs(Hit.normal.z) > Mathf.Epsilon)
            {
                d = Axis.Z;
                front = Hit.normal.z > 0;
            }

            return new CubeMeshBuilder().AddQuad(d, front, new Vector3()).Build();
        }
    }
}