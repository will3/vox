using System.Collections.Generic;

namespace FarmVox
{
    class DesignationMap
    {
        static DesignationMap instance;

        public static DesignationMap Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DesignationMap();
                }
                return instance;
            }
        }

        HashSet<Designation> designations = new HashSet<Designation>();

        public void AddDesignation(Designation designation)
        {
            designations.Add(designation);
        }
    }
}