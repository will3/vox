using System;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class BuildingTile : MonoBehaviour
    {
        public Vector3Int[] coords;
        public BoundsInt bounds;
        public MeshRenderer meshRenderer;
        public Color baseColor = new Color(1, 1, 1);
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
    }
}