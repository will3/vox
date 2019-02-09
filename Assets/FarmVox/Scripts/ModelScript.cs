using FarmVox.GPU.Shaders;
using FarmVox.Models;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class ModelScript : MonoBehaviour
    {
        public string ModelName;
        public Model Model { get; private set; }
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public void LoadModel()
        {
            Model = ModelLoader.Load(ModelName);
            Model.InitData();
        }
        
        private void Start()
        {
            LoadModel();
            
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = Materials.GetVoxelMaterialModel();

            var mesherSettings = new MesherSettings
            {
                AoStrength = 0.4f
            };
            
            using (var mesher = new MesherGpu(32, mesherSettings))
            {
                mesher.SetColors(Model.Colors);
                mesher.SetData(Model.Data);
                mesher.Dispatch();
                var triangles = mesher.ReadTriangles();
                var meshResult = new MeshBuilder().AddTriangles(triangles).Build();
                _meshFilter.mesh = meshResult.Mesh;
            }
        }
    }
}