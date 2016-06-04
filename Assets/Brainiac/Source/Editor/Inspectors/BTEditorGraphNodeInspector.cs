using UnityEngine;
using UnityEditor;
using System;

namespace BrainiacEditor
{
	[CustomEditor(typeof(BTEditorGraphNode))]
	public class BTEditorGraphNodeInspector : Editor
	{
		private BTEditorGraphNode m_graphNode;
		private NodeInspector m_nodeInspector;

		private void OnEnable()
		{
			m_graphNode = target as BTEditorGraphNode;
			if(m_graphNode != null)
				m_nodeInspector = BTNodeInspectorFactory.CreateInspectorForNode(m_graphNode);
			else
				m_nodeInspector = null;
		}

		public override void OnInspectorGUI()
		{
			if(m_nodeInspector != null)
			{
				bool prevGUIState = GUI.enabled;

				GUI.enabled = !m_graphNode.Graph.ReadOnly;
				m_nodeInspector.OnInspectorGUI();

				GUI.enabled = prevGUIState;

				Repaint();
			}
		}
	}
}