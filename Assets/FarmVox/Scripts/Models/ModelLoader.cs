using FarmVox.Scripts.Models;
using UnityEngine;

namespace FarmVox.Models
{
    public static class ModelLoader
    {
        public static Model Load(TextAsset text)
        {
            var json = text.text;
            return JsonUtility.FromJson<Model>(json);
        }
    }
}