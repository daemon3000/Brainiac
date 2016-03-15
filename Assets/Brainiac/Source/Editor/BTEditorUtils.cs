using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	public static class BTEditorUtils
	{
		private class BTContextMenuPath
		{
			public readonly string menuPath;
			public readonly Type nodeType;

			public BTContextMenuPath(string menuPath, Type nodeType)
			{
				this.menuPath = menuPath;
				this.nodeType = nodeType;
			}
		}

		private const int BEZIER_H_OFFSET = 0;
		private const int BEZIER_WIDTH = 3;

		private static List<BTContextMenuPath> m_nodeMenuPaths;

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
				if(m_nodeMenuPaths == null)
				{
					BuildNodeMenuPaths();
				}

				foreach(var item in m_nodeMenuPaths)
				{
					menu.AddItem(new GUIContent("Add Child/" + item.menuPath), false, onCreateChild, item.nodeType);
				}

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

		private static void BuildNodeMenuPaths()
		{
			if(m_nodeMenuPaths == null)
			{
				m_nodeMenuPaths = new List<BTContextMenuPath>();
			}
			else
			{
				m_nodeMenuPaths.Clear();
			}

			Type nodeType = typeof(BehaviourNode);
			Assembly assembly = nodeType.Assembly;
			foreach(Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(nodeType)))
			{
				object[] attributes = type.GetCustomAttributes(typeof(AddBehaviourNodeMenuAttribute), false);
				if(attributes.Length > 0)
				{
					AddBehaviourNodeMenuAttribute attribute = attributes[0] as AddBehaviourNodeMenuAttribute;
					m_nodeMenuPaths.Add(new BTContextMenuPath(attribute.MenuPath, type));
				}
			}
		}
	}
}