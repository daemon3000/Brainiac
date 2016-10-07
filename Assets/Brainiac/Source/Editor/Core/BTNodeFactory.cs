using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	public static class BTNodeFactory
	{
		private static List<Tuple<Type, string>> m_nodeMenuPaths;

		static BTNodeFactory()
		{
			Type nodeType = typeof(BehaviourNode);
			Assembly assembly = nodeType.Assembly;

			m_nodeMenuPaths = new List<Tuple<Type, string>>();
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

		public static void AddChild(GenericMenu menu, BTEditorGraphNode targetNode)
		{
			GenericMenu.MenuFunction2 onCreateChild = t => targetNode.Graph.OnNodeCreateChild(targetNode, t as Type);

			foreach(var item in m_nodeMenuPaths)
			{
				menu.AddItem(new GUIContent("Add Child/" + item.Item2), false, onCreateChild, item.Item1);
			}
		}

		public static void SwitchType(GenericMenu menu, BTEditorGraphNode targetNode)
		{
			GenericMenu.MenuFunction2 onSwitchType = t => targetNode.Graph.OnNodeSwitchType(targetNode, t as Type);
			Type nodeGroupType = typeof(NodeGroup);

			if(!(targetNode.Node is NodeGroup))
			{
				foreach(var item in m_nodeMenuPaths)
				{
					if(item.Item1.IsSameOrSubclass(nodeGroupType))
						continue;

					if(((targetNode.Node is Brainiac.Action) && item.Item1.IsSubclassOf(typeof(Brainiac.Action))) ||
						((targetNode.Node is Decorator) && item.Item1.IsSubclassOf(typeof(Decorator))) ||
						((targetNode.Node is Composite) && item.Item1.IsSubclassOf(typeof(Composite))))
					{
						menu.AddItem(new GUIContent("Switch To/" + item.Item2.Substring(item.Item2.IndexOf('/') + 1)), false, onSwitchType, item.Item1);
					}
				}
			}
		}
	}
}