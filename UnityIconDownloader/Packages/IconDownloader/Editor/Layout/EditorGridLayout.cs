using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IconDownloader.Editor.Layout
{
	internal class EditorGridLayout
	{
		private readonly int columnCount;
		private readonly int rowSpacing;
		private readonly int columnSpacing;
		private readonly Queue<Action> drawElementActions;

		public EditorGridLayout(
			int columnCount,
			int rowSpacing = 5,
			int columnSpacing = 5)
		{
			this.columnCount = columnCount;
			this.rowSpacing = rowSpacing;
			this.columnSpacing = columnSpacing;
			this.drawElementActions = new Queue<Action>();
		}

		public void AddElement(Action drawElement)
		{
			this.drawElementActions.Enqueue(drawElement);
		}

		public void Draw(bool centered = false)
		{
			var elementCount = this.drawElementActions.Count;
			var rowCount = Mathf.CeilToInt((float)elementCount / this.columnCount);

			EditorGUILayout.BeginVertical();

			for (var row = 0; row < rowCount; row++)
			{
				EditorGUILayout.BeginHorizontal();

				if (centered)
				{
					GUILayout.FlexibleSpace();
				}

				for (var col = 0; col < this.columnCount && this.drawElementActions.Count > 0; col++)
				{
					this.drawElementActions.Dequeue()?.Invoke();
					if (col != this.columnCount - 1)
					{
						EditorGUILayout.Space(this.columnSpacing);
					}
				}

				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();

				if (row != rowCount - 1)
				{
					EditorGUILayout.Space(this.rowSpacing);
				}
			}

			EditorGUILayout.EndVertical();
		}
	}
}