using UnityEngine;

namespace FarmVox
{
    public class Commander : MonoBehaviour
    {
        public Material lassoMaterial;
        public Material boxMaterial;

        GameObject boxObject;
        Box box;

        Command currentCommand;

        void Start()
		{
            boxObject = new GameObject("box");
            boxObject.transform.parent = transform;
            box = boxObject.AddComponent<Box>();
            box.material = boxMaterial;
            box.lassoMaterial = lassoMaterial;
		}

		void Update()
        {
            if (Input.GetKeyDown(KeyCode.G)) {
                currentCommand = new DigCommand();
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

            currentCommand.box = box;
            currentCommand.transform = transform;

            currentCommand.Update();

            if (currentCommand.done) {
                currentCommand = null;
            }
        }
    }
}