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
		private bool m_isSelected;
		private Vector2 m_dragOffset;

		public BehaviourNode Node
		{
			get { return m_node; }
		}

		private void Initialize()
		{
			if(m_children == null)
			{
				m_children = new List<BTEditorGraphNode>();
			}

			m_isSelected = false;
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
			BTGraphNodeStyle nodeStyle = BTEditorStyle.GetNodeStyle(m_node.GetType());
			Rect position = new Rect(m_node.Position + BTEditorCanvas.Current.Position, nodeStyle.Size);

			foreach(var child in m_children)
			{
				BTGraphNodeStyle childNodeStyle = BTEditorStyle.GetNodeStyle(child.Node.GetType());
				Rect childPosition = new Rect(child.Node.Position + BTEditorCanvas.Current.Position, childNodeStyle.Size);
				BTEditorUtils.DrawBezier(position, childPosition);
			}
		}

		private void DrawNode()
		{
			BTGraphNodeStyle nodeStyle = BTEditorStyle.GetNodeStyle(m_node.GetType());
			Rect position = new Rect(m_node.Position + BTEditorCanvas.Current.Position, nodeStyle.Size);
			EditorGUI.LabelField(position, m_node.Title, nodeStyle.GetStyle(m_isSelected));
		}

		private void HandleEvents()
		{
			BTGraphNodeStyle nodeStyle = BTEditorStyle.GetNodeStyle(m_node.GetType());
			Rect position = new Rect(m_node.Position, nodeStyle.Size);
			Vector2 mousePosition = GetMousePositionInCanvasSpace();

			if(BTEditorCanvas.Current.Event.type == EventType.MouseDown && BTEditorCanvas.Current.Event.button == SELECT_MOUSE_BUTTON)
			{
				if(position.Contains(mousePosition))
				{
					m_dragOffset = GetMousePositionInCanvasSpace() - m_node.Position;
					BTEditorCanvas.Current.Event.Use();
					BTEditorCanvas.Current.ReplaceSelection(this);
					BTEditorCanvas.Current.Repaint();
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
				if(m_isSelected)
				{
					BTEditorCanvas.Current.RecalculateCanvasSize(this);
				}
			}
			else if(BTEditorCanvas.Current.Event.type == EventType.MouseDrag && BTEditorCanvas.Current.Event.button == DRAG_MOUSE_BUTTON)
			{
				if(BTEditorCanvas.Current.CanEdit && m_isSelected)
				{
					Vector2 nodePos = mousePosition - m_dragOffset;
					if(BTEditorCanvas.Current.SnapToGrid)
					{
						float snapSize = BTEditorCanvas.Current.SnapSize;
						nodePos.x = (float)Math.Round(nodePos.x / snapSize) * snapSize;
						nodePos.y = (float)Math.Round(nodePos.y / snapSize) * snapSize;
					}

					nodePos.x = Mathf.Max(nodePos.x, 0.0f);
					nodePos.y = Mathf.Max(nodePos.y, 0.0f);

					m_node.Position = nodePos;
					BTEditorCanvas.Current.Event.Use();
					BTEditorCanvas.Current.Repaint();
				}
			}
		}

		private Vector2 GetMousePositionInCanvasSpace()
		{
			return BTEditorCanvas.Current.Event.mousePosition - BTEditorCanvas.Current.Position;
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
		}

		public void OnDeselected()
		{
			m_isSelected = false;
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
					BTEditorGraphNode graphNode = BTEditorGraphNode.Create(this, childNode);
					m_children.Add(graphNode);
				}
			}
			else if(node is Decorator)
			{
				Decorator decorator = node as Decorator;
				BehaviourNode childNode = decorator.GetChild();
				if(childNode != null)
				{
					BTEditorGraphNode graphNode = BTEditorGraphNode.Create(this, childNode);
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
					BTGraphNodeStyle nodeStyle = BTEditorStyle.GetNodeStyle(node.GetType());
					m_node.Position = m_node.Position + nodeStyle.Size * 1.5f;

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

					BTEditorGraphNode graphNode = BTEditorGraphNode.Create(this, node);
					m_children.Add(graphNode);
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
			BTEditorCanvas.Current.RemoveFromSelection(this);
			foreach(var child in m_children)
			{
				BTEditorGraphNode.DestroyImmediate(child);
			}
		}

		public static BTEditorGraphNode Create(BTEditorGraphNode parent, BehaviourNode node)
		{
			BTEditorGraphNode graphNode = ScriptableObject.CreateInstance<BTEditorGraphNode>();
			graphNode.Initialize();
			graphNode.hideFlags = HideFlags.HideAndDontSave;
			graphNode.m_parent = parent;
			graphNode.SetNode(node);

			return graphNode;
		}
	}
}