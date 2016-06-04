using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	public static class BTNodeInspectorFactory
	{
		private static Dictionary<Type, Type> m_nodeInspectors;

		static BTNodeInspectorFactory()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			m_nodeInspectors = new Dictionary<Type, Type>();
			foreach(Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(NodeInspector))))
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

		public static Type GetInspectorTypeForNode(Type nodeType)
		{
			Type inspectorType = null;

			if(m_nodeInspectors.TryGetValue(nodeType, out inspectorType))
			{
				return inspectorType;
			}
			else
			{
				if(nodeType.IsSameOrSubclass(typeof(Composite)))
				{
					return typeof(CompositeInspector);
				}
				else
				{
					return typeof(NodeInspector);
				}
			}
		}

		public static NodeInspector CreateInspectorForNode(BTEditorGraphNode graphNode)
		{
			if(graphNode != null && graphNode.Node != null)
			{
				Type inspectorType = GetInspectorTypeForNode(graphNode.Node.GetType());
				if(inspectorType != null)
				{
					NodeInspector inspector = Activator.CreateInstance(inspectorType) as NodeInspector;
					inspector.Target = graphNode.Node;
					inspector.GraphNode = graphNode;

					return inspector;
				}
			}

			return null;
		}
	}
}