using UnityEngine;
using UnityEditor;
using System;
using Brainiac;

namespace BrainiacEditor
{
	public class DesignTimeMemoryInspector : IMemoryInspector
	{
		private const int HEADER_HEIGHT = 18;
		private const int BUTTON_HEIGHT = 20;
		private const int BUTTON_WIDTH = 30;
		private const int ITEM_SPACING_VERT = 5;
		private const int ITEM_SPACING_HORZ = 4;
		private const int FIELD_HEIGHT = 20;
		private const int FIELD_SPACING_VERT = 5;
		private const int FIELD_SPACING_HORZ = 4;
		
		private SerializedObject m_serializedObject;
		private SerializedProperty m_startMemory;
		private GUIContent m_plusButtonContent;
		private GUIContent m_minusButtonContent;
		private GUIContent m_itemValueContent;
		private MemoryItem.ItemType? m_itemToAdd;

		public DesignTimeMemoryInspector(SerializedObject serializedObject)
		{
			m_serializedObject = serializedObject;
			m_startMemory = serializedObject.FindProperty("m_startMemory");
			m_plusButtonContent = new GUIContent(EditorGUIUtility.Load("ol plus.png") as Texture);
			m_minusButtonContent = new GUIContent(EditorGUIUtility.Load("ol minus.png") as Texture);
			m_itemValueContent = new GUIContent("");
			m_itemToAdd = null;
		}

		public void DrawGUI()
		{
			m_serializedObject.Update();

			if(m_itemToAdd.HasValue)
			{
				CreateNewMemoryItem(m_itemToAdd.Value);
				m_itemToAdd = null;
			}
			
			DrawMemory();

			m_serializedObject.ApplyModifiedProperties();
		}

		private void DrawMemory()
		{
			EditorGUILayout.Space();

			float itemCount = m_startMemory.arraySize;
			float itemHeight = CalculateMemoryItemHeight();

			Rect groupRect = GUILayoutUtility.GetRect(0, CalculateContentHeight(), GUILayout.ExpandWidth(true));
			Rect headerRect = new Rect(0.0f, 0.0f, groupRect.width, HEADER_HEIGHT);
			Rect bgRect = new Rect(headerRect.x, headerRect.yMax, headerRect.width, itemCount * itemHeight + ITEM_SPACING_VERT * 2);
			Rect itemRect = new Rect(bgRect.x + ITEM_SPACING_HORZ, bgRect.y + ITEM_SPACING_VERT, bgRect.width - ITEM_SPACING_HORZ * 2, bgRect.height - ITEM_SPACING_VERT);
			Rect buttonRect = new Rect(itemRect.xMax - (BUTTON_WIDTH - 4.0f), itemRect.yMax - 4.0f, BUTTON_WIDTH, BUTTON_HEIGHT);

			GUI.BeginGroup(groupRect);

			EditorGUI.LabelField(headerRect, "Start Memory", BTEditorStyle.ListHeader);
			GUI.Box(bgRect, "", BTEditorStyle.ListBackground);

			GUI.BeginGroup(itemRect);

			for(int i = 0; i < m_startMemory.arraySize; i++)
			{
				bool remove = false;
				DrawMemoryItem(new Rect(0.0f, i * itemHeight, itemRect.width, itemHeight), m_startMemory.GetArrayElementAtIndex(i), out remove);

				if(remove)
				{
					m_startMemory.DeleteArrayElementAtIndex(i--);
				}
			}

			GUI.EndGroup();

			EditorGUI.LabelField(buttonRect, "", BTEditorStyle.ListBackground);
			if(GUI.Button(buttonRect, m_plusButtonContent, BTEditorStyle.ListButton))
			{
				ShowCreateItemMenu();
			}

			GUI.EndGroup();
		}

		private void DrawMemoryItem(Rect position, SerializedProperty memoryItem, out bool remove)
		{
			SerializedProperty name = memoryItem.FindPropertyRelative("m_name");
			SerializedProperty value = GetMemoryItemValue(memoryItem);
			string label = string.Format("{0} ({1})", !string.IsNullOrEmpty(name.stringValue) ? name.stringValue : "Item",  GetMemoryItemType(memoryItem));

			Rect headerRect = new Rect(0.0f, 0.0f, position.width, HEADER_HEIGHT);
			Rect bgRect = new Rect(headerRect.x, headerRect.yMax, headerRect.width, FIELD_HEIGHT * 2 + FIELD_SPACING_VERT * 2);
			Rect fieldRect = new Rect(bgRect.x + FIELD_SPACING_HORZ, bgRect.y + FIELD_SPACING_VERT, bgRect.width - FIELD_SPACING_HORZ * 2, bgRect.height - FIELD_SPACING_VERT * 2);
			Rect buttonRect = new Rect(fieldRect.xMax - (BUTTON_WIDTH - 4.0f), fieldRect.yMax + 1.0f, BUTTON_WIDTH, BUTTON_HEIGHT);
			
			GUI.BeginGroup(position);
			
			EditorGUI.LabelField(headerRect, label, BTEditorStyle.ListHeader);
			GUI.Box(bgRect, "", BTEditorStyle.ListBackground);

			GUI.BeginGroup(fieldRect);

			EditorGUI.LabelField(new Rect(0, 0, 50, 16), "Name");
			name.stringValue = EditorGUI.TextField(new Rect(50, 0, fieldRect.width - 50, 16), name.stringValue);

			EditorGUI.LabelField(new Rect(0, FIELD_HEIGHT, 50, 16), "Value");
			EditorGUI.PropertyField(new Rect(50, FIELD_HEIGHT, fieldRect.width - 50, 16), value, m_itemValueContent);

			GUI.EndGroup();

			EditorGUI.LabelField(buttonRect, "", BTEditorStyle.ListBackground);
			remove = GUI.Button(buttonRect, m_minusButtonContent, BTEditorStyle.ListButton);

			GUI.EndGroup();
		}

		private SerializedProperty GetMemoryItemValue(SerializedProperty memoryItem)
		{
			SerializedProperty itemTypeProp = memoryItem.FindPropertyRelative("m_type");
			var itemType = (MemoryItem.ItemType)itemTypeProp.enumValueIndex;

			switch(itemType)
			{
			case MemoryItem.ItemType.Boolean:
				return memoryItem.FindPropertyRelative("m_valueBool");
			case MemoryItem.ItemType.Integer:
				return memoryItem.FindPropertyRelative("m_valueInteger");
			case MemoryItem.ItemType.Float:
				return memoryItem.FindPropertyRelative("m_valueFloat");
			case MemoryItem.ItemType.String:
				return memoryItem.FindPropertyRelative("m_valueString");
			case MemoryItem.ItemType.Vector2:
				return memoryItem.FindPropertyRelative("m_valueVector2");
			case MemoryItem.ItemType.Vector3:
				return memoryItem.FindPropertyRelative("m_valueVector3");
			case MemoryItem.ItemType.GameObject:
				return memoryItem.FindPropertyRelative("m_valueGameObject");
			case MemoryItem.ItemType.Asset:
				return memoryItem.FindPropertyRelative("m_valueAsset");
			}

			return null;
		}

		private MemoryItem.ItemType GetMemoryItemType(SerializedProperty memoryItem)
		{
			SerializedProperty itemTypeProp = memoryItem.FindPropertyRelative("m_type");
			return (MemoryItem.ItemType)itemTypeProp.enumValueIndex;
		}

		private float CalculateContentHeight()
		{
			float itemCount = m_startMemory.arraySize;
			float itemHeight = CalculateMemoryItemHeight();

			return itemCount * itemHeight + HEADER_HEIGHT + BUTTON_HEIGHT + ITEM_SPACING_VERT * 2;
		}

		private float CalculateMemoryItemHeight()
		{
			int fieldCount = 2;
			return HEADER_HEIGHT + fieldCount * FIELD_HEIGHT + FIELD_SPACING_VERT * 2 + BUTTON_HEIGHT;
		}

		private void ShowCreateItemMenu()
		{
			GenericMenu menu = new GenericMenu();
			foreach(MemoryItem.ItemType item in Enum.GetValues(typeof(MemoryItem.ItemType)))
			{
				menu.AddItem(new GUIContent(item.ToString()), false, (obj) => { m_itemToAdd = (MemoryItem.ItemType)obj; }, item);
			}

			menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
		}

		private void CreateNewMemoryItem(MemoryItem.ItemType itemType)
		{
			m_startMemory.InsertArrayElementAtIndex(Mathf.Max(m_startMemory.arraySize - 1, 0));
			SerializedProperty item = m_startMemory.GetArrayElementAtIndex(m_startMemory.arraySize - 1);
			SerializedProperty nameProp = item.FindPropertyRelative("m_name");
			SerializedProperty itemTypeProp = item.FindPropertyRelative("m_type");

			nameProp.stringValue = "";
			itemTypeProp.enumValueIndex = (int)itemType;
		}
	}
}