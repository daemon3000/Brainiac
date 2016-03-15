using UnityEngine;
using System;
using System.Collections;

namespace BrainiacEditor
{
	public class UndoNodeSelected : BTUndoState
	{
		private BTEditorGraph m_graph;
		private BTEditorGraphNode m_selectedNode;

		public override bool CanUndo
		{
			get
			{
				return m_graph != null && m_selectedNode != null;
			}
		}

		public override bool CanRedo
		{
			get
			{
				return m_graph != null && m_selectedNode != null;
			}
		}

		public UndoNodeSelected(BTEditorGraph graph, BTEditorGraphNode node)
		{
			m_graph = graph;
			m_selectedNode = node;
			Title = "Selection changed";
		}

		public override void Undo()
		{
			if(CanUndo)
			{
				m_graph.RemoveNodeFromSelection(m_selectedNode);
				m_selectedNode.OnDeselected();
			}
		}

		public override void Redo()
		{
			if(CanRedo)
			{
				m_graph.AddNodeToSelection(m_selectedNode);
				m_selectedNode.OnSelected();
			}
		}
	}
}