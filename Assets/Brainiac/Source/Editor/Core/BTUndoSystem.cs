using UnityEngine;
using System;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	public static class BTUndoSystem
	{
		private const int MAX_UNDO_STACK_SIZE = 32;

		private static FixedSizeStack<BTUndoState> m_undoStack;
		private static FixedSizeStack<BTUndoState> m_redoStack;

		public static bool CanUndo
		{
			get { return m_undoStack.Count > 0; }
		}

		public static bool CanRedo
		{
			get { return m_redoStack.Count > 0; }
		}

		static BTUndoSystem()
		{
			m_undoStack = new FixedSizeStack<BTUndoState>(MAX_UNDO_STACK_SIZE);
			m_redoStack = new FixedSizeStack<BTUndoState>(MAX_UNDO_STACK_SIZE);
		}

		public static void BeginUndoGroup(string title)
		{
			if(m_undoStack.Count > 0)
			{
				BTUndoGroup oldGroup = m_undoStack.Peek() as BTUndoGroup;
				if(oldGroup != null && oldGroup.IsOpen)
				{
					Debug.LogWarningFormat("You have to call BTUndoSystem.EndUndoGroup before begining a new group. Old group is '{0}', new group is '{1}'", oldGroup.Title, title);
					if(oldGroup.IsEmpty)
					{
						m_undoStack.Pop();
					}
					else
					{
						oldGroup.Close();
					}
				}
			}

			BTUndoGroup group = new BTUndoGroup(title);
			m_undoStack.Push(group);
		}

		public static void EndUndoGroup()
		{
			if(m_undoStack.Count > 0)
			{
				BTUndoGroup oldGroup = m_undoStack.Peek() as BTUndoGroup;
				if(oldGroup != null && oldGroup.IsOpen)
				{
					if(oldGroup.IsEmpty)
					{
						m_undoStack.Pop();
					}
					else
					{
						oldGroup.Close();
					}
				}
			}
		}

		public static void RegisterUndo(BTUndoState undoState)
		{
			if(undoState != null && undoState.CanUndo)
			{
				if(m_undoStack.Count > 0)
				{
					BTUndoGroup topGroup = m_undoStack.Peek() as BTUndoGroup;
					if(topGroup != null && topGroup.IsOpen)
					{
						topGroup.AddUndoState(undoState);
						m_redoStack.Clear();
						return;
					}
				}

				m_undoStack.Push(undoState);
				m_redoStack.Clear();
			}
		}

		public static void Undo()
		{
			if(m_undoStack.Count > 0)
			{
				BTUndoState undoState = m_undoStack.Pop();
				if(undoState.CanUndo)
				{
					undoState.Undo();
					if(undoState.CanRedo)
					{
						m_redoStack.Push(undoState);
					}
				}

				while(m_undoStack.Count > 0 && !m_undoStack.Peek().CanUndo)
				{
					m_undoStack.Pop();
				}
			}
		}

		public static void Redo()
		{
			if(m_redoStack.Count > 0)
			{
				BTUndoState undoState = m_redoStack.Pop();
				if(undoState.CanRedo)
				{
					undoState.Redo();
					if(undoState.CanUndo)
					{
						m_undoStack.Push(undoState);
					}
				}

				while(m_redoStack.Count > 0 && !m_redoStack.Peek().CanRedo)
				{
					m_redoStack.Pop();
				}
			}
		}

		public static BTUndoState PeekUndo()
		{
			return m_undoStack.Peek();
		}

		public static BTUndoState PeekRedo()
		{
			return m_redoStack.Peek();
		}

		public static void Clear()
		{
			m_undoStack.Clear();
			m_redoStack.Clear();
		}
	}
}