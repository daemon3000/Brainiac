using UnityEngine;
using Brainiac;

namespace BrainiacEditor
{
	[CustomNodeInspector(typeof(NodeGroup))]
	public class NodeGroupInspector : NodeInspector
	{
		public override void OnInspectorGUI()
		{
			if(Target != null && Target is NodeGroup)
			{
				string label = BTEditorCanvas.Current.IsDebuging ? "Preview" : "Open";
				bool prevGUIState = GUI.enabled;

				DrawHeader();

				GUI.enabled = true;
				if(GUILayout.Button(label, GUILayout.Height(26.0f)))
				{
					GraphNode.Graph.OnPushNodeGroup(GraphNode);
				}
				GUI.enabled = prevGUIState;

				RepaintCanvas();
			}
		}
	}
}