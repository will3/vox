using UnityEngine;

namespace FarmVox
{
    public static class UserLayers {
        public static int terrian {
            get {
                var mask = LayerMask.NameToLayer("terrian");
                return mask;
            }
        }

        public static int trees
        {
            get
            {
                var mask = LayerMask.NameToLayer("trees");
                return mask;
            }
        }

        public static int water
        {
            get
            {
                var mask = LayerMask.NameToLayer("water");
                return mask;
            }
        }
    }
}