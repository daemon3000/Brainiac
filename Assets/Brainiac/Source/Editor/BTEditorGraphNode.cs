using UnityEngine;
using System;
using System.Collections.Generic;
using Brainiac;
using UnityEditor;

namespace BrainiacEditor
{
	public class BTEditorGraphNode : ScriptableObject
	{
		private const int DRAG_MOUSE_BUTTON = 0;
		private const int SELECT_MOUSE_BUTTON = 0;
		private const int CONTEXT_MOUSE_BUTTON = 1;

		private List<BTEditorGraphNode> m_children;
		private BehaviourNode m_node;
		private BTEditorGraphNode m_parent;
		private BTEditorGraph m_graph;
		private bool m_isSelected;
		private bool m_isDragging;
		private Vector2 m_dragOffset;

		public BehaviourNode Node
		{
			get { return m_node; }
		}

		private void OnCreated()
		{
			if(m_children == null)
			{
				m_children = new List<BTEditorGraphNode>();
			}

			m_isSelected = false;
			m_isDragging = false;
			m_dragOffset = Vector2.zero;
		}

		public void DrawGUI()
		{
			DrawTransitions();
			DrawNode();
			HandleEvents();
			DrawChildren();
		}

		private void DrawTransitions()
		{
			Rect position = new Rect(m_node.Position + BTEditorCanvas.Current.Position, m_node.Size);

			foreach(var child in m_children)
			{
				Rect childPosition = new Rect(child.Node.Position + BTEditorCanvas.Current.Position, child.Node.Size);
				BTEditorUtils.DrawBezier(position, childPosition);
			}
		}

		private void DrawNode()
		{
			BTGraphNodeStyle nodeStyle = BTEditorStyle.GetNodeStyle(m_node.GetType());
			Rect position = new Rect(m_node.Position + BTEditorCanvas.Current.Position, m_node.Size);
			EditorGUI.LabelField(position, m_node.Title, nodeStyle.GetStyle(m_isSelected));
		}

		private void HandleEvents()
		{
			Rect position = new Rect(m_node.Position, m_node.Size);
			Vector2 mousePosition = BTEditorCanvas.Current.WindowSpaceToCanvasSpace(BTEditorCanvas.Current.Event.mousePosition);

			if(BTEditorCanvas.Current.Event.type == EventType.MouseDown && BTEditorCanvas.Current.Event.button == SELECT_MOUSE_BUTTON)
			{
				if(position.Contains(mousePosition))
				{
					if(!m_isSelected)
					{
						m_graph.OnNodeSelected(this);
					}
					if(BTEditorCanvas.Current.CanEdit)
					{
						m_graph.OnNodeBeginDrag(this, mousePosition);
					}
					BTEditorCanvas.Current.Event.Use();
				}
			}
			else if(BTEditorCanvas.Current.Event.type == EventType.MouseDown && BTEditorCanvas.Current.Event.button == CONTEXT_MOUSE_BUTTON)
			{
				if(BTEditorCanvas.Current.CanEdit && position.Contains(mousePosition))
				{
					ShowContextMenu();
					BTEditorCanvas.Current.Event.Use();
				}
			}
			else if(BTEditorCanvas.Current.Event.type == EventType.MouseUp && BTEditorCanvas.Current.Event.button == SELECT_MOUSE_BUTTON)
			{
				if(m_isDragging)
				{
					m_graph.OnNodeEndDrag(this);
					BTEditorCanvas.Current.Event.Use();
				}
			}
			else if(BTEditorCanvas.Current.Event.type == EventType.MouseDrag && BTEditorCanvas.Current.Event.button == DRAG_MOUSE_BUTTON)
			{
				if(m_isDragging)
				{
					m_graph.OnNodeDrag(this, mousePosition);
					BTEditorCanvas.Current.Event.Use();
				}
			}
			else if(m_graph.SelectionBox.HasValue)
			{
				if(m_graph.SelectionBox.Value.Contains(position.center))
				{
					if(!m_isSelected)
					{
						m_graph.OnNodeSelected(this);
					}
				}
				else
				{
					if(m_isSelected)
					{
						m_graph.OnNodeDeselected(this);
					}
				}
			}
		}

		private void DrawChildren()
		{
			foreach(var child in m_children)
			{
				child.DrawGUI();
			}
		}

		public void OnSelected()
		{
			m_isSelected = true;
			Selection.activeObject = this;
			BTEditorCanvas.Current.Repaint();
		}

		public void OnDeselected()
		{
			m_isSelected = false;
			m_isDragging = false;
			if(Selection.activeObject == this)
			{
				Selection.activeObject = null;
			}
			BTEditorCanvas.Current.Repaint();
		}

		public void OnBeginDrag(Vector2 position)
		{
			m_dragOffset = position - m_node.Position;
			m_isDragging = true;
		}

		public void OnDrag(Vector2 position)
		{
			Vector2 nodePos = position - m_dragOffset;
			if(BTEditorCanvas.Current.SnapToGrid)
			{
				float snapSize = BTEditorCanvas.Current.SnapSize;
				nodePos.x = (float)Math.Round(nodePos.x / snapSize) * snapSize;
				nodePos.y = (float)Math.Round(nodePos.y / snapSize) * snapSize;
			}

			nodePos.x = Mathf.Max(nodePos.x, 0.0f);
			nodePos.y = Mathf.Max(nodePos.y, 0.0f);

			m_node.Position = nodePos;

			BTEditorCanvas.Current.Repaint();
		}

		public void OnEndDrag()
		{
			m_isDragging = false;
		}

		private void SetNode(BehaviourNode node)
		{
			DeleteChildren();

			m_node = node;
			m_isSelected = false;

			if(node is Composite)
			{
				Composite composite = node as Composite;
				for(int i = 0; i < composite.ChildCount; i++)
				{
					BehaviourNode childNode = composite.GetChild(i);
					BTEditorGraphNode graphNode = BTEditorGraphNode.Create(m_graph, this, childNode);
					m_children.Add(graphNode);
				}
			}
			else if(node is Decorator)
			{
				Decorator decorator = node as Decorator;
				BehaviourNode childNode = decorator.GetChild();
				if(childNode != null)
				{
					BTEditorGraphNode graphNode = BTEditorGraphNode.Create(m_graph, this, childNode);
					m_children.Add(graphNode);
				}
			}
		}

		private void ShowContextMenu()
		{
			GenericMenu menu = BTEditorUtils.CreateContextMenu(m_node, CreateChild, Delete, DeleteChildren);
			menu.DropDown(new Rect(BTEditorCanvas.Current.Event.mousePosition, Vector2.zero));
		}

		private void CreateChild(object childType)
		{
			Type type = childType as Type;
			if(type != null)
			{
				BehaviourNode node = BTUtils.CreateNode(type);
				if(node != null)
				{
					Vector2 nodePos = m_node.Position + node.Size * 1.5f;
					nodePos.x = Mathf.Max(nodePos.x, 0.0f);
					nodePos.y = Mathf.Max(nodePos.y, 0.0f);

					node.Position = nodePos;

					if(m_node is Composite)
					{
						Composite composite = m_node as Composite;
						composite.AddChild(node);
					}
					else if(m_node is Decorator)
					{
						Decorator decorator = m_node as Decorator;

						DeleteChildren();
						decorator.ReplaceChild(node);
					}

					BTEditorGraphNode graphNode = BTEditorGraphNode.Create(m_graph, this, node);
					m_children.Add(graphNode);
					
					Vector2 canvasSize = BTEditorCanvas.Current.Size;
					canvasSize.x = Mathf.Max(node.Position.x + 250.0f, canvasSize.x);
					canvasSize.y = Mathf.Max(node.Position.y + 250.0f, canvasSize.y);

					BTEditorCanvas.Current.Size = canvasSize;
				}
			}
		}

		private void Delete()
		{
			if(m_parent != null)
			{
				m_parent.RemoveChild(this);
			}

			BTEditorGraphNode.DestroyImmediate(this);
		}

		private void DeleteChildren()
		{
			for(int i = 0; i < m_children.Count; i++)
			{
				BTEditorGraphNode.DestroyImmediate(m_children[i]);
			}

			if(m_node is Composite)
			{
				((Composite)m_node).RemoveAllChildren();
			}
			else if(m_node is Decorator)
			{
				((Decorator)m_node).RemoveChild();
			}

			m_children.Clear();
		}

		private void RemoveChild(BTEditorGraphNode child)
		{
			if(m_children.Remove(child))
			{
				if(m_node is Composite)
				{
					Composite composite = m_node as Composite;
					composite.RemoveChild(child.Node);
				}
				else if(m_node is Decorator)
				{
					Decorator decorator = m_node as Decorator;
					decorator.RemoveChild();
				}
			}
		}

		private void OnDestroy()
		{
			if(m_isSelected)
			{
				m_graph.RemoveNodeFromSelection(this);
				if(Selection.activeObject == this)
				{
					Selection.activeObject = null;
				}
			}

			foreach(var child in m_children)
			{
				BTEditorGraphNode.DestroyImmediate(child);
			}
		}

		public static BTEditorGraphNode Create(BTEditorGraph graph, BTEditorGraphNode parent, BehaviourNode node)
		{
			BTEditorGraphNode graphNode = ScriptableObject.CreateInstance<BTEditorGraphNode>();
			graphNode.OnCreated();
			graphNode.hideFlags = HideFlags.HideAndDontSave;
			graphNode.m_parent = parent;
			graphNode.m_graph = graph;
			graphNode.SetNode(node);

			return graphNode;
		}
	}
}