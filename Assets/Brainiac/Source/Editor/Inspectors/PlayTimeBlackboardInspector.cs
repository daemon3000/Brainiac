using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BrainiacEditor
{
	public class PlayTimeBlackboardInspector : IBlackboardInspector
	{
		private const int HEADER_HEIGHT = 18;
		private const int ITEM_HEIGHT = 20;

		private IDictionary<string, object> m_values;

		public PlayTimeBlackboardInspector(IDictionary<string, object> values)
		{
			m_values = values;
		}

		public void DrawGUI()
		{
			EditorGUILayout.Space();

			float itemCount = m_values.Count;
			float contentHeight = HEADER_HEIGHT + itemCount * ITEM_HEIGHT + 6;

			Rect groupRect = GUILayoutUtility.GetRect(0, contentHeight, GUILayout.ExpandWidth(true));
			Rect headerRect = new Rect(0.0f, 0.0f, groupRect.width, HEADER_HEIGHT);
			Rect bgRect = new Rect(headerRect.x, headerRect.yMax, headerRect.width, contentHeight - HEADER_HEIGHT);
			Rect contentRect = new Rect(bgRect.x + 5.0f, bgRect.y + 2.0f, bgRect.width - 15.0f, bgRect.height - 8.0f);
			
			GUI.BeginGroup(groupRect);

			EditorGUI.LabelField(headerRect, "", BTEditorStyle.ListHeader);
			GUI.Box(bgRect, "", BTEditorStyle.ListBackground);

			GUI.BeginGroup(contentRect);

			List<string> keys = new List<string>(m_values.Keys);
			
			for(int i = 0; i < keys.Count; i++)
			{
				object value = null;
				if(TryDrawItem(new Rect(0.0f, i * ITEM_HEIGHT, contentRect.width, ITEM_HEIGHT), keys[i], m_values[keys[i]], out value))
				{
					m_values[keys[i]] = value;
				}
			}

			GUI.EndGroup();
			GUI.EndGroup();
		}

		private bool TryDrawItem(Rect position, string itemName, object itemValue, out object newValue)
		{
			Rect nameRect = new Rect(position.x, position.y + 2, position.width / 2, position.height - 4);
			Rect valueRect = new Rect(nameRect.xMax + 5, nameRect.y, nameRect.width - 5, nameRect.height);

			EditorGUI.TextField(nameRect, itemName);
			bool success = TryToDrawField(valueRect, itemValue, out newValue);

			return success;
		}

		private bool TryToDrawField(Rect position, object value, out object newValue)
		{
			bool success = true;

			if(value is bool)
			{
				newValue = EditorGUI.Toggle(position, (bool)value);
			}
			else if(value is int)
			{
				newValue = EditorGUI.IntField(position, (int)value);
			}
			else if(value is float)
			{
				newValue = EditorGUI.FloatField(position, (float)value);
			}
			else if(value is string)
			{
				string val = (string)value;
				newValue = EditorGUI.TextField(position, val != null ? val : "");
			}
			else if(value is Vector2)
			{
				Rect valueXRect = new Rect(position.x, position.y, (position.width - 5) / 2, position.height);
				Rect valueYRect = new Rect(valueXRect.xMax + 5, valueXRect.y, valueXRect.width, valueXRect.height);
				Vector2 vec = (Vector2)value;

				vec.x = EditorGUI.FloatField(valueXRect, vec.x);
				vec.y = EditorGUI.FloatField(valueYRect, vec.y);

				newValue = vec;
			}
			else if(value is Vector3)
			{
				Rect valueXRect = new Rect(position.x, position.y, (position.width - 10) / 3, position.height);
				Rect valueYRect = new Rect(valueXRect.xMax + 5, valueXRect.y, valueXRect.width, valueXRect.height);
				Rect valueZRect = new Rect(valueYRect.xMax + 5, valueYRect.y, valueYRect.width, valueYRect.height);
				Vector3 vec = (Vector3)value;

				vec.x = EditorGUI.FloatField(valueXRect, vec.x);
				vec.y = EditorGUI.FloatField(valueYRect, vec.y);
				vec.z = EditorGUI.FloatField(valueZRect, vec.z);

				newValue = vec;
			}
			else if(value is GameObject)
			{
				newValue = (GameObject)EditorGUI.ObjectField(position, (GameObject)value, typeof(GameObject), true);
			}
			else if(value is UnityEngine.Object)
			{
				newValue = EditorGUI.ObjectField(position, (UnityEngine.Object)value, typeof(UnityEngine.Object), false);
			}
			else if(value == null)
			{
				EditorGUI.LabelField(position, "NULL");
				success = false;
				newValue = null;
			}
			else
			{
				EditorGUI.LabelField(position, "Value type mismatch!");
				success = false;
				newValue = null;
			}

			return success;
		}
	}
}