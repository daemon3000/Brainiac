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
		public Rect Area { get; set; }
		public bool IsDebuging { get; set; }
		public string Clipboard { get; set; }
		
		public BTEditorCanvas()
		{
			Position = Vector2.zero;
			Area = new Rect();
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
			Rect canvasArea = Area;

			if(canvasArea.xMax < windowSize.x)
			{
				canvasArea.xMax = windowSize.x;
			}
			if(canvasArea.yMax < windowSize.y)
			{
				canvasArea.yMax = windowSize.y;
			}

			if(Event.current.type == EventType.MouseDrag && Event.current.button == DRAG_MOUSE_BUTTON)
			{
				if(screenRect.Contains(Event.current.mousePosition))
				{
					canvasPosition += Event.current.delta;
					Event.current.Use();
				}
			}

			canvasPosition.x = Mathf.Clamp(canvasPosition.x, -(canvasArea.xMax - windowSize.x), -canvasArea.x);
			canvasPosition.y = Mathf.Clamp(canvasPosition.y, -(canvasArea.yMax - windowSize.y), -canvasArea.y);

			Position = canvasPosition;
			Area = canvasArea;
		}

		public void RecalculateSize(Vector2 referencePosition)
		{
			Rect canvasArea = Area;
			float xMax = Mathf.Max(referencePosition.x + 250.0f, canvasArea.xMax);
			float yMax = Mathf.Max(referencePosition.y + 150.0f, canvasArea.yMax);

			canvasArea.x = Mathf.Min(referencePosition.x - 150.0f, Mathf.Min(canvasArea.x, 0.0f));
			canvasArea.y = Mathf.Min(referencePosition.y - 150.0f, Mathf.Min(canvasArea.y, 0.0f));
			canvasArea.width = xMax - canvasArea.x;
			canvasArea.height = yMax - canvasArea.y;

			Area = canvasArea;
		}

		public void RecalculateSize(Rect referencePosition)
		{
			Rect canvasArea = Area;
			float xMax = Mathf.Max(referencePosition.xMax + 250.0f, canvasArea.xMax);
			float yMax = Mathf.Max(referencePosition.yMax + 150.0f, canvasArea.yMax);

			canvasArea.x = Mathf.Min(referencePosition.x - 150.0f, Mathf.Min(canvasArea.x, 0.0f));
			canvasArea.y = Mathf.Min(referencePosition.y - 150.0f, Mathf.Min(canvasArea.y, 0.0f));
			canvasArea.width = xMax - canvasArea.x;
			canvasArea.height = yMax - canvasArea.y;

			Area = canvasArea;
		}

		public void CenterOnPosition(Vector2 position, Vector2 windowSize)
		{
			Position = new Vector2((windowSize.x / 2.0f - position.x), (100 - position.y));
		}

		public Vector2 WindowSpaceToCanvasSpace(Vector2 mousePosition)
		{
			return mousePosition - Position;
		}
	}
}