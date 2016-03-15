using UnityEngine;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	public class BTEditorGraph : ScriptableObject
	{
		private const int SELECT_MOUSE_BUTTON = 0;

		private List<BTEditorGraphNode> m_selection;
		private BTEditorGraphNode m_root;

		private bool m_drawSelectionBox;
		private Vector2 m_selectionBoxStartPos;
		
		public Rect? SelectionBox { get; set; }

		private void OnCreated()
		{
			m_drawSelectionBox = false;
			m_selectionBoxStartPos = Vector2.zero;
			SelectionBox = null;
		}

		public void SetBehaviourTree(BehaviourTree behaviourTree)
		{
			if(m_root == null)
			{
				m_root = BTEditorGraphNode.Create(this, null, behaviourTree.Root);
			}
		}

		public void DrawGUI()
		{
			m_root.DrawGUI();
			DrawSelectionBox();
			HandleEvents();
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

		private void HandleEvents()
		{
			if(BTEditorCanvas.Current.Event.type == EventType.MouseDown && BTEditorCanvas.Current.Event.button == SELECT_MOUSE_BUTTON)
			{
				ClearSelection();
				
				m_selectionBoxStartPos = BTEditorCanvas.Current.Event.mousePosition;
				BTEditorCanvas.Current.Event.Use();
			}
			else if(BTEditorCanvas.Current.Event.type == EventType.MouseDrag && BTEditorCanvas.Current.Event.button == SELECT_MOUSE_BUTTON)
			{
				m_drawSelectionBox = true;
			}
			else if(BTEditorCanvas.Current.Event.type == EventType.MouseUp && BTEditorCanvas.Current.Event.button == SELECT_MOUSE_BUTTON)
			{
				m_drawSelectionBox = false;
				BTEditorCanvas.Current.Event.Use();
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
				for(int i = 0; i < m_selection.Count; i++)
				{
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
				Vector2 canvasSize = BTEditorCanvas.Current.Size;

				for(int i = 0; i < m_selection.Count; i++)
				{
					canvasSize.x = Mathf.Max(m_selection[i].Node.Position.x + 250.0f, canvasSize.x);
					canvasSize.y = Mathf.Max(m_selection[i].Node.Position.y + 250.0f, canvasSize.y);
					m_selection[i].OnEndDrag();
				}

				BTEditorCanvas.Current.Size = canvasSize;
			}
		}

		public void RemoveNodeFromSelection(BTEditorGraphNode node)
		{
			m_selection.Remove(node);
		}

		private void ClearSelection()
		{
			for(int i = 0; i < m_selection.Count; i++)
			{
				m_selection[i].OnDeselected();
			}

			m_selection.Clear();
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