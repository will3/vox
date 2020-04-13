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
            if (Input.GetKeyUp(KeyCode.Return))
            {
                if (!_showingConsole)
                {
                    _showingConsole = true;
                }
            }
        }

        private void OnGUI()
        {
            if (!_showingConsole)
            {
                return;
            }

            var enterPressed = Event.current.Equals(Event.KeyboardEvent("return"));
            if (enterPressed)
            {
                var args = _text.Split(' ').Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();

                if (args.Length > 0)
                {
                    var command = GetComponents<ICommand>().SingleOrDefault(x =>
                        string.Equals(x.Name, args[0], StringComparison.OrdinalIgnoreCase));
                    command?.Run(args);
                }

                _text = "";
                _showingConsole = false;
            }

            const int textFieldLeftPadding = 8;

            ApplyBoxBackgroundColorIfNeeded();
            GUI.BeginGroup(new Rect(gap, Screen.height - height - gap, width, height));

            GUI.Box(new Rect(0, 0, width, height), "", boxStyle);

            var textFieldRect = new Rect(textFieldLeftPadding, 0, width - textFieldLeftPadding * 2, height);
            GUI.SetNextControlName("input");
            _text = GUI.TextField(textFieldRect, _text, textFieldStyle);

            GUI.FocusControl("input");

            GUI.EndGroup();
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