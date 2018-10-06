using UnityEngine.AI;

namespace FarmVox
{
    public static class NavMeshAreas
    {
        public static int Walkable
        {
            get
            {
                return NavMesh.GetAreaFromName("Walkable");
            }
        }

        public static int NotWalkable
        {
            get
            {
                return NavMesh.GetAreaFromName("Not Walkable");
            }
        }

        public static int Jump
        {
            get
            {
                return NavMesh.GetAreaFromName("Jump");
            }
        }
    }
}