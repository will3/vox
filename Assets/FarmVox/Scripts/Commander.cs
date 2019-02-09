using UnityEngine;

namespace FarmVox.Scripts
{
    public class Commander : MonoBehaviour
    {
        private HighlightHoveredSurface _highlight;

        // Use this for initialization
        private void Start()
        {
            var go = new GameObject();
            go.AddComponent<HighlightBuildingGrid>();
        }
    }
}