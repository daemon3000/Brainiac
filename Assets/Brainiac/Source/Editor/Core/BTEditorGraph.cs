using UnityEngine;
using System.Collections.Generic;
using Brainiac;
using UnityEditor;
using System;

namespace BrainiacEditor
{
	public class BTEditorGraph : ScriptableObject
	{
		private const int SELECT_MOUSE_BUTTON = 0;
		private const int CONTEXT_MOUSE_BUTTON = 1;

		private List<BTEditorGraphNode> m_selection;
		private BTEditorGraphNode m_root;
		private string m_serializedCopyTarget;
		private bool m_drawSelectionBox;
		private bool m_isBehaviourTreeReadOnly;
		private bool m_canBeginBoxSelection;
		private Vector2 m_selectionBoxStartPos;

		public bool ReadOnly
		{
			get
			{
				return m_isBehaviourTreeReadOnly || EditorApplication.isPlaying;
			}
		}

		public Rect? SelectionBox { get; set; }
		public Vector2 MinNodePosition { get; private set; }

		private void OnCreated()
		{
			m_root = null;
			m_drawSelectionBox = false;
			m_isBehaviourTreeReadOnly = false;
			m_selectionBoxStartPos = Vector2.zero;
			SelectionBox = null;
		}

		public void SetBehaviourTree(BehaviourTree behaviourTree)
		{
			if(m_root != null)
			{
				BTEditorGraphNode.DestroyImmediate(m_root);
				m_root = null;
			}

			m_isBehaviourTreeReadOnly = behaviourTree.ReadOnly;
			m_root = BTEditorGraphNode.Create(this, behaviourTree.Root);
			BTUndoSystem.Clear();
		}

		public void DrawGUI(Rect screenRect)
		{
			if(m_root != null)
			{
				MinNodePosition = screenRect.min;

				m_root.Update();
				m_root.Draw();
				DrawSelectionBox();
				HandleEvents(screenRect);
			}
		}

		private void DrawSelectionBox()
		{
			if(m_drawSelectionBox)
			{
				Vector2 mousePosition = BTEditorCanvas.Current.Event.mousePosition;
				Rect position = new Rect();
				position.x = Mathf.Min(m_selectionBoxStartPos.x, mousePosition.x);
				position.y = Mathf.Min(m_selectionBoxStartPos.y, mousePosition.y);
				position.width = Mathf.Abs(mousePosition.x - m_selectionBoxStartPos.x);
				position.height = Mathf.Abs(mousePosition.y - m_selectionBoxStartPos.y);

				GUI.Box(position, "", BTEditorStyle.SelectionBox);
				BTEditorCanvas.Current.Repaint();

				SelectionBox = new Rect(BTEditorCanvas.Current.WindowSpaceToCanvasSpace(position.position), position.size);
			}
			else
			{
				SelectionBox = null;
			}
		}

		private void HandleEvents(Rect screenRect)
		{
			if(BTEditorCanvas.Current.Event.type == EventType.MouseDown && BTEditorCanvas.Current.Event.button == SELECT_MOUSE_BUTTON)
			{
				if(screenRect.Contains(BTEditorCanvas.Current.Event.mousePosition))
				{
					ClearSelection();

					m_canBeginBoxSelection = true;
					m_selectionBoxStartPos = BTEditorCanvas.Current.Event.mousePosition;
					BTEditorCanvas.Current.Event.Use();
				}
			}
			if(!ReadOnly)
			{
				if(BTEditorCanvas.Current.Event.type == EventType.MouseDrag && BTEditorCanvas.Current.Event.button == SELECT_MOUSE_BUTTON)
				{
					if(screenRect.Contains(BTEditorCanvas.Current.Event.mousePosition))	
					{
						if(!m_drawSelectionBox && m_canBeginBoxSelection)
						{
							m_drawSelectionBox = true;
						}

						BTEditorCanvas.Current.Event.Use();
					}
				}
				else if(BTEditorCanvas.Current.Event.type == EventType.MouseUp)
				{
					if(screenRect.Contains(BTEditorCanvas.Current.Event.mousePosition))
					{
						if(BTEditorCanvas.Current.Event.button == SELECT_MOUSE_BUTTON)
						{
							if(m_drawSelectionBox)
							{
								m_drawSelectionBox = false;
							}

							BTEditorCanvas.Current.Event.Use();
						}
						else if(BTEditorCanvas.Current.Event.button == CONTEXT_MOUSE_BUTTON)
						{
							GenericMenu menu = BTEditorUtils.CreateGraphContextMenu();
							menu.DropDown(new Rect(BTEditorCanvas.Current.Event.mousePosition, Vector2.zero));

							BTEditorCanvas.Current.Event.Use();
						}
					}

					m_canBeginBoxSelection = false;
				}
			}
		}

		public void OnNodeSelected(BTEditorGraphNode node)
		{
			if(BTEditorCanvas.Current.Event.control || SelectionBox.HasValue)
			{
				if(!m_selection.Contains(node))
				{
					m_selection.Add(node);
					node.OnSelected();
				}
			}
			else
			{
				ClearSelection();
				m_selection.Add(node);
				node.OnSelected();
			}
		}

		public void OnNodeDeselected(BTEditorGraphNode node)
		{
			if(m_selection.Remove(node))
			{
				node.OnDeselected();
			}
		}

		public void OnNodeBeginDrag(BTEditorGraphNode node, Vector2 position)
		{
			if(m_selection.Contains(node))
			{
				BTUndoSystem.BeginUndoGroup("Moved node(s)");
				for(int i = 0; i < m_selection.Count; i++)
				{
					BTUndoSystem.RegisterUndo(new UndoNodeMoved(m_selection[i]));
					m_selection[i].OnBeginDrag(position);
				}
			}
		}

		public void OnNodeDrag(BTEditorGraphNode node, Vector2 position)
		{
			if(m_selection.Contains(node))
			{
				for(int i = 0; i < m_selection.Count; i++)
				{
					m_selection[i].OnDrag(position);
				}
			}
		}

		public void OnNodeEndDrag(BTEditorGraphNode node)
		{
			if(m_selection.Contains(node))
			{
				for(int i = 0; i < m_selection.Count; i++)
				{
					m_selection[i].OnEndDrag();
				}

				BTUndoSystem.EndUndoGroup();
			}
		}

		public void OnNodeCreateChild(BTEditorGraphNode parent, Type childType)
		{
			if(parent != null && childType != null)
			{
				BTEditorGraphNode child = parent.OnCreateChild(childType);
				if(child != null)
				{
					BTUndoSystem.RegisterUndo(new UndoNodeCreated(child));
				}
			}
		}

		public void OnNodeDelete(BTEditorGraphNode node)
		{
			if(node != null)
			{
				BTUndoSystem.RegisterUndo(new UndoNodeDeleted(node));
				node.OnDelete();
			}
		}

		public void OnNodeDeleteChildren(BTEditorGraphNode node)
		{
			if(node != null)
			{
				BTUndoSystem.BeginUndoGroup("Delete children");
				int childIndex = 0;
				while(node.ChildCount > 0)
				{
					BTUndoSystem.RegisterUndo(new UndoNodeDeleted(node.GetChild(0), childIndex));
					node.OnDeleteChild(0);
					childIndex++;
				}
				BTUndoSystem.EndUndoGroup();
			}
		}

		public bool CanCopy(BTEditorGraphNode source)
		{
			return source != null && source.Node != null;
		}

		public void OnCopyNode(BTEditorGraphNode source)
		{
			if(CanCopy(source))
			{
				m_serializedCopyTarget = BTUtils.SerializeNode(source.Node);
			}
		}

		public bool CanPaste(BTEditorGraphNode destination)
		{
			if(destination != null && destination.Node != null && !string.IsNullOrEmpty(m_serializedCopyTarget))
			{
				if(destination.Node is Composite)
				{
					return true;
				}
				else if(destination.Node is Decorator)
				{
					return destination.ChildCount == 0;
				}
			}

			return false;
		}

		public void OnPasteNode(BTEditorGraphNode destination)
		{
			if(CanPaste(destination))
			{
				BehaviourNode node = BTUtils.DeserializeNode(m_serializedCopyTarget);
				BTEditorGraphNode child = BTEditorGraphNode.Create(destination, node);
				if(child != null)
				{
					ClearSelection();
					SelectNodeHierarchy(child);

					var undoState = new UndoNodeCreated(child);
					undoState.Title = "Pasted " + child.Node.Title;

					BTUndoSystem.RegisterUndo(undoState);
				}
			}
		}

		public void RemoveNodeFromSelection(BTEditorGraphNode node)
		{
			if(node != null)
			{
				m_selection.Remove(node);
			}
		}

		public void AddNodeToSelection(BTEditorGraphNode node)
		{
			if(node != null && !m_selection.Contains(node))
			{
				m_selection.Add(node);
			}
		}

		public string GetNodeHash(BTEditorGraphNode node)
		{
			List<byte> path = new List<byte>();
			for(BTEditorGraphNode n = node; n != null && n.Parent != null; n = n.Parent)
			{
				path.Add((byte)n.Parent.GetChildIndex(n));
			}
			path.Reverse();

			return Convert.ToBase64String(path.ToArray());
		}

		public BTEditorGraphNode GetNodeByHash(string path)
		{
			byte[] actualPath = Convert.FromBase64String(path);
			if(actualPath != null)
			{
				BTEditorGraphNode node = m_root;

				for(int i = 0; i < actualPath.Length; i++)
				{
					node = node.GetChild(actualPath[i]);
					if(node == null)
					{
						return null;
					}
				}

				return node;
			}

			return null;
		}

		private void SelectNodeHierarchy(BTEditorGraphNode node)
		{
			m_selection.Add(node);
			node.OnSelected();

			for(int i = 0; i < node.ChildCount; i++)
			{
				SelectNodeHierarchy(node.GetChild(i));
			}
		}

		private void ClearSelection()
		{
			if(m_selection.Count > 0)
			{
				for(int i = 0; i < m_selection.Count; i++)
				{
					m_selection[i].OnDeselected();
				}

				m_selection.Clear();
			}
		}

		private void OnDestroy()
		{
			BTEditorGraphNode.DestroyImmediate(m_root);
		}

		public static BTEditorGraph Create()
		{
			BTEditorGraph graph = ScriptableObject.CreateInstance<BTEditorGraph>();
			graph.OnCreated();
			graph.hideFlags = HideFlags.HideAndDontSave;
			graph.m_selection = new List<BTEditorGraphNode>();

			return graph;
		}
	}
}