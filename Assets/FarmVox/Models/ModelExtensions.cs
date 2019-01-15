using UnityEngine;

namespace FarmVox.Models
{
    public static class ModelExtensions
    {
        public static Vector3 CalcPivot(this Model model)
        {
            var sum = new Vector3();
            foreach (var voxel in model.Voxels)
            {
                sum += new Vector3(voxel.X, voxel.Y, voxel.Z);
            }

            sum /= model.Voxels.Length;
            sum += new Vector3(0.5f, 0.5f, 0.5f);

            return -sum;
        } 
    }
}