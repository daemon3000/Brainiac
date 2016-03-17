using UnityEngine;
using UnityEditor;

namespace BrainiacEditor
{
	[CustomEditor(typeof(BTEditorGraphNode))]
	public class BTEditorGraphNodeInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			BTEditorGraphNode graphNode = target as BTEditorGraphNode;
			if(graphNode.Node != null)
			{
				bool prevGUIState = GUI.enabled;

				GUI.enabled = !graphNode.Graph.ReadOnly;
				graphNode.Node.OnGUI();
				GUI.enabled = prevGUIState;

				Repaint();
			}
		}
	}
}