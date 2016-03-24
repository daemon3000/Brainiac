using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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

			List<string> keys = new List<string>(m_memory.Keys);
			int itemIndex = 0;

			foreach(string key in keys)
			{
				object value = null;
				if(TryDrawMemoryItem(new Rect(0.0f, itemIndex * itemHeight, itemRect.width, itemHeight), key, m_memory[key], out value))
				{
					m_memory[key] = value;
				}

				itemIndex++;
			}

			GUI.EndGroup();
			GUI.EndGroup();
		}

		private bool TryDrawMemoryItem(Rect position, string memoryItemName, object memoryItemValue, out object newValue)
		{
			string label = !string.IsNullOrEmpty(memoryItemName) ? memoryItemName : "Item";
			bool success = false;

			Rect headerRect = new Rect(0.0f, 0.0f, position.width, HEADER_HEIGHT);
			Rect bgRect = new Rect(headerRect.x, headerRect.yMax, headerRect.width, FIELD_HEIGHT * 2 + FIELD_SPACING_VERT * 2);
			Rect fieldRect = new Rect(bgRect.x + FIELD_SPACING_HORZ, bgRect.y + FIELD_SPACING_VERT, bgRect.width - FIELD_SPACING_HORZ * 2, bgRect.height - FIELD_SPACING_VERT * 2);

			GUI.BeginGroup(position);

			EditorGUI.LabelField(headerRect, label, BTEditorStyle.ListHeader);
			GUI.Box(bgRect, "", BTEditorStyle.ListBackground);

			GUI.BeginGroup(fieldRect);

			EditorGUI.TextField(new Rect(0, 0, fieldRect.width, 16), "Name", memoryItemName);
			success = TryToDrawField(new Rect(0, FIELD_HEIGHT, fieldRect.width, 16), "Value", memoryItemValue, out newValue);

			GUI.EndGroup();
			GUI.EndGroup();

			return success;
		}

		private bool TryToDrawField(Rect position, string label, object value, out object newValue)
		{
			bool success = true;

			if(value is bool)
			{
				newValue = EditorGUI.Toggle(position, label, (bool)value);
			}
			else if(value is int)
			{
				newValue = EditorGUI.IntField(position, label, (int)value);
			}
			else if(value is float)
			{
				newValue = EditorGUI.FloatField(position, label, (float)value);
			}
			else if(value is string)
			{
				string val = (string)value;
				newValue = EditorGUI.TextField(position, label, val != null ? val : "");
			}
			else if(value is Vector2)
			{
				newValue = EditorGUI.Vector2Field(position, label, (Vector2)value);
			}
			else if(value is Vector3)
			{
				newValue = EditorGUI.Vector3Field(position, label, (Vector3)value);
			}
			else if(value is UnityEngine.Object)
			{
				newValue = EditorGUI.ObjectField(position, label, (UnityEngine.Object)value, value.GetType(), false);
			}
			else
			{
				EditorGUI.LabelField(position, label, "Item type mismatch!");
				success = false;
				newValue = null;
			}

			return success;
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