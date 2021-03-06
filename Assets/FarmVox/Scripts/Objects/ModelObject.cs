using System.Collections.Generic;
using FarmVox.Models;
using FarmVox.Scripts.Models;
using UnityEngine;

namespace FarmVox.Objects
{
    public class ModelObject : IPlaceableObject
    {
        private readonly Model _model;
        private readonly Dictionary<Vector3Int, Model.Voxel> _map = new Dictionary<Vector3Int, Model.Voxel>();

        public ModelObject(Model model)
        {
            _model = model;
            foreach (var voxel in _model.voxels)
            {
                _map[new Vector3Int(voxel.x, voxel.y, voxel.z)] = voxel;
            }
        }

        public Vector3Int GetSize()
        {
            var size = _model.size;
            return new Vector3Int(size[0], size[1], size[2]);
        }

        public float GetValue(Vector3Int coord)
        {
            return _map.ContainsKey(coord) ? 1 : 0;
        }

        public Color GetColor(Vector3Int coord)
        {
            return _map.TryGetValue(coord, out var voxel) ? _model.palette[voxel.colorIndex] : default;
        }
    }
}