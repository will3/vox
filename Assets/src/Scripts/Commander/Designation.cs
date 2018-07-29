using UnityEngine;

namespace FarmVox
{
    class Designation
    {
        readonly Vector3Int coord;

        public Designation(Vector3Int coord)
        {
            this.coord = coord;
        }
        GameObject gameObject;

        public void AddObject(Transform transform, Material material)
        {
            if (gameObject != null)
            {
                return;
            }
            gameObject = new GameObject("des," + coord.ToString());
            gameObject.transform.parent = transform;
            gameObject.AddComponent<MeshRenderer>().material = material;
            gameObject.AddComponent<MeshFilter>().mesh = Cube.CubeMesh;
            var padding = 0.01f;
            gameObject.transform.localScale += new Vector3(padding * 2, padding * 2, padding * 2);
            gameObject.transform.position = coord - new Vector3(padding, padding, padding);
        }
    }
}