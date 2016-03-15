using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;

namespace BrainiacEditor
{
	public class BTEditorCanvas
	{
		private const int DRAG_MOUSE_BUTTON = 2;
		
		public event UnityAction OnRepaint;
		
		private int m_snapSize;
		private static BTEditorCanvas m_instance;

		public static BTEditorCanvas Current
		{
			get
			{
				return m_instance;
			}
			set
			{
				m_instance = value;
			}
		}

		public Event Event
		{
			get
			{
				return Event.current;
			}
		}
		
		public bool CanEdit
		{
			get { return !EditorApplication.isPlaying && !IsDebuging; }
		}

		public int SnapSize
		{
			get { return m_snapSize; }
			set { m_snapSize = Mathf.Max(value, 1); }
		}

		public Vector2 Position { get; set; }
		public Vector2 Size { get; set; }
		public bool IsDebuging { get; set; }
		public bool SnapToGrid { get; set; }
		
		public BTEditorCanvas()
		{
			Position = Vector2.zero;
			Size = new Vector2(1000, 1000);
			IsDebuging = false;
			SnapToGrid = true;
			SnapSize = 10;
		}

		public void Repaint()
		{
			if(OnRepaint != null)
			{
				OnRepaint();
			}
		}

		public void HandleEvents(EditorWindow window)
		{
			Vector2 canvasPosition = Position;
			Vector2 canvasSize = Size;

			if(canvasSize.x < window.position.width)
			{
				canvasSize.x = window.position.width;
			}
			if(canvasSize.y < window.position.height)
			{
				canvasSize.y = window.position.height;
			}

			if(Event.current.type == EventType.MouseDrag && Event.current.button == DRAG_MOUSE_BUTTON)
			{
				canvasPosition += Event.current.delta;
				Event.current.Use();
			}

			canvasPosition.x = Mathf.Clamp(canvasPosition.x, -(canvasSize.x - window.position.width), 0.0f);
			canvasPosition.y = Mathf.Clamp(canvasPosition.y, -(canvasSize.y - window.position.height), 0.0f);

			Position = canvasPosition;
			Size = canvasSize;
		}

		public Vector2 WindowSpaceToCanvasSpace(Vector2 mousePosition)
		{
			return mousePosition - Position;
		}
	}
}