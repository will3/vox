using System;
using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class DebugConsole : MonoBehaviour
    {
        public float width = 200;
        public float height = 40;
        public float gap = 12;
        private string _text = "";
        public GUIStyle boxStyle;
        public GUIStyle textFieldStyle;
        private bool _hasAppliedBoxBackgroundColor;
        private bool _showingConsole;
        private string _output = "";

        private void ApplyBoxBackgroundColorIfNeeded()
        {
            if (_hasAppliedBoxBackgroundColor)
            {
                return;
            }

            boxStyle.normal.background = MakeTexture(2, 2, new Color(1f, 1f, 1f, 0.2f));
            _hasAppliedBoxBackgroundColor = false;
        }

        private void Update()
        {
            if (!Input.GetKeyUp(KeyCode.Return)) return;
            if (!_showingConsole)
            {
                _showingConsole = true;
            }
        }

        private void OnGUI()
        {
            if (!_showingConsole)
            {
                return;
            }

            ProcessEnter();

            const int textFieldLeftPadding = 8;

            ApplyBoxBackgroundColorIfNeeded();
            GUI.BeginGroup(new Rect(gap, Screen.height - height - gap, width, height));

            GUI.Box(new Rect(0, 0, width, height), "", boxStyle);

            var textFieldRect = new Rect(textFieldLeftPadding, 0, width - textFieldLeftPadding * 2, height);
            GUI.SetNextControlName("input");

            if (_output.Length == 0)
            {
                _text = GUI.TextField(textFieldRect, _text, textFieldStyle);
            }
            else
            {
                GUI.Label(textFieldRect, _output, textFieldStyle);
            }

            GUI.FocusControl("input");

            GUI.EndGroup();
        }

        private void ProcessEnter()
        {
            var enterPressed = Event.current.Equals(Event.KeyboardEvent("return"));
            if (!enterPressed)
            {
                return;
            }

            if (_output.Length > 0)
            {
                _output = "";
                _showingConsole = false;
                return;
            }

            var args = _text.Split(' ').Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();

            if (args.Length == 0)
            {
                _text = "";
                _showingConsole = false;
                return;
            }

            var command = CommandManager.Instance.Get(args[0]);
            if (command == null)
            {
                _showingConsole = false;
                _text = "";
                return;
            }

            _output = command.Run(args);

            if (_output.Length == 0)
            {
                _showingConsole = false;
            }

            _text = "";
        }

        private static Texture2D MakeTexture(int w, int h, Color col)
        {
            var pix = new Color[w * h];
            for (var i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }

            var result = new Texture2D(w, h);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}