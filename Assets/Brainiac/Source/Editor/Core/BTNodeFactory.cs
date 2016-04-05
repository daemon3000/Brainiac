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
	}
}