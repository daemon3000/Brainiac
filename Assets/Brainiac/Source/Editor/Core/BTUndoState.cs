using UnityEngine;
using System;
using System.Collections;

namespace BrainiacEditor
{
	public abstract class BTUndoState
	{
		public string Title { get; set; }
		public abstract bool CanUndo { get; }
		public abstract bool CanRedo { get; }

		public abstract void Undo();
		public abstract void Redo();
	}
}