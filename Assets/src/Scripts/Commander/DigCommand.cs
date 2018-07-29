using UnityEngine;

namespace FarmVox
{
    public class DigCommand : Command
    {
        public override void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                var result = VoxelRaycast.TraceMouse(1 << UserLayers.terrian);
                if (result != null)
                {
                    box.AddCoord(result.GetCoord());
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                var designationObject = new GameObject("designation");
                designationObject.transform.parent = transform;
                var designation = designationObject.AddComponent<Designation>();
                designation.start = box.Min;
                designation.end = box.Max;
                designation.type = DesignationType.Dig;

                var designationBox = designationObject.AddComponent<Box>();
                designationBox.Copy(box);

                box.Clear();

                done = true;
            }
        }
    }
}