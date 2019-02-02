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
            if (Model == null)
            {
                Model = ModelLoader.Load(ModelName);    
            }
        }
        
        private void Start()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = Materials.GetVoxelMaterialModel();
            
            Model.InitData();

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
                var size = Terrian.Instance.Config.Size;
                var mesh = new MeshBuilder(size).AddTriangles(triangles).Build();
                _meshFilter.mesh = mesh;
            }
        }
    }
}