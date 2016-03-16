using UnityEngine;
using System;
using Brainiac;

namespace BrainiacEditor
{
	public class UndoNodeCreated : BTUndoState
	{
		private BTEditorGraph m_graph;
		private string m_createdNodePath;
		private string m_parentNodePath;
		private string m_serializedNode;

		public override bool CanUndo
		{
			get
			{
				return m_createdNodePath != null && m_graph != null;
			}
		}

		public override bool CanRedo
		{
			get
			{
				return m_parentNodePath != null && m_graph != null && !string.IsNullOrEmpty(m_serializedNode);
			}
		}

		public UndoNodeCreated(BTEditorGraphNode node)
		{
			m_graph = node.Graph;
			m_createdNodePath = m_graph.GetNodePath(node);
			m_parentNodePath = null;
			m_serializedNode = null;
			Title = "Created " + node.Node.Title;
		}

		public override void Undo()
		{
			if(CanUndo)
			{
				BTEditorGraphNode createdNode = m_graph.GetNodeAtPath(m_createdNodePath);

				m_parentNodePath = m_graph.GetNodePath(createdNode.Parent);
				m_serializedNode = BTUtils.SaveNode(createdNode.Node);

				createdNode.OnDelete();
				m_createdNodePath = null;
			}
		}

		public override void Redo()
		{
			if(CanRedo)
			{
				BTEditorGraphNode parentNode = m_graph.GetNodeAtPath(m_parentNodePath);
				BehaviourNode node = BTUtils.LoadNode(m_serializedNode);
				BTEditorGraphNode createdNode = BTEditorGraphNode.Create(parentNode, node);

				m_createdNodePath = m_graph.GetNodePath(createdNode);
				m_parentNodePath = null;
				m_serializedNode = null;
			}
		}
	}
}