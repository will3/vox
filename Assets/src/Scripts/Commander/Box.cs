using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class Box : MonoBehaviour
    {
        public Vector3Int? startCoord;
        public Vector3Int? endCoord;
        public Material material;
        public bool showBox;
        public Material lassoMaterial;

        public void Copy(Box box) {
            startCoord = box.startCoord;
            endCoord = box.endCoord;
            material = box.material;
            showBox = box.showBox;
            lassoMaterial = box.lassoMaterial;
        }

        public Vector3Int Min
        {
            get
            {
                var minx = startCoord.Value.x < endCoord.Value.x ? startCoord.Value.x : endCoord.Value.x;
                var miny = startCoord.Value.y < endCoord.Value.y ? startCoord.Value.y : endCoord.Value.y;
                var minz = startCoord.Value.z < endCoord.Value.z ? startCoord.Value.z : endCoord.Value.z;

                return new Vector3Int(minx, miny, minz);
            }
        }

        public Vector3Int Max
        {
            get
            {
                var maxx = startCoord.Value.x > endCoord.Value.x ? startCoord.Value.x : endCoord.Value.x;
                var maxy = startCoord.Value.y > endCoord.Value.y ? startCoord.Value.y : endCoord.Value.y;
                var maxz = startCoord.Value.z > endCoord.Value.z ? startCoord.Value.z : endCoord.Value.z;

                return new Vector3Int(maxx, maxy, maxz);
            }
        }

        public Vector3Int Size
        {
            get
            {
                return Max - Min;
            }
        }

        public Bounds Bounds {
            get {
                return new Bounds(Min, Size);
            }
        }

        public void AddCoord(Vector3Int coord)
        {
            if (startCoord == null)
            {
                startCoord = coord;
            }
            else
            {
                endCoord = coord;
            }
        }

        LineRenderer lineRenderer;
        MeshRenderer meshRenderer;

        void Start() {
            gameObject.AddComponent<MeshFilter>().sharedMesh = Cube.CubeMesh;
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.sharedMaterial = lassoMaterial;
            var width = 1;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.textureMode = LineTextureMode.RepeatPerSegment;
        }

        void Update()
        {
            if (startCoord == null || endCoord == null) {
                meshRenderer.enabled = false;
                lineRenderer.enabled = false;
                return;
            }

            if (showBox) {
                meshRenderer.enabled = true;
                meshRenderer.sharedMaterial = material;
                float paddingAmount = 0.1f;
                var padding = new Vector3(paddingAmount, paddingAmount, paddingAmount);
                gameObject.transform.position = Min;
                gameObject.transform.position -= padding;
                gameObject.transform.localScale = Size + padding * 2;    
            }

            lineRenderer.enabled = true;
            lineRenderer.SetPositions(new Vector3[] {
                new Vector3(0, 0, 0),
                new Vector3(100, 100, 100)
            });

            var mask = 1 << UserLayers.terrian;
            var positions = GetLassoCoordsForBox(startCoord.Value, endCoord.Value, mask).ToArray();
            lineRenderer.SetPositions(positions);
            lineRenderer.positionCount = positions.Length;
        }

        List<Vector3> GetLassoCoordsForBox(Vector3Int start, Vector3Int end, int mask) {
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
            startCoord = null;
            endCoord = null;
        }

		public void OnDestroy()
		{
            Destroy(gameObject);
		}
	}
}