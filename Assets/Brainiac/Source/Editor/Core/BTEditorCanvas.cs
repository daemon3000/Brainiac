using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

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
				if(Event.current == null)
				{
					Event evt = new Event();
					evt.type = EventType.Ignore;
					return evt;
				}

				return Event.current;
			}
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

		public void HandleEvents(Rect screenRect)
		{
			Vector2 canvasPosition = Position;
			Vector2 canvasSize = Size;

			if(canvasSize.x < screenRect.width)
			{
				canvasSize.x = screenRect.width;
			}
			if(canvasSize.y < screenRect.height)
			{
				canvasSize.y = screenRect.height;
			}

			if(Event.current.type == EventType.MouseDrag && Event.current.button == DRAG_MOUSE_BUTTON)
			{
				if(screenRect.Contains(Event.current.mousePosition))
				{
					canvasPosition += Event.current.delta;
					Event.current.Use();
				}
			}

			canvasPosition.x = Mathf.Clamp(canvasPosition.x, -(canvasSize.x - screenRect.width), 0.0f);
			canvasPosition.y = Mathf.Clamp(canvasPosition.y, -(canvasSize.y - screenRect.height), 0.0f);

			Position = canvasPosition;
			Size = canvasSize;
		}

		public Vector2 WindowSpaceToCanvasSpace(Vector2 mousePosition)
		{
			return mousePosition - Position;
		}
	}
}