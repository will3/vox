using FarmVox.Models;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class PivotedModel : MonoBehaviour
    {
        public string ModelName;
        
        private GameObject _pivotObject;
        private ModelScript _model;
        
        private void Start()
        {
            _pivotObject = new GameObject();
            _pivotObject.transform.parent = transform;

            _model = _pivotObject.AddComponent<ModelScript>();
            _model.ModelName = ModelName;
            _model.LoadModel();

            var pivot = _model.Model.CalcPivot();
            _pivotObject.transform.localPosition = pivot;
        }
    }
}