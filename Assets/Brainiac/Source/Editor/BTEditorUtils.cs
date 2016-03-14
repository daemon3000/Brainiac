using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using Brainiac;
using System;

namespace BrainiacEditor
{
	public static class BTEditorUtils
	{
		private const int BEZIER_H_OFFSET = 0;
		private const int BEZIER_WIDTH = 3;

		public static void DrawBezier(Rect a, Rect b)
		{
			Handles.DrawBezier(a.center, b.center,
							   new Vector3(a.center.x + BEZIER_H_OFFSET, a.center.y, 0),
							   new Vector3(b.center.x - BEZIER_H_OFFSET, b.center.y, 0),
							   Color.white, null, BEZIER_WIDTH);
		}

		public static void DrawBezier(Vector2 a, Vector2 b)
		{
			Handles.DrawBezier(a, b, new Vector3(a.x + BEZIER_H_OFFSET, a.y, 0),
							   new Vector3(b.x - BEZIER_H_OFFSET, b.y, 0),
							   Color.white, null, BEZIER_WIDTH);
		}

		public static GenericMenu CreateContextMenu(BehaviourNode m_node, GenericMenu.MenuFunction2 onCreateChild, GenericMenu.MenuFunction onDelete, GenericMenu.MenuFunction onDeleteChildren)
		{
			GenericMenu menu = new GenericMenu();
			bool canAddChild = false;

			if(m_node is Composite)
			{
				canAddChild = true;
			}
			else if(m_node is Decorator)
			{
				canAddChild = ((Decorator)m_node).GetChild() == null;
			}

			if(canAddChild)
			{
				menu.AddItem(new GUIContent("Add Child/Actions/Debug"), false, onCreateChild, typeof(DebugLog));
				menu.AddItem(new GUIContent("Add Child/Actions/Move"), false, onCreateChild, typeof(Move));
				menu.AddItem(new GUIContent("Add Child/Actions/Timer"), false, onCreateChild, typeof(Timer));
				menu.AddItem(new GUIContent("Add Child/Actions/Yield"), false, onCreateChild, typeof(Yield));
				menu.AddItem(new GUIContent("Add Child/Composite/Sequence"), false, onCreateChild, typeof(Sequence));
				menu.AddItem(new GUIContent("Add Child/Composite/Selector"), false, onCreateChild, typeof(Selector));
				menu.AddItem(new GUIContent("Add Child/Decorator/Inverter"), false, onCreateChild, typeof(Inverter));
				menu.AddItem(new GUIContent("Add Child/Decorator/Succeder"), false, onCreateChild, typeof(Succeder));

				menu.AddSeparator("");
			}

			if(m_node is Root)
			{
				menu.AddDisabledItem(new GUIContent("Delete"));
			}
			else
			{
				menu.AddItem(new GUIContent("Delete"), false, onDelete);
			}

			if(m_node is Composite)
			{
				if(((Composite)m_node).ChildCount > 0)
				{
					menu.AddItem(new GUIContent("Delete Children"), false, onDeleteChildren);
				}
				else
				{
					menu.AddDisabledItem(new GUIContent("Delete Children"));
				}
			}
			else if(m_node is Decorator)
			{
				if(((Decorator)m_node).GetChild() != null)
				{
					menu.AddItem(new GUIContent("Delete Child"), false, onDeleteChildren);
				}
				else
				{
					menu.AddDisabledItem(new GUIContent("Delete Child"));
				}
			}

			return menu;
		}
	}
}