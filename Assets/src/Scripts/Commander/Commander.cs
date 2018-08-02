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

        Texture2D blankTexture;

        public static Commander Instance {
            get {
                return instance;
            }
        }

		private void Awake()
		{
            blankTexture = Resources.Load<Texture2D>("blank");
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

            if (Input.GetKeyUp(KeyCode.Escape)) {
                showDebugWindow = true;
                focusNextInput = true;
            }
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
        bool showDebugWindow = false;
        bool focusNextInput = false;

		void OnGUI()
		{
            if (showDebugWindow) {
                var screenWidth = Screen.width;
                var screenHeight = Screen.height;
                var height = 20;

                var style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.normal.background = blankTexture;
                style.focused = style.normal;
                // style.normal.background = null;

                var maxLines = 4;

                var leftPadding = 4;
                var topPadding = 8;

                GUI.BeginGroup(new Rect(leftPadding, 0, screenWidth, height * maxLines + topPadding));



                //GUI.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.8f);
                //GUI.Box(new Rect(0, 0, screenWidth, height * maxLines), "", style);

                //GUI.backgroundColor = Color.clear;
                GUI.SetNextControlName("input");
                currentText = GUI.TextField(new Rect(leftPadding, topPadding, screenWidth - leftPadding - leftPadding, height), currentText, style);
                if (focusNextInput) {
                    GUI.FocusControl("input");
                    focusNextInput = false;
                }

                for (var i = 0; i < lines.Count; i++)
                {
                    var content = lines[i];
                    GUI.Label(new Rect(leftPadding, height * (lines.Count - i) + topPadding, screenWidth - leftPadding - leftPadding, height), content, style);
                }

                if (Event.current.keyCode == KeyCode.Return)
                {
                    if (currentText != null && currentText.Length > 0)
                    {
                        ProcessCommand(currentText);
                        AddLine(currentText);
                        currentText = "";
                    }
                }

                GUI.EndGroup();
            }
		}

        void ProcessCommand(string text) {
     
            var args = text.Split(new [] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
            var command = args[0];

            if (command == "b") {
                BuildCommand(args);
            } else if (command == "exit") {
                showDebugWindow = false;
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