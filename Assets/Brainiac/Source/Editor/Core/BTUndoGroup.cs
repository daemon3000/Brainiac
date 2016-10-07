using System.Collections.Generic;

namespace BrainiacEditor
{
	public class BTUndoGroup : BTUndoState
	{
		private const int MAX_UNDO_STEPS = 32;

		private List<BTUndoState> m_content;
		private bool m_isOpen;

		public override bool CanUndo
		{
			get { return true; }
		}

		public override bool CanRedo
		{
			get { return true; }
		}

		public bool IsOpen
		{
			get { return m_isOpen; }
		}

		public bool IsEmpty
		{
			get { return m_content.Count == 0; }
		}

		public BTUndoGroup(string title)
		{
			m_content = new List<BTUndoState>();
			m_isOpen = true;
			Title = title;
		}
		
		public void AddUndoState(BTUndoState undoState)
		{
			if(m_isOpen && m_content.Count < MAX_UNDO_STEPS)
			{
				m_content.Add(undoState);
			}
		}

		public override void Undo()
		{
			for(int i = m_content.Count - 1; i >= 0; i--)
			{
				m_content[i].Undo();
			}
		}

		public override void Redo()
		{
			for(int i = 0; i < m_content.Count; i++)
			{
				m_content[i].Redo();
			}
		}

		public void Close()
		{
			m_isOpen = false;
		}
	}
}