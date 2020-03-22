using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FarmVox.Scripts
{
    public static class ObjectFinder
    {
        public static T InjectSingleInstance<T>(this MonoBehaviour _, T existing = null) where T : Object
        {
            if (existing != null)
            {
                return existing;
            }

            var objs = Object.FindObjectsOfType<T>();
            if (objs.Length == 0)
            {
                throw new Exception($"Cannot find object of type {typeof(T)}");
            }

            if (objs.Length > 1)
            {
                throw new Exception($"More than one object of type {typeof(T)} found");
            }

            return objs[0];
        }
    }
}