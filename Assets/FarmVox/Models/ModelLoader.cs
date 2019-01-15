using System.IO;
using UnityEngine;

namespace FarmVox.Models
{
    public static class ModelLoader
    {
        public static Model Load(string name)
        {
            var json = Resources.Load<TextAsset>(Path.Combine("models", name)).text;

            return JsonUtility.FromJson<Model>(json);
        }
    }
}