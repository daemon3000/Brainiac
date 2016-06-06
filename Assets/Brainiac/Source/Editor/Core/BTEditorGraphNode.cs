using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	public class BTEditorGraphNode : ScriptableObject
	{
		private const int DRAG_MOUSE_BUTTON = 0;
		private const int SELECT_MOUSE_BUTTON = 0;
		private const int CONTEXT_MOUSE_BUTTON = 1;
		private const float DOUBLE_CLICK_THRESHOLD = 0.4f;

		private List<BTEditorGraphNode> m_children;
		private BehaviourNode m_node;
		private BTEditorGraphNode m_parent;
		private BTEditorGraph m_graph;
		private Vector2 m_dragOffset;
		private float? m_lastClickTime;
		private bool m_isSelected;
		private bool m_isDragging;
		private bool m_canBeginDragging;
		
		public BehaviourNode Node
		{
			get { return m_node; }
		}

		public BTEditorGraphNode Parent
		{
			get { return m_parent; }
		}

		public BTEditorGraph Graph
		{
			get { return m_graph; }
		}

		public int ChildCount
		{
			get { return m_children.Count; }
		}

		public bool IsRoot
		{
			get { return m_graph.IsRoot(this); }
		}

		private bool CanUpdateChildren
		{
			get
			{
				return !(m_node is NodeGroup) || m_graph.IsRoot(this);
			}
		}

		private bool CanDrawChildren
		{
			get
			{
				return !(m_node is NodeGroup) || m_graph.IsRoot(this);
			}
		}

		private Vector2 NodePositon
		{
			get
			{
				return IsRoot ? Vector2.zero : m_node.Position;
			}
			set
			{
				if(!IsRoot)
				{
					m_node.Position = value;
				}
			}
		}
		
		private void OnCreated()
		{
			if(m_children == null)
			{
				m_children = new List<BTEditorGraphNode>();
			}
			
			m_isSelected = false;
			m_isDragging = false;
			m_canBeginDragging = false;
			m_dragOffset = Vector2.zero;
			m_lastClickTime = null;
		}

		public void Update()
		{
			if(CanUpdateChildren)
				UpdateChildren();

			HandleEvents();
		}

		private void UpdateChildren()
		{
			for(int i = m_children.Count - 1; i >= 0; i--)
			{
				m_children[i].Update();
			}
		}

		private void HandleEvents()
		{
			Rect position = new Rect(NodePositon, BTEditorStyle.GetNodeSize(m_node));
			Vector2 mousePosition = BTEditorCanvas.Current.WindowSpaceToCanvasSpace(BTEditorCanvas.Current.Event.mousePosition);

			if(BTEditorCanvas.Current.Event.type == EventType.MouseDown && BTEditorCanvas.Current.Event.button == SELECT_MOUSE_BUTTON)
			{
				if(position.Contains(mousePosition))
				{
					if(!m_isSelected)
					{
						m_graph.OnNodeSelected(this);
					}

					if(m_lastClickTime.HasValue)
					{
						if(Time.realtimeSinceStartup <= m_lastClickTime.Value + DOUBLE_CLICK_THRESHOLD)
						{
							OnDoubleClicked();
						}
						m_lastClickTime = null;
					}
					else
					{
						m_lastClickTime = Time.realtimeSinceStartup;
					}

					m_canBeginDragging = !IsRoot;
					BTEditorCanvas.Current.Event.Use();
				}
			}
			else if(BTEditorCanvas.Current.Event.type == EventType.MouseDown && BTEditorCanvas.Current.Event.button == CONTEXT_MOUSE_BUTTON)
			{
				if(position.Contains(mousePosition))
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
				m_canBeginDragging = false;
			}
			else if(BTEditorCanvas.Current.Event.type == EventType.MouseDrag && BTEditorCanvas.Current.Event.button == DRAG_MOUSE_BUTTON)
			{
				if(!m_graph.ReadOnly && !m_isDragging && m_canBeginDragging && position.Contains(mousePosition))
				{
					m_graph.OnNodeBeginDrag(this, mousePosition);
					BTEditorCanvas.Current.Event.Use();
				}
				else if(m_isDragging)
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

		public void Draw()
		{
			if(CanDrawChildren)
				DrawTransitions();

			DrawSelf();

			if(CanDrawChildren)
				DrawChildren();
		}

		private void DrawTransitions()
		{
			Vector2 nodeSize = BTEditorStyle.GetNodeSize(m_node);
			Rect position = new Rect(NodePositon + BTEditorCanvas.Current.Position, nodeSize);
			BTEditorTreeLayout treeLayout = BTEditorStyle.TreeLayout;

			foreach(var child in m_children)
			{
				Vector2 childNodeSize = BTEditorStyle.GetNodeSize(child.Node);
				Rect childPosition = new Rect(child.Node.Position + BTEditorCanvas.Current.Position, childNodeSize);
				BehaviourNodeStatus childStatus = BTEditorCanvas.Current.IsDebuging ? child.Node.Status : BehaviourNodeStatus.None;
				Color color = BTEditorStyle.GetTransitionColor(childStatus);
				Vector2 nodeCenter = position.center;
				Vector2 childCenter = childPosition.center;

				if(treeLayout == BTEditorTreeLayout.Horizontal)
				{
					if(Mathf.Approximately(nodeCenter.y, childCenter.y) || Mathf.Approximately(nodeCenter.x, childCenter.x))
					{
						BTEditorUtils.DrawLine(nodeCenter, childCenter, color);
					}
					else
					{
						BTEditorUtils.DrawLine(nodeCenter, nodeCenter + Vector2.right * (childCenter.x - nodeCenter.x) / 2, color);

						BTEditorUtils.DrawLine(nodeCenter + Vector2.right * (childCenter.x - nodeCenter.x) / 2,
											   childCenter + Vector2.right * (nodeCenter.x - childCenter.x) / 2, color);

						BTEditorUtils.DrawLine(childCenter, childCenter + Vector2.right * (nodeCenter.x - childCenter.x) / 2, color);
					}
				}
				else if(treeLayout == BTEditorTreeLayout.Vertical)
				{
					if(Mathf.Approximately(nodeCenter.y, childCenter.y) || Mathf.Approximately(nodeCenter.x, childCenter.x))
					{
						BTEditorUtils.DrawLine(nodeCenter, childCenter, color);
					}
					else
					{
						BTEditorUtils.DrawLine(nodeCenter, nodeCenter + Vector2.up * (childCenter.y - nodeCenter.y) / 2, color);

						BTEditorUtils.DrawLine(nodeCenter + Vector2.up * (childCenter.y - nodeCenter.y) / 2,
											   childCenter + Vector2.up * (nodeCenter.y - childCenter.y) / 2, color);

						BTEditorUtils.DrawLine(childCenter, childCenter + Vector2.up * (nodeCenter.y - childCenter.y) / 2, color);
					}
				}
				else
				{
					BTEditorUtils.DrawLine(nodeCenter, childCenter, color);
				}
			}
		}

		private void DrawSelf()
		{
			string label = string.IsNullOrEmpty(m_node.Name) ? m_node.Title : m_node.Name;
			BTGraphNodeStyle nodeStyle = BTEditorStyle.GetNodeStyle(m_node);
			Vector2 nodeSize = BTEditorStyle.GetNodeSize(m_node);
			Rect position = new Rect(NodePositon + BTEditorCanvas.Current.Position, nodeSize);
			BehaviourNodeStatus status = BTEditorCanvas.Current.IsDebuging ? m_node.Status : BehaviourNodeStatus.None;

			EditorGUI.LabelField(position, label, nodeStyle.GetStyle(status, m_isSelected));

			if(m_node.Breakpoint != Breakpoint.None)
			{
				Rect imgPosition;
				if(m_node is Brainiac.Action)
				{
					imgPosition = new Rect(position.x + 4, position.yMax - 20, 16, 16);
				}
				else
				{
					imgPosition = new Rect(position.x + 22, position.yMax - 14, 16, 16);
				}
				
				GUI.DrawTexture(imgPosition, BTEditorStyle.Breakpoint);
			}
		}

		private void DrawChildren()
		{
			for(int i = 0; i < m_children.Count; i++)
			{
				m_children[i].Draw();
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
			m_dragOffset = position - NodePositon;
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

			NodePositon = nodePos;

			BTEditorCanvas.Current.RecalculateSize(NodePositon);
			BTEditorCanvas.Current.Repaint();
		}

		public void OnEndDrag()
		{
			m_isDragging = false;
		}

		private void OnDoubleClicked()
		{
			if(m_node is RunBehaviour)
			{
				RunBehaviour rb = (RunBehaviour)m_node;
				if(rb.BehaviourTreeAsset != null)
				{
					if(BTEditorCanvas.Current.IsDebuging && rb.BehaviourTree != null)
					{
						BehaviourTreeEditor.OpenSubtreeDebug(rb.BehaviourTreeAsset, rb.BehaviourTree);
					}
					else
					{
						BehaviourTreeEditor.OpenSubtree(rb.BehaviourTreeAsset);
					}
				}
			}
			else if(m_node is NodeGroup)
			{
				if(IsRoot)
					m_graph.OnPopNodeGroup();
				else
					m_graph.OnPushNodeGroup(this);
			}
		}

		private void SetExistingNode(BehaviourNode node)
		{
			DestroyChildren();

			m_node = node;
			m_isSelected = false;

			if(node is Composite)
			{
				Composite composite = node as Composite;
				for(int i = 0; i < composite.ChildCount; i++)
				{
					BehaviourNode childNode = composite.GetChild(i);
					BTEditorGraphNode graphNode = BTEditorGraphNode.CreateExistingNode(this, childNode);
					m_children.Add(graphNode);
				}
			}
			else if(node is Decorator)
			{
				Decorator decorator = node as Decorator;
				BehaviourNode childNode = decorator.GetChild();
				if(childNode != null)
				{
					BTEditorGraphNode graphNode = BTEditorGraphNode.CreateExistingNode(this, childNode);
					m_children.Add(graphNode);
				}
			}
		}

		private void ShowContextMenu()
		{
			GenericMenu menu = BTContextMenuFactory.CreateNodeContextMenu(this);
			menu.DropDown(new Rect(BTEditorCanvas.Current.Event.mousePosition, Vector2.zero));
		}

		public BTEditorGraphNode OnCreateChild(Type type)
		{
			if(type != null)
			{
				BehaviourNode node = BTUtils.CreateNode(type);
				if(node != null)
				{
					Vector2 nodeSize = BTEditorStyle.GetNodeSize(node);
					Vector2 nodePos = NodePositon + nodeSize * 1.5f;
					nodePos.x = Mathf.Max(nodePos.x, 0.0f);
					nodePos.y = Mathf.Max(nodePos.y, 0.0f);

					node.Position = nodePos;

					return OnCreateChild(node);
				}
			}

			return null;
		}

		public BTEditorGraphNode OnCreateChild(BehaviourNode node)
		{
			if(node != null && ((m_node is Composite) || (m_node is Decorator)))
			{
				if(m_node is Composite)
				{
					Composite composite = m_node as Composite;
					composite.AddChild(node);
				}
				else if(m_node is Decorator)
				{
					Decorator decorator = m_node as Decorator;

					DestroyChildren();
					decorator.SetChild(node);
				}

				BTEditorGraphNode graphNode = BTEditorGraphNode.CreateExistingNode(this, node);
				m_children.Add(graphNode);

				BTEditorCanvas.Current.RecalculateSize(node.Position);
				return graphNode;
			}

			return null;
		}

		public BTEditorGraphNode OnInsertChild(int index, BehaviourNode node)
		{
			if(node != null && ((m_node is Composite) || (m_node is Decorator)))
			{
				BTEditorGraphNode graphNode = null;

				if(m_node is Composite)
				{
					Composite composite = m_node as Composite;
					composite.InsertChild(index, node);

					graphNode = BTEditorGraphNode.CreateExistingNode(this, node);
					m_children.Insert(index, graphNode);
				}
				else if(m_node is Decorator)
				{
					Decorator decorator = m_node as Decorator;

					DestroyChildren();
					decorator.SetChild(node);

					graphNode = BTEditorGraphNode.CreateExistingNode(this, node);
					m_children.Add(graphNode);
				}

				BTEditorCanvas.Current.RecalculateSize(node.Position);
				return graphNode;
			}

			return null;
		}

		public void OnDelete()
		{
			if(m_parent != null)
			{
				m_parent.RemoveChild(this);
				BTEditorGraphNode.DestroyImmediate(this);
			}
		}

		public void OnDeleteChild(int index)
		{
			BTEditorGraphNode child = GetChild(index);
			if(child != null)
			{
				child.OnDelete();
			}
		}

		public int GetChildIndex(BTEditorGraphNode child)
		{
			return m_children.IndexOf(child);
		}

		public BTEditorGraphNode GetChild(int index)
		{
			if(index >= 0 && index < m_children.Count)
			{
				return m_children[index];
			}

			return null;
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
					decorator.SetChild(null);
				}
			}
		}

		private void DestroyChildren()
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
				((Decorator)m_node).SetChild(null);
			}

			m_children.Clear();
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

		private static BTEditorGraphNode CreateEmptyNode()
		{
			BTEditorGraphNode graphNode = ScriptableObject.CreateInstance<BTEditorGraphNode>();
			graphNode.OnCreated();
			graphNode.hideFlags = HideFlags.HideAndDontSave;

			return graphNode;
		}

		private static BTEditorGraphNode CreateExistingNode(BTEditorGraphNode parent, BehaviourNode node)
		{
			BTEditorGraphNode graphNode = BTEditorGraphNode.CreateEmptyNode();
			graphNode.m_parent = parent;
			graphNode.m_graph = parent.Graph;
			graphNode.SetExistingNode(node);

			return graphNode;
		}

		public static BTEditorGraphNode Create(BTEditorGraph graph, Root node)
		{
			if(graph != null && node != null)
			{
				BTEditorGraphNode graphNode = CreateEmptyNode();
				graphNode.m_graph = graph;
				graphNode.m_parent = null;
				graphNode.SetExistingNode(node);

				return graphNode;
			}

			return null;
		}

		public static BTEditorGraphNode Create(BTEditorGraphNode parent, BehaviourNode node)
		{
			if(parent != null && node != null)
			{
				return parent.OnCreateChild(node);
			}

			return null;
		}

		public static BTEditorGraphNode Create(BTEditorGraphNode parent, BehaviourNode node, int index)
		{
			if(parent != null && node != null)
			{
				return parent.OnInsertChild(index, node);
			}

			return null;
		}
	}
}