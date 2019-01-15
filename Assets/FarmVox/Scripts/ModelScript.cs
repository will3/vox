using FarmVox.GPU.Shaders;
using FarmVox.Models;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class ModelScript : MonoBehaviour
    {
        public string Model;
        private Model _model;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        
        private void Start()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = Materials.GetVoxelMaterialModel();
            
            _model = ModelLoader.Load(Model);

            _model.InitData();
            
            Debug.Log(JsonUtility.ToJson(_model));

            var mesherSettings = new MesherSettings
            {
                AoStrength = 0.4f
            };
            
            using (var mesher = new MesherGpu(32, mesherSettings))
            {
                mesher.SetColors(_model.Colors);
                mesher.SetData(_model.Data);
                mesher.Dispatch();
                var triangles = mesher.ReadTriangles();
                var mesh = new MeshBuilder().AddTriangles(triangles).Build();
                _meshFilter.mesh = mesh;
            }
        }
    }
}