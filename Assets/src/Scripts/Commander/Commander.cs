using UnityEngine;

namespace FarmVox
{
    public class Commander : MonoBehaviour
    {
        public Material lassoMaterial;
        public Material boxMaterial;

        GameObject boxObject;

        Command currentCommand;

        static Commander instance;

        public static Commander Instance {
            get {
                return instance;
            }
        }

		void Update()
        {
            instance = this;

            if (Input.GetKeyDown(KeyCode.G)) {
                currentCommand = new DigCommand();
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                currentCommand = new StorageCommand();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                currentCommand = new BuildWallCommand();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape)) {
                currentCommand = null;
            }

            ProcessCommand();
        }

        void ProcessCommand() {
            if (currentCommand == null) {
                return;
            }

            currentCommand.commander = this;

            if (currentCommand.Update()) {
                currentCommand = null;
            }
        }
    }
}