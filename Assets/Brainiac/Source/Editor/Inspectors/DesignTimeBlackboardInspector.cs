using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Brainiac;
using System;

namespace BrainiacEditor
{
	public class DesignTimeBlackboardInspector : IBlackboardInspector
	{
		private SerializedObject m_serializedObject;
		private SerializedProperty m_startMemory;
		private ReorderableList m_startMemoryList;
		private MemoryType? m_memoryToAdd;

		public DesignTimeBlackboardInspector(SerializedObject serializedObject)
		{
			m_serializedObject = serializedObject;
			m_startMemory = serializedObject.FindProperty("m_startMemory");
			m_startMemoryList = new ReorderableList(serializedObject, m_startMemory, true, false, true, true);
			m_startMemoryList.drawElementCallback += DrawMemory;
			m_startMemoryList.onAddDropdownCallback += ShowAddMenu;
		}

		public void DrawGUI()
		{
			m_serializedObject.Update();

			EditorGUILayout.Space();

			if(m_memoryToAdd.HasValue)
			{
				AddMemory(m_memoryToAdd.Value);
				m_memoryToAdd = null;
			}

			m_startMemoryList.DoLayoutList();

			m_serializedObject.ApplyModifiedProperties();
		}

		private void DrawMemory(Rect position, int index, bool isActive, bool isFocused)
		{
			SerializedProperty memory = m_startMemory.GetArrayElementAtIndex(index);
			SerializedProperty type = memory.FindPropertyRelative("m_type");

			switch((MemoryType)type.enumValueIndex)
			{
			case MemoryType.Boolean:
				DrawBooleanMemory(position, memory);
				break;
			case MemoryType.Integer:
				DrawIntegerMemory(position, memory);
				break;
			case MemoryType.Float:
				DrawFloatMemory(position, memory);
				break;
			case MemoryType.String:
				DrawStringMemory(position, memory);
				break;
			case MemoryType.Vector2:
				DrawVector2Memory(position, memory);
				break;
			case MemoryType.Vector3:
				DrawVector3Memory(position, memory);
				break;
			case MemoryType.GameObject:
				DrawGameObjectMemory(position, memory);
				break;
			case MemoryType.Asset:
				DrawAssetMemory(position, memory);
				break;
			}
		}

		private void DrawBooleanMemory(Rect position, SerializedProperty memory)
		{
			SerializedProperty name = memory.FindPropertyRelative("m_name");
			SerializedProperty value = memory.FindPropertyRelative("m_valueBool");
			Rect nameRect = new Rect(position.x, position.y + 2, position.width / 2, position.height - 4);
			Rect valueRect = new Rect(nameRect.xMax + 5, nameRect.y, position.width / 2 - 5, nameRect.height);

			name.stringValue = EditorGUI.TextField(nameRect, name.stringValue);
			value.boolValue = EditorGUI.Toggle(valueRect, value.boolValue);
		}

		private void DrawIntegerMemory(Rect position, SerializedProperty memory)
		{
			SerializedProperty name = memory.FindPropertyRelative("m_name");
			SerializedProperty value = memory.FindPropertyRelative("m_valueInteger");
			Rect nameRect = new Rect(position.x, position.y + 2, position.width / 2, position.height - 4);
			Rect valueRect = new Rect(nameRect.xMax + 5, nameRect.y, position.width / 2 - 5, nameRect.height);

			name.stringValue = EditorGUI.TextField(nameRect, name.stringValue);
			value.intValue = EditorGUI.IntField(valueRect, value.intValue);
		}

		private void DrawFloatMemory(Rect position, SerializedProperty memory)
		{
			SerializedProperty name = memory.FindPropertyRelative("m_name");
			SerializedProperty value = memory.FindPropertyRelative("m_valueFloat");
			Rect nameRect = new Rect(position.x, position.y + 2, position.width / 2, position.height - 4);
			Rect valueRect = new Rect(nameRect.xMax + 5, nameRect.y, position.width / 2 - 5, nameRect.height);

			name.stringValue = EditorGUI.TextField(nameRect, name.stringValue);
			value.floatValue = EditorGUI.FloatField(valueRect, value.floatValue);
		}

		private void DrawStringMemory(Rect position, SerializedProperty memory)
		{
			SerializedProperty name = memory.FindPropertyRelative("m_name");
			SerializedProperty value = memory.FindPropertyRelative("m_valueString");
			Rect nameRect = new Rect(position.x, position.y + 2, position.width / 2, position.height - 4);
			Rect valueRect = new Rect(nameRect.xMax + 5, nameRect.y, position.width / 2 - 5, nameRect.height);

			name.stringValue = EditorGUI.TextField(nameRect, name.stringValue);
			value.stringValue = EditorGUI.TextField(valueRect, value.stringValue);
		}

		private void DrawVector2Memory(Rect position, SerializedProperty memory)
		{
			SerializedProperty name = memory.FindPropertyRelative("m_name");
			SerializedProperty value = memory.FindPropertyRelative("m_valueVector2");
			Rect nameRect = new Rect(position.x, position.y + 2, position.width / 2, position.height - 4);
			Rect valueXRect = new Rect(nameRect.xMax + 5, nameRect.y, ((position.width / 2 - 5) / 2 - 5), nameRect.height);
			Rect valueYRect = new Rect(valueXRect.xMax + 5, nameRect.y, valueXRect.width, nameRect.height);
			Vector2 vec = value.vector2Value;

			name.stringValue = EditorGUI.TextField(nameRect, name.stringValue);
			vec.x = EditorGUI.FloatField(valueXRect, vec.x);
			vec.y = EditorGUI.FloatField(valueYRect, vec.y);
			value.vector2Value = vec;
		}

		private void DrawVector3Memory(Rect position, SerializedProperty memory)
		{
			SerializedProperty name = memory.FindPropertyRelative("m_name");
			SerializedProperty value = memory.FindPropertyRelative("m_valueVector3");
			Rect nameRect = new Rect(position.x, position.y + 2, position.width / 2, position.height - 4);
			Rect valueXRect = new Rect(nameRect.xMax + 5, nameRect.y, ((position.width / 2 - 5) / 3 - 10), nameRect.height);
			Rect valueYRect = new Rect(valueXRect.xMax + 5, nameRect.y, valueXRect.width, nameRect.height);
			Rect valueZRect = new Rect(valueYRect.xMax + 5, nameRect.y, valueXRect.width, nameRect.height);
			Vector3 vec = value.vector3Value;

			name.stringValue = EditorGUI.TextField(nameRect, name.stringValue);
			vec.x = EditorGUI.FloatField(valueXRect, vec.x);
			vec.y = EditorGUI.FloatField(valueYRect, vec.y);
			vec.z = EditorGUI.FloatField(valueZRect, vec.z);
			value.vector3Value = vec;
		}

		private void DrawGameObjectMemory(Rect position, SerializedProperty memory)
		{
			SerializedProperty name = memory.FindPropertyRelative("m_name");
			SerializedProperty value = memory.FindPropertyRelative("m_valueGameObject");
			Rect nameRect = new Rect(position.x, position.y + 2, position.width / 2, position.height - 4);
			Rect valueRect = new Rect(nameRect.xMax + 5, nameRect.y, position.width / 2 - 5, nameRect.height);

			name.stringValue = EditorGUI.TextField(nameRect, name.stringValue);
			value.objectReferenceValue = EditorGUI.ObjectField(valueRect, value.objectReferenceValue, typeof(GameObject), true);
		}

		private void DrawAssetMemory(Rect position, SerializedProperty memory)
		{
			SerializedProperty name = memory.FindPropertyRelative("m_name");
			SerializedProperty value = memory.FindPropertyRelative("m_valueAsset");
			Rect nameRect = new Rect(position.x, position.y + 2, position.width / 2, position.height - 4);
			Rect valueRect = new Rect(nameRect.xMax + 5, nameRect.y, position.width / 2 - 5, nameRect.height);

			name.stringValue = EditorGUI.TextField(nameRect, name.stringValue);
			value.objectReferenceValue = EditorGUI.ObjectField(valueRect, value.objectReferenceValue, typeof(UnityEngine.Object), false);
		}

		private void ShowAddMenu(Rect buttonRect, ReorderableList list)
		{
			GenericMenu menu = new GenericMenu();
			foreach(MemoryType memoryType in Enum.GetValues(typeof(MemoryType)))
			{
				menu.AddItem(new GUIContent(memoryType.ToString()), false, obj => m_memoryToAdd = (MemoryType)obj, memoryType);
			}

			menu.DropDown(new Rect(buttonRect.position, Vector2.zero));
		}

		private void AddMemory(MemoryType memoryType)
		{
			int index = m_startMemoryList.serializedProperty.arraySize++;
			m_startMemoryList.index = index;

			SerializedProperty memory = m_startMemoryList.serializedProperty.GetArrayElementAtIndex(index);
			SerializedProperty type = memory.FindPropertyRelative("m_type");
			SerializedProperty name = memory.FindPropertyRelative("m_name");

			name.stringValue = string.Concat("My", memoryType.ToString(), name.GetHashCode().ToString());
			type.enumValueIndex = (int)memoryType;
		}
	}
}