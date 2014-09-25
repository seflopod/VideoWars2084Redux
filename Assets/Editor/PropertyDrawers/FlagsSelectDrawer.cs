using UnityEngine;
using UnityEditor;

namespace VideoWars.Utility.Attributes
{
	[CustomPropertyDrawer(typeof(FlagsSelectAttribute))]
	public class FlagsSelectDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent lbl)
		{
			prop.intValue = EditorGUI.MaskField( pos, new GUIContent(flagsSelectAttribute.name), prop.intValue, prop.enumNames );
		}

		//property for easier internal access to the attribute
		private FlagsSelectAttribute flagsSelectAttribute { get { return (FlagsSelectAttribute)attribute; } }
	}
}
