using System.Collections.Generic;
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

        string currentText = "";

		void OnGUI()
		{
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            var height = 30;

            currentText = GUI.TextField(new Rect(0, screenHeight - height, screenWidth, height), currentText);

            for (var i = 0; i < lines.Count; i++) {
                GUI.Label(new Rect(0, screenHeight - height - height * i, screenWidth, height), currentText, lines[i]);
            }

            if (GUI.changed) {
                if (Event.current.keyCode == KeyCode.Return)
                {
                    ProcessCommand(currentText);
                    AddLine(currentText);
                    currentText = "";
                }
            }
		}

        void ProcessCommand(string text) {
            var args = text.Split(new [] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
            var command = args[0];

            if (command == "b") {
                BuildCommand(args);
            }
        }

        void BuildCommand(string[] args) {
            if (args.Length > 2) {
                AddLine("usage: b [object]");
            }
            if (args[1] == "house") {
                
            }
        }

        readonly List<string> lines = new List<string>();

        void AddLine(string line) {
            lines.Add(line);    
        }
	}
}