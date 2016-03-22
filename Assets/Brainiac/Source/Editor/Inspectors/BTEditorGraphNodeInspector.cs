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
			m_graphNode = (BTEditorGraphNode)target;
			if(m_graphNode != null && m_graphNode.Node != null)
			{
				Type inspectorType = BTEditorUtils.GetInspectorTypeForNode(m_graphNode.Node.GetType());
				m_nodeInspector = Activator.CreateInstance(inspectorType) as NodeInspector;
				m_nodeInspector.SetTarget(m_graphNode.Node);
			}
			else
			{
				m_nodeInspector = null;
			}
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