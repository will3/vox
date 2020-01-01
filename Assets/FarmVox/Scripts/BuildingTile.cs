using System;
using System.Linq;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class BuildingTile : MonoBehaviour
    {
        public Vector3Int[] coords;
        public BoundsInt bounds;
        public MeshRenderer meshRenderer;
        public Color baseColor = new Color(1, 1, 1);
        public bool hasBuilding = false;
        
        private static readonly int Color = Shader.PropertyToID("_Color");

        public void SetHighlightAmount(float amount)
        {
            if (Mathf.Abs(amount) < Mathf.Epsilon)
            {
                meshRenderer.enabled = false;
                return;
            }

            meshRenderer.enabled = true;
            var color = baseColor;
            color.a = amount;
            meshRenderer.material.SetColor(Color, color);
        }

        public float CalcDistance(Vector3 position)
        {
            return (position - bounds.center).magnitude;
        }

        public Vector3Int GetBuildingCoord()
        {
            var minY = coords.Min(c => c.y);
            return new Vector3Int(bounds.min.x, minY + 1, bounds.min.z);
        }

        public Vector2 GetCenterXz()
        {
            return bounds.center.GetXz();
        }
    }
}