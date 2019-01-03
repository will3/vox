using System;
using System.Collections.Generic;
using System.Linq;
using FarmVox.Scripts.Commands;
using UnityEngine;

namespace FarmVox.Scripts
{
	public class DevConsole : MonoBehaviour
	{
		public GUISkin Skin;

		public static DevConsole Instance;
		
		private List<IDevCommand> _commands = new List<IDevCommand>();

		private void Awake()
		{
			Instance = this;
		}

		// Use this for initialization
		private void Start () {
			_commands = new List<IDevCommand>
			{
				new SaveCommand(),
				new ListCommand(),
				new LoadCommand(),
				new RemoveCommand(),
				new ReloadCommand()
			};
		}
	
		// Update is called once per frame
		private void Update () {
		
		}

		private string _text = "";
		private bool _hidden = true;

		private Texture2D SolidTexture(int width, int height, Color color)
		{
			var pix = new Color[width * height];
			for (var i = 0; i < pix.Length; ++i)
			{
				pix[i] = color;
			}

			var result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();
			return result;
		}

		public float LineHeight = 40;
		public float TotalLines = 10;
		public RectOffset ConsoleMargin;
		public RectOffset ConsolePadding;
		
		private List<string> lines = new List<string>();

		private Rect GetLineRect(int index)
		{
			var consoleRect = GetConsoleRect();
			return new Rect(
				consoleRect.xMin + ConsolePadding.left,
				consoleRect.yMax - LineHeight * (index + 1) - ConsolePadding.bottom,
				consoleRect.width - ConsolePadding.left - ConsolePadding.right,
				LineHeight);
		}

		private Rect GetConsoleRect()
		{
			var height = LineHeight * TotalLines + ConsolePadding.top + ConsolePadding.bottom;
			return new Rect(
				ConsoleMargin.left, 
				Screen.height - height - ConsoleMargin.bottom, 
				Screen.width - ConsoleMargin.left - ConsoleMargin.right, 
				height);
		}
		
		private void OnGUI()
		{
			const string inputName = "input";

			var enter = Event.current.Equals(Event.KeyboardEvent("return"));
			var escape = Event.current.Equals(Event.KeyboardEvent("`"));

			_text = _text.Replace("`", "");

			if (escape)
			{
				_hidden = !_hidden;
			}
		
			if (!_hidden)
			{
				GUI.Box(GetConsoleRect(), (string) null, Skin.box);
				GUI.SetNextControlName(inputName);		
				_text = GUI.TextField(GetLineRect(0), _text, Skin.textField);

				for (var i = 1; i < TotalLines; i++)
				{
					var lineIndex = lines.Count - i;
					if (lineIndex < 0)
					{
						continue;
					}
					var lineText = lines[lineIndex];
					GUI.Label(GetLineRect(i), lineText, Skin.textField);
				}
			}

			if (escape)
			{
				if (_hidden)
				{
					_text = "";
				}
				else
				{
					GUI.FocusControl(inputName);
				}	
			}

			if (enter)
			{
				AddLine(_text);
				ProcessCommand(_text);	
				_text = "";
			}
		}

		private void ProcessCommand(string input)
		{
			if (input.Length == 0)
			{
				return;
			}
			
			var args = input.Split(' ');
			var commandName = args[0];

			var matched = false;
			foreach (var command in _commands)
			{
				if (command.Names.Contains(commandName))
				{
					try
					{
						command.Execute(args);
					}
					catch (Exception e)
					{
						AddLine(e.Message);
					}

					matched = true;
					break;
				}
			}

			if (!matched)
			{
				AddLine("Unknown command " + commandName);
			}
		}

		public void AddLine(string line)
		{
			lines.Add(line);
		}
	}
}
