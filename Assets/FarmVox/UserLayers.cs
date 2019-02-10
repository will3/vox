using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public static class UserLayers {
        public static int Terrian {
            get {
                return NameToLayer("terrian");
            }
        }

        public static int Trees
        {
            get
            {
                return NameToLayer("trees");
            }
        }

        public static int Water
        {
            get
            {
                return NameToLayer("water");
            }
        }

        public static int Wall
        {
            get
            {
                return NameToLayer("wall");
            }
        }

        private static readonly Dictionary<string, int> _layers = new Dictionary<string, int>();
        
        private static int NameToLayer(string name)
        {
            int layer;
            if (_layers.TryGetValue(name, out layer))
            {
                return layer;
            }
            
            layer = LayerMask.NameToLayer(name);
            _layers[name] = layer;
            return layer;
        }
    }
}