using UnityEngine;
using UnityEditor;
using System;
using Brainiac;

namespace BrainiacEditor
{
	public static class BTContextMenuFactory
	{
		public static GenericMenu CreateNodeContextMenu(BTEditorGraphNode targetNode)
		{
			GenericMenu menu = new GenericMenu();
			bool canAddChild = false;

			if(targetNode.Node is Composite)
			{
				canAddChild = true;
			}
			else if(targetNode.Node is Decorator)
			{
				canAddChild = ((Decorator)targetNode.Node).GetChild() == null;
			}

			if(!targetNode.Graph.ReadOnly)
			{
				if(canAddChild)
				{
					BTNodeFactory.AddChild(menu, targetNode);
				}

				if(targetNode.Node is Root)
				{
					menu.AddDisabledItem(new GUIContent("Copy"));
					menu.AddDisabledItem(new GUIContent("Cut"));
				}
				else
				{
					menu.AddItem(new GUIContent("Copy"), false, () => targetNode.Graph.OnCopyNode(targetNode));
					menu.AddItem(new GUIContent("Cut"), false, () =>
					{
						targetNode.Graph.OnCopyNode(targetNode);
						targetNode.Graph.OnNodeDelete(targetNode);
					});
				}

				if(targetNode.Graph.CanPaste(targetNode))
				{
					menu.AddItem(new GUIContent("Paste"), false, () => targetNode.Graph.OnPasteNode(targetNode));
				}
				else
				{
					menu.AddDisabledItem(new GUIContent("Paste"));
				}

				if(targetNode.Node is Root)
				{
					menu.AddDisabledItem(new GUIContent("Delete"));
				}
				else
				{
					menu.AddItem(new GUIContent("Delete"), false, () => targetNode.Graph.OnNodeDelete(targetNode));
				}

				if(targetNode.Node is Composite)
				{
					if(((Composite)targetNode.Node).ChildCount > 0)
					{
						menu.AddItem(new GUIContent("Delete Children"), false, () => targetNode.Graph.OnNodeDeleteChildren(targetNode));
					}
					else
					{
						menu.AddDisabledItem(new GUIContent("Delete Children"));
					}
				}
				else if(targetNode.Node is Decorator)
				{
					if(((Decorator)targetNode.Node).GetChild() != null)
					{
						menu.AddItem(new GUIContent("Delete Child"), false, () => targetNode.Graph.OnNodeDeleteChildren(targetNode));
					}
					else
					{
						menu.AddDisabledItem(new GUIContent("Delete Child"));
					}
				}

				menu.AddSeparator("");
			}

			foreach(Breakpoint item in Enum.GetValues(typeof(Breakpoint)))
			{
				menu.AddItem(new GUIContent("Breakpoint/" + item.ToString()), targetNode.Node.Breakpoint.Has(item), (obj) =>
				{
					Breakpoint option = (Breakpoint)obj;
					if(option == Breakpoint.None)
					{
						targetNode.Node.Breakpoint = Breakpoint.None;
					}
					else
					{
						if(targetNode.Node.Breakpoint.Has(option))
						{
							targetNode.Node.Breakpoint = targetNode.Node.Breakpoint.Remove(option);
						}
						else
						{
							if(targetNode.Node.Breakpoint == Breakpoint.None)
								targetNode.Node.Breakpoint = option;
							else
								targetNode.Node.Breakpoint = targetNode.Node.Breakpoint.Add(option);
						}
					}
				}, item);
			}

			return menu;
		}

		public static GenericMenu CreateGraphContextMenu(BTEditorGraph graph)
		{
			GenericMenu menu = new GenericMenu();

			if(BTUndoSystem.CanUndo)
			{
				BTUndoState topUndo = BTUndoSystem.PeekUndo();
				menu.AddItem(new GUIContent(string.Format("Undo \"{0}\"", topUndo.Title)), false, () => BTUndoSystem.Undo());
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Undo"));
			}

			if(BTUndoSystem.CanRedo)
			{
				BTUndoState topRedo = BTUndoSystem.PeekRedo();
				menu.AddItem(new GUIContent(string.Format("Redo \"{0}\"", topRedo.Title)), false, () => BTUndoSystem.Redo());
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Redo"));
			}

			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Select All"), false, () => graph.SelectEntireGraph());

			return menu;
		}

		public static GenericMenu CreateOptionsMenu(BehaviourTreeEditor editor)
		{
			GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("New"), false, editor.CreateNewBehaviourTree);
			menu.AddItem(new GUIContent("Open"), false, editor.OpenBehaviourTree);
			menu.AddSeparator("");

			if(BTEditorCanvas.Current.ReadOnly)
			{
				menu.AddDisabledItem(new GUIContent("Save"));
			}
			else
			{
				menu.AddItem(new GUIContent("Save"), false, editor.SaveBehaviourTree);
				AssetDatabase.SaveAssets();
			}

			return menu;
		}
	}
}