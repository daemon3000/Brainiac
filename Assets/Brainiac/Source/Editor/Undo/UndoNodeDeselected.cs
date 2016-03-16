using UnityEngine;
using System;
using System.Collections;

namespace BrainiacEditor
{
	public class UndoNodeDeselected : BTUndoState
	{
		private BTEditorGraph m_graph;
		private string m_selectedNodePath;

		public override bool CanUndo
		{
			get
			{
				return m_graph != null && m_selectedNodePath != null;
			}
		}

		public override bool CanRedo
		{
			get
			{
				return m_graph != null && m_selectedNodePath != null;
			}
		}

		public UndoNodeDeselected(BTEditorGraph graph, BTEditorGraphNode node)
		{
			m_graph = graph;
			m_selectedNodePath = m_graph.GetNodePath(node);
			Title = "Selection changed";
		}

		public override void Undo()
		{
			if(CanUndo)
			{
				var selectedNode = m_graph.GetNodeAtPath(m_selectedNodePath);
				m_graph.AddNodeToSelection(selectedNode);
				selectedNode.OnSelected();
			}
		}

		public override void Redo()
		{
			if(CanRedo)
			{
				var selectedNode = m_graph.GetNodeAtPath(m_selectedNodePath);
				m_graph.RemoveNodeFromSelection(selectedNode);
				selectedNode.OnDeselected();
			}
		}
	}
}