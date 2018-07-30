using UnityEngine;

namespace FarmVox
{
    public class DigCommand : Command
    {
        GameObject designationObject;
        public override bool Update()
        {
            if (DragBox()) {
                RemoveBox();

                var designation = new DigDesignation(box.bounds);
                designation.Start();

                DesignationMap.Instance.AddDesignation(designation);
                return true;
            }

            return false;
        }
    }
}