using Brainiac;

namespace BrainiacEditor
{
	public class UndoNodeGroupPush : BTUndoState
	{
		private BTEditorGraph m_graph;
		private string m_nodeGroupHash;

		public override bool CanUndo
		{
			get
			{
				return m_graph != null;
			}
		}

		public override bool CanRedo
		{
			get
			{
				return m_graph != null && !string.IsNullOrEmpty(m_nodeGroupHash);
			}
		}

		public UndoNodeGroupPush(BTEditorGraphNode node)
		{
			if(!(node.Node is NodeGroup))
				throw new System.ArgumentException("BT graph node is not of type NodeGroup", "node");

			m_graph = node.Graph;
			m_nodeGroupHash = m_graph.GetNodeHash(node);
			Title = "Open " + (string.IsNullOrEmpty(node.Node.Name) ? node.Node.Title : node.Node.Name);
		}

		public override void Undo()
		{
			m_graph.DecreaseEditingDepth();
		}

		public override void Redo()
		{
			BTEditorGraphNode node = m_graph.GetNodeByHash(m_nodeGroupHash);
			m_graph.IncreaseEditingDepth(node);
		}
	}
}