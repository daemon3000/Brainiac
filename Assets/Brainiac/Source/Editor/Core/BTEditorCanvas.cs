using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using Brainiac;

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

		public bool ReadOnly
		{
			get { return IsDebuging || EditorApplication.isPlaying; }
		}

		public bool IsPlaying
		{
			get { return EditorApplication.isPlaying; }
		}

		public bool SnapToGrid
		{
			get
			{
				return EditorPrefs.GetBool("Brainiac.Editor.SnapToGrid", true);
			}
			set
			{
				EditorPrefs.SetBool("Brainiac.Editor.SnapToGrid", value);
			}
		}

		public Vector2 Position { get; set; }
		public Vector2 Size { get; set; }
		public bool IsDebuging { get; set; }
		public string Clipboard { get; set; }
		
		public BTEditorCanvas()
		{
			Position = Vector2.zero;
			Size = Vector2.zero;
			IsDebuging = false;
			SnapSize = 10;
			Clipboard = null;
		}

		public void Repaint()
		{
			if(OnRepaint != null)
			{
				OnRepaint();
			}
		}

		public void HandleEvents(Rect screenRect, Vector2 windowSize)
		{
			Vector2 canvasPosition = Position;
			Vector2 canvasSize = Size;

			if(canvasSize.x < windowSize.x)
			{
				canvasSize.x = windowSize.x;
			}
			if(canvasSize.y < windowSize.y)
			{
				canvasSize.y = windowSize.y;
			}

			if(Event.current.type == EventType.MouseDrag && Event.current.button == DRAG_MOUSE_BUTTON)
			{
				if(screenRect.Contains(Event.current.mousePosition))
				{
					canvasPosition += Event.current.delta;
					Event.current.Use();
				}
			}

			canvasPosition.x = Mathf.Clamp(canvasPosition.x, -(canvasSize.x - windowSize.x), 0.0f);
			canvasPosition.y = Mathf.Clamp(canvasPosition.y, -(canvasSize.y - windowSize.y), 0.0f);

			Position = canvasPosition;
			Size = canvasSize;
		}

		public void RecalculateSize(Vector2 referencePosition)
		{
			Vector2 canvasSize = Size;
			canvasSize.x = Mathf.Max(referencePosition.x + 250.0f, canvasSize.x);
			canvasSize.y = Mathf.Max(referencePosition.y + 250.0f, canvasSize.y);

			Size = canvasSize;
		}

		public Vector2 WindowSpaceToCanvasSpace(Vector2 mousePosition)
		{
			return mousePosition - Position;
		}
	}
}