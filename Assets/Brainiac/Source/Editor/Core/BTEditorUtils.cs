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
		private const int BEZIER_H_OFFSET = 0;
		private const int BEZIER_WIDTH = 3;

		private static List<Tuple<Type, string>> m_nodeMenuPaths;
		private static Dictionary<Type, Type> m_nodeInspectors;

		public static void DrawBezier(Rect a, Rect b, Color color)
		{
			Handles.DrawBezier(a.center, b.center,
							   new Vector3(a.center.x + BEZIER_H_OFFSET, a.center.y, 0),
							   new Vector3(b.center.x - BEZIER_H_OFFSET, b.center.y, 0),
							   color, null, BEZIER_WIDTH);
		}

		public static void DrawBezier(Vector2 a, Vector2 b, Color color)
		{
			Handles.DrawBezier(a, b, new Vector3(a.x + BEZIER_H_OFFSET, a.y, 0),
							   new Vector3(b.x - BEZIER_H_OFFSET, b.y, 0),
							   color, null, BEZIER_WIDTH);
		}

		public static Type GetInspectorTypeForNode(Type nodeType)
		{
			if(m_nodeInspectors == null)
			{
				BuildNodeInspectorTable();
			}

			Type inspectorType = null;
			if(m_nodeInspectors.TryGetValue(nodeType, out inspectorType))
			{
				return inspectorType;
			}
			else
			{
				if(nodeType.IsSameOrSubclass(typeof(Composite)))
				{
					return typeof(GenericCompositeInspector);
				}
				else
				{
					return typeof(GenericNodeInspector);
				}
			}
		}

		public static GenericMenu CreateNodeContextMenu(BTEditorGraphNode targetNode)
		{
			GenericMenu menu = new GenericMenu();
			bool canAddChild = false;

			if(targetNode.Node is Composite)
			{
				canAddChild = true;
			}
			else if(targetNode.Node is Decorator)
			{
				canAddChild = ((Decorator)targetNode.Node).GetChild() == null;
			}

			if(canAddChild)
			{
				if(m_nodeMenuPaths == null)
				{
					BuildNodeMenuPathList();
				}

				GenericMenu.MenuFunction2 onCreateChild = t => targetNode.Graph.OnNodeCreateChild(targetNode, t as Type);

				foreach(var item in m_nodeMenuPaths)
				{
					menu.AddItem(new GUIContent("Add Child/" + item.Item2), false, onCreateChild, item.Item1);
				}

				menu.AddSeparator("");
			}

			if(targetNode.Node is Root)
			{
				menu.AddDisabledItem(new GUIContent("Copy"));
			}
			else
			{
				menu.AddItem(new GUIContent("Copy"), false, () => targetNode.Graph.OnCopyNode(targetNode));
			}

			if(targetNode.Graph.CanPaste(targetNode))
			{
				menu.AddItem(new GUIContent("Paste"), false, () => targetNode.Graph.OnPasteNode(targetNode));
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Paste"));
			}

			if(targetNode.Node is Root)
			{
				menu.AddDisabledItem(new GUIContent("Delete"));
			}
			else
			{
				menu.AddItem(new GUIContent("Delete"), false, () => targetNode.Graph.OnNodeDelete(targetNode));
			}

			if(targetNode.Node is Composite)
			{
				if(((Composite)targetNode.Node).ChildCount > 0)
				{
					menu.AddItem(new GUIContent("Delete Children"), false, () => targetNode.Graph.OnNodeDeleteChildren(targetNode));
				}
				else
				{
					menu.AddDisabledItem(new GUIContent("Delete Children"));
				}
			}
			else if(targetNode.Node is Decorator)
			{
				if(((Decorator)targetNode.Node).GetChild() != null)
				{
					menu.AddItem(new GUIContent("Delete Child"), false, () => targetNode.Graph.OnNodeDeleteChildren(targetNode));
				}
				else
				{
					menu.AddDisabledItem(new GUIContent("Delete Child"));
				}
			}

			menu.AddSeparator("");
			foreach(DebugOptions item in Enum.GetValues(typeof(DebugOptions)))
			{
				menu.AddItem(new GUIContent("Debug/" + item.ToString()), targetNode.Node.DebugOptions.Has(item), (obj) =>
				{
					DebugOptions option = (DebugOptions)obj;
					if(option == DebugOptions.None)
					{
						targetNode.Node.DebugOptions = DebugOptions.None;
					}
					else
					{
						if(targetNode.Node.DebugOptions.Has(option))
						{
							targetNode.Node.DebugOptions = targetNode.Node.DebugOptions.Remove(option);
						}
						else
						{
							if(targetNode.Node.DebugOptions == DebugOptions.None)
								targetNode.Node.DebugOptions = option;
							else
								targetNode.Node.DebugOptions = targetNode.Node.DebugOptions.Add(option);
						}
					}
				}, item);
			}

			return menu;
		}

		public static GenericMenu CreateGraphContextMenu()
		{
			GenericMenu menu = new GenericMenu();

			if(BTUndoSystem.CanUndo)
			{
				BTUndoState topUndo = BTUndoSystem.PeekUndo();
				menu.AddItem(new GUIContent(string.Format("Undo \"{0}\"", topUndo.Title)), false, () => BTUndoSystem.Undo());
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Undo"));
			}

			if(BTUndoSystem.CanRedo)
			{
				BTUndoState topRedo = BTUndoSystem.PeekRedo();
				menu.AddItem(new GUIContent(string.Format("Redo \"{0}\"", topRedo.Title)), false, () => BTUndoSystem.Redo());
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Redo"));
			}

			return menu;
		}

		private static void BuildNodeMenuPathList()
		{
			if(m_nodeMenuPaths == null)
			{
				m_nodeMenuPaths = new List<Tuple<Type, string>>();
			}
			else
			{
				m_nodeMenuPaths.Clear();
			}

			Type nodeType = typeof(BehaviourNode);
			Assembly assembly = nodeType.Assembly;
			foreach(Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(nodeType)))
			{
				object[] attributes = type.GetCustomAttributes(typeof(AddNodeMenuAttribute), false);
				if(attributes.Length > 0)
				{
					AddNodeMenuAttribute attribute = attributes[0] as AddNodeMenuAttribute;
					m_nodeMenuPaths.Add(new Tuple<Type, string>(type, attribute.MenuPath));
				}
			}
		}

		private static void BuildNodeInspectorTable()
		{
			if(m_nodeInspectors == null)
			{
				m_nodeInspectors = new Dictionary<Type, Type>();
			}
			else
			{
				m_nodeInspectors.Clear();
			}

			Assembly assembly = Assembly.GetExecutingAssembly();

			foreach(Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(INodeInspector))))
			{
				object[] attributes = type.GetCustomAttributes(typeof(CustomNodeInspectorAttribute), false);
				if(attributes.Length > 0)
				{
					CustomNodeInspectorAttribute attribute = attributes[0] as CustomNodeInspectorAttribute;
					if(!m_nodeInspectors.ContainsKey(attribute.NodeType))
					{
						m_nodeInspectors.Add(attribute.NodeType, type);
					}
				}
			}
		}
	}
}