using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class HighlightLine : MonoBehaviour
    {
        public List<Vector3> points = new List<Vector3>();

        public void AddPoint(Vector3 point) {
            float minDis = 4.0f;
            if (points.Count == 0) {
                points.Add(point);
            }
            var lastPoint = points[points.Count - 1];
            var dis = (lastPoint - point).magnitude;
            if (dis < minDis) {
                return;
            }
            points.Add(point);
        }


        LineRenderer lineRenderer;

        static Material lassoMaterial;

        void Start()
        {
            if (lassoMaterial == null)
            {
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
    }
}