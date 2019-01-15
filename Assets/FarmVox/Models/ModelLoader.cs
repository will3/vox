using System.IO;
using UnityEngine;

namespace FarmVox.Models
{
    public static class ModelLoader
    {
        public static Model Load(string name)
        {
            var path = Path.Combine("models", name);
            var asset = Resources.Load<TextAsset>(path);
            var json = asset.text;

            return JsonUtility.FromJson<Model>(json);
        }
    }
}