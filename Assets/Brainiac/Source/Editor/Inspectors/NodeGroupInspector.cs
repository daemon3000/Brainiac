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
				string label = GraphNode.IsRoot ? "Collapse" : "Expand";
				
				DrawHeader();

				if(GUILayout.Button(label, GUILayout.Height(24.0f)))
				{
					if(GraphNode.IsRoot)
						GraphNode.Graph.OnPopNodeGroup();
					else
						GraphNode.Graph.OnPushNodeGroup(GraphNode);
				}

				DrawConditionsAndServices();
				RepaintCanvas();
			}
		}
	}
}