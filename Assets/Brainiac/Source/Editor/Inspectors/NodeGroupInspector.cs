using UnityEngine;
using Brainiac;

namespace BrainiacEditor
{
	[CustomNodeInspector(typeof(NodeGroup))]
	public class NodeGroupInspector : NodeInspector
	{
		protected override void DrawProperties()
		{
			string label = GraphNode.IsRoot ? "Collapse" : "Expand";
			bool prevGUIState = GUI.enabled;

			GUI.enabled = true;
			if(GUILayout.Button(label, GUILayout.Height(24.0f)))
			{
				if(GraphNode.IsRoot)
					GraphNode.Graph.OnPopNodeGroup();
				else
					GraphNode.Graph.OnPushNodeGroup(GraphNode);
			}
			GUI.enabled = prevGUIState;
		}
	}
}