using System;
using UnityEditor;
using UnityEngine;

namespace IconDownloader.Editor
{
	public static class EditorGuiLayoutHelpers
	{
		public static void DrawHorizontalLine() => EditorGUILayout
			.LabelField("", GUI.skin.horizontalSlider);

		public static void DrawTexture(Texture2D texture, int height)
		{
			if (texture == null)
			{
				return;
			}
			
			GUI.DrawTexture(
				EditorGUILayout.GetControlRect(false, height, GUILayout.Width(height)),
				texture,
				ScaleMode.ScaleToFit);
		}

		public static void DrawHorizontallyCentered(Action drawElements)
		{
	        EditorGUILayout.BeginHorizontal();
	        GUILayout.FlexibleSpace();
	        drawElements?.Invoke();
	        GUILayout.FlexibleSpace();
	        EditorGUILayout.EndHorizontal();
		}

		public static void DrawCenteredLabel(string label)
		{
	        var centeredStyle = GUI.skin.GetStyle("Label");
	        centeredStyle.alignment = TextAnchor.MiddleCenter;
	        EditorGUILayout.LabelField(label, centeredStyle);
		}
	}
}