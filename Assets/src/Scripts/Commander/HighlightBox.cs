using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{

    public class HighlightBox : MonoBehaviour
    {
        public bool hasStartCoord;
        public BoundsInt bounds = new BoundsInt();
        public Material material;

        public void Copy(HighlightBox box) {
            hasStartCoord = box.hasStartCoord;
            bounds = box.bounds;
        }

        public void AddCoord(Vector3Int coord)
        {
            if (!hasStartCoord) {
                bounds.min = coord;
                hasStartCoord = true;
            } else {
                bounds.max = coord;
            }
        }

        public void SetBounds(BoundsInt bounds) {
            this.bounds = bounds;
            this.hasStartCoord = true;
        }

        LineRenderer lineRenderer;

        static Material lassoMaterial;

        void Start() {
            if (lassoMaterial == null) {
                lassoMaterial = Resources.Load<Material>("commander/lasso");
            }

            gameObject.AddComponent<MeshFilter>().sharedMesh = Cube.CubeMesh;

            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.sharedMaterial = lassoMaterial;
            var width = 1;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.textureMode = LineTextureMode.RepeatPerSegment;
        }

        void Update()
        {
            if (!hasStartCoord) {
                lineRenderer.enabled = false;
                return;
            }

            lineRenderer.enabled = true;

            var mask = 1 << UserLayers.terrian;
            var positions = GetLassoCoordsForBox(mask).ToArray();
            lineRenderer.SetPositions(positions);
            lineRenderer.positionCount = positions.Length;
        }

        List<Vector3> GetLassoCoordsForBox(int mask) {
            var start = bounds.min;
            var end = bounds.max;
            var maxY = start.y > end.y ? start.y : end.y;

            var set = new List<Vector3>();

            GetLassoCoordsForLine(new Vector3Int(start.x, start.y, start.z), new Vector3Int(end.x, start.y, start.z), set, mask);
            GetLassoCoordsForLine(new Vector3Int(end.x, start.y, start.z), new Vector3Int(end.x, start.y, end.z), set, mask);
            GetLassoCoordsForLine(new Vector3Int(end.x, start.y, end.z), new Vector3Int(start.x, start.y, end.z), set, mask);
            GetLassoCoordsForLine(new Vector3Int(start.x, start.y, end.z), new Vector3Int(start.x, start.y, start.z), set, mask);

            return set;
        }

        void GetLassoCoordsForLine(Vector3Int start, Vector3Int end, List<Vector3> set, int mask) {
            var dis = (start - end).magnitude;
            var minDisBetween = 2.0f;
            var segments = Mathf.Floor(dis / minDisBetween);
            var disBetween = dis / segments;
            var dir = ((Vector3)(end - start)).normalized;

            var yOffset = 2;

            for (var i = 0; i < segments + 1; i ++) {
                var point = start + dir * i * disBetween;

                var result =
                    GetLassoPositionForPoint(point, yOffset, mask) ??
                    GetLassoPositionForPoint(point, yOffset * 2, mask) ??
                    GetLassoPositionForPoint(point, yOffset * 4, mask);

                if (result != null) {
                    set.Add(result.hit.point + new Vector3(0, 1.0f, 0));
                }
            }
        }

        RaycastResult GetLassoPositionForPoint(Vector3 point, float yOffset, int mask) {
            point += new Vector3(0.5f, 0, 0.5f);
            var ray = new Ray(point + new Vector3(0, yOffset, 0), Vector3.down);
            var result = VoxelRaycast.TraceRay(ray, mask);
            return result;
        }

        public void Clear() {
            hasStartCoord = false;
            bounds = new BoundsInt();
        }

		public void OnDestroy()
		{
            Destroy(gameObject);
		}
	}
}