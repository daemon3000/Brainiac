using UnityEngine;
using System;
using Brainiac;

namespace BrainiacEditor
{
	public class UndoNodeDeleted : BTUndoState
	{
		private BTEditorGraph m_graph;
		private string m_createdNodeHash;
		private string m_parentNodeHash;
		private string m_serializedNode;
		private int m_childIndex;

		public override bool CanUndo
		{
			get
			{
				return m_parentNodeHash != null && !string.IsNullOrEmpty(m_serializedNode);
			}
		}

		public override bool CanRedo
		{
			get
			{
				return m_createdNodeHash != null;
			}
		}

		public UndoNodeDeleted(BTEditorGraphNode node)
		{
			m_graph = node.Graph;
			m_parentNodeHash = m_graph.GetNodeHash(node.Parent);
			m_serializedNode = BTUtils.SaveNode(node.Node);
			m_childIndex = node.Parent.GetChildIndex(node);
			Title = "Deleted " + node.Node.Title;

			m_createdNodeHash = null;
		}

		public UndoNodeDeleted(BTEditorGraphNode node, int childIndex)
		{
			m_graph = node.Graph;
			m_parentNodeHash = m_graph.GetNodeHash(node.Parent);
			m_serializedNode = BTUtils.SaveNode(node.Node);
			m_childIndex = childIndex;
			Title = "Deleted " + node.Node.Title;

			m_createdNodeHash = null;
		}

		public override void Undo()
		{
			if(CanUndo)
			{
				BehaviourNode node = BTUtils.LoadNode(m_serializedNode);
				if(m_childIndex >= 0)
				{
					var parentNode = m_graph.GetNodeByHash(m_parentNodeHash);
					var createdNode = BTEditorGraphNode.Create(parentNode, node, m_childIndex);
					m_createdNodeHash = m_graph.GetNodeHash(createdNode);
				}
				else
				{
					var parentNode = m_graph.GetNodeByHash(m_parentNodeHash);
					var createdNode = BTEditorGraphNode.Create(parentNode, node);
					m_createdNodeHash = m_graph.GetNodeHash(createdNode);
				}

				m_parentNodeHash = null;
				m_serializedNode = null;
			}
		}

		public override void Redo()
		{
			if(CanRedo)
			{
				var createdNode = m_graph.GetNodeByHash(m_createdNodeHash);
				m_parentNodeHash = m_graph.GetNodeHash(createdNode.Parent);
				m_serializedNode = BTUtils.SaveNode(createdNode.Node);
				m_childIndex = createdNode.Parent.GetChildIndex(createdNode);

				createdNode.OnDelete();
				m_createdNodeHash = null;
			}
		}
	}
}