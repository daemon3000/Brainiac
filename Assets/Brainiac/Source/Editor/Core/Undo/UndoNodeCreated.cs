using UnityEngine;
using System;
using Brainiac;

namespace BrainiacEditor
{
	public class UndoNodeCreated : BTUndoState
	{
		private BTEditorGraph m_graph;
		private string m_createdNodeHash;
		private string m_parentNodeHash;
		private string m_serializedNode;

		public override bool CanUndo
		{
			get
			{
				return m_createdNodeHash != null && m_graph != null;
			}
		}

		public override bool CanRedo
		{
			get
			{
				return m_parentNodeHash != null && m_graph != null && !string.IsNullOrEmpty(m_serializedNode);
			}
		}

		public UndoNodeCreated(BTEditorGraphNode node)
		{
			m_graph = node.Graph;
			m_createdNodeHash = m_graph.GetNodeHash(node);
			m_parentNodeHash = null;
			m_serializedNode = null;
			Title = "Created " + node.Node.Title;
		}

		public override void Undo()
		{
			if(CanUndo)
			{
				BTEditorGraphNode createdNode = m_graph.GetNodeByHash(m_createdNodeHash);

				m_parentNodeHash = m_graph.GetNodeHash(createdNode.Parent);
				m_serializedNode = BTUtils.SaveNode(createdNode.Node);

				createdNode.OnDelete();
				m_createdNodeHash = null;
			}
		}

		public override void Redo()
		{
			if(CanRedo)
			{
				BTEditorGraphNode parentNode = m_graph.GetNodeByHash(m_parentNodeHash);
				BehaviourNode node = BTUtils.LoadNode(m_serializedNode);
				BTEditorGraphNode createdNode = BTEditorGraphNode.Create(parentNode, node);

				m_createdNodeHash = m_graph.GetNodeHash(createdNode);
				m_parentNodeHash = null;
				m_serializedNode = null;
			}
		}
	}
}