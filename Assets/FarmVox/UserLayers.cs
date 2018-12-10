using UnityEngine;

namespace FarmVox
{
    public static class UserLayers {
        public static int Terrian {
            get {
                var mask = LayerMask.NameToLayer("terrian");
                return mask;
            }
        }

        public static int Trees
        {
            get
            {
                var mask = LayerMask.NameToLayer("trees");
                return mask;
            }
        }

        public static int Water
        {
            get
            {
                var mask = LayerMask.NameToLayer("water");
                return mask;
            }
        }
    }
}