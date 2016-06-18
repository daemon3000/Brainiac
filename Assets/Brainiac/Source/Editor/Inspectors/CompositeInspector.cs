using UnityEngine;
using UnityEditor;
using Brainiac;

namespace BrainiacEditor
{
	public class CompositeInspector : NodeInspector
	{
		protected const int HEADER_HEIGHT = 18;
		protected const int ITEM_SPACING_VERT = 5;
		protected const int ITEM_SPACING_HORZ = 4;
		protected const int FIELD_HEIGHT = 20;

		protected override void DrawProperties()
		{
			Composite composite = (Composite)Target;

			DrawDefaultProperties();

			EditorGUILayout.Space();
			DrawChildren(composite);
		}

		protected void DrawChildren(Composite composite)
		{
			float itemCount = composite.ChildCount;
			float itemHeight = FIELD_HEIGHT;

			Rect groupRect = GUILayoutUtility.GetRect(0, CalculateContentHeight(composite), GUILayout.ExpandWidth(true));
			Rect headerRect = new Rect(0.0f, 0.0f, groupRect.width, HEADER_HEIGHT);
			Rect bgRect = new Rect(headerRect.x, headerRect.yMax, headerRect.width, itemCount * itemHeight + ITEM_SPACING_VERT * 2);
			Rect itemRect = new Rect(bgRect.x + ITEM_SPACING_HORZ, bgRect.y + ITEM_SPACING_VERT, bgRect.width - ITEM_SPACING_HORZ * 2, bgRect.height - ITEM_SPACING_VERT);
			
			GUI.BeginGroup(groupRect);

			EditorGUI.LabelField(headerRect, "Children", BTEditorStyle.ListHeader);
			GUI.Box(bgRect, "", BTEditorStyle.ListBackground);

			GUI.BeginGroup(itemRect);

			for(int i = 0; i < composite.ChildCount; i++)
			{
				Rect handleRect = new Rect(0, i * itemHeight + itemHeight / 4, 10, itemHeight);
				Rect childRect = new Rect(15, i * itemHeight, itemRect.width - 65, itemHeight);
				BehaviourNode child = composite.GetChild(i);
				string childName = string.IsNullOrEmpty(child.Name) ? child.Title : child.Name;
				

				EditorGUI.LabelField(handleRect, "", BTEditorStyle.ListDragHandle);
				EditorGUI.LabelField(childRect, childName);

				if(!BTEditorCanvas.Current.ReadOnly)
				{
					Rect upButtonRect = new Rect(childRect.xMax + 5, childRect.y, 20, FIELD_HEIGHT - 2);
					Rect downButtonRect = new Rect(upButtonRect.xMax + 2, childRect.y, 20, FIELD_HEIGHT - 2);
					bool previousGUIState = GUI.enabled;

					GUI.enabled = i > 0;
					if(GUI.Button(upButtonRect, BTEditorStyle.ArrowUp))
					{
						composite.MoveChildPriorityUp(i);
					}

					GUI.enabled = i < composite.ChildCount - 1;
					if(GUI.Button(downButtonRect, BTEditorStyle.ArrowDown))
					{
						composite.MoveChildPriorityDown(i);
					}

					GUI.enabled = previousGUIState;
				}
			}

			GUI.EndGroup();
			GUI.EndGroup();
		}

		private float CalculateContentHeight(Composite composite)
		{
			float itemCount = composite.ChildCount;
			float itemHeight = FIELD_HEIGHT;

			return itemCount * itemHeight + HEADER_HEIGHT + ITEM_SPACING_VERT * 2;
		}
	}
}
