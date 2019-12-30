using UnityEngine;

namespace FarmVox.Objects
{
    public interface IPlaceableObject
    {
        Vector3Int GetSize();
        float GetValue(Vector3Int coord);
        Color GetColor(Vector3Int coord);
    }
}