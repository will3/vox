using UnityEngine;

namespace FarmVox
{
    public class StorageCommand : Command { 
        public override bool Update()
        {
            if (DragBox())
            {
                RemoveBox();

                var designation = new StorageDesignation(box.bounds);
                designation.Start();

                DesignationMap.Instance.AddDesignation(designation);
                return true;
            }

            return false;
        }
    }
}