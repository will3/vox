using UnityEngine;

namespace FarmVox
{
    public class RaycastResult
    {
        public Vector3 HitPos;
        public Vector3 HitNormal;

        public Vector3Int GetCoord()
        {
            var point = HitPos - HitNormal * 0.5f;
            return new Vector3Int(Mathf.FloorToInt(point.x),
                                  Mathf.FloorToInt(point.y),
                                  Mathf.FloorToInt(point.z));
        }

        public Mesh GetQuad()
        {
            int d = 0;
            var front = false;
            if (Mathf.Abs(HitNormal.x) > Mathf.Epsilon)
            {
                d = 0;
                front = HitNormal.x > 0;
            }
            else if (Mathf.Abs(HitNormal.y) > Mathf.Epsilon)
            {
                d = 1;
                front = HitNormal.y > 0;
            }
            else if (Mathf.Abs(HitNormal.z) > Mathf.Epsilon)
            {
                d = 2;
                front = HitNormal.z > 0;
            }

            return Cube.GetQuad(d, front);
        }
    }
}