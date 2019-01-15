using FarmVox.GPU.Shaders;
using FarmVox.Models;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class ModelMesh : MonoBehaviour
    {
        public string Model;
        private Model _model;
        
        private void Start()
        {
            _model = ModelLoader.Load(Model);

            var mesher = new MesherGpu(32, 0.1f);
            
            var mesh = mesher.Dispatch();
        }
    }
}