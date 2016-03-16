using UnityEngine;
using System;
using System.Collections;

namespace BrainiacEditor
{
	public class UndoNodeMoved : BTUndoState
	{
		private BTEditorGraph m_graph;
		private string m_nodePath;
		private Vector2 m_startPosition;
		private Vector2 m_endPosition;

		public override bool CanUndo
		{
			get
			{
				return m_nodePath != null;
			}
		}

		public override bool CanRedo
		{
			get
			{
				return m_nodePath != null;
			}
		}

		public UndoNodeMoved(BTEditorGraphNode node)
		{
			m_graph = node.Graph;
			m_nodePath = m_graph.GetNodePath(node);
			m_startPosition = node.Node.Position;
			m_endPosition = Vector2.zero;
		}

		public override void Undo()
		{
			if(CanUndo)
			{
				var node = m_graph.GetNodeAtPath(m_nodePath);
				m_endPosition = node.Node.Position;
				node.Node.Position = m_startPosition;
			}
		}

		public override void Redo()
		{
			if(CanRedo)
			{
				var node = m_graph.GetNodeAtPath(m_nodePath);
				node.Node.Position = m_endPosition;
			}
		}
	}
}