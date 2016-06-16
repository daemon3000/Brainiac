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
					menu.AddSeparator("");
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

			if(!graph.ReadOnly)
			{
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
			}

			if(graph.Depth > 0)
			{
				menu.AddItem(new GUIContent("Go Up"), false, () => graph.OnPopNodeGroup());
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Go Up"));
			}

			if(!graph.ReadOnly)
				menu.AddItem(new GUIContent("Select All"), false, () => graph.SelectEntireGraph());

			return menu;
		}

		public static GenericMenu CreateOptionsMenu(BehaviourTreeEditor editor)
		{
			GenericMenu menu = new GenericMenu();
			BTEditorTreeLayout treeLayout = BTEditorStyle.TreeLayout;

			menu.AddItem(new GUIContent("New"), false, editor.CreateNewBehaviourTree);
			menu.AddItem(new GUIContent("Open"), false, editor.OpenBehaviourTree);
			if(BTEditorCanvas.Current.ReadOnly)
			{
				menu.AddDisabledItem(new GUIContent("Save"));
			}
			else
			{
				menu.AddItem(new GUIContent("Save"), false, editor.SaveBehaviourTree);
				AssetDatabase.SaveAssets();
			}
			menu.AddSeparator("");

			menu.AddItem(new GUIContent("Snap To Grid"), BTEditorCanvas.Current.SnapToGrid, () =>
			{
				BTEditorCanvas.Current.SnapToGrid = !BTEditorCanvas.Current.SnapToGrid;
			});

			foreach(BTEditorTreeLayout layout in Enum.GetValues(typeof(BTEditorTreeLayout)))
			{
				menu.AddItem(new GUIContent("Layout/" + layout.ToString()), treeLayout == layout, (obj) =>
				{
					BTEditorStyle.TreeLayout = (BTEditorTreeLayout)obj;
				}, layout);
			}

			return menu;
		}

		public static GenericMenu CreateNodeInspectorContextMenu(BehaviourNode targetNode)
		{
			GenericMenu menu = new GenericMenu();
			BTConstraintFactory.AddConstraint(menu, targetNode);
			BTServiceFactory.AddService(menu, targetNode);

			return menu;
		}

		public static GenericMenu CreateConstraintContextMenu(BehaviourNode targetNode, int constraintIndex)
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Move Up"), false, () => 
			{
				if(constraintIndex > 0)
				{
					var temp = targetNode.Constraints[constraintIndex];
					targetNode.Constraints[constraintIndex] = targetNode.Constraints[constraintIndex - 1];
					targetNode.Constraints[constraintIndex - 1] = temp;
				}
			});

			menu.AddItem(new GUIContent("Move Down"), false, () => 
			{
				if(constraintIndex < targetNode.Constraints.Count - 1)
				{
					var temp = targetNode.Constraints[constraintIndex];
					targetNode.Constraints[constraintIndex] = targetNode.Constraints[constraintIndex + 1];
					targetNode.Constraints[constraintIndex + 1] = temp;
				}
			});

			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Remove"), false, () => 
			{
				targetNode.Constraints.RemoveAt(constraintIndex);
			});

			return menu;
		}

		public static GenericMenu CreateServiceContextMenu(BehaviourNode targetNode, int serviceIndex)
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Move Up"), false, () =>
			{
				if(serviceIndex > 0)
				{
					var temp = targetNode.Services[serviceIndex];
					targetNode.Services[serviceIndex] = targetNode.Services[serviceIndex - 1];
					targetNode.Services[serviceIndex - 1] = temp;
				}
			});

			menu.AddItem(new GUIContent("Move Down"), false, () =>
			{
				if(serviceIndex < targetNode.Services.Count - 1)
				{
					var temp = targetNode.Services[serviceIndex];
					targetNode.Services[serviceIndex] = targetNode.Services[serviceIndex + 1];
					targetNode.Services[serviceIndex + 1] = temp;
				}
			});

			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Remove"), false, () =>
			{
				targetNode.Services.RemoveAt(serviceIndex);
			});

			return menu;
		}
	}
}