using FarmVox.Scripts.Models;
using UnityEngine;

namespace FarmVox.Models
{
    public static class ModelExtensions
    {
        public static Vector3 CalcPivot(this Model model)
        {
            var sum = new Vector3();
            foreach (var voxel in model.voxels)
            {
                sum += new Vector3(voxel.x, voxel.y, voxel.z);
            }

            sum /= model.voxels.Length;
            sum += new Vector3(0.5f, 0.5f, 0.5f);

            return -sum;
        } 
    }
}