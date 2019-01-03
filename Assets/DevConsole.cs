using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevConsole : MonoBehaviour
{
	private GUIStyle _style;
	
	// Use this for initialization
	private void Start () {
		_style = new GUIStyle {
			fontSize = 32, 
			normal =
			{
				textColor = Color.white,
				background = SolidTexture(1, 1, new Color(1.0f, 1.0f, 1.0f, 0.5f))
			}
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
			GUI.SetNextControlName(inputName);		
			_text = GUI.TextField(new Rect(0, 0, 400, 40), _text, _style);	
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
			_hidden = true;
			_text = "";
		}
	}
}
