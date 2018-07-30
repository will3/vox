using UnityEngine;

namespace FarmVox
{
    public interface IInventory {
        float GetWeight();
    }

    public class Block : IInventory
    {
        public float GetWeight()
        {
            return 1;
        }
    }

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