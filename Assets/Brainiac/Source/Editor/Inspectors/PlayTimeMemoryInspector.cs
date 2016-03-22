using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	public class PlayTimeMemoryInspector : IMemoryInspector
	{
		private const int HEADER_HEIGHT = 18;
		private const int ITEM_SPACING_VERT = 5;
		private const int ITEM_SPACING_HORZ = 4;
		private const int FIELD_HEIGHT = 20;
		private const int FIELD_SPACING_VERT = 5;
		private const int FIELD_SPACING_HORZ = 4;

		private IDictionary<string, object> m_memory;

		public PlayTimeMemoryInspector(IDictionary<string, object> memory)
		{
			m_memory = memory;
		}

		public void DrawGUI()
		{
			DrawMemory();
		}

		private void DrawMemory()
		{
			EditorGUILayout.Space();

			float itemCount = m_memory.Count;
			float itemHeight = CalculateMemoryItemHeight();

			Rect groupRect = GUILayoutUtility.GetRect(0, CalculateContentHeight(), GUILayout.ExpandWidth(true));
			Rect headerRect = new Rect(0.0f, 0.0f, groupRect.width, HEADER_HEIGHT);
			Rect bgRect = new Rect(headerRect.x, headerRect.yMax, headerRect.width, itemCount * itemHeight + ITEM_SPACING_VERT * 2);
			Rect itemRect = new Rect(bgRect.x + ITEM_SPACING_HORZ, bgRect.y + ITEM_SPACING_VERT, bgRect.width - ITEM_SPACING_HORZ * 2, bgRect.height - ITEM_SPACING_VERT);
			
			GUI.BeginGroup(groupRect);

			EditorGUI.LabelField(headerRect, "Start Memory", BTEditorStyle.ListHeader);
			GUI.Box(bgRect, "", BTEditorStyle.ListBackground);

			GUI.BeginGroup(itemRect);

			int itemIndex = 0;
			foreach(var entry in m_memory)
			{
				DrawMemoryItem(new Rect(0.0f, itemIndex * itemHeight, itemRect.width, itemHeight), entry.Key, entry.Value);
				itemIndex++;
			}

			GUI.EndGroup();
			GUI.EndGroup();
		}

		private void DrawMemoryItem(Rect position, string memoryItemName, object memoryItemValue)
		{
			string label = !string.IsNullOrEmpty(memoryItemName) ? memoryItemName : "Item";

			Rect headerRect = new Rect(0.0f, 0.0f, position.width, HEADER_HEIGHT);
			Rect bgRect = new Rect(headerRect.x, headerRect.yMax, headerRect.width, FIELD_HEIGHT * 2 + FIELD_SPACING_VERT * 2);
			Rect fieldRect = new Rect(bgRect.x + FIELD_SPACING_HORZ, bgRect.y + FIELD_SPACING_VERT, bgRect.width - FIELD_SPACING_HORZ * 2, bgRect.height - FIELD_SPACING_VERT * 2);
			
			GUI.BeginGroup(position);

			EditorGUI.LabelField(headerRect, label, BTEditorStyle.ListHeader);
			GUI.Box(bgRect, "", BTEditorStyle.ListBackground);

			GUI.BeginGroup(fieldRect);

			EditorGUI.TextField(new Rect(0, 0, fieldRect.width, 16), "Name", memoryItemName);
			TryToDrawField(new Rect(0, FIELD_HEIGHT, fieldRect.width, 16), "Value", memoryItemValue);

			GUI.EndGroup();
			GUI.EndGroup();
		}

		private void TryToDrawField(Rect position, string label, object value)
		{
			if(value is bool)
			{
				EditorGUI.Toggle(position, label, (bool)value);
			}
			else if(value is int)
			{
				EditorGUI.IntField(position, label, (int)value);
			}
			else if(value is float)
			{
				EditorGUI.FloatField(position, label, (float)value);
			}
			else if(value is string)
			{
				string val = (string)value;
				EditorGUI.TextField(position, label, val != null ? val : "");
			}
			else if(value is Vector2)
			{
				EditorGUI.Vector2Field(position, label, (Vector2)value);
			}
			else if(value is Vector3)
			{
				EditorGUI.Vector3Field(position, label, (Vector3)value);
			}
			else if(value is UnityEngine.Object)
			{
				EditorGUI.ObjectField(position, label, (UnityEngine.Object)value, value.GetType(), false);
			}
			else
			{
				EditorGUI.LabelField(position, label, "Item type mismatch!");
			}
		}

		private float CalculateContentHeight()
		{
			float itemCount = m_memory.Count;
			float itemHeight = CalculateMemoryItemHeight();

			return itemCount * itemHeight + HEADER_HEIGHT + ITEM_SPACING_VERT * 2;
		}

		private float CalculateMemoryItemHeight()
		{
			int fieldCount = 2;
			return HEADER_HEIGHT + fieldCount * FIELD_HEIGHT + FIELD_SPACING_VERT * 2;
		}
	}
}