using System;
using UnityEngine;

namespace FarmVox.Scripts
{
    public static class Logger
    {
        public static void LogComponentNotFound(Type type)
        {
            Debug.LogError($"Cannot find component of type {type}");
        } 
    }
}