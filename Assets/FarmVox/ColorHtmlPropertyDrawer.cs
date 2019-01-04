using UnityEditor;
using UnityEngine;

namespace FarmVox
{
    [CustomPropertyDrawer(typeof(ColorHtmlPropertyAttribute))]
    public class ColorHtmlPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var htmlField = new Rect(position.x, position.y, position.width - 100, position.height);
            var colorField = new Rect(position.x + htmlField.width, position.y, position.width - htmlField.width, position.height);

            var htmlValue = EditorGUI.TextField(htmlField, label, "#" + ColorUtility.ToHtmlStringRGBA(property.colorValue));

            Color newCol;
            if (ColorUtility.TryParseHtmlString(htmlValue, out newCol))
                property.colorValue = newCol;

            property.colorValue = EditorGUI.ColorField(colorField, property.colorValue);
        }
    }
}